using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = System.Random;

namespace ImprovedCougar
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
        private static List<Vector3> GetSpawnRegionsByRegion(string region)
        {
            switch (region)
            {
                case "LakeRegion":
                    return new List<Vector3>() { ML_WoodsCabin, ML_Backwoods, ML_DeadfallAreaForest, ML_LoggingRoad, ML_LookoutMountain, ML_LakeHill, ML_Plateau };
                case "RuralRegion":
                    return new List<Vector3>() { PV_MountainRoad, PV_SignalHill, PV_PicnicArea, PV_ThompsonForest, PV_ThompsonWaterfall, PV_MiningRoad, PV_DerelictCabins, PV_MisticFalls, PV_SkeetersRidge, PV_Backroad, PV_ThreeStrikes, PV_RiverWoods };
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
                foreach(var pos in spawns)
                {
                    GameObject spawnRegionCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    UnityEngine.Object.Destroy(spawnRegionCylinder.GetComponent<Collider>());
                    spawnRegionCylinder.transform.localScale = new Vector3(diameter, 100f, diameter);
                    spawnRegionCylinder.transform.position = pos;
                    spawnRegionCylinder.GetComponent<Renderer>().material.color = orange;
                    GameObject spawnRegionTop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(spawnRegionTop.GetComponent<Collider>());
                    spawnRegionTop.transform.localScale = new Vector3(diameter * 3f, diameter * 3f, diameter * 3f);
                    spawnRegionTop.transform.position = pos + new Vector3(0, 100f, 0);
                    spawnRegionTop.GetComponent<Renderer>().material.color = orange;
                    spawnRegionTop.transform.SetParent(spawnRegionCylinder.transform);
                }
            }

        }

    }
}
