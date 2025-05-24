using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;

namespace ImprovedCougar.Pathfinding
{
    internal class PathfindingFunctions
    {

        private static float GetAStarPathCost(Vector3 from, Vector3 to)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path))
            {
                float cost = 0f;
                for (int i = 1; i < path.corners.Length; i++)
                {
                    cost += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                return cost;
            }
            return Mathf.Infinity; // No valid path
        }

        public static List<Vector3> FindAStarPath(Vector3 start, List<Vector3> coverPoints, Vector3 finalTarget, bool directPaths = false)
        {
            List<Vector3> allPoints = new List<Vector3>(coverPoints);
            allPoints.Insert(0, start);
            allPoints.Add(finalTarget);

            Dictionary<Vector3, List<Vector3>> neighbors = PathfindingFunctions.BuildNeighborGraph(allPoints);

            var openSet = new List<PathNode> { new PathNode(start) };
            var allNodes = new Dictionary<Vector3, PathNode> { [start] = openSet[0] };
            HashSet<Vector3> closedSet = new HashSet<Vector3>();

            while (openSet.Count > 0)
            {
                openSet.Sort((a, b) => a.F.CompareTo(b.F));
                PathNode current = openSet[0];
                openSet.RemoveAt(0);

                if (current.Position == finalTarget)
                    return PathfindingFunctions.ReconstructPath(current);

                closedSet.Add(current.Position);

                foreach (Vector3 neighborPos in neighbors[current.Position])
                {
                    if (closedSet.Contains(neighborPos))
                        continue;

                    float tentativeG = current.G + GetAStarPathCost(current.Position, neighborPos);
                    if (!allNodes.TryGetValue(neighborPos, out PathNode neighbor))
                    {
                        neighbor = new PathNode(neighborPos);
                        allNodes[neighborPos] = neighbor;
                    }

                    if (!openSet.Contains(neighbor) || tentativeG < neighbor.G)
                    {
                        neighbor.Parent = current;
                        neighbor.G = tentativeG;
                        neighbor.H = Vector3.Distance(neighborPos, finalTarget); // Euclidean heuristic

                        /***
                        if (!directPaths && neighborPos == finalTarget && Vector3.Distance(start, finalTarget) > 30f)
                        {
                            neighbor.H *= 2f; // discourage direct path to player from far away
                        } ***/

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return new List<Vector3>(); // No path found
        }

        private static Dictionary<Vector3, List<Vector3>> BuildNeighborGraph(List<Vector3> points, float maxDist = 50f)
        {

            float maxLinkDistance = maxDist;

            Dictionary<Vector3, List<Vector3>> graph = new();
            foreach (Vector3 a in points)
            {
                graph[a] = new List<Vector3>();
                foreach (Vector3 b in points)
                {
                    if (a == b) continue;
                     
                    if (Vector3.Distance(a, b) > maxLinkDistance) continue;

                    NavMeshPath path = new NavMeshPath();
                    if (NavMesh.CalculatePath(a, b, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
                        graph[a].Add(b);
                }
            }
            return graph;
        }

        private static List<Vector3> ReconstructPath(PathNode end)
        {
            var path = new List<Vector3>();
            while (end != null)
            {
                path.Add(end.Position);
                end = end.Parent;
            }
            path.Reverse();
            return path;
        }

    }
}
