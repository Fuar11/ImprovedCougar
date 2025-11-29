using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedCougar.SpawnRegions
{
    internal class SpawnRegionTerritory
    {

        public Vector3 position;
        public List<Vector3> carcassPositions;

        public SpawnRegionTerritory(Vector3 pos, List<Vector3> carcassPos) { 
            position = pos;
            carcassPositions = carcassPos;
        }


    }
}
