using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;
using HarmonyLib;

namespace ImprovedCougar.Patches
{
    internal class GunPatches
    {

        [HarmonyPatch(nameof(vp_FPSWeapon), nameof(vp_FPSWeapon.PlayFireAnimation))]

        public class CheckShootPosition
        {

            public static void Postfix()
            {



            }

        }

    }
}
