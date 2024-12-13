﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedCougar.Settings
{
    internal class Settings : JsonModSettings
    {

        [Section("Cougar Attributes")]

        [Name("Cougar Max Health")]
        [Description("Max health for the cougar. \n(Vanilla = 300)")]
        [Slider(100f, 300f)]
        public float cougarHP = 250f;

        [Name("Cougar Attack Speed")]
        [Description("Run speed for the cougar when attacking. \n(Vanilla = 14)")]
        [Slider(14f, 20f)]
        public float cougarSpeed = 18f;

        [Section("Cougar Territory")]

        [Name("Show icon Map")]
        [Description("Show cougar territory icon on the map.")]
        public bool showIcon = false;

        [Name("Reveal on Map")]
        [Description("Reveal cougar territory and surrounding area on the map.")]
        public bool showTerritory = false;

    }

    internal static class CustomSettings
    {

        internal static readonly Settings settings = new Settings();

        public static void OnLoad()
        {
            settings.AddToModSettings("Improved Cougar", MenuType.Both); 
        }

    }
}
