using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC23.Solutions
{
    public class Day9 : Solution
    {
        public Day9(ILogger logger) : base(logger) { }
        public override string GetProblemName() => "Day9";

        public class Node
        {
            public long value;

            public Node? parentA;
            public Node? parentB;
            public Node? nextA;
            public Node? nextB;

            public Node(long value, Node? parentA=null, Node? parentB=null)
            {
                this.value = value;
                this.parentA = parentA;
                this.parentB = parentB;
            }
        }

        public override Answer Solve(string problemContents)
        {
            _logger.LogAsync(LogSeverity.Info, this, "Deciphering history");
            string[] problemLines = problemContents.Split('\n');
            long[] extrapolatedTotal = new long[problemLines.Length];
            long[] backwardsTotal = new long[problemLines.Length];

            _logger.LogAsync(LogSeverity.Info, this, "Constructing pyramid schemes");
            for (int i=0; i < problemLines.Length; i++)
            {
                long[] values = problemLines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToArray();
                List<Node> rootRow = values.Select(x => new Node(x)).ToList();

                List<Node> currentRow = rootRow.ToList();
                while (currentRow.Any(x => x.value != currentRow[0].value))
                {
                    currentRow = CalculateNextRow(currentRow);
                }


                Node extrapolatedNode = ExtrapolateNode(rootRow);
                extrapolatedTotal[i] = extrapolatedNode.value;

                Node backwardsNode = ExtrapolateNode(rootRow, true);
                backwardsTotal[i] = backwardsNode.value;
            }

            _logger.LogAsync(LogSeverity.Info, this, "Guess the pyramids WERE built in a day :D");
            return new(extrapolatedTotal.Sum().ToString(), backwardsTotal.Sum().ToString());
        }

        public List<Node> CalculateNextRow(List<Node> parentRow)
        {
            List<Node> nextRow = new();
            for (int i=0; i < parentRow.Count-1; i++)
            {
                Node nodeA = parentRow[i];
                Node nodeB = parentRow[i + 1];

                long newValue = nodeB.value - nodeA.value;
                Node newNode = new(newValue, nodeA, nodeB);

                nodeA.nextB = newNode;
                nodeB.nextA = newNode;

                nextRow.Add(newNode);
            }
            return nextRow;
        }

        public Node ExtrapolateNode(List<Node> rootRow, bool backwards=false)
        {
            //Add new nodes then step back down calculating as we go
            if (backwards)
            {
                rootRow.Reverse();
            }

            Node referenceNode = rootRow.Last();
            Node currentNode = new(0);
            rootRow.Add(currentNode);

            List<Node> currentRow = rootRow.ToList();
            bool reachedEnd = false;
            while (!reachedEnd)
            {
                Node newNode = new(0, referenceNode, currentNode);
                currentNode.nextA = newNode;
                currentNode = newNode;

                if (referenceNode == null || currentRow.All(x => x.value == currentRow[0].value)) break;
                if (backwards) currentRow = currentRow.Select(x => x.nextA != null ? x.nextA : x.nextB).ToList();
                else currentRow = currentRow.Select(x => x.nextB != null ? x.nextB : x.nextA).ToList();
                currentRow.RemoveAt(currentRow.Count-1);
                referenceNode = currentRow.Last();
            }

            //Walk back and calculate each node
            while (currentNode.parentA != null)
            {
                currentNode.parentB.value = backwards ? currentNode.parentA.value - currentNode.value : currentNode.parentA.value + currentNode.value;
                currentNode = currentNode.parentB;
            }

            //Return the root node of the new line
            return currentNode;
        }

        public void DebugTree(List<Node> nodes)
        {
            bool reachedEnd = false;
            while (!reachedEnd)
            {
                _logger.LogAsync(LogSeverity.Debug, this, string.Join(" ", nodes.Select(x => x.value.ToString())));

                if (nodes.All(x => x.value == 0)) break;

                List<Node> nextNodes = nodes.Where(x => x.nextA != null).Select(x => x.nextA).ToList();
                nodes = nextNodes;
            }
        }
    }
}
