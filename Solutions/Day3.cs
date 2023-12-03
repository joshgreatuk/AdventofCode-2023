using AOC23.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC23.Solutions
{
    /// <summary>
    /// https://adventofcode.com/2023/day/3
    /// </summary>
    
    public class Day3 : Solution
    {
        public Day3(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day3";

        //Laziness :)
        private readonly char[] symbolChars = "*=/@£&%-#+$".ToCharArray();

        public override Answer Solve(string problemContents)
        {
            _logger.LogAsync(LogSeverity.Info, this, $"Deciphering grid");
            string[] problemLines = problemContents.Split('\n');

            List<EnginePart> engineParts = new();
            List<Vector2> symbols = new();
            List<Vector2> gears = new();
            
            //Find parts and symbols
            for (int i=0; i < problemLines.Length; i++)
            {
                char[] line = problemLines[i].ToCharArray();

                string workingNumber = "";
                int startX = -1;

                for (int j=0; j < line.Length; j++) 
                {
                    char current = line[j];
                    if (Char.IsDigit(current))
                    {
                        if (startX == -1) startX = j;
                        workingNumber += current;
                        continue;
                    }
                    else if (startX != -1)
                    {
                        engineParts.Add(new(int.Parse(workingNumber), i, new(startX, j-1)));
                        startX = -1;
                        workingNumber = "";
                    }

                    if (symbolChars.Contains(current))
                    {
                        symbols.Add(new(j, i));

                        if (current != '*') continue;
                        gears.Add(new(j, i));
                    }
                }
            }

            _logger.LogAsync(LogSeverity.Info, this, $"Reading the manual. . .");

            //Process parts and symbols
            int partsTotal = 0;
            int gearsTotal = 0;
            for (int i=0; i < symbols.Count; i++)
            {
                Vector2 symbolCoord = symbols[i];

                //Do some maths!
                EnginePart[] adjacent = engineParts.Where(x => 
                    x.yCoord >= symbolCoord.Y-1 && 
                    x.yCoord <= symbolCoord.Y+1 && 
                    (x.xRange.X >= symbolCoord.X-1 &&
                    x.xRange.X <= symbolCoord.X+1 ||
                    x.xRange.Y >= symbolCoord.X-1 &&
                    x.xRange.Y <= symbolCoord.X+1))
                    .ToArray();

                if (gears.Contains(symbolCoord) && adjacent.Length == 2)
                {
                    int gearRatio = adjacent.Select(x => x.number).Aggregate((x, n) => x *= n);
                    gearsTotal += gearRatio;
                }

                for (int j=0; j < adjacent.Length; j++)
                {
                    EnginePart part = adjacent[j];
                    partsTotal += part.number;
                    engineParts.Remove(part);
                }
            }

            _logger.LogAsync(LogSeverity.Info, this, $"Starting it up?");
            return new(partsTotal.ToString(), gearsTotal.ToString());
        }

        public struct EnginePart
        {
            public int number;
            public int yCoord;
            public Vector2 xRange;

            public EnginePart(int number, int yCoord, Vector2 xRange)
            {
                this.number = number;
                this.yCoord = yCoord;
                this.xRange = xRange;
            }
        }
    }
}
