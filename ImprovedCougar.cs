using ComplexLogger;
using Il2Cpp;
using ExpandedAiFramework;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;
using ImprovedCougar.SpawnRegions;

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

            IMapDataManager ImapDataManager;
            if(EAFManager.Instance.DataManager.MapDataManagers.TryGetValue(typeof(WanderPath), out ImapDataManager))
            {

                WanderPathManager wanderPathManager = (WanderPathManager)ImapDataManager;
                wanderPathManager.ScheduleLoadAdditional("ImprovedCougar/WanderPaths.json");
                Logger.Log("Loading paths from file.", FlaggedLoggingLevel.Trace);

            }
            return EAFManager.Instance.RegisterSpawnableAi(typeof(CustomCougar), CustomCougar.CustomCougarSettings);
        }

        public override void OnUpdate()
        {

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.DownArrow))
            {
                SpawnRegionPositions.AddMarkersToSpawnRegions(SceneUtilities.GetActiveSceneName());
            }


        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (SceneUtilities.IsSceneMenu(sceneName))
            {
                GameObject.Destroy(GameObject.Find("CustomCougarManager"));
                CustomCougarManager = null;
                return;
            }
            if (SceneUtilities.IsScenePlayable(sceneName))
            {

                if (!sceneName.Contains("_SANDBOX") && !sceneName.Contains("_DLC") && !sceneName.Contains("_WILDLIFE") && !sceneName.Contains("_VFX"))
                {

                    if (CustomCougarManager == null)
                    {
                        GameObject ccm = new() { name = "CustomCougarManager", layer = vp_Layer.Default };
                        UnityEngine.Object.Instantiate(ccm, GameManager.GetVpFPSPlayer().transform);
                        GameObject.DontDestroyOnLoad(ccm);
                        CustomCougarManager ??= ccm.AddComponent<CustomCougarManager>();
                        EAFManager.Instance.RegisterSubmanager(typeof(CustomCougar), CustomCougarManager);
                    }
                    else
                    {
                        Logger.Log("Setting toMoveSpawnRegion to true", FlaggedLoggingLevel.Debug);
                        CustomCougarManager.toMoveSpawnRegion = true;
                    }
                }
            }
           

        }

        



    }
}
