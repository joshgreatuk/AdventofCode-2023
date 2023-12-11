using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC23.Solutions
{
    public class Day10 : Solution
    {
        public Day10(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day10";

        public class Connection
        {
            public Vector2[] connections;

            public Connection(Vector2[] connections)
            {
                this.connections = connections;
            }

            public bool Contains(Vector2 value) => connections.Contains(value);
        }

        public struct GridResult
        {
            public Map map;
            public Vector2 startPosition;

            public GridResult(char[,] map, Vector2 startPosition, Vector2 size)
            {
                this.map = new(map, (int)size.X, (int)size.Y);
                this.startPosition = startPosition;
            }
        }

        public class Map
        {
            public char[,] map;
            public int height;
            public int width;

            public char this[Vector2 vec]
            {
                get
                {
                    return map[(int)vec.X, (int)vec.Y];
                }
            }

            public char this[int x, int y]
            {
                get
                {
                    return map[x, y];
                }
            }

            public Map(char[,] map, int height, int width)
            {
                this.map = map;
                this.height = height;
                this.width = width;
            }
        }

        private Vector2[] directions = new Vector2[]
        {
            new(0, -1),
            new(1, 0),
            new(0, 1),
            new(-1, 0)
        };

        private readonly Dictionary<char, Connection> pipeCodes = new()
        {
            {'|', new(new Vector2[] {new(0, 1), new(0, -1) }) },
            {'-', new(new Vector2[] { new(1, 0), new(-1, 0) }) },
            {'L', new(new Vector2[] { new(0, 1), new(-1, 0) }) },
            {'J', new(new Vector2[] { new(0, 1), new(1, 0) }) },
            {'7', new(new Vector2[] { new(0, -1), new(1, 0) }) },
            {'F', new(new Vector2[] { new(0, -1), new(-1, 0) }) },
            {'S', new(new Vector2[] {new(0, -1), new(1, 0), new(0, 1), new(-1, 0)}) }
        };

        private readonly float maxTimeout = 10;

        public override Answer Solve(string problemContents)
        {
            _logger.LogAsync(LogSeverity.Info, this, "Mapping pipes");
            Stopwatch timeout = new();
            GridResult result = ParseGrid(problemContents.Split("\n", StringSplitOptions.TrimEntries));

            Map map = result.map;
            Vector2 startPosition = result.startPosition;

            timeout.Start();
            bool endFound = false;

            _logger.LogAsync(LogSeverity.Info, this, "Crawling through the pipes");
            Vector2 previousPosition = new(-1, -1);
            Vector2 currentPosition = startPosition;
            Connection currentConnection = pipeCodes['S'];
            int routeLength = 0;
            while (!endFound)
            {
                //Scan around and find the next connection
                bool stepComplete = false;
                for (int i=0; i < directions.Length; i++)
                {
                    Vector2 directionRelative = directions[i];
                    Vector2 checkPosition = currentPosition + directionRelative;

                    if (checkPosition == currentPosition ||
                        (checkPosition.X < 0 || checkPosition.X >= map.width ||
                        checkPosition.Y < 0 || checkPosition.Y >= map.height) ||
                        checkPosition == previousPosition) continue;


                    char checkChar = map[checkPosition];
                    if (pipeCodes.TryGetValue(checkChar, out Connection con))
                    {
                        if (con.Contains(directionRelative) && currentConnection.Contains(-directionRelative))
                        {
                            previousPosition = currentPosition;
                            currentPosition = checkPosition;
                            currentConnection = con;
                            break;
                        }
                    }
                }

                routeLength++;

                if (currentPosition == startPosition)
                {
                    _logger.LogAsync(LogSeverity.Info, this, "I recognise this place!");
                    endFound = true;
                }

                //if (timeout.Elapsed.Seconds >= maxTimeout)
                //{
                //    _logger.LogAsync(LogSeverity.Info, this, $"Solver timeout reached after {AOC23Launcher.ParseTimer(timeout)} seconds");
                //    break;
                //}
            }

            _logger.LogAsync(LogSeverity.Info, this, "Hope you kept count :D");
            return new((routeLength/2).ToString());
        }

        public GridResult ParseGrid(string[] lines)
        {
            Vector2 startPosition = new(0, 0);
            char[,] map = new char[lines.Length, lines[0].Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                for (int j = 0; j < lines[i].Length; j++)
                {
                    map[j, i] = line[j];
                    if (line[j] == 'S') startPosition = new(j, i);
                }
            }
            return new(map, startPosition, new(lines.Length, lines[0].Length));
        }
    }
}
