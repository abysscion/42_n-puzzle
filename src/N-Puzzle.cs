using System;
using System.Collections.Generic;
using System.Globalization;

namespace N_Puzzle
{
    internal static class NPuzzleSolver
    {
        private static void Main(string[] args)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);

                if (args.Length < 1)
                    PrintUsage();
                else
                    ProceedArguments(args);
            }
            catch (Exception e)
            {
                //TODO: uncomment before release 
                // Console.WriteLine("[Error] " + (e.Message.Length != 0 ? e.Message : e.ToString()));
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void ProceedArguments(string[] args)
        {
            OptionsParser.Parse(args);
            var solver = new Solver(args[^1]);
            const int n = 3;
            var solved = SolvedStates.GetSolvedState(SolvedStateType.ZeroFirst, n);
            var puzzle = new List<int> {4,2,5,1,0,6,3,8,7};
            solver.SolvePuzzle(SolvedStateType.ZeroFirst, HeuristicType.Manhattan);
            
        }

        private static void PrintUsage()
        {
            Console.WriteLine("N-Puzzle [options] <path/to/file/puzzle.txt>");
            Console.WriteLine("options:\n" +
                              "\t-s\t\t- option 1\n" +
                              "\t-r\t\t- option 2");
        }
    }
}