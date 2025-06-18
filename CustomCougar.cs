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

        public CustomCougar(IntPtr ptr) : base(ptr) { }

        //this will be used for the cougar to wander outside of it's territory
        protected WanderPath wanderPath;
        protected bool WanderPathConnected = false;
        protected override float m_MinWaypointDistance { get { return 100.0f; } }
        protected override float m_MaxWaypointDistance { get { return 1000.0f; } }

        //territory
        private bool isInTerritory = true;

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

        public override void Initialize(BaseAi ai, TimeOfDay timeOfDay, SpawnRegion spawnRegion, SpawnModDataProxy proxy)
        {
            base.Initialize(ai, timeOfDay, spawnRegion, proxy);
            mBaseAi.m_DefaultMode = AiMode.Wander;
            mBaseAi.m_StartMode = AiMode.Wander;
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

            Main.Logger.Log("Cougar initialized", ComplexLogger.FlaggedLoggingLevel.Debug);
        }

        protected override bool ProcessCustom()
        {

            //Main.Logger.Log("Processing custom cougar logic", ComplexLogger.FlaggedLoggingLevel.Debug);

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
                    //go back to what it was doing before stalking
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
                        timeToFreezeFor = 15f;
                    }
                    else if (currentDistance <= 70f && currentDistance >= 30)
                    {
                        timeToFreezeFor = 7f;
                    }
                    else timeToFreezeFor = 5f;

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
                    Main.Logger.Log("Cougar is in open area, maybe!", ComplexLogger.FlaggedLoggingLevel.Debug);

                    if (currentDistance <= 35)
                    {
                        mBaseAi.StartPath(playerPosition, currentSpeed);
                    }
                    else
                    {
                        mBaseAi.SetAiMode((AiMode)CustomCougarAiMode.Retreat);
                    } 


                }
            }
            else
            {
                

                //needs obvious work
                Main.Logger.Log("Attacking!", ComplexLogger.FlaggedLoggingLevel.Debug);
                mBaseAi.SetAiMode(AiMode.Attack);
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

            //wait a few seconds to flee

            timeSinceFreezing += Time.deltaTime;
            if (timeSinceFreezing > timeToFreezeFor || stopFreezing) //add some slight movements here, a cougar freezing isn't completely still, maybe, fact check this
            {

                //add more complex behavior decision making here
                if (IsPlayerFacingCougar(player, cougar))
                {
                    Main.Logger.Log("Cougar is freezing for too long and player is looking. Gonna flee...", ComplexLogger.FlaggedLoggingLevel.Debug);

                    //change state here  
                    currentSpeed = spottedRetreatSpeed;
                    retreatPosition = lastPosition != Vector3.zero ? lastPosition : (Vector3)FindRetreatCoverPoint(cougar, player, 50f, Utils.m_PhysicalCollisionLayerMask);
                    SetAiMode((AiMode)CustomCougarAiMode.Retreat);
                }
                else
                {
                    Main.Logger.Log("Cougar is freezing for too long and player is not looking. Teleporting cougar to retreat point and hiding...", ComplexLogger.FlaggedLoggingLevel.Debug);
                    retreatPosition = lastPosition != Vector3.zero ? lastPosition : (Vector3)FindRetreatCoverPoint(cougar, player, 50f, Utils.m_PhysicalCollisionLayerMask);
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
            float pointDistance = Vector3.Distance(cougar.position, spawnPosition);


            //if still in territory fall back not too far and go into observe state or keep stalking
            //else fall back all the way into territory and go into observe state

            if (retreatPosition == Vector3.zero)
            {
                retreatPosition = spawnPosition;
                Main.Logger.Log("Retreat position is invalid. Falling back to spawn location instead", ComplexLogger.FlaggedLoggingLevel.Debug);
            }
            mBaseAi.StartPath(retreatPosition, currentSpeed);

            if (Vector3.Distance(cougar.position, retreatPosition) < reachThreshold)
            {
                if (retreatPosition == spawnPosition)
                {
                    Main.Logger.Log("Cougar has reached it's territory, going back to wandering", ComplexLogger.FlaggedLoggingLevel.Debug);
                    mBaseAi.SetAiMode(AiMode.Stalking); //temporary until we get wander/patrol working
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

            mBaseAi.m_StalkingAudioID = 0; //probably stops the initial growl
            mBaseAi.m_StalkingAudio = "";
            mBaseAi.m_EnterStalkingAudio = "";

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

        protected void SetupFollowPath()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            if (!WanderPathConnected)
            {
                if (!TryGetSavedWanderPath(mModDataProxy))
                {
                    mManager.DataManager.GetNearestWanderPathAsync(mBaseAi.transform.position, WanderPathTypes.IndividualPath, AttachWanderPath, 3);
                }
            }

            Vector3 closestPoint = GetNearestPointOnPath(wanderPath, cougar.position);
            mBaseAi.m_MoveAgent.transform.position = closestPoint;
            mBaseAi.m_MoveAgent.Warp(closestPoint, 2.0f, true, -1);
            currentSpeed = wanderSpeed; //for consistency, i know this is useless
            mBaseAi.m_AiGoalSpeed = wanderSpeed;
            mBaseAi.SetAiMode(AiMode.FollowWaypoints);
            Main.Logger.Log($"Following path {wanderPath.Guid.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);
        }


        protected void PreProcessingFollowPath()
        {


            //this is where you can change out of state, since the main AiMode isn't being overridden

        }

        private void MaybeWanderOutOfTerritory()
        {

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

        private void DoSomethingWithAudio()
        {
            
        }


        //wander path stuff

        private bool TryGetSavedWanderPath(SpawnModDataProxy proxy)
        {
            if (proxy == null)
            {
                this.LogTraceInstanced("Null Proxy, getting new wander path");
                return false;
            }
            if (proxy.CustomData == null)
            {
                this.LogTraceInstanced($"Null custom data on proxy with guid <<<{proxy.Guid}>>>, getting new wander path");
                return false;
            }
            if (proxy.CustomData.Length == 0)
            {
                this.LogTraceInstanced($"Zero-length custom data on proxy with guid <<<{proxy.Guid}>>>, getting new wander path");
                return false;
            }
            Guid spotGuid = new Guid(proxy.CustomData[0]);
            if (spotGuid == Guid.Empty)
            {
                this.LogTraceInstanced($"Empty GUID on proxy with guid <<<{proxy.Guid}>>>, getting new wander path");
                return false;
            }
            if (!mManager.DataManager.AvailableWanderPaths.TryGetValue(spotGuid, out WanderPath wanderPath))
            {
                this.LogTraceInstanced($"Could not fetch WanderPath with guid {spotGuid} from proxy with guid <<{proxy.Guid}>>>, getting new wander path");
                return false;
            }
            AttachWanderPath(wanderPath);
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

        //debug
        private void DoOnUpdate()
        {

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.UpArrow))
            {
                Main.Logger.Log("Activating wander path mode!", ComplexLogger.FlaggedLoggingLevel.Debug);
                SetupFollowPath();
            }

        }

    }
}
