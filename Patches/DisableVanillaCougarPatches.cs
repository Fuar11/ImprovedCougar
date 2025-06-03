using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;
using HarmonyLib;
using Il2CppTLD.AI;

namespace ImprovedCougar.Patches
{
    internal class DisableVanillaCougarPatches
    {

        [HarmonyPatch(nameof(CougarManager), nameof(CougarManager.Update))]

        public class DisableUpdateLoop
        {
            public static bool Prefix() => false;
        }
    }
}
