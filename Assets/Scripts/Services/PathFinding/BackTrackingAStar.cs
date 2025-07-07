using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace AStar
{
    public sealed class BackTrackingAStar
    {
        public static readonly Func<BackTrackingAStar, Vector2Int, bool> AvoidEverything = (pathFinder, position) =>
        {
            var ent = MovementService.CheckPlace(position.x, position.y, out int error);
            if (ent != null && pathFinder._avoid.Contains(ent.GetType())) return true;
            if (error == -1) return true;
            return false;
        };
        public static BackTrackingAStar GlobalInstance = new();
        public Func<int, int, int, int, int> DistanceCounter { get; set; } = CountDistance;
        private static Func<BackTrackingAStar, Vector2Int, bool> AvoidLogic { get; set; }
        private int? _minOptimalLength;
        private Vector2Int target;
        private HashSet<Type> _avoid;
        public List<(int X, int Y, int, int)> GetPath(Entity seeker, Entity hidden, Func<BackTrackingAStar, Vector2Int, bool> logic = null, int? minOptimal = null, params Type[] avoid) =>
            GetPath(seeker.GridPosition, hidden.GridPosition, logic, minOptimal, avoid);
        public List<(int X,int Y,int,int)> GetPath(Vector2Int seeker, Vector2Int hidden, Func<BackTrackingAStar, Vector2Int, bool> logic = null, int? minOptimal = null, params Type[] avoid)
        {
            target = hidden;
            _avoid = avoid.ToHashSet();
            AvoidLogic = logic;
            _minOptimalLength = minOptimal;
            List<(int, int, int, int)> list = new() { (seeker.x, seeker.y, 0, 0) };
            FindPath(list);
            return list;
        }
        private void FindPath(List<(int X, int Y, int StepsRemained, int StepsExpired)> list) //поработать над поиском тупиков
        {
            //добавить лимит пути которого мы ищем
            var listWithWeights = new List<(int X, int Y, int StepsRemained, int StepsExpired)>();
            var path = new List<(int X, int Y, int StepsRemained, int StepsExpired)> { list.First() };
            var minPath = new List<(int X, int Y, int StepsRemained, int StepsExpired)>();

            var blackList = new HashSet<(int X, int Y)>();
            var tempBlackList = new HashSet<(int X, int Y)>();
            var visited = new HashSet<(int X, int Y)> { (list.First().X, list.First().Y) };
            
            var prevPos = list.First();
            var forks = new Dictionary<(int X, int Y, int StepsRemained, int StepsExpired), Queue<(int X, int Y, int StepsRemained, int StepsExpired)>>();
            var expiredForks = new Dictionary<(int X, int Y, int StepsRemained, int StepsExpired), (int X, int Y, int StepsRemained, int StepsExpired)>();
            var forkPaths = new Dictionary<(int X, int Y, int StepsRemained, int StepsExpired), List<(int X, int Y, int StepsRemained, int StepsExpired)>>();

            bool forksDone = false;
            bool deadEnd = false;

            while (!forksDone)
            {
                listWithWeights.Clear();
                var gluedPath = GlueToPath(minPath, prevPos);

                if (visited.Contains((target.x, target.y))
                    || (_minOptimalLength != null && path.Count > _minOptimalLength)
                    || (path.Count + CountDistance(path.Last().X, path.Last().Y) >= minPath.Count && minPath.Any())
                    || gluedPath.Count() > 0
                    || deadEnd)
                {
                    if (gluedPath.Count() > 0)
                    {
                        foreach (var step in gluedPath.Skip(1))
                        {
                            path.Add(step);
                            visited.Add((step.X, step.Y));
                        }
                    }

                    if ((path.Count + CountDistance(path.Last().X, path.Last().Y) < minPath.Count || !minPath.Any()) && !deadEnd)
                    {
                        minPath = path.ToList();
                        RemoveLessValuableForks(minPath, forks, expiredForks, forkPaths);
                    }

                    if (!forks.Any())
                    {
                        forksDone = true;
                        continue;
                    }

                    deadEnd = false;
                    tempBlackList.Clear();
                    JumpToLastFork(ref path, ref visited, forks, forkPaths, expiredForks, ref prevPos);
                    continue;
                }

                AddNeighbours(listWithWeights, prevPos);
                listWithWeights.RemoveAll(x => visited.Contains((x.X, x.Y)));

                listWithWeights.Sort((a, b) =>
                (a.StepsRemained + a.StepsExpired).CompareTo(b.StepsRemained + b.StepsExpired));
                var s_mins = listWithWeights
                    .Where(c => !blackList.Contains((c.X, c.Y)))
                    .Where(c => !tempBlackList.Contains((c.X, c.Y)))
                    .Take(3);

                if (s_mins.Count() > 1)
                {
                    if (!forks.ContainsKey(prevPos) && !expiredForks.ContainsKey(prevPos))
                    {
                        var p_Forks = minPath.Any()
                            ? s_mins.Skip(1).Where(x => path.Count + CountDistance(x.X, x.Y) < minPath.Count)
                            : s_mins.Skip(1); //

                        forks[prevPos] = new Queue<(int, int, int, int)>(p_Forks);
                        forkPaths[prevPos] = path.ToList();
                    }
                }

                var candidates = s_mins.OrderBy(x => x.StepsExpired).ToArray();

                if (candidates.Length == 0)
                {
                    if (path.Count < 2)
                    {
                        deadEnd = true;
                        continue;
                    }

                    tempBlackList.Add((prevPos.X, prevPos.Y));
                    visited.Remove((path.Last().X, path.Last().Y));
                    path.Remove(path.Last());
                    prevPos = path.Last();
                    continue;
                }

                var min = candidates.First();
                prevPos = min;
                path.Add(prevPos);
                visited.Add((min.X, min.Y));
            }

            list.Clear();
            list.AddRange(minPath);
        }

        private void RemoveLessValuableForks(List<(int X, int Y, int StepsRemained, int StepsExpired)> minPath,
            Dictionary<(int X, int Y, int StepsRemained, int StepsExpired), Queue<(int X, int Y, int StepsRemained, int StepsExpired)>> forks,
            Dictionary<(int X, int Y, int StepsRemained, int StepsExpired), (int X, int Y, int StepsRemained, int StepsExpired)> expiredForks,
            Dictionary<(int X, int Y, int StepsRemained, int StepsExpired), List<(int X, int Y, int StepsRemained, int StepsExpired)>> forkPaths)
        {
            if (!minPath.Any()) return;

            var forkPoses = forks.Where(x => forkPaths[x.Key].Count() + CountDistance(x.Key.X, x.Key.Y) >= minPath.Count).Select(y => y.Key).ToList();
            foreach (var fork in forkPoses)
            {
                forks.Remove(fork);
                expiredForks.Remove(fork);
                forkPaths.Remove(fork);
            }
        }
        /// <summary>
        /// Возвращает вторую часть пути с этим элементом.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        private IEnumerable<(int X, int Y, int StepsRemained, int StepsExpired)> GlueToPath(List<(int X, int Y, int StepsRemained, int StepsExpired)> path, (int X, int Y, int StepsRemained, int StepsExpired) element)
            => path.SkipWhile(x => !(x.X == element.X && x.Y == element.Y));
        private void JumpToLastFork(
            ref List<(int X, int Y, int StepsRemained, int StepsExpired)> path,
            ref HashSet<(int X, int Y)> visited,
            Dictionary<(int, int, int, int), Queue<(int, int, int, int)>> forks,
            Dictionary<(int, int, int, int), List<(int, int, int, int)>> forkPaths,
            Dictionary<(int, int, int, int), (int, int, int, int)> expiredForks,
            ref (int, int, int, int) prevPos)
        {
            var lastFork = forks.LastOrDefault();
            var forkPos = lastFork.Key;

            var nextOptions = lastFork.Value;

            if (nextOptions.Count > 0)
            {
                prevPos = nextOptions.Dequeue();
                path = forkPaths[forkPos].ToList();
                path.Add(prevPos);
                visited = path.Select(x => (x.X, x.Y)).ToHashSet();

                if (nextOptions.Count == 0)
                {
                    forks.Remove(forkPos);
                    expiredForks[forkPos] = prevPos;
                    forkPaths.Remove(forkPos);
                }
            }
            else
                forks.Remove(forkPos);
        }
        private void AddNeighbours(List<(int X, int Y, int StepsRemained, int StepsExpired)> listWithWeights, (int X, int Y, int StepsRemained, int StepsExpired) prevPos)
        {
            (int X, int Y)[] Displaces =
            {
                (prevPos.X + 1, prevPos.Y),
                (prevPos.X - 1, prevPos.Y),
                (prevPos.X, prevPos.Y + 1),
                (prevPos.X, prevPos.Y - 1),
            };
            foreach (var (X, Y) in Displaces)
            {
                if (AvoidLogic != null && AvoidLogic(this, new(X, Y))) continue;
                AddWeigthsToPoint(X, Y, prevPos.StepsExpired + 1, listWithWeights);
            }
        }
        private void AddWeigthsToPoint(int X, int Y, int step, List<(int X, int Y, int StepsRemained, int StepsExpired)> list)
        {
            (int X, int Y, int StepsRemained, int StepsExpired) item = (X, Y, CountDistance(X, Y), step);
            list.Add(item);
        }
        private int CountDistance(int from_x, int from_y)
            => DistanceCounter(from_x, from_y, target.x, target.y);
        public static int CountDistance(int from_x, int from_y, int to_x, int to_y)
        {
            int f_Cat = Math.Abs(to_x - from_x);
            int s_Cat = Math.Abs(to_y - from_y);
            return s_Cat + f_Cat;
        }  
        public static int CountDistanceDiagonally(int from_x, int from_y, int to_x, int to_y)
        {
            int f_Cat = Math.Abs(to_x - from_x);
            int s_Cat = Math.Abs(to_y - from_y);
            return Math.Max(s_Cat, f_Cat);
        }
    }
}
