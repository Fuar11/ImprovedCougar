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
using static Il2Cpp.CarcassSite;

namespace ImprovedCougar
{
    [RegisterTypeInIl2Cpp]
    internal class CustomCougarManager : MonoBehaviour, ISubManager
    {

        protected EAFManager mManager;

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

        public void Start()
        {

            //i think this checks if the feature is enabled
            if (!GameManager.GetCougarManager().IsEnabled) return;

            //call load data stuff here

            //if load data null 
           
            if(!cougarArrived && daysToArrive == 0)
            {
                var random = new System.Random();

                daysToArrive += random.Next(maxTimeTillCougarArrivalInDays, maxTimeTillCougarArrivalInDays);

                //debug
                cougarArrived = true;
                //remove this after
            }


        }

        public void Update()
        {

            if(GameManager.m_IsPaused || GameManager.s_IsGameplaySuspended) return;
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
                SpawnRegion spawnRegion = spawnRegionObject.GetComponent<SpawnRegion>();
                if (spawnRegion != null && spawnRegion.m_Registered == false)
                {
                    Main.Logger.Log($"Adding and registering SpawnRegion in region {GameManager.m_ActiveScene}", FlaggedLoggingLevel.Debug);
                    mManager.SpawnRegionManager.Add(spawnRegion);
                    spawnRegion.m_Registered = true; //idk if this is needed anymore
                }
                else
                {
                    Main.Logger.Log("Cannot find SpawnRegion!", FlaggedLoggingLevel.Error);
                    return;
                }
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

        public void UpdateFromManager()
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
        }

        public void OnSaveGame()
        {
        }

        public void OnLoadGame()
        {
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
