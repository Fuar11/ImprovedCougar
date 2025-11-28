using ExpandedAiFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedCougar
{
    internal class CougarPathLoader : WanderPathLoader
    {
        public CougarPathLoader(CustomBaseAi ai, SpawnModDataProxy proxy, DataManager dataManager, Func<WanderPath, bool> filter = null) : base(ai, proxy, dataManager)
        {
            mFilter = (path) => path.WanderPathFlags.IsSet(WanderPathFlags.Cougar);
        }
        protected override void SaveDetails() { }
    }
}
