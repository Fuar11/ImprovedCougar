global using VanillaCougarManager = Il2CppTLD.AI.CougarManager;
using ComplexLogger;
using ExpandedAiFramework;
using Il2Cpp;
using Il2CppTLD.AI;
using ImprovedCougar.SpawnRegions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using ModData;
using MelonLoader.TinyJSON; 
using static Il2Cpp.CarcassSite;

namespace ImprovedCougar
{
    [RegisterTypeInIl2Cpp]
    internal class CustomCougarManager : MonoBehaviour, ICougarManager
    {
        // Logger (though you can also use EAF's which is thread safe)
        internal static ComplexLogger<Main> Logger = new();

        // EAF ICougarManager Implementation
        protected VanillaCougarManager mVanillaManager;
        public VanillaCougarManager VanillaCougarManager
        { 
            get 
            {
                if (mVanillaManager == null)
                {
                    mVanillaManager = GameManager.m_CougarManager;
                }
                return mVanillaManager;
            }
        }
        
        bool ICougarManager.OverrideCustomSpawnRegionType(SpawnRegion spawnRegion, SpawnRegionModDataProxy proxy, TimeOfDay timeOfDay, out CustomSpawnRegion customSpawnRegion)
        {
            customSpawnRegion = new CustomCougarSpawnRegion(spawnRegion, proxy, timeOfDay);
            return false;
        }

        // Because you are running as a mono and not running on EAF's loop, these are important to prevent running during main menu and such
        public bool IsMenuScene = true; // initialize TRUE to prevent updates at start during main menu
        public bool HasStarted = true; // initialize TRUE to prevent start running in main menu
        
        // ModData 
        private ModDataManager modData = new ModDataManager("ImprovedCougar", false);

        protected EAFManager mManager;

        //spawn region

        public SpawnRegion spawnRegion = null;

        //spawn region positions
        public Vector3? currentSpawnRegion = Vector3.zero;

        public Vector3? lastSpawnRegionML = null;
        public Vector3? lastSpawnRegionPV = null;
        public Vector3? lastSpawnRegionMT = null;
        public Vector3? lastSpawnRegionTWM = null;
        public Vector3? lastSpawnRegionBRM = null;
        public Vector3? lastSpawnRegionBR = null;
        public Vector3? lastSpawnRegionAC = null;
        public Vector3? lastSpawnRegionHRV = null;
        public Vector3? lastSpawnRegionFA = null;
        public Vector3? lastSpawnRegionSP = null;

        //add modded regions here

        //scene

        private string latestRegion = string.Empty;

        //flags
        public bool cougarArrived = true;
        public bool toMoveSpawnRegion = false;
        public bool toSetNewSpawnRegion = false;

        //time
        public float timeToMoveSpawnRegion = 0;
        protected float debugTimeToMoveSpawnRegionInHours = 0;
        public int maxTimeTillNextSpawnRegionMoveInHours = Settings.CustomSettings.settings.maxTimeToMove; 
        public int minTimeTillNextSpawnRegionMoveInHours = Settings.CustomSettings.settings.minTimeToMove; 

        public int daysToArrive = 0;
        public int maxTimeTillCougarArrivalInDays = 2; //Settings.CustomSettings.settings.maxTimeToArrive; 
        public int minTimeTillCougarArrivalInDays = 1; //Settings.CustomSettings.settings.minTimeToArrive;

        [Serializable]
        private class LoadData 
        {
            public bool CougarArrived;
            public int DaysToArrive;
            public LoadData() {}
        }


        public void Start()
        {
            if (HasStarted)
            {
                return;
            }
            HasStarted = true;

            //i think this checks if the feature is enabled
            if (!GameManager.GetCougarManager().IsEnabled) return;

            // You can do other stuff here if you want, I configured this to run every scene start based on EAF's timing of InitializeScene
            // But honestly now that it's persistent across saves and uses ModData properly, you can probably just drop Start()...
        }

        public void Update()
        {
            if (IsMenuScene) return;
            if (GameManager.m_IsPaused || GameManager.s_IsGameplaySuspended) return;
            if (SaveGameSystem.IsRestoreInProgress()) return;
            if (!GameManager.GetCougarManager().IsEnabled) return;

            PlayerStruggle struggle = GameManager.m_PlayerStruggle;
            if (struggle != null && struggle.InStruggle()) return;

            //handle spawning and spawn point moving, wander paths, all that stuff

            float currentDays = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() / 24f;

            //this is slightly more efficient
            if (!cougarArrived && currentDays >= daysToArrive) cougarArrived = true;

            if (cougarArrived)
            {
                UpdateSpawnRegion();

                //debug
                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Z))
                {
                    Main.Logger.Log("Moving spawn region using key press.", FlaggedLoggingLevel.Debug);
                    SetSpawnRegion();
                    if (toMoveSpawnRegion) UpdateCougarSpawnRegionPosition(latestRegion);
                } 
            }
        }

        public void UpdateSpawnRegion()
        {

            if (!GameManager.GetWeatherComponent().IsIndoorScene())
            {

                //check if it's time to pick a new territory, eventually there will be more conditions than just this
                if (GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() >= timeToMoveSpawnRegion || toSetNewSpawnRegion)
                {
                    Main.Logger.Log("Player is outside, executing new spawn region logic", FlaggedLoggingLevel.Debug);
                    SetSpawnRegion();
                }
                else if (HasChangedRegions())
                {
                    Main.Logger.Log("Region has changed. Checking for existing spawn regions in this region.", FlaggedLoggingLevel.Debug);
                    SetCurrentSpawnRegionToExistingSpawnRegion(SceneUtilities.GetActiveSceneName());
                    if(currentSpawnRegion == null)
                    {
                        Main.Logger.Log("Region has changed but player has not yet been to this region. Setting a new spawn region.", FlaggedLoggingLevel.Debug);
                        SetSpawnRegion();
                    }

                    latestRegion = SceneUtilities.GetActiveSceneName();
                }

                if (toMoveSpawnRegion) UpdateCougarSpawnRegionPosition(latestRegion);
            }
        }
        public void SetSpawnRegion()
        {

            var random = new System.Random();
            var region = SceneUtilities.GetActiveSceneName();

            if (!RegionHasCougar(region)) return;

            latestRegion = SceneUtilities.GetActiveSceneName();

            //random for now
            currentSpawnRegion = SpawnRegionPositions.GetRandomSpawnRegion(region);
            if(currentSpawnRegion == null)
            {
                Main.Logger.Log("Unable to set spawn region!", FlaggedLoggingLevel.Error);
                return;
            }

            SetPerRegionSpawnRegion((Vector3)currentSpawnRegion, region);

            int timeToAdd = random.Next(minTimeTillNextSpawnRegionMoveInHours, maxTimeTillNextSpawnRegionMoveInHours);
            timeToMoveSpawnRegion = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + timeToAdd;
            debugTimeToMoveSpawnRegionInHours = timeToAdd; //this is just to view the time in hours to wait until the next one in ue
            toMoveSpawnRegion = true;
            toSetNewSpawnRegion = false;
            Main.Logger.Log($"New spawn region {currentSpawnRegion} set!", FlaggedLoggingLevel.Debug);
        }

        public void UpdateCougarSpawnRegionPosition(string scene)
        {

            if (RegionHasCougar(scene))
            {

                //grab initial territory object and move it around, gotta modify this for specific regions
                GameObject territoryObject = GameObject.Find("CougarTerritoryZone_a_T1");

                if (territoryObject == null)
                {
                    Main.Logger.Log("Cannot find cougar territory object!", FlaggedLoggingLevel.Error);
                    return;
                }

                if(currentSpawnRegion == null)
                {
                    Main.Logger.Log("Unable to move spawn region!", FlaggedLoggingLevel.Error);
                    return;
                }

                Main.Logger.Log($"Old spawn region position {territoryObject.transform.position.ToString()}", FlaggedLoggingLevel.Debug);

                territoryObject.transform.position = (Vector3)currentSpawnRegion;
                //territoryObject.transform.position = new Vector3(102.14f, 2.65f, 79.10f); //temporary testing spawn at trapper's

                Main.Logger.Log($"New spawn region position {territoryObject.transform.position.ToString()}", FlaggedLoggingLevel.Debug);

                GameObject spawnRegionObject = territoryObject.transform.GetChild(0).gameObject;
                spawnRegionObject.gameObject.SetActive(true); //set spawn region object to true
                territoryObject.transform.GetChild(1).gameObject.SetActive(true); //set audio object to true
                territoryObject.transform.GetChild(2).gameObject.SetActive(true); //set wander region object to true, idk if this is used

                //I don't know how this spawn region is set to active, or whatever equivalent we have in eaf
                spawnRegion = spawnRegionObject.GetComponent<SpawnRegion>();
                // merry christmas, mf
                EAFManager.Instance.SpawnRegionManager.Add(spawnRegion);
            }

            toMoveSpawnRegion = false;
        }

        private void SetCurrentSpawnRegionToExistingSpawnRegion(string region)
        {

            if (!RegionHasCougar(region)) return;

            switch (region)
            {
                case "LakeRegion":
                    currentSpawnRegion = lastSpawnRegionML;
                    break;
                case "RuralRegion":
                    currentSpawnRegion = lastSpawnRegionPV;
                    break;
                case "MountainTownRegion":
                    currentSpawnRegion = lastSpawnRegionMT;
                    break;
                case "CrashMountainRegion":
                    currentSpawnRegion = lastSpawnRegionTWM;
                    break;
                case "TracksRegion":
                    currentSpawnRegion = lastSpawnRegionBR;
                    break;
                case "AshCanyonRegion":
                    currentSpawnRegion = lastSpawnRegionAC;
                    break;
                case "BlackrockRegion":
                    currentSpawnRegion = lastSpawnRegionAC;
                    break;
                case "RiverValleyRegion": 
                    currentSpawnRegion = lastSpawnRegionHRV;
                    break;
                case "AirfieldRegion":
                    currentSpawnRegion = lastSpawnRegionFA;
                    break;
                case "MountainPassRegion":
                    currentSpawnRegion = lastSpawnRegionSP;
                    break;
                default: break;
            }

        }

        private void SetPerRegionSpawnRegion(Vector3 spawnRegion, string region)
        {

            switch (region)
            {
                case "LakeRegion":
                    lastSpawnRegionML = currentSpawnRegion;
                    break;
                case "RuralRegion":
                     lastSpawnRegionPV = currentSpawnRegion;
                    break;
                case "MountainTownRegion":
                     lastSpawnRegionMT = currentSpawnRegion;
                    break;
                case "CrashMountainRegion":
                    lastSpawnRegionTWM = currentSpawnRegion;
                    break;
                case "TracksRegion":
                    currentSpawnRegion = lastSpawnRegionBR;
                    break;
                case "AshCanyonRegion":
                    lastSpawnRegionAC = currentSpawnRegion;
                    break;
                case "BlackrockRegion":
                    lastSpawnRegionAC = currentSpawnRegion;
                    break;
                case "RiverValleyRegion":
                    lastSpawnRegionHRV = currentSpawnRegion;
                    break;
                case "AirfieldRegion":
                    lastSpawnRegionFA = currentSpawnRegion;
                    break;
                case "MountainPassRegion":
                    lastSpawnRegionSP = currentSpawnRegion;
                    break;
                default: break;
            }

        }

        public bool RegionHasCougar(string scene) //gotta add TLDev regions to this list at some point
        {
            return scene == "LakeRegion" || scene == "RuralRegion" || scene == "TracksRegion" || scene == "MountainCrashRegion" || scene == "MountainTownRegion" || scene == "AshCanyonRegion" || scene == "BlackrockRegion" || scene == "RiverValleyRegion" || scene == "AirfieldRegion" || scene == "MiningRegion" || scene == "MountainPassRegion" ? true : false;
        }

        private bool HasChangedRegions()
        {
            return latestRegion != SceneUtilities.GetActiveSceneName() ? true : false;
        }

        public void Initialize(EAFManager manager)
        {
            mManager = manager;
            Main.Logger.Log("CustomCougarManager initialized!", FlaggedLoggingLevel.Always);
        }

        void ISubManager.UpdateFromManager()
        {
        }

        public void Shutdown()
        {
        }

        public void OnStartNewGame()
        {
        }

        public void OnLoadScene(string sceneName)
        {
        }

        public void OnInitializedScene(string sceneName)
        {            
            if (SceneUtilities.IsSceneMenu(sceneName))
            {
                IsMenuScene = true;
                return;
            }
            if (SceneUtilities.IsScenePlayable(sceneName) 
                && !sceneName.Contains("_SANDBOX")
                && !sceneName.Contains("_DLC") 
                && !sceneName.Contains("_WILDLIFE") 
                && !sceneName.Contains("_VFX"))
            {
                Logger.Log("Setting toMoveSpawnRegion to true", FlaggedLoggingLevel.Debug);
                toMoveSpawnRegion = true;
                IsMenuScene = false;
            }
        }

        public void OnSaveGame()
        {
            LoadData loadData = new LoadData();
            loadData.CougarArrived = cougarArrived;
            loadData.DaysToArrive = daysToArrive;
            string json = JSON.Dump(loadData, EncodeOptions.PrettyPrint | EncodeOptions.NoTypeHints);
            if (json == null || json == string.Empty)
            {
                return;
            }
            modData.Save(json, "LoadData");
        }

        public void OnLoadGame()
        {         
            string loadDataJSON = modData.Load("LoadData");
            if (string.IsNullOrEmpty(loadDataJSON))
            {
                return;
            }
            Variant loadDataVariant = JSON.Load(loadDataJSON);
            LoadData loadData = new LoadData();
            JSON.Populate(loadDataVariant, loadData);
            if(!loadData.CougarArrived && loadData.DaysToArrive == 0)
            {
                var random = new System.Random();

                loadData.DaysToArrive += random.Next(maxTimeTillCougarArrivalInDays, maxTimeTillCougarArrivalInDays);

                //debug
                loadData.CougarArrived = true;
                //remove this after
            }
            
            daysToArrive = loadData.DaysToArrive;
            cougarArrived = loadData.CougarArrived;
        }

        public void OnQuitToMainMenu()
        {
        }

        public bool ShouldInterceptSpawn(CustomSpawnRegion region) => false;


        public void PostProcessNewSpawnModDataProxy(SpawnModDataProxy proxy)
        {
            proxy.ForceSpawn = true;
        }

        public Type SpawnType { get { return typeof(CustomCougarSpawnRegion); } }

    }
}
