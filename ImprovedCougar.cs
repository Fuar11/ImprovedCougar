using ComplexLogger;
using Il2Cpp;
using ExpandedAiFramework;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;
using ImprovedCougar.SpawnRegions;
using AudioMgr;

namespace ImprovedCougar
{
    public class Main : MelonMod
    {
        internal static ComplexLogger<Main> Logger = new();
        internal static CustomCougarManager CustomCougarManager;
        internal static ClipManager cougarAudioManager;

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

        // This now instantiates only once. Moved other things to EAF scene trigger handlers.
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (CustomCougarManager == null)
            {
                GameObject ccm = new() { name = "CustomCougarManager", layer = vp_Layer.Default };
                GameObject.DontDestroyOnLoad(ccm);
                CustomCougarManager ??= ccm.AddComponent<CustomCougarManager>();
                EAFManager.Instance.HotSwapSubManager(EAFManager.HotSwappableSubManagers.CougarManager, CustomCougarManager);
            }

            if (sceneName.ToLowerInvariant().Contains("menu"))
            {
                cougarAudioManager = AudioMaster.NewClipManager();
                cougarAudioManager.LoadClipsFromDir("ImprovedCougar/Audio", ClipManager.LoadType.Compressed);
            }

        }
    }
}
