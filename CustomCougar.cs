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

namespace ImprovedCougar
{
    [RegisterTypeInIl2Cpp]
    internal class CustomCougar : BaseCougar
    {

        internal static Settings.Settings CustomCougarSettings = new Settings.Settings();
        private enum CustomCougarAiMode : int
        {
            Hide = AiMode.Disabled + 1,
            COUNT
        }

        public CustomCougar(IntPtr ptr) : base(ptr) { }

        //this will be eventually be used for the cougar to wander outside of it's territory
        protected WanderPath m_wanderPath;
        protected override float m_MinWaypointDistance { get { return 100.0f; } }
        protected override float m_MaxWaypointDistance { get { return 1000.0f; } }

        private const float followDistance = 10f; //for now

        private Vector3? coverPosition = null;

        public override void Initialize(BaseAi ai, TimeOfDay timeOfDay, SpawnRegion spawnRegion)
        {
            base.Initialize(ai, timeOfDay, spawnRegion);
            mBaseAi.m_DefaultMode = AiMode.Wander;
            mBaseAi.m_StartMode = AiMode.Wander;
            Main.Logger.Log("Cougar initialized", ComplexLogger.FlaggedLoggingLevel.Debug);
        }

        protected override bool ProcessCustom()
        {

            //Main.Logger.Log("Processing custom cougar logic", ComplexLogger.FlaggedLoggingLevel.Debug);

            switch (CurrentMode)
            {
                case AiMode.Stalking:
                    ProcessStalkingCustom();
                    return false;
                case (AiMode)CustomCougarAiMode.Hide:
                    ProcessHiding();
                    return false;
                default:
                    return base.ProcessCustom();
            }
        }

        protected void ProcessStalkingCustom()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            //Main.Logger.Log("Cougar is stalking player, processing...", ComplexLogger.FlaggedLoggingLevel.Debug);


            if (IsPlayerFacingCougar(player, cougar)) //this works
            {
                //player is looking at cougar

                if (AiUtils.PositionVisible(mBaseAi.GetEyePos(), cougar.forward, mBaseAi.m_CurrentTarget.GetEyePos(), 100f, mBaseAi.m_DetectionFOV, 0f, Utils.m_PhysicalCollisionLayerMask))
                {
                    Main.Logger.Log("Cougar can see player and player is looking. Getting to cover.", ComplexLogger.FlaggedLoggingLevel.Debug);
                    InterfaceManager.GetPanel<Panel_BodyHarvest>().DisplayErrorMessage("Cougar can see player looking!");
                    //cougar needs to quickly get into cover to hide

                    coverPosition = FindNearestCover(cougar, player, 80f, Utils.m_PhysicalCollisionLayerMask);

                    if (coverPosition != null)
                    {
                        Main.Logger.Log($"Cover position found: {coverPosition.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);

                        //change state here  
                        SetAiMode((AiMode)CustomCougarAiMode.Hide);
                    }
                    else Main.Logger.Log("Cannot find cover position. Continue stalking...", ComplexLogger.FlaggedLoggingLevel.Error);
                }
            }
            else
            {

                Vector3 playerPosition = player.position; //uses curr player position
                Vector3 currentCougarPosition = cougar.position;
                float currentDistance = Vector3.Distance(currentCougarPosition, playerPosition);

                if (currentDistance >= followDistance)
                {

                    Vector3 followDirection = (currentCougarPosition - playerPosition).normalized;
                    Vector3 rawFollowPosition = playerPosition + followDirection * (followDistance * 0.80f);
                    AiUtils.GetClosestNavmeshPos(out Vector3 followPosition, rawFollowPosition, rawFollowPosition);

                    //maybe check if it can reach the player here

                    //keep following player
                    mBaseAi.StartPath(player.position, mBaseAi.m_StalkSpeed);

                }
                else
                {
                    Main.Logger.Log("Cougar has reached attack distance", ComplexLogger.FlaggedLoggingLevel.Debug);
                    //set mode to attack and go nuts
                }
            }
        }

        protected void ProcessHiding()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            //check player distance, if it gets too close to a hiding cougar it can despawn and move to a more advantageous position

            if (cougar.position == coverPosition) mBaseAi.MoveAgentStop();

            if (!IsPlayerFacingCougar(player, cougar)) //this works
            {
                //player is NOT looking at cougar

                Main.Logger.Log("Cougar is hiding and player is not looking. Continuing to stalk...", ComplexLogger.FlaggedLoggingLevel.Debug);
                //cougar can come out and keep pursuing the player

                //change state here  
                SetAiMode(AiMode.Stalking);

            }
            //else we need something that prevents the player from trapping the cougar behind cover indefinitely, teleport maybe?
        }

        protected void BeginStalking()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            //do something to make the Cougar shut up here

            mBaseAi.MoveAgentStop();
            mBaseAi.m_CurrentTarget = GameManager.GetPlayerManagerComponent().m_AiTarget;
            Main.Logger.Log("Cougar is stalking player", ComplexLogger.FlaggedLoggingLevel.Debug);
            InterfaceManager.GetPanel<Panel_BodyHarvest>().DisplayErrorMessage("Cougar is stalking player!");
        }

        protected void BeginHiding()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            mBaseAi.MoveAgentStop();
            //mBaseAi.ClearTarget(); 

            //if we get here coverPosition MUST be true
            mBaseAi.StartPath((Vector3)coverPosition, mBaseAi.m_ChasePlayerSpeed);
            Main.Logger.Log("Moving towards cover", ComplexLogger.FlaggedLoggingLevel.Debug);
            InterfaceManager.GetPanel<Panel_BodyHarvest>().DisplayErrorMessage("Cougar is moving to cover!");

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



        //this method works a lot better. The cougar will go to the opposite side of the cover object and truly hide from the player. But it'll often go backwards to do it which looks a little goofy
        public Vector3? FindNearestCover(Transform cougar, Transform player, float searchRadius, LayerMask coverObstructionMask)
        {
            HashSet<Collider> cougarColliders = new HashSet<Collider>(cougar.GetComponentsInChildren<Collider>());
            Collider[] nearbyObjects = Physics.OverlapSphere(cougar.position, searchRadius, coverObstructionMask);
            Vector3 cougarEye = mBaseAi.GetEyePos();
            Vector3 playerEye = mBaseAi.m_CurrentTarget.GetEyePos();
            float cougarHeight = GetCougarHeight(cougar);

            Collider bestCollider = null;
            float closestDistance = Mathf.Infinity;

            Main.Logger.Log($"Cougar position: {cougar.position.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);

            foreach (Collider col in nearbyObjects)
            {
                // Get the closest collider to the cougar

                if (cougarColliders.Contains(col))
                    continue;

                if (col.bounds.Contains(cougar.position))
                    continue;

                if (col.bounds.size.y < cougarHeight) continue;


                Vector3 dirToCollider = (col.bounds.center - playerEye).normalized;
                float distanceToCollider = Vector3.Distance(playerEye, col.bounds.center);

                if (Physics.Raycast(playerEye, dirToCollider, out RaycastHit hit, distanceToCollider, coverObstructionMask))
                {
                    if (hit.collider == col)
                    {
                        float distanceToCougar = Vector3.Distance(cougar.position, col.bounds.center);
                        if (distanceToCougar < closestDistance)
                        {
                            closestDistance = distanceToCougar;
                            bestCollider = col;
                        }
                    }
                }
            }

            if (bestCollider != null)
            {

                DebugTools.HighlightColliderBounds(bestCollider);

                // Get a point behind the collider (relative to the player)
                Vector3 dirFromPlayer = (bestCollider.bounds.center - playerEye).normalized;
                Vector3 initialCoverPoint = bestCollider.bounds.center + dirFromPlayer * bestCollider.bounds.extents.magnitude * 0.25f;

                DebugTools.CreateDebugMarker(initialCoverPoint, Color.red);

                Vector3? finalCoverPoint = ForcePointToGround(initialCoverPoint, bestCollider, 25f, 2f, coverObstructionMask);

                return finalCoverPoint != initialCoverPoint ? finalCoverPoint : initialCoverPoint;

                /***

                //checks to see if it can lower the point to the ground, since sometimes it's too high for the cougar to reach
                if (Physics.Raycast(initialCoverPoint, Vector3.down, out RaycastHit groundHit, 10f))
                {
                    Vector3 groundedPoint = groundHit.point;
                    Main.Logger.Log($"To ground raycast hit! Found point {groundedPoint.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);
                    DebugTools.CreateDebugMarker(groundedPoint, Color.green);
                    return groundedPoint;
                }
                else
                {

                    Vector3 forcedDownPoint = initialCoverPoint;
                    forcedDownPoint.y -= 15.0f; // Drop it by a lot to get below any branches and other blocking objects

                    DebugTools.CreateDebugMarker(forcedDownPoint, Color.magenta);

                    if (Physics.Raycast(forcedDownPoint, Vector3.down, out RaycastHit groundHit2, 10f))
                    {
                        Vector3 groundedPoint = groundHit2.point;

                        

                        Main.Logger.Log($"To ground raycast 2 hit! Found point {groundedPoint.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);
                        DebugTools.CreateDebugMarker(groundedPoint, Color.green);
                        return groundedPoint;
                    }
                } ***/

                //return initialCoverPoint;
            }

            return null;
        }

        public Vector3? ForcePointToGround(Vector3 startPoint, Collider excludedCollider, float maxDrop = 10f, float stepSize = 0.5f, LayerMask groundMask = default)
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
                        Main.Logger.Log($"To ground raycast hit! Found point {hit.point.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);
                        DebugTools.CreateDebugMarker(hit.point, Color.green);
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
                default:
                    LogVerbose($"EnterAiModeCustom: mode is {mode}, deferring.");
                    return true;
            }
        }

        protected override bool IsMoveStateCustom(AiMode mode, out bool isMoveState)
        {
            switch (mode)
            {
                case (AiMode)CustomCougarAiMode.Hide:
                    LogVerbose($"IsMoveStateCustom: mode is {mode}, setting isMoveState true.");
                    isMoveState = true;
                    return false;
                default:
                    LogVerbose($"IsMoveStateCustom: mode is {mode}, deferring.");
                    isMoveState = false;
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
                default:
                    LogVerbose($"GetAiAnimationStateCustom: mode is {mode}, deffering");
                    overrideState = AiAnimationState.Invalid;
                    return true;
            }
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



    }
}
