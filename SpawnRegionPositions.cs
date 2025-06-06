using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = System.Random;

namespace ImprovedCougar
{
    internal class SpawnRegionPositions
    {

        public static Vector3 ML_Clearcut = new Vector3(0f, 0f, 0f);
        public static Vector3 ML_WoodsCabin = new Vector3(-38f, 84f, 928.75f); //cabin near unnamed pond
        public static Vector3 ML_Backwoods = new Vector3(8f, -6f, 368f); //between trapper's and unnamed pond
        public static Vector3 ML_DeadfallArea = new Vector3(658f, 17f, 341f);
        public static Vector3 ML_TrainTunnelArea = new Vector3(409f, 62f, -35f); //area between trapper's and the train tunnel
        public static Vector3 ML_LoggingRoad = new Vector3(1112f, 56f, 1489f); //area near the logging road
        public static Vector3 ML_LookoutMountain = new Vector3(692f, 122f, 903.04f); //the mountain the forestry lookout sits upon
        public static Vector3 ML_LakeHill = new Vector3(1573.59f, 60.55f, 597.42f); //hill behind Mystery Lake towards the Dam
        public static Vector3 ML_LakeOverlook = new Vector3(942.59f, 72.62f, -81.59f); //hill behind Mystery Lake where the prepper cache is
        private static List<Vector3> GetSpawnRegionsByRegion(string region)
        {
            switch (region)
            {
                case "LakeRegion":
                    return new List<Vector3>() { ML_WoodsCabin, ML_Backwoods, ML_DeadfallArea, ML_TrainTunnelArea, ML_LoggingRoad, ML_LookoutMountain, ML_LakeHill, ML_LakeOverlook };
                default: return null;
            }
        }

        public static Vector3 GetRandomSpawnRegion(string region)
        {
            var random = new Random();
            List<Vector3> list = GetSpawnRegionsByRegion(region);
            return list[random.Next(list.Count)];
        }

    }
}
