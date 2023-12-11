using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Map = AOC23.Solutions.Day10.Map;

namespace AOC23.Solutions
{
    public class Day11 : Solution
    {
        public Day11(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day11";

        public struct GridResult
        {
            public char[,] map;
            public Vector2[] galaxies;
            public int height;
            public int width;

            public GridResult(char[,] map, Vector2[] galaxies, Vector2 size)
            {
                this.map = map;
                this.galaxies = galaxies;
                this.height = (int)size.Y;
                this.width = (int)size.X;
            }
        }
        
        public struct Connection
        {
            public Vector2 galaxyA;
            public Vector2 galaxyB;
            public int distance;

            public Connection(Vector2 galaxyA, Vector2 galaxyB, int distance)
            {
                this.galaxyA = galaxyA;
                this.galaxyB = galaxyB;
                this.distance = distance;
            }
        }

        public override Answer Solve(string problemContents)
        {
            _logger.LogAsync(LogSeverity.Info, this, "Reading map");
            string[] problemLines = problemContents.Split("\n", StringSplitOptions.TrimEntries);

            Connection[] normalExpansions = CalculateConnections(problemLines, 2);
            Connection[] expandedExpansions = CalculateConnections(problemLines, 1000000);

            return new(normalExpansions.Sum(x => x.distance).ToString(), expandedExpansions.Sum(x => (long)x.distance).ToString());
        }

        public Connection[] CalculateConnections(string[] lines, int expansion)
        {
            Vector2[] galaxies = ParseGrid(lines, expansion);
            _logger.LogAsync(LogSeverity.Info, this, $"Calculating pair distances");
            List<Connection> connections = new List<Connection>();
            //Find the nearby connections 
            for (int i = 0; i < galaxies.Length-1; i++)
            {
                Vector2 galaxyA = galaxies[i];
                for (int j = i+1; j < galaxies.Length; j++)
                {
                    Vector2 galaxyB = galaxies[j];
                    //if (galaxyA == galaxyB || connections.Any(x =>
                    //x.galaxyA == galaxyA && x.galaxyB == galaxyB ||
                    //x.galaxyB == galaxyA && x.galaxyA == galaxyB)) continue;

                    Vector2 difference = Vector2.Abs(galaxyB - galaxyA);

                    int distance = (int)difference.X + (int)difference.Y;
                    Connection newConnection = new(galaxyA, galaxyB, distance);
                    connections.Add(newConnection);
                }
            }
            return connections.ToArray();
        }

        public Vector2[] ParseGrid(string[] lines, int expansion)
        {
            List<List<char>> map = new();
            List<Vector2> galaxies = new();

            expansion -= 1;

            _logger.LogAsync(LogSeverity.Info, this, "Constructing raw universe");
            //Map the raw universe
            for (int i = 0; i < lines.Length; i++)
            {
                List<char> row = new();
                for (int j = 0; j < lines[0].Length; j++)
                {
                    row.Add(lines[i][j]);
                    if (lines[i][j] == '#') galaxies.Add(new(j, i));
                }
                map.Add(row);
            }
            _logger.LogAsync(LogSeverity.Info, this, $"Found {galaxies.Count} galaxies ({galaxies.Count * (galaxies.Count - 1) / 2} pairs)");

            List<Vector2> galaxiesExpanded = galaxies.ToList();

            _logger.LogAsync(LogSeverity.Info, this, "E X P A N D I N G rows");
            //Now find empty columns and rows and E X P A N D
            //Rows first
            for (int i=0; i < map.Count; i++)
            {
                //Check the row
                if (map[i].All(x => x == '.'))
                {
                    //Add a new row below
                    for (int j=0; j < galaxies.Count; j++)
                    {
                        if (galaxies[j].Y > i) galaxiesExpanded[j] += new Vector2(0, expansion);
                    }
                }
            }

            _logger.LogAsync(LogSeverity.Info, this, "E X P A N D I N G columns");
            //Now columns
            for (int i=0; i < map[0].Count; i++)
            {
                if (map.Select(x => x[i]).All(x => x == '.'))
                {
                    //Add a new column to the right
                    for (int j = 0; j < galaxies.Count; j++)
                    {
                        if (galaxies[j].X > i) galaxiesExpanded[j] += new Vector2(expansion, 0);
                    }
                }
            }

            _logger.LogAsync(LogSeverity.Info, this, "Universe mapped");
            return galaxiesExpanded.ToArray();
        }

        
    }
}
