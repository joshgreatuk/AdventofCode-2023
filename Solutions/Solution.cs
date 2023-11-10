using System;
using System.Collections.Generic;

namespace AOC23.Solutions
{
    public abstract class Solution 
    {
        protected readonly ILogger _logger;

        public Solution(ILogger logger) => _logger = logger;

        public abstract string GetProblemName();
        public abstract string Solve(string problemContents);
    }
}