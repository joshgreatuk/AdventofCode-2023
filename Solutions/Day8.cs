using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC23.Solutions
{
    public class Day8 : Solution
    {
        public Day8(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day8";

        public struct Node
        {
            public string left;
            public string right;

            public Node(string left, string right)
            {
                this.left = left;
                this.right = right;
            }
        }

        public override Answer Solve(string problemContents)
        {
            _logger.LogAsync(LogSeverity.Info, this, "Reading the map");
            string[] problemLines = problemContents.Split('\n');
            char[] instructions = problemLines[0].Trim().ToCharArray();

            List<string> orderedNodes = new();
            Dictionary<string, Node> nodes = new();

            //Parse the map first
            for (int i = 2; i < problemLines.Length; i++)
            {
                string[] splitLine = problemLines[i].Split("=", StringSplitOptions.RemoveEmptyEntries);
                string[] paths = splitLine[1].Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
                nodes.Add(splitLine[0].Trim(), new(paths[1].Trim(), paths[2].Trim()));
                orderedNodes.Add(splitLine[0].Trim());
            }

            _logger.LogAsync(LogSeverity.Info, this, "Listening to the instructions");
            //Now process the instructions and count
            int normalMoveCount = ReadMap(orderedNodes, nodes, instructions, new int[] { orderedNodes.IndexOf("AAA") });

            //Process ghostly instructions
            int[] ghostlyStartingIndexes = orderedNodes.Where(x => x[2] == 'A').Select(x => orderedNodes.IndexOf(x)).ToArray();
            int ghostlyMoveCount = ReadMap(orderedNodes, nodes, instructions, ghostlyStartingIndexes, true);

            _logger.LogAsync(LogSeverity.Info, this, "Now thats what you call map reading!");
            return new(normalMoveCount.ToString(), ghostlyMoveCount.ToString());
        }

        public int ReadMap(List<string> orderedNodes, Dictionary<string, Node> nodes, char[] instructions, int[] startingIndexes, bool onlyTargetLast=false)
        {
            if (startingIndexes[0] == -1)
            {
                _logger.LogAsync(LogSeverity.Error, this, "Part 1 isn't possible with this puzzle input (Doesn't have 'AAA')");
                return -1;
            }

            int moveCount = 0;
            int[] targetIndexes = startingIndexes.ToArray();
            bool allOnZ = false;

            while (!allOnZ) 
            {
                if (!onlyTargetLast && targetIndexes.All(x => orderedNodes[x] == "ZZZ")
                        || onlyTargetLast && targetIndexes.All(x => orderedNodes[x][2] == 'Z'))
                {
                    allOnZ = true;
                    break;
                }

                char instruction = instructions[moveCount - ((int)MathF.Floor(moveCount / instructions.Length) * instructions.Length)];

                for (int i = 0; i < targetIndexes.Length; i++)
                {
                    Node node = nodes[orderedNodes[targetIndexes[i]]];
                    targetIndexes[i] = orderedNodes.IndexOf(instruction == 'L' ? node.left : node.right);
                }

                moveCount++;
            }
            return moveCount;
        }
    }
}
