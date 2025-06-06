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

                CustomCougarManager.MoveSpawnRegion();

            }

        }

        



    }
}
