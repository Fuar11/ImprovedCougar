using Il2CppTLD.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImprovedCougar.Settings;
using UnityEngine.AI;
using ExpandedAiFramework;
using CougarManager = Il2CppTLD.AI.CougarManager;
using UnityEngine;

namespace ImprovedCougar.Patches
{
    internal class CougarPatches
    {
        
        [HarmonyLib.HarmonyPatch(typeof(CougarManager), "MaybeIncreaseThreatLevel")]
        public class DisableOtherTerritory
        {
            public static bool Prefix(CougarManager __instance)
            {
                return false;
            }
        }


        [HarmonyPatch(typeof(GameAudioManager), "PlaySoundWithPositionTracking", new Type[] { typeof(Il2CppAK.Wwise.Event), typeof(GameObject), typeof(AkCallbackManager.EventCallback), typeof(GameAudioManager.PlayOptions) })]

        public class RemoveCougarAudio
        {
            public static bool Prefix(ref Il2CppAK.Wwise.Event soundEvent, ref GameObject go)
            {                
                    return soundEvent.Name == "play_Cougar_prefabWildlifeCougar_Spawn" ? false : true;
            }
        }

    }
}
