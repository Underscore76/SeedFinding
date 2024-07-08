using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Data.HashFunction;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;

namespace SeedFinding
{
    public class SCDepthB
    {
        public static int DropShaft(int floor, int gameId,int day)
        {
            Random random = Utility.CreateRandom(floor,gameId,day-1);
            int floorDrop = random.Next(3,9);
            if (random.NextDouble() < 0.1) floorDrop = 2*floorDrop-1;
            if (floor < 220 && floor+floorDrop>220) return 220-floor;
            return floorDrop;
        }
        public static Tuple<int,List<int>,int> RunToFloor(int endFloor,int day,int gameId,bool storePath,int maxTime = 64787)
        {
            if (endFloor < 121) throw new Exception("Minimum end floor is 121");
            Tuple<int,int> bestTimeToFloor;
            int currentFloor = 120;
            int failCount = 0;
            Dictionary<int,List<Tuple<int,int>>> timesToFloor = new()
            {
                { 121,new(){new(120,0)}}
            }; //<floor,List<Tuple<previous floor,time>>>
            for (int floor = 121; floor <= endFloor; floor++)
            {
                if (!timesToFloor.ContainsKey(floor)) //early cancel due to maxTime logic
                {
                    failCount++;
                    if (failCount > 15) break;
                    continue;
                }
                failCount = 0;
                //get best time
                bestTimeToFloor = timesToFloor[floor].Aggregate(new Tuple<int,int>(floor-1,(floor-121)*3),
                (best,next) => next.Item2 < best.Item2 ? next : best);
                if (bestTimeToFloor.Item2>maxTime) continue; //no accidental skipping floors 
                currentFloor = floor > currentFloor ? floor : currentFloor; //track max depth obtained
                timesToFloor[floor].Clear();
                timesToFloor[floor].Add(bestTimeToFloor);
                //add ladder+dropshaft states to future floors
                if (!timesToFloor.ContainsKey(floor+1)) timesToFloor.Add(floor+1,new());
                timesToFloor[floor+1].Add(new(floor,bestTimeToFloor.Item2+3)); //add ladder state
                if (floor==220) continue;
                var drop = DropShaft(floor,gameId,day);
                if (drop < 4) continue; //mathematically worse
                if (!timesToFloor.ContainsKey(floor+drop)) timesToFloor.Add(floor+drop,new());
                timesToFloor[floor+drop].Add(new(floor,bestTimeToFloor.Item2+11));
            }
            if (!storePath)
            {
                var maxFloorBest = timesToFloor[currentFloor][0];
                return new(maxFloorBest.Item2,new(){currentFloor},gameId);
            }
            return BestFloorPath(timesToFloor,gameId,currentFloor);
        }
        public static Tuple<int,List<int>,int> BestFloorPath(Dictionary<int,List<Tuple<int,int>>> bestFloorTimes,int gameId,int maxFloorObtained)
        {
            List<int> visitedFloors = new();
            int currentFloor = maxFloorObtained;
            var currentFloorVal = bestFloorTimes[currentFloor][0];
            int endTime = currentFloorVal.Item2;
            while (currentFloor > 120)
            {
                visitedFloors.Add(currentFloor);
                currentFloor = bestFloorTimes[currentFloor][0].Item1;
            }
            return new(endTime, visitedFloors,gameId);
        }
        public static List<Tuple<int,List<int>,int>> SeedRange(int startSeed,int endSeed,int minTimeKnown, int endFloor, int day,bool storePath)
        {
            var maxDepths = new List<Tuple<int, List<int>,int>>();
            int maxDepth = 0;
            Tuple<int,List<int>,int> pathForSeed;
            for (int gameId = startSeed; gameId <endSeed; gameId++)
            {
                pathForSeed = RunToFloor(endFloor,day,gameId,storePath);
                if (pathForSeed.Item2[0] < maxDepth) continue;
                if (pathForSeed.Item2[0] > maxDepth)
                {
                    maxDepth = pathForSeed.Item2[0] > maxDepth? pathForSeed.Item2[0] : maxDepth;
                    maxDepths.Clear();
                    Console.WriteLine("Best depth {1}:{0}",maxDepth,gameId);
                }
                maxDepths.Add(pathForSeed);
            }
            return maxDepths;
        }
        public static List<Tuple<int,List<int>,int>> SeedRangeTime(int startSeed,int endSeed,int minTimeKnown, int endFloor, int day,bool storePath)
        {
            var minTimes = new List<Tuple<int, List<int>,int>>();
            Tuple<int,List<int>,int> pathForSeed;
            for (int gameId = startSeed; gameId <endSeed; gameId++)
            {
                pathForSeed = RunToFloor(endFloor,day,gameId,storePath);
                if (pathForSeed.Item1 > minTimeKnown) continue;
                if (pathForSeed.Item1 < minTimeKnown)
                {
                    minTimeKnown = pathForSeed.Item1 < minTimeKnown? pathForSeed.Item1 : minTimeKnown;
                    minTimes.Clear();
                    Console.WriteLine("Best time {1}:{0}",minTimeKnown,gameId);
                }
                minTimes.Add(pathForSeed);
            }
            return minTimes;
        }
        public static double SearchParallel(int numSeeds,int blockSize,out List<Tuple<int,List<int>,int>> bestPaths,int endFloor,int day,bool storePath = false, int thresholdTime=64787, bool endDepth=true)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<Tuple<int,List<int>,int>>();
            var partioner = Partitioner.Create(0,numSeeds,blockSize);
            Func<int,int,int,int,int,bool,List<Tuple<int,List<int>,int>>> seedRange;
            if (endDepth) seedRange = SeedRange;
            else seedRange = SeedRangeTime;
            Parallel.ForEach(partioner, (range,loopState) =>
            {
                var bestLocalPaths = seedRange(range.Item1,range.Item2,thresholdTime,endFloor,day,storePath);
                foreach (var bestPath in bestLocalPaths) 
                {
                    bag.Add(bestPath); 
                }                
            });
            double seconds = stopwatch.Elapsed.TotalSeconds;
            
            var bestPathsRaw = bag.ToList();
            
            if (endDepth)
            {
                int bestFloor = bestPathsRaw.Min(x => x.Item2[0]);
                bestPaths = bestPathsRaw.Where(x => x.Item2[0] == bestFloor).ToList();
                return seconds;
            }
            int bestTime = bestPathsRaw.Min(x => x.Item1);
            bestPaths = bestPathsRaw.Where(x => x.Item1 == bestTime).ToList();
            return seconds;
        }
    }
}