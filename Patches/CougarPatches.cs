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

        [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.InstantiateSpawnInternal), new Type[] { typeof(GameObject), typeof(WildlifeMode), typeof(Vector3), typeof(Quaternion) })]

        public class AdjustSpawnPoints
        {

            public static void Prefix(SpawnRegion __instance, ref Vector3 pos)
            {
                if (__instance.m_SpawnablePrefab == null) return;
                if (__instance.m_SpawnablePrefab.GetComponent<BaseAi>().m_AiSubType != AiSubType.Cougar) return;

                Main.Logger.Log($"Checking point chosen {pos}", ComplexLogger.FlaggedLoggingLevel.Debug);

                UnityEngine.AI.NavMeshHit hit;
                if (UnityEngine.AI.NavMesh.SamplePosition(pos, out hit, 50.0f, NavMesh.AllAreas))
                {
                    pos = hit.position;
                    Main.Logger.Log($"New point chosen {pos}", ComplexLogger.FlaggedLoggingLevel.Debug);
                }
                else
                {
                    Main.Logger.Log("Couldn't find any valid points on navmesh near chosen point.", ComplexLogger.FlaggedLoggingLevel.Debug);
                }

            }

        }

        /***

        [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.InstantiateSpawn))]

        public class Test5
        {

            public static void Prefix(SpawnRegion __instance)
            {
                if (__instance.m_SpawnablePrefab == null) return;
                if (__instance.m_SpawnablePrefab.GetComponent<BaseAi>().m_AiSubType != AiSubType.Cougar) return;

                Main.Logger.Log("InstantiateSpawn called for cougar", ComplexLogger.FlaggedLoggingLevel.Debug);
            }


        }

        [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.InstantiateAndPlaceSpawn))]

        public class Test4
        {

            public static void Prefix(SpawnRegion __instance)
            {
                if (__instance.m_SpawnablePrefab == null) return;
                if (__instance.m_SpawnablePrefab.GetComponent<BaseAi>().m_AiSubType != AiSubType.Cougar) return;

                Main.Logger.Log("InstantiateAndPlaceSpawn called for cougar", ComplexLogger.FlaggedLoggingLevel.Debug);
            }


        }

        [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.Spawn))]

        public class Test3
        {

            public static void Prefix(SpawnRegion __instance)
            {
                if (__instance.m_SpawnablePrefab == null) return;
                if (__instance.m_SpawnablePrefab.GetComponent<BaseAi>().m_AiSubType != AiSubType.Cougar) return;

                Main.Logger.Log("Spawn called for cougar", ComplexLogger.FlaggedLoggingLevel.Debug);
            }


        }

        [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.AddActiveSpawns))]

        public class Test2
        {

            public static void Prefix(SpawnRegion __instance)
            {
                if (__instance.m_SpawnablePrefab == null) return;
                if (__instance.m_SpawnablePrefab.GetComponent<BaseAi>().m_AiSubType != AiSubType.Cougar) return;

                Main.Logger.Log("AddActiveSpawns called for cougar", ComplexLogger.FlaggedLoggingLevel.Debug);
            }


        }

        [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.AdjustActiveSpawnRegionPopulation))]

        public class Test1
        {

            public static void Prefix(SpawnRegion __instance)
            {
                if (__instance.m_SpawnablePrefab == null) return;
                if (__instance.m_SpawnablePrefab.GetComponent<BaseAi>().m_AiSubType != AiSubType.Cougar) return;

                Main.Logger.Log("AdjustActiveSpawnRegionPopulation called for cougar", ComplexLogger.FlaggedLoggingLevel.Debug);
            }


        }

        [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.CalculateTargetPopulation))]

        public class TestPop
        {

            public static void Postfix(SpawnRegion __instance, ref int __result)
            {
                if (__instance.m_SpawnablePrefab == null) return;
                if (__instance.m_SpawnablePrefab.GetComponent<BaseAi>().m_AiSubType != AiSubType.Cougar) return;

                Main.Logger.Log($"CalculateTargetPopulation called for cougar with result {__result}", ComplexLogger.FlaggedLoggingLevel.Debug);
            }


        }

        [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.SpawnRegionCloseEnoughForSpawning))]

        public class TestSuppr
        {

            public static void Postfix(SpawnRegion __instance, ref bool __result)
            {
                if (__instance.m_SpawnablePrefab == null) return;
                if (__instance.m_SpawnablePrefab.GetComponent<BaseAi>().m_AiSubType != AiSubType.Cougar) return;

                Main.Logger.Log($"SpawnRegionCloseEnoughForSpawning called for cougar with result {__result}", ComplexLogger.FlaggedLoggingLevel.Debug);
            }


        } ***/

    }
}
