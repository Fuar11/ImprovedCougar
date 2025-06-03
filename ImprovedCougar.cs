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

                UpdateCougarTerritory(sceneName);

            }

        }

        private void UpdateCougarTerritory(string scene)
        {

            if(scene == "LakeRegion")
            {

                //grab initial territory object and move it around
                GameObject territoryObject = GameObject.Find("Design/Cougar/AttackZoneArea_a/CougarTerritoryZone_a_T1");
                territoryObject.transform.position = new Vector3(107.14f, 2.36f, 23.92f);

                GameObject spawnRegionObject = territoryObject.transform.GetChild(0).gameObject;
                spawnRegionObject.gameObject.SetActive(true); //set spawn region object to true
                territoryObject.transform.GetChild(1).gameObject.SetActive(true); //set audio object to true
                territoryObject.transform.GetChild(2).gameObject.SetActive(true); //set wander region object to true, idk if this is used

                SpawnRegion sr = spawnRegionObject.GetComponent<SpawnRegion>();
                Il2Cpp.SpawnRegionManager spawnRegionManager = GameManager.GetSpawnRegionManager();

                if (sr != null)
                {
                    if (!sr.m_Registered)
                    {
                        if (spawnRegionManager != null)
                        {
                            spawnRegionManager.Add(sr);
                            sr.m_Registered = true;
                        }
                    }
                }

                if (spawnRegionManager != null)
                {

                    spawnRegionManager.MaybeEnableSpawnRegionsInRange(sr, 100, true);

                }

            }

            
        }



    }
}
