using Il2CppTLD.AI;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppTMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace ImprovedCougar.Patches
{
    internal class MapPatches
    {

        [HarmonyPatch(nameof(CougarTerritoryZoneTrigger), nameof(CougarTerritoryZoneTrigger.ActivateZone))]

        public class tst
        {
            public static void Prefix(ref bool enable)
            {
                if (enable) { Main.Logger.Log("enabling cougar territory", ComplexLogger.FlaggedLoggingLevel.Debug); }
            }
        }

        [HarmonyPatch(nameof(Panel_Map), nameof(Panel_Map.DoNearbyDetailsCheck))]

        public class PreventMappingOnCougarTerritory
        {

            public static bool Prefix(Panel_Map __instance)
            {
                //get the caller method from the stack trace
                StackTrace stackTrace = new StackTrace();

                foreach (var frame in stackTrace.GetFrames())
                {
                    Main.Logger.Log($"Method: {frame.GetMethod().Name}", ComplexLogger.FlaggedLoggingLevel.Debug);
                }

                if (stackTrace.GetFrames().Any(frame => frame.GetMethod().Name.Contains("ActivateZone")))
                {
                    return false;
                }
                return true;
            }

        }

        [HarmonyPatch(nameof(MapDetail), nameof(MapDetail.ShowOnMap))]

        public class RemoveCougarTerritoryFromMap
        {
            public static void Prefix(MapDetail __instance, ref bool isShownOnMap) 
            {

                if (__instance.m_SpriteName == "ico_CougarMap")
                {
                    isShownOnMap = true;
                }
                isShownOnMap = false;
            }

        }
    }
}
