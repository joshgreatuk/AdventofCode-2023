using AOC23.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC23.Solutions
{
    public class Day1 : Solution
    {
        /// <summary>
        /// https://adventofcode.com/2023/day/1
        /// </summary>

        public Day1(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day1";

        private readonly Dictionary<string, int> digitStrings = new()
        {
            { "one", 1 }, { "two", 2 }, { "three", 3 }, 
            { "four", 4 }, { "five", 5 }, { "six", 6 }, 
            { "seven", 7 }, { "eight", 8 }, { "nine", 9 },
        };

        public override Answer Solve(string problemContents)
        {
            _logger.LogAsync(LogSeverity.Info, this, $"Splitting lines");
            string[] lines = problemContents.Split('\n');
            int[] values = new int[lines.Length];
            int[] valuesWDigits = new int[lines.Length];

            _logger.LogAsync(LogSeverity.Info, this, $"Calibrating values");
            for (int i=0; i < lines.Length; i++)
            {
                List<int> numbersInLine = lines[i].Where(x => int.TryParse(x.ToString(), out int y)).Select(x => int.Parse(x.ToString())).ToList();
                values[i] = numbersInLine.Count > 0 ? int.Parse($"{numbersInLine.First()}{numbersInLine.Last()}") : 0;

                SortedDictionary<int, int> lineNumbersWIndex = new();
                for (int j=0; j < numbersInLine.Count; j++)
                {
                    int index = lines[i].IndexOf(numbersInLine[j].ToString());
                    while (lineNumbersWIndex.ContainsKey(index)) index = lines[i].IndexOf(numbersInLine[j].ToString(), index + 1);
                    lineNumbersWIndex.Add(index, numbersInLine[j]);
                }

                foreach (KeyValuePair<string, int> digitPair in digitStrings)
                { //This is ugly as hell :D
                    int index = lines[i].IndexOf(digitPair.Key);
                    if (index == -1 || lineNumbersWIndex.ContainsKey(index)) continue;

                    lineNumbersWIndex.Add(index, digitPair.Value);
                    int digitEndIndex = index + digitPair.Key.Length - 1;

                    if (lineNumbersWIndex.ContainsKey(digitEndIndex)) lineNumbersWIndex.Remove(digitEndIndex);
                    lineNumbersWIndex.Add(digitEndIndex, 0);
                }

                int[] keysToRemove = lineNumbersWIndex.Where(x => x.Value == 0).Select(x => x.Key).ToArray();
                for (int j = 0; j < keysToRemove.Length; j++) lineNumbersWIndex.Remove(keysToRemove[j]);
                valuesWDigits[i] = int.Parse($"{lineNumbersWIndex.First().Value}{lineNumbersWIndex.Last().Value}");
            }

            _logger.LogAsync(LogSeverity.Info, this, $"Counting sheep");
            return new(values.Sum().ToString(), valuesWDigits.Sum().ToString());
        }
    }
}
