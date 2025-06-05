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
                if (!__instance.m_SpawnablePrefab.name.ToLowerInvariant().Contains("cougar")) return;

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

    }
}
