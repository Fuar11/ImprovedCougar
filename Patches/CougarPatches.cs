using Il2CppTLD.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImprovedCougar.Settings;
using UnityEngine.AI;

namespace ImprovedCougar.Patches
{
    internal class CougarPatches
    {
        // Nick: Do an override on CustomCougar.EnterAiMode() with a catch on AiMode.Dead, I'm not 100% sure this will fire anymore with EAF.
        [HarmonyPatch(nameof(AiCougar), nameof(AiCougar.EnterDead))]

        public class WhenKilled
        {

            public static void Postfix()
            {

                Main.Logger.Log("Cougar killed!", ComplexLogger.FlaggedLoggingLevel.Debug);
                Main.Logger.Log("Setting threat level to 1.", ComplexLogger.FlaggedLoggingLevel.Debug);
                GameManager.GetCougarManager().MaybeSetThreatLevel(1);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(CougarManager), "MaybeIncreaseThreatLevel")]
        public class DisableOtherTerritory
        {
            public static bool Prefix(CougarManager __instance)
            {
                return false;
            }

        }
    }
}
