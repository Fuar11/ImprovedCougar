using Il2CppTLD.AI;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImprovedCougar.Settings;

namespace ImprovedCougar.Patches
{
    internal class MapPatches
    {

        [HarmonyPatch(nameof(CougarTerritoryZoneTrigger), nameof(CougarTerritoryZoneTrigger.ActivateZone))]

        public class SetMapRadiusToZero
        {
            public static void Prefix(CougarTerritoryZoneTrigger __instance)
            {
                if(CustomSettings.settings.showTerritory) __instance.m_MapRevealRadius = 0;
            }
        }

        [HarmonyPatch(nameof(Panel_Map), nameof(Panel_Map.DoNearbyDetailsCheck))]

        public class PreventMappingOnCougarTerritory
        {
            public static bool Prefix(Panel_Map __instance, ref float radius) => radius == 0 ? false : true;
        }

        [HarmonyPatch(nameof(MapDetail), nameof(MapDetail.ShowOnMap))]

        public class RemoveCougarTerritoryFromMap
        {
            public static void Prefix(MapDetail __instance, ref bool isShownOnMap) 
            {
                if (CustomSettings.settings.showIcon) return;

                isShownOnMap = GameManager.GetCougarManager().m_ActiveTerritory.m_CougarState == CougarManager.CougarState.HasArrived && __instance.m_SpriteName == "ico_CougarMap" ? true : false;
            }
        }
    }
}
