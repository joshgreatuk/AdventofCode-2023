using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace AOC23.Solutions
{
    public class Day6 : Solution
    {
        /// <summary>
        /// https://adventofcode.com/2023/day/6
        /// </summary>

        public Day6(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day6";

        public override Answer Solve(string problemContents)
        {
            _logger.LogAsync(LogSeverity.Info, this, "Preparing to race");
            string[] problemLines = problemContents.Split('\n');
            int[] raceTimes = ParseRaceLine(problemLines[0]);
            int[] raceRecords = ParseRaceLine(problemLines[1]);

            _logger.LogAsync(LogSeverity.Info, this, "3. 2. 1. GOGOGO!");
            int[] possibilityTotals = new int[raceTimes.Length];
            for (int i = 0; i < raceTimes.Length; i++)
            {
                for (int j = 0; j < raceTimes[i]; j++)
                {
                    int distance = (raceTimes[i] - j) * j;
                    if (distance <= raceRecords[i]) continue;

                    possibilityTotals[i]++;
                }
            }

            _logger.LogAsync(LogSeverity.Info, this, $"Oh wait. . . Its only 1 race! Going around!");
            long raceTime = ParseRaceArray(raceTimes);
            long raceRecord = ParseRaceArray(raceRecords);
            int possibilityTotal = 0;
            for (int i=14; i < raceTime; i++)
            {
                long distance = (raceTime - i) * i;
                if (distance <= raceRecord) continue;

                possibilityTotal++;
            }


            _logger.LogAsync(LogSeverity.Info, this, "Did we win?");
            return new(possibilityTotals.Aggregate((x, n) => x *= n).ToString(), possibilityTotal.ToString());
        }

        public int[] ParseRaceLine(string line) => line
                .Split(":", StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x))
                .ToArray();

        public long ParseRaceArray(int[] array) => long
                .Parse(string
                .Join("", array
                .Select(x => x
                .ToString())
                .ToArray()));
    }
}
