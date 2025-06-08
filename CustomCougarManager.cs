using ComplexLogger;
using ExpandedAiFramework;
using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ImprovedCougar
{
    [RegisterTypeInIl2Cpp]
    internal class CustomCougarManager : MonoBehaviour
    {

        public Vector3? currentSpawnRegion = Vector3.zero;
        public bool toUpdateSpawnRegion = false;
        public string lastRegion = ""; //simply used to get the region
        public bool cougarArrived = true;

        //time
        public float timeToMoveSpawnRegion = 0;
        protected float debugTimeToMoveSpawnRegionInHours = 0;
        public int maxTimeTillNextSpawnRegionMoveInHours = 2; //Settings.CustomSettings.settings.maxTimeToMove; 
        public int minTimeTillNextSpawnRegionMoveInHours = 1; //Settings.CustomSettings.settings.minTimeToMove; 

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

            //this way is slightly more efficient
            if (!cougarArrived && currentDays >= daysToArrive) cougarArrived = true;

            if (cougarArrived)
            {
                UpdateSpawnRegion();

                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Z))
                {
                    Main.Logger.Log("Moving spawn region using key press.", ComplexLogger.FlaggedLoggingLevel.Debug);
                    SetSpawnRegion();
                }
            }

            

        }

        public void SetSpawnRegion()
        {

            var random = new System.Random();

            //random for now
            currentSpawnRegion = SpawnRegionPositions.GetRandomSpawnRegion(GetRegion());
            if(currentSpawnRegion == null)
            {
                Main.Logger.Log("Unable to set spawn region!", FlaggedLoggingLevel.Error);
                return;
            }
            int timeToAdd = random.Next(minTimeTillNextSpawnRegionMoveInHours, maxTimeTillNextSpawnRegionMoveInHours);
            timeToMoveSpawnRegion = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + timeToAdd;
            debugTimeToMoveSpawnRegionInHours = timeToAdd; //this is just to view the time in hours to wait until the next one in ue
            toUpdateSpawnRegion = true;
            Main.Logger.Log($"New spawn region {currentSpawnRegion} set!", FlaggedLoggingLevel.Debug);
        }

        public void UpdateSpawnRegion()
        {

            //check if it's time to pick a new territory, eventually there will be more conditions than just this
            if (!GameManager.GetWeatherComponent().IsIndoorScene())
            {
                if(GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() >= timeToMoveSpawnRegion)
                {
                    Main.Logger.Log("Player is outside, executing new spawn region logic", FlaggedLoggingLevel.Debug);
                    SetSpawnRegion();
                }

                if(toUpdateSpawnRegion) UpdateCougarTerritory(GetRegion());
            }
        }

        public void UpdateCougarTerritory(string scene)
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

                SpawnRegion sr = spawnRegionObject.GetComponent<SpawnRegion>();
                Il2Cpp.SpawnRegionManager spawnRegionManager = GameManager.GetSpawnRegionManager();

                if (sr != null)
                {
                    sr.m_Center = sr.transform.position;
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

            toUpdateSpawnRegion = false;
        }


        public bool RegionHasCougar(string scene) //gotta add TLDev regions to this list at some point
        {
            return scene == "LakeRegion" || scene == "RuralRegion" || scene == "TracksRegion" || scene == "MountainCrashRegion" || scene == "MountainTownRegion" || scene == "AshCanyonRegion" || scene == "BlackrockRegion" || scene == "RiverValleyRegion" || scene == "AirfieldRegion" || scene == "MiningRegion" || scene == "MountainPassRegion" ? true : false;
        }

        //region stuff

        public string GetRegion()
        {

            string scene = SceneUtilities.GetActiveSceneName();
            List<string> regions = GetAllRegions();

            if (regions.Any(r => scene.Contains(r.ToString())))
            {
                lastRegion = scene; 
            }
            else
            {
                if (!regions.Any(r => scene.Contains(r.ToString()))) scene = lastRegion;
            }

            return scene;
        }

        private List<string> GetAllRegions()
        {
            List<string> names = Enum.GetNames(typeof(GameRegion)).ToList();

            names.Add("LongRailTransitionZone");
            names.Add("RavineTransitionZone");
            names.Add("DamRiverTransitionZoneB");
            names.Add("HubRegion");
            names.Add("MountainPassRegion");

            //TLDev
            names.Add("ModPrecariousCauseway");
            names.Add("ModShatteredMarsh");
            names.Add("ModForsakenShore");
            names.Add("ModRockyThoroughfare");
            names.Add("ModMountainPass");

            return names;
        }


    }
}
