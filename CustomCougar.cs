using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;
using ExpandedAiFramework;
using Il2CppTLD.AI;
using UnityEngine;
using Il2CppRewired;
using Il2CppTMPro;
using UnityEngine.AI;
using ImprovedCougar.Pathfinding;
using System.Drawing;
using Color = UnityEngine.Color;
using static Il2Cpp.UIAtlas;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Tilemaps;
using Il2CppSystem.Xml;

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

        //this will be eventually be used for the cougar to wander outside of it's territory
        protected WanderPath m_wanderPath;
        protected override float m_MinWaypointDistance { get { return 100.0f; } }
        protected override float m_MaxWaypointDistance { get { return 1000.0f; } }

        //territory
        private bool isInTerritory = true;

        //speeds
        private float currentStalkSpeed = 0f;

        private float baseStalkSpeed = 0f;
        private float closeStalkSpeed = 0f;
        private float spottedStalkSpeed = 0f;
        private float attackSpeed = 0f;

        //distances
        private const float attackDistance = 20f; //for now
        private const float maxStalkDistance = 100f; //for now

        //states
        private bool toTeleport = false;
        private bool toHide = false;
        private bool toFreeze = false;
        private bool isInvisible = false;

        //pathfinding
        List<Vector3> path;
        int currentIndex = 0;
        float reachThreshold = 0.5f;
        bool pathBroken = false;

        Vector3 spawnPosition = Vector3.zero; //temporary

        //time
        float timeSinceLastPath = 0f;
        float recalcInterval = 1.5f;

        float timeSinceHiding = 0f;
        float timeToComeOut = 5f;

        float timeSinceFreezing = 0f;

        public override void Initialize(BaseAi ai, TimeOfDay timeOfDay, SpawnRegion spawnRegion)
        {
            base.Initialize(ai, timeOfDay, spawnRegion);
            mBaseAi.m_DefaultMode = AiMode.Wander;
            mBaseAi.m_StartMode = AiMode.Wander;
            baseStalkSpeed = mBaseAi.m_StalkSpeed + 2;
            closeStalkSpeed = mBaseAi.m_StalkSpeed;
            spottedStalkSpeed = mBaseAi.m_ChasePlayerSpeed;
            attackSpeed = Settings.CustomSettings.settings.cougarSpeed; //i think this will work idk
            //this is temporary
            spawnPosition = mBaseAi.transform.position;

            Main.Logger.Log("Cougar initialized", ComplexLogger.FlaggedLoggingLevel.Debug);
        }

        protected override bool ProcessCustom()
        {

            //Main.Logger.Log("Processing custom cougar logic", ComplexLogger.FlaggedLoggingLevel.Debug);

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

        protected void ProcessStalkingAdvanced()
        {

            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;
            Vector3 playerPosition = player.position;
            Vector3 currentCougarPosition = cougar.position;
            float currentDistance = Vector3.Distance(currentCougarPosition, playerPosition);

            if (currentDistance >= attackDistance)
            {

                timeSinceLastPath += Time.deltaTime;
                if (timeSinceLastPath > recalcInterval || pathBroken)
                {
                    Main.Logger.Log("Calculating path...", ComplexLogger.FlaggedLoggingLevel.Debug);
                    StartStalkingPathfinding();
                }

                Main.Logger.Log($"Cougar Distance: {currentDistance}", ComplexLogger.FlaggedLoggingLevel.Debug);

                if (CougarCanSeePlayerLooking(player, cougar))
                {

                    if (currentDistance >= 35f)
                    {
                        Main.Logger.Log("Cougar can see player looking, running faster to cover.", ComplexLogger.FlaggedLoggingLevel.Debug);
                        currentStalkSpeed = spottedStalkSpeed;
                    }
                    else //change state to freeze & flee?
                    {
                        SetAiMode((AiMode)CustomCougarAiMode.Freeze);
                    }
                }

                if (path != null && currentIndex < path.Count)
                {

                    Vector3 target = path[currentIndex];

                    //Main.Logger.Log($"Moving to position: {target.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);

                    mBaseAi.StartPath(target, currentStalkSpeed);

                    if (!mBaseAi.CanPathfindToPosition(target))
                    {
                        TeleportCougarToPosition(target, cougar);
                    }

                    if (Vector3.Distance(cougar.position, target) < reachThreshold)
                    {
                        currentStalkSpeed = baseStalkSpeed;
                        if (toHide && currentIndex != 0)
                        {
                            SetAiMode((AiMode)CustomCougarAiMode.Hide);
                        }
                        currentIndex++;
                    }
                    else //still moving towards node
                    {
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
                        }

                    }
                }
                else
                {
                    //if no path found it probably means there is no cover nearby, so it isn't stealthy enough to keep stalking. If close enough keep going 

                    //i can probably change this to detect for allowed areas or something

                    Main.Logger.Log("Cougar is in open area, maybe!", ComplexLogger.FlaggedLoggingLevel.Debug);

                    if (currentDistance <= 35)
                    {
                        mBaseAi.StartPath(playerPosition, currentStalkSpeed);
                    }
                    else
                    {
                        mBaseAi.SetAiMode((AiMode)CustomCougarAiMode.Retreat);
                    }


                }
            }
            else
            {
                //AiModeSet to attack! or something like that
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
            else
            {
                //make the cougar invisible! this use case is most likely to involve the player walking backwards, or moving towards the hiding cougar
            }
        }

        protected void ProcessFreezing()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            //wait a few seconds to flee

            timeSinceFreezing += Time.deltaTime;
            if (timeSinceFreezing > 5f)
            {

                if (IsPlayerFacingCougar(player, cougar))
                {
                    Main.Logger.Log("Cougar is freezing for too long and player is looking. Gonna flee...", ComplexLogger.FlaggedLoggingLevel.Debug);

                    //change state here  
                    SetAiMode((AiMode)CustomCougarAiMode.Retreat);
                }
                else
                {
                    Main.Logger.Log("Cougar is freezing for too long and player is not looking. Back to stalking flee...", ComplexLogger.FlaggedLoggingLevel.Debug);
                    SetAiMode(AiMode.Stalking);
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

                mBaseAi.StartPath(spawnPosition, currentStalkSpeed);

                if (Vector3.Distance(cougar.position, spawnPosition) < reachThreshold)
                {
                    Main.Logger.Log("Cougar has reached it's territory, going back to wandering", ComplexLogger.FlaggedLoggingLevel.Debug);
                    mBaseAi.SetAiMode(AiMode.Wander);
                }
            
        }

        protected void BeginStalking()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            //do something to make the Cougar shut up here

            mBaseAi.MoveAgentStop();
            mBaseAi.m_CurrentTarget = GameManager.GetPlayerManagerComponent().m_AiTarget;
            currentStalkSpeed = baseStalkSpeed;
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
            currentStalkSpeed = baseStalkSpeed;
            Main.Logger.Log("Cougar is retreating!", ComplexLogger.FlaggedLoggingLevel.Debug);
        }

        //checks to see if the player is facing the cougar regardless of line of sight
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

            if(cougar == null)
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

        //overrides

        protected override bool EnterAiModeCustom(AiMode mode)
        {
            switch (mode)
            {
                case (AiMode)CustomCougarAiMode.Hide:
                    LogVerbose($"EnterAiModeCustom: mode is {mode}, routing to BeginHiding");
                    BeginHiding();
                    return false;
                case AiMode.Stalking:
                    LogVerbose($"EnterAiModeCustom: mode is {mode}, routing to BeginStalking");
                    BeginStalking();
                    return false;
                case (AiMode)CustomCougarAiMode.Freeze:
                    LogVerbose($"EnterAiModeCustom: mode is {mode}, routing to BeginFreezing");
                    BeginFreezing();
                    return false;
                case (AiMode)CustomCougarAiMode.Retreat:
                    LogVerbose($"EnterAiModeCustom: mode is {mode}, routing to BeginRetreating");
                    BeginRetreating();
                    return false;
                default:
                    LogVerbose($"EnterAiModeCustom: mode is {mode}, deferring.");
                    return true;
            }
        }

        protected override bool ExitAiModeCustom(AiMode mode)
        {
            switch (mode)
            {
                case (AiMode)CustomCougarAiMode.Hide:
                    LogVerbose($"ExitAiModeCustom: mode is {mode}, routing to StopHiding");
                    StopHiding();
                    return false;
                default:
                    LogVerbose($"ExitAiModeCustom: mode is {mode}, deferring.");
                    return true;
            }
        }

        protected override bool GetAiAnimationStateCustom(AiMode mode, out AiAnimationState overrideState)
        {
            switch (mode)
            {
                case (AiMode)CustomCougarAiMode.Hide:
                    LogVerbose($"GetAiAnimationStateCustom: mode is {mode}, setting overrideState to Stalking.");
                    overrideState = AiAnimationState.Stalking;
                    return false;
                case (AiMode)CustomCougarAiMode.Retreat:
                    LogVerbose($"GetAiAnimationStateCustom: mode is {mode}, setting overrideState to Stalking.");
                    overrideState = AiAnimationState.Stalking;
                    return false;
                case (AiMode)CustomCougarAiMode.Freeze:
                    LogVerbose($"GetAiAnimationStateCustom: mode is {mode}, setting overrideState to Stalking.");
                    overrideState = AiAnimationState.Stalking;
                    return false;
                default:
                    LogVerbose($"GetAiAnimationStateCustom: mode is {mode}, deffering");
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


    }
}
