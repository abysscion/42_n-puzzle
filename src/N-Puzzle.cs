using System;
using System.Globalization;

namespace N_Puzzle
{
    internal static class NPuzzleSolver
    {
        private static void Main(string[] args)
        {
            //PuzzleTester.TestPuzzleSolving("C:\\Born2Code\\C#\\42_n-puzzle\\correctPuzzles\\");
			//PuzzleTester.TestPuzzleSolving("C:\\Born2Code\\C#\\42_n-puzzle\\incorrectPuzzles\\");
            // PuzzleTester.TestPuzzleSolvability("C:\\Born2Code\\C#\\42_n-puzzle\\correctPuzzles\\");
             PuzzleTester.TestPuzzleSolvability("C:\\Born2Code\\C#\\42_n-puzzle\\incorrectPuzzles\\");
            // Utilities.CreateRandomPuzzles();
            // Utilities.CreateRandomIncorrectPuzzles();

/*
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
            }
*/
        }

        private static void ProceedArguments(string[] args)
        {
            OptionsParser.Parse(args);
            var solver = new Solver(args[^1]);
            var puzzleInfo = solver.SolvePuzzle(OptionsParser.GoalFlag, OptionsParser.HeuristicFlag,
                OptionsParser.AlgorithmFlag, OptionsParser.TimeLimitFlag);
            PrintInfo(puzzleInfo);
        }

        private static void PrintInfo(SolvedPuzzleInfo info)
        {
            Console.WriteLine();
            Console.WriteLine($"Selected heuristic: {info.Heuristic}");
            Console.WriteLine($"Total ever states selected (complexity in time): {info.StatesEverSelected}");
            Console.WriteLine($"Maximum states in memory (complexity in size): {info.StatesInMemoryAtTheSameTime}");
            if (info.SolvedNode != null)
                Console.WriteLine($"Turns to solve puzzle: {info.TurnsCount}");
            Console.WriteLine($"Time elapsed: {info.TimeThing.ElapsedMilliseconds}ms");
            Console.WriteLine();

            if (info.SolvedNode == null)
            {
                Console.WriteLine("Status: not solved");
                return;
            }

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
            Console.WriteLine("N-Puzzle [options] <path/to/file/puzzle.txt>\n\t(by default it uses Manhattan heuristic and looks for Zerolast solution)");
            Console.WriteLine("options:\n" +
                              "\t-algorithm:<name>\t- use specific algorithm. Currently available <name>: \"Astar\", \"IDAstar\".\n" +
                              "\t-heuristic:<name>\t- use specific heuristic. Currently available <name>: \"Manhattan\", \"LinearConflicts\", \"Hamming\".\n" +
                              "\t-goal:<name>\t\t- use specific goal state. Currently available <name>: \"ZeroLast\", \"ZeroFirst\", \"Snail\".\n" +
                              "\t-ts\t\t\t- print solving steps as tables.\n" +
                              "\t-v\t\t\t- print info about solving as it goes.\n" +
                              "\t-t:<n>\t\t\t- time limit, where <n> is a number of milliseconds in range [0, max int). If n is zero, no limit will be applied.");
        }
    }
}