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

        public override void UpdateFromManager() {} // EAF does not drive this spawn region


        protected override Type OverrideSpawnType() => typeof(CustomCougar);

        public void SpawnCougar(Vector3 position, Quaternion rotation, Action<CustomCougar> callback)
        {
            mDataManager.ScheduleSpawnModDataProxyRequest(new GetNextAvailableSpawnRequest(mModDataProxy.Guid, mModDataProxy.Scene, false, (availableProxy, result) =>
            {
                if (result == RequestResult.Succeeded)
                {
                    callback(MaybeSpawnCougar(availableProxy, position, rotation));
                }
                else
                {
                    GenerateNewRandomSpawnModDataProxy((s) =>
                    {
                        callback(MaybeSpawnCougar(s, position, rotation));
                    }, WildlifeMode.Normal, true);
                }
            }, false), WildlifeMode.Normal);
        }

        private CustomCougar MaybeSpawnCougar(SpawnModDataProxy proxy, Vector3 position, Quaternion rotation)
        {
            proxy.CurrentPosition = position;
            proxy.CurrentRotation = rotation;
            CustomBaseAi maybeCustomCougar = InstantiateSpawn(proxy);
            if (maybeCustomCougar is CustomCougar customCougar)
            {
                return customCougar;
            }
            else
            {
                Main.Logger.Log("Failed to spawn cougar from proxy", ComplexLogger.FlaggedLoggingLevel.Error);
                return null;
            }
        }
    }
}
