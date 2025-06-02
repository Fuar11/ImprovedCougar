using ComplexLogger;
using Il2Cpp;
using ExpandedAiFramework;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

namespace ImprovedCougar
{
    public class Main : MelonMod
    {
        internal static ComplexLogger<Main> Logger = new();
        internal static CustomCougarManager CustomCougarManager;

        public override void OnInitializeMelon()
        {
            Logger.Log("Improved Cougar is online", FlaggedLoggingLevel.Always);
            Initialize();
            //Settings.CustomSettings.OnLoad();
        }
        
        protected bool Initialize()
        {
            CustomCougar.CustomCougarSettings.AddToModSettings("Improved Cougar", MenuType.Both);
            return EAFManager.Instance.RegisterSpawnableAi(typeof(CustomCougar), CustomCougar.CustomCougarSettings);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (SceneUtilities.IsScenePlayable(sceneName))
            {
                GameObject ccm = new() { name = "CustomCougarManager", layer = vp_Layer.Default };
                UnityEngine.Object.Instantiate(ccm, GameManager.GetVpFPSPlayer().transform);
                GameObject.DontDestroyOnLoad(ccm);
                CustomCougarManager ??= ccm.AddComponent<CustomCougarManager>();

                AddNewSpawnRegions(sceneName);

            }

        }

        private void AddNewSpawnRegions(string sceneName)
        {

            GameObject baseSpawnRegionObj = new() { layer = vp_Layer.TriggerIgnoreRaycast };

            if (sceneName == "LakeRegion")
            {

                string path = "Design/Cougar/AttackZoneArea_a";
                GameObject parent = GameObject.Find(path);
                SpawnRegion baseSpawnRegion = GameObject.Find(path + "/CougarTerritoryZone_a_T1").transform.GetChild(0).GetComponent<SpawnRegion>();
                baseSpawnRegionObj.name = "ModCougarTerritoryZone1";
                Vector3 pos = new Vector3(164.56f, 1.91f, 11.00f);
                GameObject newSpawnRegionObj = UnityEngine.Object.Instantiate(baseSpawnRegionObj, parent.transform);
                Logger.Log("Added spawnregion object to scene", FlaggedLoggingLevel.Debug);
                SpawnRegion sr = newSpawnRegionObj.AddComponent<SpawnRegion>();
                Logger.Log($"Added spawnregion component to object: {sr != null}", FlaggedLoggingLevel.Debug);
                sr = baseSpawnRegion;
                newSpawnRegionObj.transform.position = pos;

            }


        }


    }
}
