using Il2CppTLD.AI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Color = UnityEngine.Color;
using Random = System.Random;

namespace ImprovedCougar.SpawnRegions
{
    internal class SpawnRegionPositions
    {

        public static SpawnRegionTerritory ML_Clearcut = new SpawnRegionTerritory(new Vector3(0f, 0f, 0f), new List<Vector3>() { });
        public static SpawnRegionTerritory ML_WoodsCabin = new SpawnRegionTerritory(new Vector3(-38f, 84f, 928.75f), new List<Vector3>() { new Vector3(169.60f, 11.50f, 764.19f), new Vector3(201.65f, 24.32f, 910.32f), new Vector3(199.01f, 26.62f, 964.87f), new Vector3(-14.68f, 49.79f, 1007.36f), new Vector3(-4.42f, 69.27f, 809.43f), new Vector3(221.14f, 21.53f, 1227.39f)}); //cabin near unnamed pond
        public static SpawnRegionTerritory ML_Backwoods = new SpawnRegionTerritory(new Vector3(8f, -6f, 368f), new List<Vector3>() { new Vector3(46.46f, -5.14f, 419.20f), new Vector3(-2.52f, 48.91f, 218.84f), new Vector3(97.42f, 9.75f, 491.74f), new Vector3(327.20f, 46.20f, 570.46f), new Vector3(365.78f, -0.58f, 407.19f)}); //between trapper's and unnamed pond
        public static SpawnRegionTerritory ML_DeadfallAreaForest = new SpawnRegionTerritory(new Vector3(640f, 7.49f, 32.42f), new List<Vector3>() { new Vector3(614.33f, 4.28f, 239.52f), new Vector3(426.62f, 37.93f, 71.10f), new Vector3(491.21f, 3.19f, 247.46f), new Vector3(607.29f, 49.88f, 419.69f) });
        public static SpawnRegionTerritory ML_LoggingRoad = new SpawnRegionTerritory(new Vector3(1112f, 56f, 1489f), new List<Vector3>() { new Vector3(606.73f, 77.16f, 1522.16f), new Vector3(847.70f, 26.10f, 1261.09f), new Vector3(1079.73f, 25.62f, 1422.35f), new Vector3(1274.21f, 48.96f, 1434.48f), new Vector3(825.67f, 63.00f, 1139.06f), new Vector3(464.85f, 41.26f, 1352.52f)}); //area near the logging road
        public static SpawnRegionTerritory ML_LookoutMountain = new SpawnRegionTerritory(new Vector3(692f, 122f, 903.04f), new List<Vector3>() { new Vector3(589.36f, 120.71f, 1016.96f), new Vector3(688.90f, 130.41f, 922.56f), new Vector3(589.59f, 14.42f, 602.80f), new Vector3(877.56f, 54.25f, 789.79f), new Vector3(907.93f, 69.14f, 855.30f), new Vector3(735.14f, 190.38f, 1007.96f)}); //the mountain the forestry lookout sits upon
        public static SpawnRegionTerritory ML_LakeHill = new SpawnRegionTerritory(new Vector3(1573.59f, 60.55f, 597.42f), new List<Vector3>() { new Vector3(1469.59f, 40.39f, 521.82f), new Vector3(1539.27f, 46.20f, 557.00f), new Vector3(1596.05f, 20.13f, 536.98f), new Vector3(1483.42f, 18.09f, 371.29f), new Vector3(1684.89f, 33.05f, 221.12f), new Vector3(1593.92f, 28.94f, -32.59f)}); //hill behind Mystery Lake towards the Dam
        public static SpawnRegionTerritory ML_Plateau = new SpawnRegionTerritory(new Vector3(1227.20f, 93.22f, 889.08f), new List<Vector3>() { new Vector3(1204.18f, 89.43f, 535.87f), new Vector3(1022.61f, 52.86f, 651.01f), new Vector3(1153.65f, 82.27f, 947.53f), new Vector3(1385.63f, 18.95f, 1119.52f), new Vector3(1527.52f, 75.37f, 945.73f), new Vector3(1214.81f, 141.71f, 678.76f)}); //area behind the lake overlook

        public static SpawnRegionTerritory PV_MountainRoad = new SpawnRegionTerritory(new Vector3(2509.84f, 168.93f, 672.24f), new List<Vector3>() { new Vector3(2357.49f, 170.34f, 860.07f), new Vector3(2601.74f, 138.78f, 847.17f), new Vector3(2389.77f, 187.54f, 449.24f), new Vector3(2613.50f, 137.79f, 630.81f)}); //around hunting blind near signal hill
        public static SpawnRegionTerritory PV_SignalHill = new SpawnRegionTerritory(new Vector3(2057.61f, 178.17f, 432.87f), new List<Vector3>() { new Vector3(1900.81f, 223.68f, 432.52f), new Vector3(1901.31f, 112.43f, 207.08f), new Vector3(2215.81f, 177.00f, 390.97f), new Vector3(2183.68f, 162.49f, 708.53f) }); //path up to signal hill
        public static SpawnRegionTerritory PV_PicnicArea = new SpawnRegionTerritory(new Vector3(2389.61f, 93.58f, 1543.72f), new List<Vector3>() { new Vector3(2559.53f, 116.59f, 1546.41f), new Vector3(2617.32f, 127.25f, 1078.92f), new Vector3(2174.70f, 59.69f, 1661.42f), new Vector3(2240.31f, 51.98f, 1838.66f)});
        public static SpawnRegionTerritory PV_ThompsonForest = new SpawnRegionTerritory(new Vector3(2616f, 68.47f, 2016.72f), new List<Vector3>() { new Vector3(2424.76f, 48.60f, 2025.47f), new Vector3(2633.76f, 95.68f, 1754.76f),  }); //forest near thompson's crossing
        public static SpawnRegionTerritory PV_ThompsonWaterfall = new SpawnRegionTerritory(new Vector3(2654.14f, 101.9f, 2480.72f), new List<Vector3>() { new Vector3(2536.12f, 71.97f, 2423.88f), new Vector3(2682.28f, 90.06f, 2321.09f), new Vector3(2746.56f, 101.24f, 2654.91f), new Vector3(2445.54f, 116.15f, 2659.22f), new Vector3(2426.40f, 42.01f, 2347.02f)}); //waterfall near thompson's crossing
        public static SpawnRegionTerritory PV_MiningRoad = new SpawnRegionTerritory(new Vector3(2108.14f, 107.98f, 2869.4f), new List<Vector3>() { new Vector3(2067.45f, 90.43f, 2775.30f), new Vector3(2291.46f, 118.03f, 2866.70f), new Vector3(2370.17f, 127.17f, 2842.29f), new Vector3(2495.25f, 103.21f, 2689.58f), new Vector3(1762.29f, 103.06f, 2762.24f), new Vector3(1995.30f, 82.91f, 2621.89f)});
        public static SpawnRegionTerritory PV_DerelictCabins = new SpawnRegionTerritory(new Vector3(1026.14f, 114.32f, 2652.62f), new List<Vector3>() { new Vector3(1184.90f, 81.35f, 2629.14f), new Vector3(1191.08f, 63.97f, 2439.69f), new Vector3(1020.79f, 75.08f, 2492.41f), new Vector3(1220.89f, 80.67f, 2744.75f), new Vector3(922.06f, 118.40f, 2576.31f)}); //road split near derelict cabins
        public static SpawnRegionTerritory PV_MisticFalls = new SpawnRegionTerritory(new Vector3(539.14f, 228.43f, 2539.51f), new List<Vector3>() { new Vector3(376.81f, 141.26f, 2596.71f), new Vector3(540.79f, 143.06f, 2374.10f), new Vector3(659.50f, 154.82f, 2510.72f), new Vector3(633.51f, 170.78f, 2136.25f), new Vector3(443.97f, 147.17f, 2143.74f)});
        public static SpawnRegionTerritory PV_SkeetersRidge = new SpawnRegionTerritory(new Vector3(324.06f, 170.92f, 1846.26f), new List<Vector3>() { new Vector3(346.52f, 156.38f, 1696.91f), new Vector3(199.55f, 156.72f, 1567.64f), new Vector3(351.93f, 153.35f, 1479.23f), new Vector3(567.08f, 152.91f, 1537.32f), new Vector3(755.53f, 161.30f, 1932.40f), new Vector3(533.21f, 152.88f, 1847.37f), new Vector3(434.59f, 188.71f, 1601.19f), new Vector3(989.19f, 41.72f, 1821.14f), new Vector3(724.36f, 47.60f, 1427.51f) }); //behind plane crash at Skeeter's Ridge 
        public static SpawnRegionTerritory PV_Backroad = new SpawnRegionTerritory(new Vector3(302.9f, 133.3f, 1060.92f), new List<Vector3>() { new Vector3(521.74f, 64.26f, 1195.02f), new Vector3(439.43f, 97.59f, 1090.19f), new Vector3(256.45f, 128.67f, 1080.35f), new Vector3(421.20f, 82.69f, 920.36f), new Vector3(572.84f, 52.93f, 890.46f) }); //back road up to Skeeter's Ridge
        public static SpawnRegionTerritory PV_ThreeStrikes = new SpawnRegionTerritory(new Vector3(490.07f, 93.23f, 348f), new List<Vector3>() { new Vector3(610.84f, 74.33f, 274.25f), new Vector3(506.76f, 88.25f, 314.22f), new Vector3(295.16f, 100.91f, 488.28f), new Vector3(724.02f, 68.24f, 534.19f) });
        public static SpawnRegionTerritory PV_RiverWoods = new SpawnRegionTerritory(new Vector3(1076.07f, 48.20f, 497.29f), new List<Vector3>() { new Vector3(1050.33f, 49.80f, 576.61f), new Vector3(1060.52f, 46.49f, 367.16f), new Vector3(1448.38f, 61.56f, 181.60f), new Vector3(1642.31f, 48.31f, 580.73f), new Vector3(1224.04f, 69.46f, 809.62f) }); //in the woods across the river near the bunker

        public static SpawnRegionTerritory MT_PlanePath = new SpawnRegionTerritory(new Vector3(1894.64f, 443.31f, 1511.15f), new List<Vector3>() { new Vector3(1638.70f, 383.07f, 1673.34f) }); //path up to the plane crash
        public static SpawnRegionTerritory MT_CaveField = new SpawnRegionTerritory(new Vector3(1375.72f, 385.45f, 2276.16f), new List<Vector3>() { new Vector3(1185.60f, 360.19f, 2254.68f), new Vector3(1362.64f, 376.93f, 2210.47f), new Vector3(1300.99f, 388.52f, 2154.70f), new Vector3(1627.02f, 379.66f, 1998.36f)}); //field near HRV cave
        public static SpawnRegionTerritory MT_LoggingTrailer = new SpawnRegionTerritory(new Vector3(881f, 333.57f, 2379.16f), new List<Vector3>() { new Vector3(756.72f, 305.59f, 2258.10f), new Vector3(1058.39f, 334.54f, 2121.85f), new Vector3(1058.62f, 361.31f, 2334.03f) });
        public static SpawnRegionTerritory MT_Church = new SpawnRegionTerritory(new Vector3(496.87f, 307.57f, 2014.70f), new List<Vector3>() { new Vector3(625.70f, 289.55f, 2010.83f), new Vector3(620.40f, 249.42f, 1758.11f), new Vector3(558.07f, 296.32f, 2141.63f), new Vector3(772.01f, 287.83f, 2090.41f)});
        public static SpawnRegionTerritory MT_FarmHill = new SpawnRegionTerritory(new Vector3(873.48f, 247.36f, 1299.84f), new List<Vector3>() { }); //hill towards the milton park from the farm
        public static SpawnRegionTerritory MT_Farm = new SpawnRegionTerritory(new Vector3(468.21f, 267.26f, 1851.89f), new List<Vector3>() { }); //default position at farm
        public static SpawnRegionTerritory MT_WoodLot = new SpawnRegionTerritory(new Vector3(1346.21f, 270.72f, 1498.89f), new List<Vector3>() { new Vector3(1348.78f, 263.05f, 1381.58f), new Vector3(1208.64f, 284.20f, 1217.63f), new Vector3(1089.95f, 282.93f, 1211.36f), new Vector3(1255.65f, 277.68f, 1552.14f) });

        public static SpawnRegionTerritory HRV_Stairsteps = new SpawnRegionTerritory(new Vector3(331.21f, 119.21f, 732.03f), new List<Vector3>() { new Vector3(460.04f, 234.86f, 231.75f), new Vector3(228.64f, 129.03f, 588.78f), new Vector3(276.58f, 117.42f, 657.84f), new Vector3(310.78f, 116.49f, 879.08f), new Vector3(375.41f, 161.95f, 958.16f), new Vector3(430.39f, 119.58f, 700.59f), new Vector3(360.39f, 200.62f, 508.15f)});
        public static SpawnRegionTerritory HRV_OffsetFalls = new SpawnRegionTerritory(new Vector3(252.76f, 116.4f, 1174.4f), new List<Vector3>() { new Vector3(244.79f, 78.94f, 1021.88f), new Vector3(167.87f, 97.64f, 1197.42f), new Vector3(427.99f, 101.57f, 1192.21f), new Vector3(261.62f, 101.51f, 1235.94f), new Vector3(473.06f, 152.30f, 1351.18f)});
        public static SpawnRegionTerritory HRV_NorthCliffs = new SpawnRegionTerritory(new Vector3(473.89f, 105.91f, 1573.81f), new List<Vector3>() { new Vector3(496.30f, 114.55f, 1417.88f), new Vector3(739.53f, 111.74f, 1374.36f), new Vector3(696.70f, 93.04f, 1522.35f), new Vector3(668.17f, 119.73f, 1739.54f), new Vector3(570.63f, 102.72f, 1584.62f), new Vector3(439.91f, 100.83f, 1516.06f)});
        public static SpawnRegionTerritory HRV_HushedRiverForest = new SpawnRegionTerritory(new Vector3(638.89f, 100.70f, 952.95f), new List<Vector3>() { new Vector3(596.93f, 75.91f, 1207.41f), new Vector3(600.98f, 96.23f, 893.00f), new Vector3(542.11f, 113.06f, 929.80f), new Vector3(758.38f, 93.88f, 1017.26f), new Vector3(717.13f, 106.38f, 798.61f), new Vector3(661.42f, 95.25f, 671.74f)}); //forest along the hushed river
        public static SpawnRegionTerritory HRV_MammothFalls2 = new SpawnRegionTerritory(new Vector3(878.63f, 178.02f, 1521.75f), new List<Vector3>() { new Vector3(1026.04f, 180.36f, 1529.99f), new Vector3(896.98f, 177.44f, 1563.58f), new Vector3(826.17f, 204.27f, 1672.26f), new Vector3(1164.35f, 206.56f, 1678.88f), new Vector3(1124.69f, 172.77f, 1580.28f)}); //2nd waterfall off mammoth falls
        public static SpawnRegionTerritory HRV_MonolithLake = new SpawnRegionTerritory(new Vector3(1321f, 211.77f, 1441.26f), new List<Vector3>() { new Vector3(1251.36f, 204.30f, 1694.55f), new Vector3(1504.30f, 204.88f, 1759.24f), new Vector3(1618.13f, 264.82f, 1421.08f), new Vector3(1416.58f, 276.26f, 1379.95f), new Vector3(1216.64f, 209.08f, 1460.85f), new Vector3(1231.06f, 225.52f, 1286.18f)});

        public static SpawnRegionTerritory BR_Substation = new SpawnRegionTerritory(new Vector3(-561f, 140.33f, -211.86f), new List<Vector3>() { new Vector3(-461.28f, 111.59f, -544.08f), new Vector3(-362.98f, 134.87f, -430.06f), new Vector3(-627.22f, 92.74f, -339.15f), new Vector3(-582.47f, 113.82f, -144.81f), new Vector3(-687.00f, 117.34f, -323.04f)});
        public static SpawnRegionTerritory BR_BearBend = new SpawnRegionTerritory(new Vector3(-852f, 139.07f, -130.33f), new List<Vector3>() { new Vector3(-722.02f, 114.47f, -157.25f), new Vector3(-822.95f, 135.46f, -147.76f)}); //cliffs above bear's bend (near the trailer)
        public static SpawnRegionTerritory BR_ForagerRemnant = new SpawnRegionTerritory(new Vector3(-824f, 141.77f, 548.57f), new List<Vector3>() { new Vector3(-737.84f, 94.01f, 270.19f), new Vector3(-914.74f, 129.63f, 308.45f), new Vector3(-867.82f, 89.07f, 421.80f), new Vector3(-679.68f, 88.49f, 377.31f), new Vector3(-556.76f, 93.09f, 444.18f), new Vector3(-530.05f, 89.92f, 191.56f)});
        public static SpawnRegionTerritory BR_JailersResidence = new SpawnRegionTerritory(new Vector3(45f, 189.29f, -657f), new List<Vector3>() { new Vector3(28.76f, 136.18f, -775.81f), new Vector3(14.66f, 154.87f, -628.06f), new Vector3(54.11f, 198.68f, -557.26f), new Vector3(19.53f, 192.35f, -423.21f), new Vector3(-159.43f, 171.37f, -502.12f)}); //cliffs above Jailer's residence
        public static SpawnRegionTerritory BR_CuttysCave = new SpawnRegionTerritory(new Vector3(360.23f, 191.94f, -660.05f), new List<Vector3>() { new Vector3(277.41f, 174.37f, -576.40f), new Vector3(268.01f, 191.18f, -680.62f), new Vector3(500.34f, 186.12f, -559.81f), new Vector3(370.15f, 223.22f, -473.70f)});
        public static SpawnRegionTerritory BR_ClearCutPath = new SpawnRegionTerritory(new Vector3(338.23f, 242.29f, 116.68f), new List<Vector3>() { new Vector3(296.03f, 232.85f, 68.35f), new Vector3(506.68f, 234.32f, 30.60f), new Vector3(624.82f, 251.39f, 191.39f), new Vector3(282.46f, 251.25f, 293.62f)}); //hidden path up in the hills near foreman's clearcut
        public static SpawnRegionTerritory BR_SprucePatch = new SpawnRegionTerritory(new Vector3(609.85f, 254.22f, -126.25f), new List<Vector3>() { new Vector3(642.77f, 244.52f, -158.75f), new Vector3(521.17f, 211.77f, -139.18f) }); //patch of spruce trees on the road to the mine
        public static SpawnRegionTerritory BR_Bricklayers = new SpawnRegionTerritory(new Vector3(1040.7f, 261.35f, 106.25f), new List<Vector3>() { new Vector3(895.47f, 231.20f, -170.10f), new Vector3(955.12f, 250.54f, 83.25f), new Vector3(814.05f, 227.75f, -48.22f), new Vector3(723.81f, 281.96f, 43.76f), new Vector3(878.13f, 261.42f, 199.11f)});
        public static SpawnRegionTerritory BR_HuntingBlind = new SpawnRegionTerritory(new Vector3(784f, 304.89f, 408f), new List<Vector3>() { new Vector3(695.52f, 267.94f, 259.49f), new Vector3(877.12f, 282.42f, 434.97f), new Vector3(1098.84f, 302.45f, 552.65f), new Vector3(853.80f, 306.22f, 706.57f), new Vector3(698.53f, 334.40f, 738.22f), new Vector3(657.02f, 360.79f, 843.18f) }); //near hunting blind along road to the mine

        public static SpawnRegionTerritory TWM_Entrance = new SpawnRegionTerritory(new Vector3(546.63f, 224.21f, 75.63f), new List<Vector3>() { new Vector3(606.80f, 163.01f, 208.77f), new Vector3(846.36f, 196.25f, 85.64f), new Vector3(553.31f, 169.35f, 487.77f)});
        public static SpawnRegionTerritory TWM_Wing = new SpawnRegionTerritory(new Vector3(1693.35f, 200.24f, 422.42f), new List<Vector3>() { new Vector3(1409.69f, 149.59f, 287.46f), new Vector3(1516.21f, 185.79f, 123.19f), new Vector3(1728.09f, 152.75f, 221.05f), new Vector3(1781.54f, 158.13f, 506.47f)});
        public static SpawnRegionTerritory TWM_River = new SpawnRegionTerritory(new Vector3(180.1f, 248.22f, 688.53f), new List<Vector3>() { new Vector3(308.20f, 239.57f, 568.29f), new Vector3(266.02f, 251.28f, 827.42f), new Vector3(448.32f, 219.88f, 880.78f), new Vector3(833.74f, 216.21f, 856.98f), new Vector3(325.43f, 219.70f, 993.36f)}); //area off the side of the river 
        public static SpawnRegionTerritory TWM_AndresPeak = new SpawnRegionTerritory(new Vector3(337.52f, 334.33f, 1231.58f), new List<Vector3>() { new Vector3(600.54f, 277.10f, 1149.13f), new Vector3(276.06f, 305.08f, 1387.16f) }); //adjacent peak to Andre's Peak. Near Blackrock cave
        public static SpawnRegionTerritory TWM_EricsFalls = new SpawnRegionTerritory(new Vector3(254.82f, 364.14f, 1600f), new List<Vector3>() { new Vector3(453.57f, 349.42f, 1599.65f), new Vector3(264.85f, 354.16f, 1631.27f), new Vector3(553.85f, 346.44f, 1760.46f)});
        public static SpawnRegionTerritory TWM_Backside = new SpawnRegionTerritory(new Vector3(993.87f, 262.67f, 1774.52f), new List<Vector3>() { new Vector3(911.60f, 323.81f, 1765.94f), new Vector3(923.19f, 291.89f, 1651.54f), new Vector3(1362.52f, 222.66f, 1814.46f), new Vector3(1153.21f, 267.02f, 1703.95f), new Vector3(1744.09f, 207.90f, 1758.78f), new Vector3(1438.78f, 213.27f, 1652.19f)}); //area behind the summit (idk if i'll keep this one, it's fairly open)
        public static SpawnRegionTerritory TWM_RavineRidge = new SpawnRegionTerritory(new Vector3(1514f, 166.9f, 1160.48f), new List<Vector3>() { new Vector3(1568.50f, 234.14f, 1284.70f), new Vector3(1520.10f, 142.09f, 1079.67f), new Vector3(1697.17f, 151.11f, 1098.15f), new Vector3(1365.73f, 144.33f, 1215.44f), new Vector3(1462.68f, 51.67f, 979.59f)}); //area near the big indoor cave along the ridge off echo ravine 

        public static SpawnRegionTerritory AC_Bunker = new SpawnRegionTerritory(new Vector3(13.34f, 162.24f, -751.76f), new List<Vector3>() { });
        public static SpawnRegionTerritory AC_PillarsFootrest = new SpawnRegionTerritory(new Vector3(-319.34f, 161.6f, -504.38f), new List<Vector3>() { new Vector3(-347.49f, 88.28f, -598.76f), new Vector3(-49.94f, 96.92f, -718.50f), new Vector3(-327.47f, 173.68f, -432.73f), new Vector3(-485.44f, 156.73f, -182.71f)}); //hill near pillar's footrest
        public static SpawnRegionTerritory AC_BirchForest = new SpawnRegionTerritory(new Vector3(-784.34f, 236.88f, -101.95f), new List<Vector3>() { new Vector3(-544.92f, 163.73f, -114.63f), new Vector3(-705.14f, 197.89f, -95.46f), new Vector3(-655.48f, 108.74f, 248.09f)});
        public static SpawnRegionTerritory AC_ClimbersPlateau1 = new SpawnRegionTerritory(new Vector3(-145.4f, 184.93f, -453.76f), new List<Vector3>() { new Vector3(32.71f, 138.12f, -121.36f), new Vector3(158.01f, 130.09f, -144.17f), new Vector3(-22.94f, 150.07f, -434.27f), new Vector3(266.42f, 126.91f, -380.85f)}); //near climber's cave 
        public static SpawnRegionTerritory AC_ClimbersPlateau2 = new SpawnRegionTerritory(new Vector3(-211.54f, 137.22f, -193.37f), new List<Vector3>() { }); //near the other cave on the plateau
        public static SpawnRegionTerritory AC_FireOverlook = new SpawnRegionTerritory(new Vector3(381.54f, 199.49f, -75.8f), new List<Vector3>() { new Vector3(675.20f, 185.14f, -167.41f), new Vector3(538.26f, 188.09f, -79.90f), new Vector3(380.50f, 199.87f, -82.96f), new Vector3(409.27f, 215.51f, 90.40f), new Vector3(740.99f, 248.70f, 143.55f), new Vector3(644.98f, 252.41f, 150.06f), new Vector3(582.04f, 221.71f, 46.19f)});
        public static SpawnRegionTerritory AC_WaterfallBasin = new SpawnRegionTerritory(new Vector3(-72f, 192.41f, 762.65f), new List<Vector3>() { new Vector3(-341.91f, 160.60f, 712.66f), new Vector3(-267.14f, 139.67f, 829.41f)});
        public static SpawnRegionTerritory AC_NarrowFalls = new SpawnRegionTerritory(new Vector3(546f, 99.65f, 500f), new List<Vector3>() { new Vector3(323.29f, 70.30f, 252.55f), new Vector3(447.39f, 81.71f, 379.13f)});

        public static SpawnRegionTerritory FA_Junkers = new SpawnRegionTerritory(new Vector3(-1125.44f, 303.96f, -1271.95f), new List<Vector3>() { new Vector3(-1015.60f, 251.16f, -1264.84f), new Vector3(-1114.84f, 236.61f, -1123.11f), new Vector3(-973.54f, 250.52f, -968.15f), new Vector3(-1142.46f, 151.00f, -775.31f)}); //hills behind Junker's Paddock
        public static SpawnRegionTerritory FA_WaterfallPlateau = new SpawnRegionTerritory(new Vector3(-1057.11f, 272.82f, -548.1f), new List<Vector3>() { new Vector3(-1160.23f, 276.92f, -579.91f), new Vector3(-1002.64f, 252.81f, -302.16f), new Vector3(-1117.84f, 234.66f, -172.97f)}); //plateau above the waterfall open cave
        public static SpawnRegionTerritory FA_FrozenRiver = new SpawnRegionTerritory(new Vector3(-562.19f, 133.54f, -909.94f), new List<Vector3>() { new Vector3(-531.27f, 133.46f, -935.38f), new Vector3(-507.74f, 151.00f, -620.96f), new Vector3(-286.30f, 165.38f, -1009.13f), new Vector3(-105.78f, 133.75f, -1191.29f), new Vector3(117.89f, 157.84f, -756.28f), new Vector3(-350.51f, 152.55f, -625.52f), new Vector3(-621.89f, 151.02f, -641.12f)}); //frozen river along the road to the airfield 
        public static SpawnRegionTerritory FA_RoadForest = new SpawnRegionTerritory(new Vector3(-1255.17f, 256.22f, 788.86f), new List<Vector3>() { new Vector3(-1002.55f, 180.51f, 673.89f), new Vector3(-1290.05f, 250.67f, 514.84f), new Vector3(-1366.94f, 238.69f, 596.65f), new Vector3(-1228.31f, 252.02f, 746.40f), new Vector3(-870.64f, 197.24f, 930.21f), new Vector3(-540.18f, 296.20f, 1342.46f), new Vector3(-902.40f, 318.77f, 1405.86f)}); //forest near the winding road passing through the middle of the field
        public static SpawnRegionTerritory FA_MindfulCabin = new SpawnRegionTerritory(new Vector3(-1175.8f, 283.08f, 1318f), new List<Vector3>() { new Vector3(-1002.55f, 180.51f, 673.89f), new Vector3(-1290.05f, 250.67f, 514.84f), new Vector3(-1366.94f, 238.69f, 596.65f), new Vector3(-1228.31f, 252.02f, 746.40f), new Vector3(-870.64f, 197.24f, 930.21f), new Vector3(-540.18f, 296.20f, 1342.46f), new Vector3(-902.40f, 318.77f, 1405.86f)});
        public static SpawnRegionTerritory FA_MindfulCabin2 = new SpawnRegionTerritory(new Vector3(-536.38f, 211.24f, 1208.07f), new List<Vector3>() { new Vector3(-719.33f, 196.44f, 1008.51f), new Vector3(-352.94f, 158.69f, 1139.04f), new Vector3(-462.57f, 207.32f, 1283.95f), new Vector3(-729.91f, 216.91f, 1185.72f), new Vector3(-768.59f, 222.87f, 1064.87f)}); //below mindful cabin (not sure if i'll keep this one)
        public static SpawnRegionTerritory FA_TransitionCave = new SpawnRegionTerritory(new Vector3(1021.28f, 255.63f, 1362.33f), new List<Vector3>() { new Vector3(910.55f, 208.20f, 1222.79f), new Vector3(1229.21f, 224.59f, 1296.63f), new Vector3(1261.88f, 175.40f, 1088.60f), new Vector3(1216.88f, 202.64f, 964.30f), new Vector3(977.08f, 191.35f, 896.28f)}); //corner of the region near the transition cave
        public static SpawnRegionTerritory FA_ChopperCrash = new SpawnRegionTerritory(new Vector3(1058.6f, 211.14f, 325.87f), new List<Vector3>() { new Vector3(1171.71f, 180.81f, 435.53f), new Vector3(1339.22f, 210.72f, 454.54f), new Vector3(1007.82f, 216.94f, 351.15f), new Vector3(1136.90f, 207.11f, 212.29f), new Vector3(1191.93f, 188.43f, 217.57f)});
        public static SpawnRegionTerritory FA_Shortcut = new SpawnRegionTerritory(new Vector3(248.9f, 284.14f, -1227.87f), new List<Vector3>() { });
        public static SpawnRegionTerritory FA_JustysHovel = new SpawnRegionTerritory(new Vector3(845.9f, 290.59f, -1108.12f), new List<Vector3>() { new Vector3(897.52f, 243.58f, -809.85f), new Vector3(1030.65f, 280.32f, -779.83f), new Vector3(1055.03f, 282.74f, -920.34f), new Vector3(882.74f, 293.39f, -1059.84f), new Vector3(729.02f, 295.90f, -959.66f)});

        public static SpawnRegionTerritory SP_Teardrop1 = new SpawnRegionTerritory(new Vector3(946.9f, 339.26f, -402.48f), new List<Vector3>() { new Vector3(824.80f, 259.62f, -321.72f), new Vector3(805.72f, 294.54f, -472.34f), new Vector3(932.96f, 291.05f, -521.14f)}); //bottom of Ogre's Teardrop
        public static SpawnRegionTerritory SP_BasinPlateau = new SpawnRegionTerritory(new Vector3(1085.2f, 278.4f, -13f), new List<Vector3>() { new Vector3(844.34f, 155.29f, 105.02f), new Vector3(1026.31f, 177.01f, -187.14f), new Vector3(734.12f, 121.72f, -34.95f), new Vector3(411.02f, 116.27f, -400.23f), new Vector3(636.29f, 126.15f, 258.22f)}); //small plateau in the valley basin                                                                                                                  //public static SpawnRegionTerritory SP_ClimbingSpot = new SpawnRegionTerritory(new Vector3(628.92f, 219.96f, -505.9f), new List<Vector3>() {}); //climbing spot above the Last Lonely House
        public static SpawnRegionTerritory SP_CaveRidge = new SpawnRegionTerritory(new Vector3(1133.16f, 432.05f, 314.58f), new List<Vector3>() { new Vector3(982.45f, 387.34f, 398.09f), new Vector3(1111.27f, 372.19f, 107.02f)}); //cliff face near the transition cave
        public static SpawnRegionTerritory SP_Teardrop2 = new SpawnRegionTerritory(new Vector3(1078.3f, 494.64f, -655.15f), new List<Vector3>() { new Vector3(1042.45f, 443.58f, -513.42f), new Vector3(967.80f, 445.34f, -678.93f), new Vector3(778.26f, 450.65f, -913.60f)}); //top of Ogre's Teardrop
        public static SpawnRegionTerritory SP_PlaneCrash = new SpawnRegionTerritory(new Vector3(1045.3f, 615.02f, -738.27f), new List<Vector3>() { new Vector3(999.47f, 598.28f, -774.73f), new Vector3(732.31f, 585.51f, -977.64f), new Vector3(851.94f, 642.26f, -1154.36f)});
        public static SpawnRegionTerritory SP_MiddlePlateau = new SpawnRegionTerritory(new Vector3(24.1f, 531.07f, -768.09f), new List<Vector3>() { }); //plateau in the middle of the region (idk if i'll keep this one)
        public static SpawnRegionTerritory SP_Mine = new SpawnRegionTerritory(new Vector3(46.06f, 650.88f, -1136.87f), new List<Vector3>() { new Vector3(190.56f, 563.24f, -992.55f)});
        public static SpawnRegionTerritory SP_FrozenWaterfalls = new SpawnRegionTerritory(new Vector3(192.85f, 242.89f, 609.67f), new List<Vector3>() { }); //frozen waterfalls along the road up the mountain
        public static SpawnRegionTerritory SP_BanefulBridge = new SpawnRegionTerritory(new Vector3(283.16f, 120.16f, 911.51f), new List<Vector3>() { new Vector3(366.02f, 62.12f, 839.02f), new Vector3(318.27f, 81.68f, 726.28f), new Vector3(307.35f, 3.01f, 1115.23f)}); //below baneful bridge
        public static SpawnRegionTerritory SP_HiddenPath = new SpawnRegionTerritory(new Vector3(225.27f, 187.92f, 993.5f), new List<Vector3>() { new Vector3(331.54f, 104.80f, 943.92f), new Vector3(223.84f, 188.97f, 999.91f), new Vector3(251.06f, 187.06f, 837.12f)}); //hidden path below baneful bridge


        public static List<GameObject> cylMarkers = new List<GameObject>();
        public static List<GameObject> topMarkers = new List<GameObject>();

        private static List<SpawnRegionTerritory> GetSpawnRegionsByRegion(string region)
        {
            switch (region)
            {
                case "LakeRegion":
                    return new List<SpawnRegionTerritory>() { ML_WoodsCabin, ML_Backwoods, ML_DeadfallAreaForest, ML_LoggingRoad, ML_LookoutMountain, ML_LakeHill, ML_Plateau };
                case "RuralRegion":
                    return new List<SpawnRegionTerritory>() { PV_MountainRoad, PV_SignalHill, PV_PicnicArea, PV_ThompsonForest, PV_ThompsonWaterfall, PV_MiningRoad, PV_DerelictCabins, PV_MisticFalls, PV_SkeetersRidge, PV_Backroad, PV_ThreeStrikes, PV_RiverWoods };
                case "MountainTownRegion":
                    return new List<SpawnRegionTerritory>() { MT_PlanePath, MT_CaveField, MT_LoggingTrailer, MT_Church, MT_FarmHill, MT_Farm, MT_WoodLot };
                case "RiverValleyRegion":
                    return new List<SpawnRegionTerritory>() { HRV_Stairsteps, HRV_OffsetFalls, HRV_NorthCliffs, HRV_MonolithLake, HRV_MammothFalls2, HRV_HushedRiverForest };
                case "BlackrockRegion":
                    return new List<SpawnRegionTerritory>() { BR_BearBend, BR_Bricklayers, BR_ClearCutPath, BR_CuttysCave, BR_ForagerRemnant, BR_HuntingBlind, BR_JailersResidence, BR_SprucePatch, BR_Substation };
                case "CrashMountainRegion":
                    return new List<SpawnRegionTerritory>() { TWM_AndresPeak, TWM_Backside, TWM_Entrance, TWM_EricsFalls, TWM_RavineRidge, TWM_River, TWM_Wing };
                case "AshCanyonRegion":
                    return new List<SpawnRegionTerritory>() { AC_BirchForest, AC_Bunker, AC_ClimbersPlateau1, AC_ClimbersPlateau2, AC_FireOverlook, AC_NarrowFalls, AC_PillarsFootrest, AC_WaterfallBasin };
                case "AirfieldRegion":
                    return new List<SpawnRegionTerritory>() { FA_ChopperCrash, FA_FrozenRiver, FA_Junkers, FA_JustysHovel, FA_MindfulCabin, FA_MindfulCabin2, FA_RoadForest, FA_Shortcut, FA_TransitionCave, FA_WaterfallPlateau };
                case "MountainPassRegion":
                    return new List<SpawnRegionTerritory>() { SP_BanefulBridge, SP_BasinPlateau, SP_CaveRidge, SP_FrozenWaterfalls, SP_HiddenPath, SP_MiddlePlateau, SP_Mine, SP_PlaneCrash, SP_Teardrop1, SP_Teardrop2 };
                default: return null;
            }
        }

        public static SpawnRegionTerritory? GetRandomSpawnRegion(string region)
        {
            var random = new Random();
            List<SpawnRegionTerritory> list = GetSpawnRegionsByRegion(region);

            if (list == null || list.Count == 0) return null;

            return list[random.Next(list.Count)];
        }

        public static void AddMarkersToSpawnRegions(string region)
        {

            cylMarkers.ForEach(marker => GameObject.Destroy(marker));
            topMarkers.ForEach(marker => GameObject.Destroy(marker));
            cylMarkers.Clear();
            topMarkers.Clear();

            List<SpawnRegionTerritory> spawns = GetSpawnRegionsByRegion(region);
            var diameter = 5f;
            Color orange = new Color(1.0f, 0.64f, 0.0f);

            if (spawns != null)
            {
                foreach (var territory in spawns)
                {

                    var pos = territory.position;

                    Color color = Main.CustomCougarManager.currentTerritory.position == pos ? Color.green : orange;

                    GameObject spawnRegionCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    UnityEngine.Object.Destroy(spawnRegionCylinder.GetComponent<Collider>());
                    spawnRegionCylinder.transform.localScale = new Vector3(diameter, 100f, diameter);
                    spawnRegionCylinder.transform.position = pos;
                    spawnRegionCylinder.GetComponent<Renderer>().material.color = color;

                    cylMarkers.Add(spawnRegionCylinder);

                    GameObject spawnRegionTop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(spawnRegionTop.GetComponent<Collider>());
                    spawnRegionTop.transform.localScale = new Vector3(diameter * 3f, diameter * 3f, diameter * 3f);
                    spawnRegionTop.transform.position = pos + new Vector3(0, 100f, 0);
                    spawnRegionTop.GetComponent<Renderer>().material.color = color;
                    spawnRegionTop.transform.SetParent(spawnRegionCylinder.transform);

                    topMarkers.Add(spawnRegionTop);
                }
            }

        }

        public static CarcassSite SpawnRavagedCarcassAtPosition(Vector3 pos)
        {

            GameObject prefab = Main.CustomCougarManager.carcassPrefab;

            if (prefab != null)
            {
                CarcassSite carcass = prefab.GetComponent<CarcassSite>();
                if (carcass == null)
                {
                    Main.Logger.Log("Could not find carcass component on prefab!", ComplexLogger.FlaggedLoggingLevel.Error);
                    return null;
                }
                CarcassSite newCarcass = GameObject.Instantiate(carcass, pos, carcass.transform.rotation);
                return newCarcass;
            }
            else
            {
                Main.Logger.Log("Could not find carcass prefab!", ComplexLogger.FlaggedLoggingLevel.Error);
                return null;
            }
        }
    }
}
