using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;
using ExpandedAiFramework;
using Il2CppTLD.AI;
using UnityEngine;
using UnityEngine.AI;
using ImprovedCougar.Pathfinding;
using Color = UnityEngine.Color;
using static Il2Cpp.CarcassSite;
using static UnityEngine.GraphicsBuffer;
using static Il2CppTLD.AI.CougarManager;
using AudioMgr;
using static Il2Cpp.ak.wwise.core;
using Random = System.Random;
using Il2CppSystem.Data;

namespace ImprovedCougar
{
    [RegisterTypeInIl2Cpp]
    internal class CustomCougar : BaseCougar
    {

        internal static Settings.Settings CustomCougarSettings = new Settings.Settings();
        private enum CustomCougarAiMode : int
        {
            Hide = AiMode.Disabled + 1,
            Freeze = AiMode.Disabled + 2,
            Retreat = AiMode.Disabled + 3,
            COUNT
        }

        public enum CougarPath : int
        {
            Cougar = 11,
        }

        public CustomCougar(IntPtr ptr) : base(ptr) { }

        protected WanderPath wanderPath;
        protected bool WanderPathConnected = false;
        protected bool mFetchingWanderPath = false;
        protected bool toStartFollowWanderPathMode = true;
        protected override float m_MinWaypointDistance { get { return 100.0f; } }
        protected override float m_MaxWaypointDistance { get { return 1000.0f; } }

        private Vector3 lastWaypointPosition;

        //speeds
        private float currentSpeed = 0f;

        private float baseStalkSpeed = 0f;
        private float closeStalkSpeed = 0f;
        private float wanderSpeed = 0f;
        private float attackSpeed = 0f;

        private float spottedRetreatSpeed = 0f;

        //distances
        private const float attackDistance = 25f; //for now
        private const float maxStalkDistance = 200f; //for now
        private const float closeDistance = 35f;

        //states
        private bool toTeleport = false;
        private bool toHide = false;
        private bool toFreeze = false;
        private bool stopFreezing = false;
        private bool isInvisible = false;

        //pathfinding
        List<Vector3> path;
        Vector3 lastPosition;
        int currentIndex = 0;
        float reachThreshold = 0.5f;
        bool pathBroken = false;

        Vector3 spawnPosition = Vector3.zero; //temporary territory node position
        Vector3 retreatPosition;

        //time
        float timeSinceLastPath = 0f;
        float recalcInterval = 1.5f;

        float timeSinceHiding = 0f;
        float timeToComeOut = 5f;

        float timeSinceFreezing = 0f;
        float timeToFreezeFor = 8f;

        float timeForNextAudio = 0f;

        //audio
        Shot audio;
        int minHoursBetweenAudio = 1;
        int maxHoursBetweenAudio = 5;

        public override void Initialize(BaseAi ai, TimeOfDay timeOfDay, SpawnRegion spawnRegion, SpawnModDataProxy proxy)
        {
            base.Initialize(ai, timeOfDay, spawnRegion, proxy);
            mBaseAi.m_DefaultMode = AiMode.Wander;
            mBaseAi.m_StartMode = AiMode.Wander;
            mBaseAi.m_AttackChanceAfterNearMissGunshot = 10;
            mBaseAi.m_AttackChanceAfterNearMissRevolverShot = 5;
            //mBaseAi.m_DetectionRange *= 2 //this is temporary
            wanderSpeed = mBaseAi.m_WalkSpeed + 1f;
            baseStalkSpeed = mBaseAi.m_StalkSpeed + 2;
            closeStalkSpeed = mBaseAi.m_StalkSpeed;
            spottedRetreatSpeed = mBaseAi.m_StalkSpeed - 1;
            attackSpeed = Settings.CustomSettings.settings.cougarSpeed;
            //this is temporary
            spawnPosition = mBaseAi.transform.position;

            mBaseAi.m_WaypointCompletionBehaviour = BaseAi.WaypointCompletionBehaviouir.Restart;
            mBaseAi.m_TargetWaypointIndex = 0;

            //audio

            audio = AudioMaster.CreateShot(mBaseAi.gameObject, AudioMaster.SourceType.SFX);

            //apply audio settings here

            audio.SetVolume(100f);
            audio._audioSource.minDistance = 20f;
            audio._audioSource.maxDistance = 1500f;
            audio._audioSource.rolloffMode = AudioRolloffMode.Custom;

            AnimationCurve curve = new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(0.1f, 0.9f),
                new Keyframe(0.3f, 0.7f),
                new Keyframe(0.6f, 0.45f),
                new Keyframe(0.9f, 0.25f),
                new Keyframe(1.0f, 0.15f),
                new Keyframe(1.5f, 0f) 
            );

            audio._audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);

            timeForNextAudio = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + 0.1f; //this probably means audio will play 1 hour after going outside 

            Main.Logger.Log($"Cougar initialized at position {mBaseAi.gameObject.transform.position.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);
        }

        protected override bool ProcessCustom()
        {

            //Main.Logger.Log("Processing custom cougar logic", ComplexLogger.FlaggedLoggingLevel.Debug);

            DoStartFollowWanderPathFirstFrame();
            MaybeReactToGunshot();
            MaybePlayCougarAudio();
            DoOnUpdate();

            switch (CurrentMode)
            {
                case AiMode.Stalking:
                    ProcessStalkingAdvanced();
                    return false;
                case (AiMode)CustomCougarAiMode.Hide:
                    ProcessHiding();
                    return false;
                case (AiMode)CustomCougarAiMode.Freeze:
                    ProcessFreezing();
                    return false;
                case (AiMode)CustomCougarAiMode.Retreat:
                    ProcessingRetreating();
                    return false;
                default:
                    return base.ProcessCustom();
            }
        }

        protected override bool PreProcessCustom()
        {
            switch (CurrentMode)
            {
                case AiMode.FollowWaypoints:
                    PreProcessingFollowPath();
                    return false;
                default:
                    return base.PreProcessCustom();
            }
        }

        

        protected void ProcessStalkingAdvanced()
        {

            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;
            Vector3 playerPosition = player.position;
            Vector3 currentCougarPosition = cougar.position;
            float currentDistance = Vector3.Distance(currentCougarPosition, playerPosition);

            if (currentDistance >= attackDistance)
            {

                if(currentDistance >= maxStalkDistance)
                {
                    //go back to what it was doing before stalking, just patrolling for now
                    StartFollowWanderPath();
                }

                timeSinceLastPath += Time.deltaTime;
                if (timeSinceLastPath > recalcInterval || pathBroken)
                {
                    Main.Logger.Log("Calculating path...", ComplexLogger.FlaggedLoggingLevel.Debug);
                    StartStalkingPathfinding();
                }

                Main.Logger.Log($"Cougar Distance: {currentDistance}", ComplexLogger.FlaggedLoggingLevel.Debug);

                if (CougarCanSeePlayerLooking(player, cougar))
                {
                    if (currentDistance >= 70f)
                    {
                        timeToFreezeFor = 5f;
                    }
                    else if (currentDistance <= 70f && currentDistance >= 35)
                    {
                        timeToFreezeFor = 3f;
                    }
                    else timeToFreezeFor = 1f;

                    Main.Logger.Log($"Cougar can see player looking, freezing for {timeToFreezeFor} seconds and assessing.", ComplexLogger.FlaggedLoggingLevel.Debug);
                    toHide = true;
                    SetAiMode((AiMode)CustomCougarAiMode.Freeze);
                }

                if(currentDistance <= closeDistance)
                {
                    currentSpeed = closeStalkSpeed;
                }

                if (path != null && currentIndex < path.Count)
                {

                    Vector3 target = path[currentIndex];

                    //Main.Logger.Log($"Moving to position: {target.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);

                    mBaseAi.StartPath(target, currentSpeed);

                    if (!mBaseAi.CanPathfindToPosition(target))
                    {
                        TeleportCougarToPosition(target, cougar);
                    }

                    if (Vector3.Distance(cougar.position, target) < reachThreshold)
                    {
                        if(currentIndex != 0 && target != player.position)
                        {
                            lastPosition = target;
                        }

                        currentSpeed = baseStalkSpeed;
                        if (toHide && currentIndex != 0) //this may not be needed but i'm keeping here just in case
                        {
                            SetAiMode((AiMode)CustomCougarAiMode.Hide);
                        }
                        currentIndex++;
                    }
                    else //still moving towards node, will revisit this stuff when it becomes relevant again
                    {
                        /*** 
                        if (currentIndex == 0 || target == playerPosition) return;

                        if (!IsPointStillCover(target, mBaseAi.m_CurrentTarget.GetEyePos(), Utils.m_PhysicalCollisionLayerMask))
                        {
                            //Main.Logger.Log($"Current point {target.ToString()} is invalid at index {currentIndex}. Repathing.", ComplexLogger.FlaggedLoggingLevel.Debug);
                            pathBroken = true;
                            toTeleport = true;
                            toHide = true;
                            return;
                        }
                        else if (toTeleport)
                        {
                            TeleportCougarToPosition(target, cougar);
                            return;
                        } ***/

                    }
                }
                else
                {
                    //if no path found it probably means there is no cover nearby, so it isn't stealthy enough to keep stalking. If close enough keep going 

                    //i can probably change this to detect for allowed areas or something
                    Main.Logger.Log("Cougar is in open area! Continuing directly to player.", ComplexLogger.FlaggedLoggingLevel.Debug);

                    mBaseAi.StartPath(playerPosition, currentSpeed);                   
                }
            }
            else
            {

                AiMode attackType = DetermineAttackType();

                if (!IsPlayerFacingCougar(player, cougar))
                {

                    if(attackType == AiMode.PassingAttack)
                    {
                        Main.Logger.Log("Passing attack!", ComplexLogger.FlaggedLoggingLevel.Debug);
                        mBaseAi.Cougar.SetWillDoPassingAttack();
                    }

                    Main.Logger.Log("Attacking!", ComplexLogger.FlaggedLoggingLevel.Debug);
                    mBaseAi.SetAiMode(AiMode.Attack);
                }   
                //else maybe do a fake charge?
            }
        }

        protected void ProcessHiding()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            if (!IsPlayerFacingCougar(player, cougar)) //this works
            {
                //wait a few seconds to keep stalking

                timeSinceHiding += Time.deltaTime;
                if (timeSinceHiding > timeToComeOut)
                {
                    Main.Logger.Log("Cougar is hiding and player is not looking. Continuing to stalk...", ComplexLogger.FlaggedLoggingLevel.Debug);

                    //change state here  
                    SetAiMode(AiMode.Stalking);
                }
            }
        }

        protected void ProcessFreezing()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;
            float currentDistance = Vector3.Distance(cougar.position, player.position);

            //wait a few seconds to flee, but if the player gets too close, then unfreeze and attack!

            if (currentDistance <= attackDistance)
            {
                //charge the player
                SetAiMode(AiMode.Attack);
            }

            timeSinceFreezing += Time.deltaTime;
            if (timeSinceFreezing > timeToFreezeFor || stopFreezing) //add some slight movements here, a cougar freezing isn't completely still, maybe, fact check this
            {

                //add more complex behavior decision making here
                if (IsPlayerFacingCougar(player, cougar))
                {
                    Main.Logger.Log("Cougar is freezing for too long and player is looking. Gonna retreat to cover...", ComplexLogger.FlaggedLoggingLevel.Debug);

                    //change state here  
                    currentSpeed = spottedRetreatSpeed;
                    retreatPosition = lastPosition != Vector3.zero ? lastPosition : (Vector3)FindRetreatCoverPoint(cougar, player, 65f, Utils.m_PhysicalCollisionLayerMask);
                    SetAiMode((AiMode)CustomCougarAiMode.Retreat);
                }
                else
                {
                    Main.Logger.Log("Cougar is freezing for too long and player is not looking. Teleporting cougar to retreat point and hiding...", ComplexLogger.FlaggedLoggingLevel.Debug);
                    retreatPosition = lastPosition != Vector3.zero ? lastPosition : (Vector3)FindRetreatCoverPoint(cougar, player, 65f, Utils.m_PhysicalCollisionLayerMask);
                    TeleportCougarToPosition(retreatPosition, cougar);
                    SetAiMode((AiMode)CustomCougarAiMode.Hide);
                }
            }
        }

        protected void ProcessingRetreating()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;
            float currentDistance = Vector3.Distance(cougar.position, player.position);

            if (retreatPosition == Vector3.zero)
            {
                retreatPosition = lastWaypointPosition;
                Main.Logger.Log("Retreat position is invalid. Falling back to last patrol point.", ComplexLogger.FlaggedLoggingLevel.Debug);
            }
            mBaseAi.StartPath(retreatPosition, currentSpeed);

            if (currentDistance <= attackDistance)
            {
                //charge the player
                SetAiMode(AiMode.Attack);
            }

            if (Vector3.Distance(cougar.position, retreatPosition) < reachThreshold)
            {
                if (retreatPosition == lastWaypointPosition)
                {
                    Main.Logger.Log("Cougar has reached it's last patrol point, going back to wandering", ComplexLogger.FlaggedLoggingLevel.Debug);
                    StartFollowWanderPath();
                }
                else
                {
                    Main.Logger.Log("Cougar has reached it's retreat position, going to hide.", ComplexLogger.FlaggedLoggingLevel.Debug);
                    mBaseAi.SetAiMode((AiMode)CustomCougarAiMode.Hide);
                }

            }

        }

        protected void BeginStalking()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            mBaseAi.m_DefaultMode = AiMode.Wander;
            mBaseAi.MoveAgentStop();
            mBaseAi.m_CurrentTarget = GameManager.GetPlayerManagerComponent().m_AiTarget;
            currentSpeed = baseStalkSpeed;
            toTeleport = false;

            StartStalkingPathfinding();

            Main.Logger.Log("Cougar is stalking player", ComplexLogger.FlaggedLoggingLevel.Debug);
            //InterfaceManager.GetPanel<Panel_BodyHarvest>().DisplayErrorMessage("Cougar is stalking player!");
        }

        protected void BeginHiding()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            timeSinceHiding = 0f;
            mBaseAi.MoveAgentStop();
            toHide = false;
            ToggleInvisibility();

            Main.Logger.Log("Hiding in cover", ComplexLogger.FlaggedLoggingLevel.Debug);
            //InterfaceManager.GetPanel<Panel_BodyHarvest>().DisplayErrorMessage("Cougar is moving to cover!");

        }

        protected void StopHiding()
        {
            Main.Logger.Log("Leaving cover", ComplexLogger.FlaggedLoggingLevel.Debug);
            ToggleInvisibility();
        }

       

        protected void StopFollowWaypoints()
        {
            Main.Logger.Log("Leaving wander path", ComplexLogger.FlaggedLoggingLevel.Debug);
            lastWaypointPosition = mBaseAi.transform.position;
        }

        protected void BeginFreezing()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            timeSinceFreezing = 0f;
            mBaseAi.MoveAgentStop();
            toFreeze = false;
            Main.Logger.Log("Cougar is freezing!", ComplexLogger.FlaggedLoggingLevel.Debug);

        }

        protected void BeginRetreating()
        {
            currentSpeed = baseStalkSpeed;
            Main.Logger.Log("Cougar is retreating!", ComplexLogger.FlaggedLoggingLevel.Debug);
        }

        protected void BeginAttacking()
        {
            currentSpeed = attackSpeed;
            mBaseAi.m_DefaultMode = AiMode.Stalking; //this is so after cougar flees from attack, he goes back to stalking
        }

        protected void DoStartFollowWanderPathFirstFrame()
        {
            if (toStartFollowWanderPathMode) StartFollowWanderPath();
        }

        protected void StartFollowWanderPath()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            if (!WanderPathConnected)
            {
                Main.Logger.Log("No wanderpath connected.", ComplexLogger.FlaggedLoggingLevel.Debug);
                if (!TryGetSavedWanderPath(mModDataProxy))
                {
                    Main.Logger.Log("No wanderpath saved.", ComplexLogger.FlaggedLoggingLevel.Debug);
                    mManager.DataManager.ScheduleMapDataRequest<WanderPath>(new GetNearestMapDataRequest<WanderPath>(mBaseAi.transform.position, mModDataProxy.Scene, (nearestSpot, result2) =>
                    {
                       AttachWanderPath(nearestSpot);
                    }, false, CheckIfWanderPathIsForCougar, 0));            
                }
            }

            if(wanderPath == null)
            {
                Main.Logger.Log("Wander path is null.", ComplexLogger.FlaggedLoggingLevel.Error);
                return;
            }

            Vector3 closestPoint = GetNearestPointOnPath(wanderPath, cougar.position);
            mBaseAi.m_MoveAgent.transform.position = closestPoint;
            mBaseAi.m_MoveAgent.Warp(closestPoint, 2.0f, true, -1);
            currentSpeed = wanderSpeed; //for consistency, i know this is useless
            mBaseAi.m_AiGoalSpeed = wanderSpeed;
            mBaseAi.SetAiMode(AiMode.FollowWaypoints);
            string pathType = (CougarPath)wanderPath.WanderPathType == CougarPath.Cougar ? "cougar" : "non-cougar";
            Main.Logger.Log($"Following {pathType} path {wanderPath.Guid.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);

            toStartFollowWanderPathMode = false;

            //debug
            DebugTools.CreateDebugMarker(cougar.position, Color.magenta, 15f);

        }

        public bool CheckIfWanderPathIsForCougar(WanderPath path)
        {
            if ((CougarPath)path.WanderPathType == CougarPath.Cougar) Main.Logger.Log("WanderPath is Cougar wanderpath", ComplexLogger.FlaggedLoggingLevel.Debug);
            else Main.Logger.Log("WanderPath is NOT cougar path.", ComplexLogger.FlaggedLoggingLevel.Debug);
            return (CougarPath)path.WanderPathType == CougarPath.Cougar ? true : false;
        }  

        protected void PreProcessingFollowPath()
        {

            //this is where you can change out of state, since the main AiMode isn't being overridden
            //also do other stuff

            Vector3 waypoint = mBaseAi.m_Waypoints[mBaseAi.m_TargetWaypointIndex];

            if (!mBaseAi.CanPathfindToPosition(waypoint))
            {
                Main.Logger.Log($"Cannot pathfind to next waypoint at {waypoint.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);
                TeleportCougarToPosition(waypoint, mBaseAi.transform);
            } 


        }

        //line of sight
        private bool IsPlayerFacingCougar(Transform player, Transform cougar)
        {
            Vector3 cougarPosition = (cougar.position - player.position).normalized;
            Vector3 playerForward = player.forward;

            float dot = Vector3.Dot(playerForward, cougarPosition);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            return angle <= 45f;
        }

        private bool CougarCanSeePlayerLooking(Transform player, Transform cougar)
        {

            if (IsPlayerFacingCougar(player, cougar))
            {
                if (AiUtils.PositionVisible(mBaseAi.GetEyePos(), cougar.forward, mBaseAi.m_CurrentTarget.GetEyePos(), 100f, mBaseAi.m_DetectionFOV, 0f, Utils.m_PhysicalCollisionLayerMask)) return true;
            }

            return false;
        }


        //pathfinding
        public List<Vector3> FindAllNearbyCover(Transform cougar, Transform player, float maxDistance, LayerMask coverObstructionMask)
        {
            Main.Logger.Log("Grabbing new cover positions relative to cougar", ComplexLogger.FlaggedLoggingLevel.Debug);
            //checkStartTime = Time.realtimeSinceStartup;

            Vector3 direction = (player.position - cougar.position).normalized;
            HashSet<Collider> cougarColliders = new HashSet<Collider>(cougar.GetComponentsInChildren<Collider>());
            RaycastHit[] sphereCastHits = Physics.SphereCastAll(cougar.position, 70f, direction, maxDistance, coverObstructionMask);
            Collider[] nearbyObjects = sphereCastHits.Select(hit => hit.collider).ToArray();
            List<Vector3> coverPoints = new();
            Vector3 cougarEye = mBaseAi.GetEyePos();
            Vector3 playerEye = mBaseAi.m_CurrentTarget.GetEyePos();
            float cougarHeight = GetCougarHeight(cougar);

            Main.Logger.Log($"Nearby cover objects: {nearbyObjects.Length}", ComplexLogger.FlaggedLoggingLevel.Debug);

            foreach (Collider col in nearbyObjects)
            {
                if (cougarColliders.Contains(col)) continue;
                if (col.bounds.Contains(cougar.position)) continue;
                if (col.bounds.size.y < cougarHeight) continue;

                DebugTools.HighlightColliderBounds(col);

                /***Vector3 dirToCougar = (cougarEye - playerEye).normalized;
                float distanceToCougar = Vector3.Distance(playerEye, cougarEye); ***/

                Vector3 dirToCollider = (col.bounds.center - playerEye).normalized;
                float distanceToCollider = Vector3.Distance(playerEye, col.bounds.center);

                //DebugTools.DrawRay(playerEye, dirToCollider, distanceToCollider, Color.red, 5f);
                if (Physics.Raycast(playerEye, dirToCollider, out RaycastHit hit, distanceToCollider, coverObstructionMask))
                {

                    if (hit.collider == col)
                    {
                        // It blocks LOS between player and cougar — consider it
                        Vector3 dirFromPlayer = (col.bounds.center - playerEye).normalized;
                        Vector3 roughCoverPoint = col.bounds.center + dirFromPlayer * col.bounds.extents.magnitude * 0.2f;

                        //DebugTools.CreateDebugMarker(roughCoverPoint, Color.red, 30f);

                        Vector3? finalCoverPoint = ForcePointToGround(roughCoverPoint, col, 30f, 2f, coverObstructionMask, 30f);
                        if (finalCoverPoint != null)
                            coverPoints.Add((Vector3)finalCoverPoint);
                        else
                            coverPoints.Add(roughCoverPoint);
                    }
                }
            }

            return coverPoints;
        }

        public Vector3? FindRetreatCoverPoint(Transform cougar, Transform player, float maxDistance, LayerMask coverObstructionMask)
        {
            Vector3 retreatDirection = (cougar.position - player.position).normalized; // opposite of stalking
            HashSet<Collider> cougarColliders = new HashSet<Collider>(cougar.GetComponentsInChildren<Collider>());
            RaycastHit[] hits = Physics.SphereCastAll(cougar.position, 70f, retreatDirection, maxDistance, coverObstructionMask);

            Vector3 cougarEye = mBaseAi.GetEyePos();
            Vector3 playerEye = mBaseAi.m_CurrentTarget.GetEyePos();
            float cougarHeight = GetCougarHeight(cougar);

            Vector3? bestCover = null;
            float closestDistance = float.MaxValue;

            foreach (RaycastHit hit in hits)
            {
                Collider col = hit.collider;
                if (cougarColliders.Contains(col)) continue;
                if (col.bounds.Contains(cougar.position)) continue;
                if (col.bounds.size.y < cougarHeight) continue;

                Vector3 dirToCollider = (col.bounds.center - playerEye).normalized;
                float distanceToCollider = Vector3.Distance(playerEye, col.bounds.center);

                if (Physics.Raycast(playerEye, dirToCollider, out RaycastHit losHit, distanceToCollider, coverObstructionMask))
                {
                    if (losHit.collider == col)
                    {
                        Vector3 dirFromPlayer = (col.bounds.center - playerEye).normalized;
                        Vector3 roughCoverPoint = col.bounds.center + dirFromPlayer * col.bounds.extents.magnitude * 0.2f;

                        Vector3? finalCoverPoint = ForcePointToGround(roughCoverPoint, col, 30f, 2f, coverObstructionMask, 30f);

                        Vector3 coverPoint = finalCoverPoint ?? roughCoverPoint;
                        float distToCougar = Vector3.Distance(cougar.position, coverPoint);

                        if (distToCougar < closestDistance)
                        {
                            closestDistance = distToCougar;
                            bestCover = coverPoint;
                        }
                    }
                }
            }

            return bestCover;
        }
        bool IsPointStillCover(Vector3 coverPoint, Vector3 playerEye, LayerMask coverMask)
        {
            Vector3 direction = (coverPoint - playerEye).normalized;
            float distance = Vector3.Distance(playerEye, coverPoint);

            return Physics.Raycast(playerEye, direction, out RaycastHit hit, distance, coverMask) ? true : false;
        }
        public Vector3? ForcePointToGround(Vector3 startPoint, Collider excludedCollider, float maxDrop = 10f, float stepSize = 0.5f, LayerMask groundMask = default, float debugDuration = 10f)
        {
            float totalDrop = 0f;
            Vector3 probePoint = startPoint;

            while (totalDrop < maxDrop)
            {
                // Check straight down from current probe point
                if (Physics.Raycast(probePoint, Vector3.down, out RaycastHit hit, stepSize, groundMask))
                {
                    if (hit.collider != excludedCollider)
                    {
                        //DebugTools.CreateDebugMarker(hit.point, Color.green, debugDuration);
                        // Found valid ground!
                        return hit.point;
                    }
                }

                // Move probe point down and try again
                probePoint.y -= stepSize;
                totalDrop += stepSize;
            }

            // Nothing found within drop range
            return null;
        }

        private void StartStalkingPathfinding()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            float dist = Vector3.Distance(player.position, cougar.position);

            List<Vector3> coverPoints = FindAllNearbyCover(cougar, player, dist, Utils.m_PhysicalCollisionLayerMask);

            path = PathfindingFunctions.FindAStarPath(cougar.position, coverPoints, player.position);

            if (path != null)
            {
                pathBroken = false;
                Main.Logger.Log($"Path found with {path.Count - 2} nodes", ComplexLogger.FlaggedLoggingLevel.Debug);
                foreach (Vector3 point in path)
                {
                    if (point == player.position || point == cougar.position) break;
                    Main.Logger.Log($"Pathed point {point.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);
                    //DebugTools.CreateDebugMarker(point, Color.green, 5f);
                }
            }

            currentIndex = 0;
            timeSinceLastPath = 0f;
        }

        //attack

        private AiMode DetermineAttackType() => Utils.RollChance(GetPlayerStrength()) ? AiMode.Attack : AiMode.PassingAttack;

        private float GetPlayerStrength()
        {
            float playerStrength = 100;

            if(GameManager.GetConditionComponent().GetNormalizedCondition() <= 55f) playerStrength -= 25f;
            if (GameManager.GetFreezingComponent().GetFreezingLevel() == FreezingLevel.Freezing) playerStrength -= 10f;
            if (GameManager.GetFatigueComponent().GetFatigueLevel() <= FatigueLevel.Tired) playerStrength -= 30f;
            if (GameManager.GetHungerComponent().GetHungerLevel() <= HungerLevel.VeryHungry) playerStrength -= 10f;
            if (GameManager.GetEncumberComponent().IsEncumbered()) playerStrength -= 35f;

            return playerStrength;
        }

        private void MaybeReactToGunshot()
        {

            if (mBaseAi.GetAiMode() == AiMode.Attack || mBaseAi.GetAiMode() == AiMode.Struggle || mBaseAi.GetAiMode() == (AiMode)CustomCougarAiMode.Hide) return;

            float currentDistance = Vector3.Distance(mBaseAi.transform.position, GameManager.GetPlayerTransform().position);


            if (Main.CustomCougarManager.gunFired && currentDistance <= 200) //this is a random value, I think it's close enough for it to take effect
            {
                Main.Logger.Log("Player has shot gun.", ComplexLogger.FlaggedLoggingLevel.Debug);
                if (IsPlayerFacingCougar(GameManager.GetPlayerTransform(), mBaseAi.transform))
                {
                    Main.Logger.Log("Player is facing cougar and shot gun. Cougar is scared shitless.", ComplexLogger.FlaggedLoggingLevel.Debug);
                    mBaseAi.SetAiMode(AiMode.Flee);
                    Main.CustomCougarManager.gunFired = false;
                }
            }
        }

        //misc
        float GetCougarHeight(Transform cougar)
        {
            Renderer[] renderers = cougar.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return 0f;

            Bounds combinedBounds = renderers[0].bounds;
            foreach (Renderer r in renderers)
                combinedBounds.Encapsulate(r.bounds);

            return combinedBounds.size.y;
        }
        private void TeleportCougarToPosition(Vector3 pos, Transform cougar)
        {

            mBaseAi.GetMoveAgent().enabled = false;

            // Move the cougar instantly
            cougar.position = pos;

            // Re-enable the NavMeshAgent
            mBaseAi.GetMoveAgent().enabled = true;

            if (toTeleport) toTeleport = false;

            Main.Logger.Log($"Teleporting cougar to position {pos.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);

        }
        private void ToggleInvisibility()
        {

            GameObject cougar = mBaseAi.gameObject.transform.GetChild(0).gameObject;

            if (cougar == null)
            {
                Main.Logger.Log("Cougar rig is null...", ComplexLogger.FlaggedLoggingLevel.Error);
                return;
            }

            if (isInvisible)
            {
                Main.Logger.Log("No more invisible...", ComplexLogger.FlaggedLoggingLevel.Error);
                cougar.active = true;
                isInvisible = false;
            }
            else
            {
                Main.Logger.Log("I am now invisible!!!", ComplexLogger.FlaggedLoggingLevel.Error);
                cougar.active = false;
                isInvisible = true;
            }

        }

        //wander path stuff
        private bool TryGetSavedWanderPath(SpawnModDataProxy proxy)
        {
            mFetchingWanderPath = true;
            if (proxy == null
                || proxy.CustomData == null
                || proxy.CustomData.Length == 0)
            {
                Main.Logger.Log($"Null proxy, null proxy custom data or no length to proxy custom data", ComplexLogger.FlaggedLoggingLevel.Debug);
                return false;
            }
            Guid spotGuid = new Guid((string)proxy.CustomData[0]);
            if (spotGuid == Guid.Empty)
            {
                Main.Logger.Log($"Proxy spot guid is empty", ComplexLogger.FlaggedLoggingLevel.Debug);
                return false;
            }
            mManager.DataManager.ScheduleMapDataRequest<WanderPath>(new GetDataByGuidRequest<WanderPath>(spotGuid, proxy.Scene, (spot, result) =>
            {
                if (result != RequestResult.Succeeded)
                {
                    Main.Logger.Log($"Can't get WanderPath with guid <<<{spotGuid}>>> from dictionary, requesting nearest instead...", ComplexLogger.FlaggedLoggingLevel.Debug);
                    mManager.DataManager.ScheduleMapDataRequest<WanderPath>(new GetNearestMapDataRequest<WanderPath>(mBaseAi.transform.position, proxy.Scene, (nearestSpot, result2) =>
                    {
                        Main.Logger.Log($"Found new nearest WanderPath with guid <<<{nearestSpot}>>>", ComplexLogger.FlaggedLoggingLevel.Debug);
                        AttachWanderPath(nearestSpot);
                    }, false, CheckIfWanderPathIsForCougar, 3));
                }
                else
                {
                    Main.Logger.Log($"Found saved WanderPath with guid <<<{spotGuid}>>>", ComplexLogger.FlaggedLoggingLevel.Debug);
                    AttachWanderPath(spot);
                }
            }, false));
            return true;
        }

        public void AttachWanderPath(WanderPath path)
        {
            wanderPath = path;
            if (mModDataProxy != null)
            {
                mModDataProxy.CustomData = [path.Guid.ToString()];
            }
            mBaseAi.m_Waypoints = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<Vector3>(wanderPath.PathPoints.Length);
            for (int i = 0, iMax = mBaseAi.m_Waypoints.Length; i < iMax; i++)
            {
                mBaseAi.m_Waypoints[i] = wanderPath.PathPoints[i];
            }
            WanderPathConnected = true;
            path.Claim();
        }

        public Vector3 GetNearestPointOnPath(WanderPath path, Vector3 cougarPosition)
        {

            Vector3 closestPoint = path.PathPoints[0];
            float closestSqrDist = (closestPoint - cougarPosition).sqrMagnitude;

            for (int i = 1; i < path.PathPoints.Count(); i++)
            {
                float sqrDist = (path.PathPoints[i] - cougarPosition).sqrMagnitude;
                if (sqrDist < closestSqrDist)
                {
                    closestSqrDist = sqrDist;
                    closestPoint = path.PathPoints[i];
                }
            }

            return closestPoint;
        }

        //audio

        private void MaybePlayCougarAudio()
        {

            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;
            Random rand = new Random();

            float currentDistance = Vector3.Distance(cougar.position, player.position);

            if(GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() >= timeForNextAudio)
            {
                if (mBaseAi.GetAiMode() == AiMode.Wander || mBaseAi.GetAiMode() == AiMode.FollowWaypoints)
                {
                    if (currentDistance >= 250f) //temporary testing value
                    {

                        int i = rand.Next(0, 2);
                        PlayCougarAudio(i);

                    }
                }
                timeForNextAudio = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + rand.Next(minHoursBetweenAudio, maxHoursBetweenAudio);
            }

        }

        private void PlayCougarAudio(int index)
        {
            Clip clip = Main.cougarAudioManager.GetClipAtIndex(index);
            audio.AssignClip(clip);
            audio.Play();
        }

        //overrides

        protected override bool EnterAiModeCustom(AiMode mode)
        {
            switch (mode)
            {
                case (AiMode)CustomCougarAiMode.Hide:
                    BeginHiding();
                    return false;
                case AiMode.Stalking:
                    BeginStalking();
                    return false;
                case (AiMode)CustomCougarAiMode.Freeze:
                    BeginFreezing();
                    return false;
                case (AiMode)CustomCougarAiMode.Retreat:
                    BeginRetreating();
                    return false;
                case AiMode.Attack:
                    BeginAttacking();
                    return false;                    
                default:
                    return true;
            }
        }

        protected override bool ExitAiModeCustom(AiMode mode)
        {
            switch (mode)
            {
                case (AiMode)CustomCougarAiMode.Hide:
                    StopHiding();
                    return false;
                default:
                    return true;
            }
        }

        protected override bool GetAiAnimationStateCustom(AiMode mode, out AiAnimationState overrideState)
        {
            switch (mode)
            {
                case (AiMode)CustomCougarAiMode.Hide:
                    overrideState = AiAnimationState.Stalking;
                    return false;
                case (AiMode)CustomCougarAiMode.Retreat:
                    overrideState = AiAnimationState.Stalking;
                    return false;
                case (AiMode)CustomCougarAiMode.Freeze:
                    overrideState = AiAnimationState.Stalking;
                    return false;
                default:
                    overrideState = AiAnimationState.Invalid;
                    return true;
            }
        }

        protected override bool IsMoveStateCustom(AiMode mode, out bool isMoveState)
        {
            switch (mode)
            {
                case (AiMode)CustomCougarAiMode.Freeze:
                    isMoveState = false;
                    return false;
                case (AiMode)CustomCougarAiMode.Retreat:
                    isMoveState = true;
                    return false;
                case (AiMode)CustomCougarAiMode.Hide:
                    isMoveState = false;
                    return false;
                default:
                    isMoveState = false;
                    return base.IsMoveStateCustom(mode, out isMoveState);
            }
        }

        //ensures the cougar never goes into imposter mode, ever
        protected override bool TestIsImposterCustom(out bool isImposter)
        {
            isImposter = false;
            return false;
        }

        //debug
        private void DoOnUpdate()
        {

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.RightArrow))
            {
                Main.Logger.Log("Activating wander path mode!", ComplexLogger.FlaggedLoggingLevel.Debug);
                StartFollowWanderPath();
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.LeftArrow))
            {
                Main.Logger.Log("Playing cougar audio clip", ComplexLogger.FlaggedLoggingLevel.Debug);
                PlayCougarAudio(0);
            }

        }

    }
}
