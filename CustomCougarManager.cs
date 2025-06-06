using ComplexLogger;
using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedCougar
{
    [RegisterTypeInIl2Cpp]
    internal class CustomCougarManager : MonoBehaviour
    {

        public Vector3 currentSpawnRegion = Vector3.zero;
        public string currentRegion = ""; //simply used to get the region

        //time
        public float timeSinceLastSpawnRegionMove = 0;
        public float maxTimeTillNextSpawnRegionMoveInHours = 24; //placeholder
        public float minTimeTillNextSpawnRegionMoveInHours = 12; //placeholder
        


        public void Update()
        {

            if(GameManager.m_IsPaused || GameManager.s_IsGameplaySuspended) return;
            if (SaveGameSystem.IsRestoreInProgress()) return;

            PlayerStruggle struggle = GameManager.m_PlayerStruggle;
            if (struggle != null && struggle.InStruggle()) return;

            //eventually handle spawning and spawn point moving, wander paths, all that stuff

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Z))
            {
                Main.Logger.Log("Moving spawn region using key press.", ComplexLogger.FlaggedLoggingLevel.Debug);
                MoveSpawnRegion();
            }

        }

        public void SetSpawnRegion()
        {
            //random for now
            currentSpawnRegion = SpawnRegionPositions.GetRandomSpawnRegion(GetRegion());
            Main.Logger.Log($"New spawn region {currentSpawnRegion}", FlaggedLoggingLevel.Debug);
        }

        public void MoveSpawnRegion()
        {
            SetSpawnRegion();

            if (!GameManager.GetWeatherComponent().IsIndoorScene())
            {
                UpdateCougarTerritory(GetRegion());
            }
        }

        private void UpdateCougarTerritory(string scene)
        {

            if (RegionHasCougar(scene))
            {

                //grab initial territory object and move it around
                GameObject territoryObject = GameObject.Find("CougarTerritoryZone_a_T1");

                if (territoryObject == null)
                {
                    Main.Logger.Log("Cannot find cougar territory object!", FlaggedLoggingLevel.Error);
                    return;
                }

                territoryObject.transform.position = currentSpawnRegion;
                //territoryObject.transform.position = new Vector3(102.14f, 2.65f, 79.10f); //temporary testing spawn at trapper's

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


        public bool RegionHasCougar(string scene) //gotta add TLDev regions to this list at some point
        {
            return scene == "LakeRegion" || scene == "RuralRegion" || scene == "TracksRegion" || scene == "MountainCrashRegion" || scene == "MountainTownRegion" || scene == "AshCanyonRegion" || scene == "BlackrockRegion" || scene == "RiverValleyRegion" || scene == "AirfieldRegion" || scene == "MiningRegion" || scene == "MountainPassRegion" ? true : false;
        }

        //region stuff

        public string GetRegion()
        {

            string scene = GameManager.m_ActiveScene;
            List<string> regions = GetAllRegions();

            if (regions.Any(r => scene.Contains(r.ToString())))
            {
                currentRegion = scene;
            }
            else
            {
                if (!regions.Any(r => scene.Contains(r.ToString()))) scene = currentRegion;
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
