using AOC23;
using AOC23.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC23.Solutions
{
    public class Day2 : Solution
    {
        /// <summary>
        /// https://adventofcode.com/2023/day/2
        /// </summary>
       
        public Day2(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day2";

        public override Answer Solve(string problemContents)
        {
            string[] problemLines = problemContents.Split('\n');
            _logger.LogAsync(LogSeverity.Info, this, $"Lining up for {problemLines.Length} games!");
            int totalIDS = 0;
            int totalPowers = 0;
            int[] gameRules = new int[] { 12, 13, 14 };

            _logger.LogAsync(LogSeverity.Info, this, $"Ready! Set! GO!");
            for (int i=0; i < problemLines.Length; i++)
            {
                string line = problemLines[i];
                string[] splitGame = line.Split(":");

                bool possible = IsGamePossible(gameRules, splitGame[1], out int cubesPower);
                totalPowers += cubesPower;

                if (!possible) continue;

                totalIDS += int.Parse(splitGame[0].Split(" ")[1]);
            }

            _logger.LogAsync(LogSeverity.Info, this, "Gaming Complete!");
            return new(totalIDS.ToString(), totalPowers.ToString());
        }

        public enum GameColours
        {
            red = 0,
            green = 1,
            blue = 2
        }

        /// <summary>
        /// Check if the game is possible
        /// </summary>
        /// <param name="maxValues">Takes max red, blue and green cubes in the form int[R, G, B]</param>
        /// <param name="gameString">Takes the game string in the form "3 blue, 4 red; 1 red, 2 green; etc"</param>
        /// <returns></returns>
        public bool IsGamePossible(int[] maxValues, string gameString, out int cubesPower)
        {
            // int[R, G, B]
            string[] gameRounds = gameString.Split(";");
            int[] minValues = new int[3];
            bool possible = true;

            for (int i = 0; i < gameRounds.Length; i++)
            {
                string[] roundItems = gameRounds[i].Split(",");
                int[] roundTotal = new int[3];

                for (int j = 0; j < roundItems.Length; j++)
                {
                    string[] itemSplit = roundItems[j].Trim().Split(" ");
                    int index = (int)Enum.Parse(typeof(GameColours), itemSplit[1]);

                    roundTotal[index] += int.Parse(itemSplit[0]);
                }

                for (int j = 0; j < maxValues.Length; j++)
                {
                    if (minValues[j] < roundTotal[j]) minValues[j] = roundTotal[j];
                    if (roundTotal[j] > maxValues[j]) possible = false;
                }
            }

            cubesPower = minValues.Aggregate((x, n) => x *= n);
            return possible;
        }
    }
}
