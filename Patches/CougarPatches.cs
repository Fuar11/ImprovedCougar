using Il2CppTLD.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImprovedCougar.Settings;

namespace ImprovedCougar.Patches
{
    internal class CougarPatches
    {

        [HarmonyPatch(nameof(BaseAi), nameof(BaseAi.Awake))]

        public class TweakCougarSettings
        {
            public static void Postfix(BaseAi __instance)
            {
                __instance.m_MaxHP = CustomSettings.settings.cougarHP;
                __instance.m_ChasePlayerSpeed = CustomSettings.settings.cougarSpeed;
            }
        }

        [HarmonyPatch(nameof(AiCougar), nameof(AiCougar.EnterDead))]

        public class WhenKilled
        {

            public static void Postfix()
            {

                Main.Logger.Log("Cougar killed!", ComplexLogger.FlaggedLoggingLevel.Debug);
                Main.Logger.Log("Setting threat level to 1.", ComplexLogger.FlaggedLoggingLevel.Debug);
                GameManager.GetCougarManager().MaybeSetThreatLevel(1);
                //this might backfire when the days counter runs out but I'm not sure
            }
        }
    }
}
