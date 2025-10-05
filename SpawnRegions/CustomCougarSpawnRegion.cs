using ExpandedAiFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedCougar.SpawnRegions
{
    internal class CustomCougarSpawnRegion : CustomSpawnRegion
    {
        public CustomCougarSpawnRegion(SpawnRegion spawnRegion, SpawnRegionModDataProxy dataProxy, TimeOfDay timeOfDay) : base(spawnRegion, dataProxy, timeOfDay)
        {

        }

        protected override bool OverrideCalculateTargetPopulation(out int customTarget)
        {
            if (VanillaSpawnRegion.m_WildlifeMode == WildlifeMode.Aurora)
            {
                customTarget = 0;
                return false;
            }
            // if cougar alive, return 1
            // if cougar dead or not ready to appear, return 0
            customTarget = 1 - mSpawnRegion.m_NumRespawnsPending; // this will work per region, but wont take into account a cougar that lives in multiple game zones or something
            customTarget = Math.Max(customTarget, 0);
            return false;
        }

        protected override Type OverrideSpawnType() => typeof(CustomCougar);
        
        /*
        Including some code from EAF for your convenience here, so you can see more easily what the impact of different values of your above override will do in the global system. 
        Pay special attention to the respawn counter, the game should still be sending signals to this spawn region when the cougar passes.


        public int CalculateTargetPopulation()
        {
            if (!OverrideCalculateTargetPopulation(out int customTarget))
            {
                this.LogTraceInstanced($"Spawning target overridden to {customTarget}!");
                return customTarget;
            }
            if (SpawningSuppressedByExperienceMode())
            {
                this.LogTraceInstanced($"Spawning suppressed by experience mode");
                return 0;
            }
            if (!mSpawnRegion.m_CanSpawnInBlizzard && GameManager.m_Weather.IsBlizzard())
            {
                this.LogTraceInstanced($"Cannot spawn in blizzard");
                return 0;
            }
            return CalculateTargetPopulationInternal();
        }


        private int CalculateTargetPopulationInternal()
        {
            int maxSimultaneousSpawns = GameManager.m_TimeOfDay.IsDay()
                ? GetMaxSimultaneousSpawnsDay()
                : GetMaxSimultaneousSpawnsNight();
            maxSimultaneousSpawns -= mSpawnRegion.m_NumTrapped;
            maxSimultaneousSpawns -= mSpawnRegion.m_NumRespawnsPending;
            return Math.Max(maxSimultaneousSpawns, 0);
        }
        */
    }
}
