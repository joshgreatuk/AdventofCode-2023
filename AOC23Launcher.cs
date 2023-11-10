using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using AOC23.Solutions;

namespace AOC23
{
    public class AOC23Launcher
    {
        private ILogger _logger;

        public static void Main(string[] args)
        {
            AOC23Launcher program = new();
            program.MainAsync();
            program.Shutdown();
        }

        public AOC23Launcher()
        {
            _logger = new InnoLogger();
        }

        private readonly Type _solutionTarget = typeof(Solution);
        private Solution solution;

        public async Task MainAsync()
        {
            Stopwatch stopWatch = new();
            stopWatch.Start();
            await _logger.LogAsync(LogSeverity.Info, this, "Initializing");

            //Create solution
            solution = Activator.CreateInstance(_solutionTarget, args: new[] {_logger}) as Solution;
            if (solution == null) 
            {
                await _logger.LogAsync(LogSeverity.Error, this, $"_solutionTarget was not of type Solution");
                return;
            }

            //Grab solution's problem
            string problemName = solution.GetProblemName();
            string path = Directory.GetCurrentDirectory() + $"/{problemName}.txt";

            //Run solution

            //Feedback result
            
        }

        public void Shutdown()
        {
            _logger.Shutdown();
        }
    }
}