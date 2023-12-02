using System;

namespace AOC23.Solutions
{
    public class TestSolution : Solution
    {
        public TestSolution(ILogger logger) : base(logger) {}

        public override string GetProblemName() => "TestProblem";

        public override Answer Solve(string problemContents)
        {
            _logger.Log(LogSeverity.Info, this, $"Hit Solver");
            return new(problemContents);
        }
    }
}