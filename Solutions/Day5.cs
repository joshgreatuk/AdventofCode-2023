using System;
using System.Collections.Generic;
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
        public Day5(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day5";

        public class SeedRange
        {
            public long start;
            public long length;

            public SeedRange(long start, long length)
            {
                this.start = start;
                this.length = length;
            }
        }

        public override Answer Solve(string problemContents)
        {
            string[] problemSections = problemContents.Split("\r\n\r\n");

            List<long> seeds = problemSections[0].Split(":")[1].Trim().Split(" ").Select(x => long.Parse(x)).ToList();
            List<long> expandedSeeds = new();
            for (int i=0; i < seeds.Count/2; i++)
            {
                long rangeStart = seeds[i*2];
                long rangeLength = seeds[i*2+1];

                for (int j=0; j < rangeLength; j++)
                {
                    expandedSeeds.Add(rangeStart + j);
                }
            }

            seeds = ProcessSeeds(seeds, problemSections);

            return new(seeds.Min().ToString());
        }

        public List<long> ProcessSeeds(List<long> seeds, string[] problemSections)
        {
            for (int i = 1; i < problemSections.Length; i++)
            {
                //Split into lines then take the numbers from those lines for seed processing
                string[] mapLines = problemSections[i].Split("\n");

                long[] processedSeeds = new long[seeds.Count];
                for (int j = 1; j < mapLines.Length; j++)
                {
                    //0 - dest start; 1 - source start; 2 - range length
                    long[] map = mapLines[j].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToArray();
                    long mapEnd = map[1] + map[2] - 1;

                    long[] seedsInRange = seeds.Where(x => x >= map[1] && x <= mapEnd).ToArray();
                    for (int k = 0; k < seedsInRange.Length; k++)
                    {
                        long diff = seedsInRange[k] - map[1];
                        int index = seeds.IndexOf(seedsInRange[k]);

                        processedSeeds[index] = map[0] + diff;
                    }
                }

                //Copy over the seeds that havent been processed
                for (int j = 0; j < seeds.Count; j++)
                {
                    if (processedSeeds[j] != 0) continue;

                    processedSeeds[j] = seeds[j];
                }

                seeds.Clear();
                seeds.AddRange(processedSeeds);
            }
            return seeds;
        }
    }
}
