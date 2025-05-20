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
    internal class CustomCougar : BaseCougar
    {

        internal static BaseCougarSettings CustomCougarSettings = new BaseCougarSettings();

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

            Main.Logger.Log("Processing custom cougar logic", ComplexLogger.FlaggedLoggingLevel.Debug);


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

            Main.Logger.Log("Cougar is stalking player, processing...", ComplexLogger.FlaggedLoggingLevel.Debug);

            if (AiUtils.PositionVisible(mBaseAi.GetEyePos(), cougar.forward, mBaseAi.m_CurrentTarget.GetEyePos(), 100f, mBaseAi.m_DetectionFOV, 0f, Utils.m_PhysicalCollisionLayerMask))
            {
                Main.Logger.Log("Cougar can see player. Getting to cover.", ComplexLogger.FlaggedLoggingLevel.Debug);
                InterfaceManager.GetPanel<Panel_BodyHarvest>().DisplayErrorMessage("Cougar has seen player!");
                //cougar needs to quickly get into cover to hide

                //change state here  
                SetAiMode((AiMode)CustomCougarAiMode.Hide);
            }
        }

        protected void ProcessHiding()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            //check player distance, if it gets too close to a hiding cougar it can despawn and move to a more advantageous position

            if (!AiUtils.PositionVisible(mBaseAi.GetEyePos(), cougar.forward, mBaseAi.m_CurrentTarget.GetEyePos(), 100f, mBaseAi.m_DetectionFOV, 0f, Utils.m_PhysicalCollisionLayerMask))
            {
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
            mBaseAi.ClearTarget();
            mBaseAi.StartPath(player.transform.position, mBaseAi.m_StalkSpeed);
            Main.Logger.Log("Cougar is stalking player", ComplexLogger.FlaggedLoggingLevel.Debug);
            InterfaceManager.GetPanel<Panel_BodyHarvest>().DisplayErrorMessage("Cougar is stalking player!");
        }

        protected void BeginHiding()
        {
            Transform player = GameManager.GetPlayerTransform();
            Transform cougar = mBaseAi.transform;

            mBaseAi.MoveAgentStop();
            mBaseAi.ClearTarget();

            Vector3? coverPosition = FindNearestCover(cougar, player, 30f, Utils.m_PhysicalCollisionLayerMask);

            if (coverPosition != null)
            {
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

        public Vector3? FindNearestCover(Transform cougar, Transform player, float searchRadius, LayerMask coverObstructionMask)
        {
            Collider[] nearbyObjects = Physics.OverlapSphere(cougar.position, searchRadius, coverObstructionMask);
            Vector3 cougarEye = mBaseAi.GetEyePos();
            Vector3 playerEye = mBaseAi.m_CurrentTarget.GetEyePos();

            Vector3? bestPosition = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider col in nearbyObjects)
            {
                // Get the closest point on this object to the cougar
                Vector3 coverPoint = col.ClosestPoint(cougar.position);

                // Test if the player can see that point
                Vector3 dirToCover = coverPoint - playerEye;
                float distToCover = dirToCover.magnitude;

                // If the first hit is this collider, it blocks line of sight
                if (Physics.Raycast(playerEye, dirToCover.normalized, out RaycastHit hit, distToCover, coverObstructionMask))
                {
                    if (hit.collider == col)
                    {
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

        protected override bool EnterAiModeCustom(AiMode mode)
        {
            switch (mode)
            {
                case (AiMode)CustomCougarAiMode.Hide:
                    LogVerbose($"EnterAiModeCustom: mode is {mode}, routing to BeginHiding");
                    BeginHiding();
                    return false;
                default:
                    LogVerbose($"EnterAiModeCustom: mode is {mode}, deferring.");
                    return true;
            }
        }


        protected override bool GetAiAnimationStateCustom(AiMode mode, out AiAnimationState overrideState)
        {
            switch (mode)
            {
                case (AiMode)CustomCougarAiMode.Hide:
                    LogVerbose($"GetAiAnimationStateCustom: mode is {mode}, setting overrideState to Paused.");
                    overrideState = AiAnimationState.Paused;
                    return false;
                default:
                    LogVerbose($"GetAiAnimationStateCustom: mode is {mode}, deffering");
                    overrideState = AiAnimationState.Invalid;
                    return true;
            }
        }


    }
}
