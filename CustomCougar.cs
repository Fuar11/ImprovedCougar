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

                    //change state here  
                    SetAiMode((AiMode)CustomCougarAiMode.Hide);
                }
            }
            else
            {
               
            }
        }

        protected void ProcessHiding()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            //check player distance, if it gets too close to a hiding cougar it can despawn and move to a more advantageous position

            //Main.Logger.Log("Cougar is hiding! Sneaky boi", ComplexLogger.FlaggedLoggingLevel.Debug);

            if (!IsPlayerFacingCougar(player, cougar)) //this works
            {
                //player is NOT looking at cougar

                Main.Logger.Log("Cougar is hiding and player is not looking. Continuing to stalk...", ComplexLogger.FlaggedLoggingLevel.Debug);
                //cougar can come out and keep pursuing the player

                //change state here  
                SetAiMode(AiMode.Stalking);

            }
        }

        protected void BeginStalking()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            //do something to make the Cougar shut up here

            mBaseAi.MoveAgentStop();
            mBaseAi.m_CurrentTarget = GameManager.GetPlayerManagerComponent().m_AiTarget;
            mBaseAi.StartPath(player.position, mBaseAi.m_StalkSpeed);
            Main.Logger.Log("Cougar is stalking player", ComplexLogger.FlaggedLoggingLevel.Debug);
            InterfaceManager.GetPanel<Panel_BodyHarvest>().DisplayErrorMessage("Cougar is stalking player!");
        }

        protected void BeginHiding()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            Main.Logger.Log("Cougar is hiding", ComplexLogger.FlaggedLoggingLevel.Debug);

            mBaseAi.MoveAgentStop();
            //mBaseAi.ClearTarget(); 

            Vector3? coverPosition = FindNearestTrueCover(cougar, player, 80f, Utils.m_PhysicalCollisionLayerMask);

            if (coverPosition != null)
            {

                Main.Logger.Log($"Cover position found: {coverPosition.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);

                mBaseAi.StartPath((Vector3)coverPosition, mBaseAi.m_ChasePlayerSpeed);
                Main.Logger.Log("Moving towards cover", ComplexLogger.FlaggedLoggingLevel.Debug);
                InterfaceManager.GetPanel<Panel_BodyHarvest>().DisplayErrorMessage("Cougar is moving to cover!");
            }
            else Main.Logger.Log("Cannot find cover position", ComplexLogger.FlaggedLoggingLevel.Error);
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

        //this method doesn't work too well. The cougar will go to cover but it won't go behind cover relative to the player. It'll usually bash it's head into a tree
        public Vector3? FindNearestCover(Transform cougar, Transform player, float searchRadius, LayerMask coverObstructionMask)
        {
            HashSet<Collider> cougarColliders = new HashSet<Collider>(cougar.GetComponentsInChildren<Collider>());
            Collider[] nearbyObjects = Physics.OverlapSphere(cougar.position, searchRadius, coverObstructionMask);
            Vector3 cougarEye = mBaseAi.GetEyePos();
            Vector3 playerEye = mBaseAi.m_CurrentTarget.GetEyePos();
            float cougarHeight = GetCougarHeight(cougar);

            Vector3? bestPosition = null;
            float closestDistance = Mathf.Infinity;

            Main.Logger.Log($"Cougar position: {cougar.position.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);

            foreach (Collider col in nearbyObjects)
            {
                // Get the closest point on this collider to the cougar

                if (cougarColliders.Contains(col))
                    continue;

                if (col.bounds.Contains(cougar.position))
                    continue;

                Vector3 coverPoint = col.ClosestPoint(cougar.position);

                Main.Logger.Log($"Cover point {coverPoint.ToString()} found", ComplexLogger.FlaggedLoggingLevel.Debug);

                if (Vector3.Distance(coverPoint, cougar.position) < 1.0f)
                    continue;

                //Main.Logger.Log("Cover is far enough from cougar", ComplexLogger.FlaggedLoggingLevel.Debug);

                if (col.bounds.size.y < cougarHeight) continue;

                //Main.Logger.Log("Cover is high enough for cougar", ComplexLogger.FlaggedLoggingLevel.Debug);

                //check if position is behind cougar or not
                Vector3 dirToCover = (coverPoint - cougar.position).normalized;
                float dot = Vector3.Dot(cougar.forward, dirToCover);
                if (dot < 0.3f)
                    continue;

                //Main.Logger.Log("Cover is in front of cougar", ComplexLogger.FlaggedLoggingLevel.Debug);

                // Test if the player can see that point
                Vector3 dirToCoverFromPlayer = coverPoint - playerEye;
                float distToCover = dirToCoverFromPlayer.magnitude;

                // If the first hit is this collider, it blocks line of sight
                if (Physics.Raycast(playerEye, dirToCoverFromPlayer.normalized, out RaycastHit hit, distToCover, coverObstructionMask))
                {

                    //Main.Logger.Log("Raycast", ComplexLogger.FlaggedLoggingLevel.Debug);

                    if (hit.collider == col)
                    {

                        //Main.Logger.Log("Raycast hit", ComplexLogger.FlaggedLoggingLevel.Debug);

                        float distToCougar = Vector3.Distance(cougar.position, coverPoint);
                        if (distToCougar < closestDistance)
                        {
                            closestDistance = distToCougar;
                            bestPosition = coverPoint;
                        }
                    }
                }
            }

            return bestPosition; // Returns null if nothing found
        }

        //this method works a lot better. The cougar will go to the opposite side of the cover object and truly hide from the player. But it'll often go backwards to do it which looks a little goofy
        public Vector3? FindNearestTrueCover(Transform cougar, Transform player, float searchRadius, LayerMask coverObstructionMask)
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
                // Get a point behind the collider (relative to the player)
                Vector3 dirFromPlayer = (bestCollider.bounds.center - playerEye).normalized;
                Vector3 coverPoint = bestCollider.bounds.center + dirFromPlayer * bestCollider.bounds.extents.magnitude * 0.9f;

                return coverPoint;
            }

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
