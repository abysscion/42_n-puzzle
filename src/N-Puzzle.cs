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
                // Console.WriteLine("[Error] " + (e.Message.Length != 0 ? e.Message : e.ToString()));
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void ProceedArguments(string[] args)
        {
            OptionsParser.Parse(args);
            var solver = new Solver(args[^1]);

            // var puzzle = new List<int>();
            // var solved = SolvedStates.GetSolvedStates_Snail(4);
            //
            // for (var i = 0; i < 16; i++)
            //     puzzle.Add(i);
            // Utilities.ShuffleList(puzzle);
            //
            // Console.WriteLine(Heuristics.GetLinearConflictsScore(puzzle, solved, 3));
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