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
            // program.TestLogger();
            Task.WaitAll(program.MainAsync());
            program.Shutdown();
        }

        public AOC23Launcher()
        {
            _logger = new InnoLogger();
        }

        private readonly Type _solutionTarget = typeof(Day3);
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
            string path = Directory.GetCurrentDirectory() + $"/Problems/{problemName}.txt";
            if (!File.Exists(path))
            {
                await _logger.LogAsync(LogSeverity.Error, this, $"Problem path doesnt exist!");
                return;
            }
            string problemContent = File.ReadAllText(path);
            long elapsedTotal = stopWatch.ElapsedMilliseconds;
            await _logger.LogAsync(LogSeverity.Info, this, $"Initialized solution in {stopWatch.ElapsedMilliseconds}ms");
            await _logger.LogAsync(LogSeverity.Info, this, $"Solving problem");

            //Run solution
            Answer result = new();
            try
            {
                result = solution.Solve(problemContent);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, this, $"Solution returned exception", ex);
            }

            //Feedback result
            stopWatch.Stop();
            await _logger.LogAsync(LogSeverity.Info, this, $"Solution returned result pt1 : {result.partOne}");
            await _logger.LogAsync(LogSeverity.Info, this, $"Solution returned result pt2 : {result.partTwo}");
            await _logger.LogAsync(LogSeverity.Info, this, $"Solution completed in {stopWatch.ElapsedMilliseconds-elapsedTotal}ms");
            await _logger.LogAsync(LogSeverity.Info, this, $"Launcher completed in {stopWatch.ElapsedMilliseconds}ms");
        }

        public void Shutdown()
        {
            _logger.Shutdown();
        }

        public async Task TestLogger()
        { //Enum -> string -> enum, efficient :P
            foreach (string value in Enum.GetNames(typeof(LogSeverity)))
            {
                await _logger.LogAsync((LogSeverity)Enum.Parse(typeof(LogSeverity), value), this, $"{value} Test");
            }
        }
    }
}