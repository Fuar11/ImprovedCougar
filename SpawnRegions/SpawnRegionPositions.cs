using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = System.Random;

namespace ImprovedCougar.SpawnRegions
{
    internal class SpawnRegionPositions
    {

        public static Vector3 ML_Clearcut = new Vector3(0f, 0f, 0f);
        public static Vector3 ML_WoodsCabin = new Vector3(-38f, 84f, 928.75f); //cabin near unnamed pond
        public static Vector3 ML_Backwoods = new Vector3(8f, -6f, 368f); //between trapper's and unnamed pond
        public static Vector3 ML_DeadfallAreaForest = new Vector3(640f, 7.49f, 32.42f);
        public static Vector3 ML_LoggingRoad = new Vector3(1112f, 56f, 1489f); //area near the logging road
        public static Vector3 ML_LookoutMountain = new Vector3(692f, 122f, 903.04f); //the mountain the forestry lookout sits upon
        public static Vector3 ML_LakeHill = new Vector3(1573.59f, 60.55f, 597.42f); //hill behind Mystery Lake towards the Dam
        public static Vector3 ML_Plateau = new Vector3(1227.20f, 93.22f, 889.08f); //area behind the lake overlook

        public static Vector3 PV_MountainRoad = new Vector3(2509.84f, 168.93f, 672.24f); //around hunting blind near signal hill
        public static Vector3 PV_SignalHill = new Vector3(2057.61f, 178.17f, 432.87f); //path up to signal hill
        public static Vector3 PV_PicnicArea = new Vector3(2389.61f, 93.58f, 1543.72f);
        public static Vector3 PV_ThompsonForest = new Vector3(2616f, 68.47f, 2016.72f); //forest near thompson's crossing
        public static Vector3 PV_ThompsonWaterfall = new Vector3(2654.14f, 101.9f, 2480.72f); //waterfall near thompson's crossing
        public static Vector3 PV_MiningRoad = new Vector3(2108.14f, 107.98f, 2869.4f);
        public static Vector3 PV_DerelictCabins = new Vector3(1026.14f, 114.32f, 2652.62f); //road split near derelict cabins
        public static Vector3 PV_MisticFalls = new Vector3(539.14f, 228.43f, 2539.51f);
        public static Vector3 PV_SkeetersRidge = new Vector3(324.06f, 170.92f, 1846.26f); //behind plane crash at Skeeter's Ridge 
        public static Vector3 PV_Backroad = new Vector3(302.9f, 133.3f, 1060.92f); //back road up to Skeeter's Ridge
        public static Vector3 PV_ThreeStrikes = new Vector3(490.07f, 93.23f, 348f);
        public static Vector3 PV_RiverWoods = new Vector3(1076.07f, 48.20f, 497.29f); //in the woods across the river near the bunker

        public static Vector3 MT_PlanePath = new Vector3(1894.64f, 443.31f, 1511.15f); //path up to the plane crash
        public static Vector3 MT_CaveField = new Vector3(1375.72f, 385.45f, 2276.16f); //field near HRV cave
        public static Vector3 MT_LoggingTrailer = new Vector3(881f, 333.57f, 2379.16f);
        public static Vector3 MT_Church = new Vector3(496.87f, 307.57f, 2014.70f);
        public static Vector3 MT_FarmHill = new Vector3(873.48f, 247.36f, 1299.84f); //hill towards the milton park from the farm
        public static Vector3 MT_Farm = new Vector3(468.21f, 267.26f, 1851.89f); //default position at farm
        public static Vector3 MT_WoodLot = new Vector3(1346.21f, 270.72f, 1498.89f);

        public static Vector3 HRV_Stairsteps = new Vector3(331.21f, 119.21f, 732.03f);
        public static Vector3 HRV_OffsetFalls = new Vector3(252.76f, 116.4f, 1174.4f);
        public static Vector3 HRV_NorthCliffs = new Vector3(473.89f, 105.91f, 1573.81f);
        public static Vector3 HRV_HushedRiverForest = new Vector3(638.89f, 100.70f, 952.95f); //forest along the hushed river
        public static Vector3 HRV_MammothFalls2 = new Vector3(878.63f, 178.02f, 1521.75f); //2nd waterfall off mammoth falls
        public static Vector3 HRV_MonolithLake = new Vector3(1321f, 211.77f, 1441.26f);

        public static Vector3 BR_Substation = new Vector3(-561f, 140.33f, -211.86f);
        public static Vector3 BR_BearBend = new Vector3(-852f, 139.07f, -130.33f); //cliffs above bear's bend (near the trailer)
        public static Vector3 BR_ForagerRemnant = new Vector3(-824f, 141.77f, 548.57f);
        public static Vector3 BR_JailersResidence = new Vector3(45f, 189.29f, -657f); //cliffs above Jailer's residence
        public static Vector3 BR_CuttysCave = new Vector3(360.23f, 191.94f, -660.05f);
        public static Vector3 BR_ClearCutPath = new Vector3(338.23f, 242.29f, 116.68f); //hidden path up in the hills near foreman's clearcut
        public static Vector3 BR_SprucePatch = new Vector3(609.85f, 254.22f, -126.25f); //patch of spruce trees on the road to the mine
        public static Vector3 BR_Bricklayers = new Vector3(1040.7f, 261.35f, 106.25f);
        public static Vector3 BR_HuntingBlind = new Vector3(784f, 304.89f, 408f); //near hunting blind along road to the mine

        public static Vector3 TWM_Entrance = new Vector3(546.63f, 224.21f, 75.63f);
        public static Vector3 TWM_Wing = new Vector3(1693.35f, 200.24f, 422.42f);
        public static Vector3 TWM_River = new Vector3(180.1f, 248.22f, 688.53f); //area off the side of the river 
        public static Vector3 TWM_AndresPeak = new Vector3(337.52f, 334.33f, 1231.58f); //adjacent peak to Andre's Peak. Near Blackrock cave
        public static Vector3 TWM_EricsFalls = new Vector3(254.82f, 364.14f, 1600f);
        public static Vector3 TWM_Backside = new Vector3(993.87f, 262.67f, 1774.52f); //area behind the summit (idk if i'll keep this one, it's fairly open)
        public static Vector3 TWM_RavineRidge = new Vector3(1514f, 166.9f, 1160.48f); //area near the big indoor cave along the ridge off echo ravine 

        public static Vector3 AC_Bunker = new Vector3(13.34f, 162.24f, -751.76f);
        public static Vector3 AC_PillarsFootrest = new Vector3(-319.34f, 161.6f, -504.38f); //hill near pillar's footrest
        public static Vector3 AC_BirchForest = new Vector3(-784.34f, 236.88f, -101.95f);
        public static Vector3 AC_ClimbersPlateau1 = new Vector3(-145.4f, 184.93f, -453.76f); //near climber's cave 
        public static Vector3 AC_ClimbersPlateau2 = new Vector3(-211.54f, 137.22f, -193.37f); //near the other cave on the plateau
        public static Vector3 AC_FireOverlook = new Vector3(381.54f, 199.49f, -75.8f);
        public static Vector3 AC_WaterfallBasin = new Vector3(-72f, 192.41f, 762.65f);
        public static Vector3 AC_NarrowFalls = new Vector3(546f, 99.65f, 500f);

        public static Vector3 FA_Junkers = new Vector3(-1125.44f, 303.96f, -1271.95f); //hills behind Junker's Paddock
        public static Vector3 FA_WaterfallPlateau = new Vector3(-1057.11f, 272.82f, -548.1f); //plateau above the waterfall open cave
        public static Vector3 FA_FrozenRiver = new Vector3(-562.19f, 133.54f, -909.94f); //frozen river along the road to the airfield 
        public static Vector3 FA_RoadForest = new Vector3(-1255.17f, 256.22f, 788.86f); //forest near the winding road passing through the middle of the field
        public static Vector3 FA_MindfulCabin = new Vector3(-1175.8f, 283.08f, 1318f);
        public static Vector3 FA_MindfulCabin2 = new Vector3(-536.38f, 211.24f, 1208.07f); //below mindful cabin (not sure if i'll keep this one)
        public static Vector3 FA_TransitionCave = new Vector3(1021.28f, 255.63f, 1362.33f); //corner of the region near the transition cave
        public static Vector3 FA_ChopperCrash = new Vector3(1058.6f, 211.14f, 325.87f);
        public static Vector3 FA_Shortcut = new Vector3(248.9f, 284.14f, -1227.87f);
        public static Vector3 FA_JustysHovel = new Vector3(845.9f, 290.59f, -1108.12f);

        public static Vector3 SP_Teardrop1 = new Vector3(946.9f, 339.26f, -402.48f); //bottom of Ogre's Teardrop
        public static Vector3 SP_BasinPlateau = new Vector3(1085.2f, 278.4f, -13f); //small plateau in the valley basin
        //public static Vector3 SP_ClimbingSpot = new Vector3(628.92f, 219.96f, -505.9f); //climbing spot above the Last Lonely House
        public static Vector3 SP_CaveRidge = new Vector3(1133.16f, 432.05f, 314.58f); //cliff face near the transition cave
        public static Vector3 SP_Teardrop2 = new Vector3(1078.3f, 494.64f, -655.15f); //top of Ogre's Teardrop
        public static Vector3 SP_PlaneCrash = new Vector3(1045.3f, 615.02f, -738.27f);
        public static Vector3 SP_MiddlePlateau = new Vector3(24.1f, 531.07f, -768.09f); //plateau in the middle of the region (idk if i'll keep this one)
        public static Vector3 SP_Mine = new Vector3(46.06f, 650.88f, -1136.87f);
        public static Vector3 SP_FrozenWaterfalls = new Vector3(192.85f, 242.89f, 609.67f); //frozen waterfalls along the road up the mountain
        public static Vector3 SP_BanefulBridge = new Vector3(283.16f, 120.16f, 911.51f); //below baneful bridge
        public static Vector3 SP_HiddenPath = new Vector3(225.27f, 187.92f, 993.5f); //hidden path below baneful bridge

        private static List<Vector3> GetSpawnRegionsByRegion(string region)
        {
            switch (region)
            {
                case "LakeRegion":
                    return new List<Vector3>() { ML_WoodsCabin, ML_Backwoods, ML_DeadfallAreaForest, ML_LoggingRoad, ML_LookoutMountain, ML_LakeHill, ML_Plateau };
                case "RuralRegion":
                    return new List<Vector3>() { PV_MountainRoad, PV_SignalHill, PV_PicnicArea, PV_ThompsonForest, PV_ThompsonWaterfall, PV_MiningRoad, PV_DerelictCabins, PV_MisticFalls, PV_SkeetersRidge, PV_Backroad, PV_ThreeStrikes, PV_RiverWoods };
                case "MountainTownRegion":
                    return new List<Vector3>() { MT_PlanePath, MT_CaveField, MT_LoggingTrailer, MT_Church, MT_FarmHill, MT_Farm, MT_WoodLot };
                case "RiverValleyRegion":
                    return new List<Vector3>() { HRV_Stairsteps, HRV_OffsetFalls, HRV_NorthCliffs, HRV_MonolithLake, HRV_MammothFalls2, HRV_HushedRiverForest };
                case "BlackrockRegion":
                    return new List<Vector3>() { BR_BearBend, BR_Bricklayers, BR_ClearCutPath, BR_CuttysCave, BR_ForagerRemnant, BR_HuntingBlind, BR_JailersResidence, BR_SprucePatch, BR_Substation };
                case "CrashMountainRegion":
                    return new List<Vector3>() { TWM_AndresPeak, TWM_Backside, TWM_Entrance, TWM_EricsFalls, TWM_RavineRidge, TWM_River, TWM_Wing };
                case "AshCanyonRegion":
                    return new List<Vector3>() { AC_BirchForest, AC_Bunker, AC_ClimbersPlateau1, AC_ClimbersPlateau2, AC_FireOverlook, AC_NarrowFalls, AC_PillarsFootrest, AC_WaterfallBasin };
                case "AirfieldRegion":
                    return new List<Vector3>() { FA_ChopperCrash, FA_FrozenRiver, FA_Junkers, FA_JustysHovel, FA_MindfulCabin, FA_MindfulCabin2, FA_RoadForest, FA_Shortcut, FA_TransitionCave, FA_WaterfallPlateau };
                case "MountainPassRegion":
                    return new List<Vector3>() { SP_BanefulBridge, SP_BasinPlateau, SP_CaveRidge, SP_FrozenWaterfalls, SP_HiddenPath, SP_MiddlePlateau, SP_Mine, SP_PlaneCrash, SP_Teardrop1, SP_Teardrop2 };
                default: return null;
            }
        }

        public static Vector3? GetRandomSpawnRegion(string region)
        {
            var random = new Random();
            List<Vector3> list = GetSpawnRegionsByRegion(region);

            if (list == null || list.Count == 0) return null;

            return list[random.Next(list.Count)];
        }

        public static void AddMarkersToSpawnRegions(string region)
        {
            List<Vector3> spawns = GetSpawnRegionsByRegion(region);
            var diameter = 5f;
            Color orange = new Color(1.0f, 0.64f, 0.0f);

            if (spawns != null)
            {
                foreach (var pos in spawns)
                {

                    Color color = Main.CustomCougarManager.currentSpawnRegion == pos ? Color.green : orange;

                    GameObject spawnRegionCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    UnityEngine.Object.Destroy(spawnRegionCylinder.GetComponent<Collider>());
                    spawnRegionCylinder.transform.localScale = new Vector3(diameter, 100f, diameter);
                    spawnRegionCylinder.transform.position = pos;
                    spawnRegionCylinder.GetComponent<Renderer>().material.color = color;
                    GameObject spawnRegionTop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(spawnRegionTop.GetComponent<Collider>());
                    spawnRegionTop.transform.localScale = new Vector3(diameter * 3f, diameter * 3f, diameter * 3f);
                    spawnRegionTop.transform.position = pos + new Vector3(0, 100f, 0);
                    spawnRegionTop.GetComponent<Renderer>().material.color = color;
                    spawnRegionTop.transform.SetParent(spawnRegionCylinder.transform);
                }
            }

        }

    }
}
