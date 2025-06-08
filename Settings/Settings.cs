using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpandedAiFramework;

namespace ImprovedCougar.Settings
{
    internal class Settings : JsonModSettings, ExpandedAiFramework.ISpawnTypePickerCandidate
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

        [Name("Minimum Time to Arrival")]
        [Description("Minimum time it takes for the cougar to arrive in the world.")]
        [Slider(10f, 30f)]
        public int minTimeToArrive = 20;

        [Name("Minimum Time to Arrival")]
        [Description("Maximum time it takes for the cougar to arrive in the world.")]
        [Slider(10f, 30f)]
        public int maxTimeToArrive = 30;

        [Name("Minimum Time to Move Territory")]
        [Description("Minimum time it takes for the cougar to choose a new area in the region to move it's territory to.")]
        [Slider(24f, 96f)]
        public int minTimeToMove = 24;

        [Name("Minimum Time to Move Territory")]
        [Description("Maximum time it takes for the cougar to choose a new area in the region to move it's territory to.")]
        [Slider(24f, 96f)]
        public int maxTimeToMove = 96;

        [Section("Debug")]

        [Name("Points")]
        [Description("Activate spheres when points are found.")]
        public bool debugPoints = false;

        [Name("Colliders")]
        [Description("Activate collider bounds highlights.")]
        public bool debugColliders = false;

        [Name("Rays")]
        [Description("Activate visualized raycasts.")]
        public bool debugRays = false;

        bool ISpawnTypePickerCandidate.CanSpawn(BaseAi baseAi) => baseAi.m_AiSubType == AiSubType.Cougar;
        int ISpawnTypePickerCandidate.SpawnWeight() => 10000;
    }

    internal static class CustomSettings
    {

        internal static readonly Settings settings = new Settings();

        /**
        public static void OnLoad()
        {
            settings.AddToModSettings("Improved Cougar", MenuType.Both); 
        }
        **/
    }
}
