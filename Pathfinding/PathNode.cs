using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedCougar.Pathfinding
{
    internal class PathNode
    {

        public Vector3 Position;
        public PathNode Parent;
        public float G; // Cost from start
        public float H; // Heuristic to end
        public float F => G + H;

        public PathNode(Vector3 pos) => Position = pos;
    }
}
