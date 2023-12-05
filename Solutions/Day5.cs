using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC23.Solutions
{
    /// <summary>
    /// https://adventofcode.com/2023/day/5
    /// </summary>

    public class Day5 : Solution
    {
        private readonly int arrayLength = Array.MaxLength / 2;
        private readonly int threadCount = 12;

        public Day5(ILogger logger) : base(logger) { arrayLength /= threadCount; }
        public override string GetProblemName() => "Day5";

        public override Answer Solve(string problemContents)
        {
            string[] problemSections = problemContents.Split("\r\n\r\n");

            _logger.LogAsync(LogSeverity.Info, this, "Warming up the tractor (with part 1)");
            long[] seeds = problemSections[0].Split(":")[1].Trim().Split(" ").Select(x => long.Parse(x)).ToArray();
            seeds = ProcessSeeds(seeds, problemSections);

            _logger.LogAsync(LogSeverity.Info, this, $"Preparing for memory hell of {arrayLength} length arrays");
            long startingMemory = Process.GetCurrentProcess().PrivateMemorySize64;
            List<long> minRangeLocations = new();

            int[] seedsRangeIterations = new int[seeds.Length / 2];
            for (int i=0; i < seeds.Length / 2; i++)
            {
                seedsRangeIterations[i] = (int)MathF.Ceiling(seeds[i*2+1] / (arrayLength*threadCount));
            }
            int totalIterations = seedsRangeIterations.Sum();
            int currentIteration = 1;
            Stopwatch timer = new();

            for (int i=0; i < seeds.Length/2; i++)
            {
                for (int j=0; j < seedsRangeIterations[i]; j++)
                {
                    timer.Restart();
                    Task<long[]>[] tasks = new Task<long[]>[threadCount];
                    for (int l = 0; l < threadCount; l++)
                    {
                        long[] expandedSeeds = new long[arrayLength];

                        if (j == seedsRangeIterations.Length - 1)
                        {
                            expandedSeeds = new long[(arrayLength * (seedsRangeIterations[i] - 1)) - seeds[i * 2 + 1]];
                        }

                        for (int k = 0; k < expandedSeeds.Length; k++)
                        {
                            expandedSeeds[k] = (seeds[i * 2] + arrayLength * (j)) + (k);
                        }

                        tasks[l] = new Task<long[]>(() => ProcessSeeds(expandedSeeds, problemSections));
                        tasks[l].Start();
                    }

                    _logger.LogAsync(LogSeverity.Info, this, $"Starting processing of range {currentIteration}/{totalIterations} with {threadCount} threads");

                    Task.WaitAll(tasks);

                    minRangeLocations.Add(tasks.Min(x => x.Result.Min()));

                    long currentMemory = Process.GetCurrentProcess().PrivateMemorySize64;
                    long memoryDifference = currentMemory - startingMemory;

                    double memoryGB = MathF.Round(memoryDifference / 1024 / 1024 / 1024, 3);
                    _logger.LogAsync(LogSeverity.Info, this, $"Processed seed range {currentIteration}/{totalIterations} with {memoryGB}GB in {timer.Elapsed.Minutes}m {timer.Elapsed.Seconds}s");

                    for (int l = 0; l < tasks.Length; l++) tasks[l].Dispose();
                    tasks = null;
                    GC.Collect();
                    currentIteration++;
                }
            }

            timer.Stop();
            _logger.LogAsync(LogSeverity.Info, this, $"Im sorry CPU");
            return new(seeds.Min().ToString(), minRangeLocations.Min().ToString());
        }

        public long[] ProcessSeeds(long[] seeds, string[] problemSections)
        {
            for (int i = 1; i < problemSections.Length; i++)
            {
                //Split into lines then take the numbers from those lines for seed processing
                string[] mapLines = problemSections[i].Split("\n");

                for (int j = 1; j < mapLines.Length; j++)
                {
                    //0 - dest start; 1 - source start; 2 - range length
                    long[] map = mapLines[j].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToArray();
                    long mapEnd = map[1] + map[2] - 1;

                    for (int k = 0; k < seeds.Length; k++)
                    {
                        if (!(seeds[k] >= map[1] && seeds[k] <= mapEnd)) continue;

                        long diff = seeds[k] - map[1];
                        seeds[k] = map[0] + diff;
                    }
                }
            }
            return seeds;
        }
    }
}
