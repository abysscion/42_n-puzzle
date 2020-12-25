using System;
using System.Globalization;

namespace N_Puzzle
{
    internal static class NPuzzleSolver
    {
        private static void Main(string[] args)
        {
            // PuzzleTester.TestFiles("C:\\Born2Code\\C#\\42_n-puzzle\\puzzles\\");

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
                Console.WriteLine("[Error] " + (e.Message.Length != 0 ? e.Message : e.ToString()));
                //TODO: delete before release 
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void ProceedArguments(string[] args)
        {
            OptionsParser.Parse(args);
            var solver = new Solver(args[^1]);
            var puzzleInfo = solver.SolvePuzzle(OptionsParser.GoalFlag, OptionsParser.HeuristicFlag);
            
            PrintInfo(puzzleInfo);
        }

        private static void PrintInfo(SolvedPuzzleInfo info)
        {
            Console.WriteLine($"Selected heuristic: {info.Heuristic}");
            Console.WriteLine($"Total ever states selected (complexity in time): {info.StatesEverSelected}");
            Console.WriteLine($"Maximum states in memory (complexity in size): {info.StatesInMemoryAtTheSameTime}");
            Console.WriteLine($"Turns to solve puzzle: {info.TurnsCount}");
            Console.WriteLine();

            if (OptionsParser.TableStepFlag)
            {
                Utilities.PrintStateAsTable(info.RootNode.State, info.PuzzleSize);
                var statesSequence = PuzzleNode.GetStatesSequenceToNode(info.SolvedNode);
                foreach (var state in statesSequence)
                    Utilities.PrintStateAsTable(state, info.PuzzleSize);
            }
            else
            {
                Console.WriteLine("\t" + Utilities.GetStateAsString(info.RootNode.State, "|"));
                var statesSequence = PuzzleNode.GetStatesSequenceToNodeAsStrings(info.SolvedNode);
                foreach (var state in statesSequence)
                    Console.WriteLine(state);
            }
        }
        
        private static void PrintUsage()
        {
            Console.WriteLine("N-Puzzle [options] <path/to/file/puzzle.txt>");
            Console.WriteLine("options:\n" +
                              "\t-heuristic:<name>\t - use specific heuristic. Currently available <name>: \"Manhattan\", \"LinearConflicts\", \"Hamming\"." +
                              "\t-goal:<name>\t - use specific goal state. Currently available <name>: \"ZeroLast\", \"ZeroFirst\", \"Snail\"." +
                              "\t-ts\t\t- print solving steps as tables\n" +
                              "\t-r\t\t- option 2");
        }
    }
}