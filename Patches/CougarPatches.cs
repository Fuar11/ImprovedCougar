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

        [HarmonyPatch(nameof(CougarManager), nameof(CougarManager.MaybeSetThreatLevel))]

        public class DisableOtherTerritory
        {

            public static void Prefix(CougarManager __instance, ref int level)
            {
                level = 1;
            }

        }
    }
}
