using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace N_Puzzle
{
    public static class PuzzleTester
    {
        private static string _currentFile;
        private static string _message;

        public static void TestPuzzleSolvability(string folderPath)
        {
            var files = Directory.GetFiles(folderPath);
            var counter = 0;
            var sw = Stopwatch.StartNew();
            using var writer = new StreamWriter(new FileStream("_solvabilty_test.txt", FileMode.Create));

            for (var i = 0; i < files.Length; i++)
            {
                _message = "";
                _currentFile = files[i];
                var puzzleName = files[i].Split('\\')[^1];
                var solver = new Solver(_currentFile);
                var moves = new List<int> {1, -1, solver.PuzzleSize, -solver.PuzzleSize};
                var initState = solver.InitialState;
                var size = solver.PuzzleSize;

                if (Utilities.IsStateSolvable(initState, size, GoalStateType.Snail))
                    counter++;
                else
                {
                    var solvable = false;
                    var zeroIndex = solver.InitialState.IndexOf(0);

                    //sometimes algorithm understands poorly if it solvable, so here he get 2-4 more tries to calculate it
                    //    from possible moves from initial state
                    foreach (var move in moves)
                    {
                        var neighbour = PuzzleNode.CreateMovedState(initState, move, zeroIndex, size);
                        if (neighbour == null)
                            continue;
                        solvable = Utilities.IsStateSolvable(neighbour, size, GoalStateType.Snail);
                        if (!solvable)
                            continue;
                        counter++;
                        break;
                    }

                    if (!solvable)
                        writer.WriteLine($"Puzzle \"{puzzleName}\" is unsolvable for {GoalStateType.Snail} type goal.");
                }
            }

            writer.WriteLine($"\nPuzzles passed test: {counter}/{files.Length}\nTime passed: {sw.Elapsed:g}");
        }

        public static void TestPuzzleSolving(string folderPath)
        {
            var files = Directory.GetFiles(folderPath);
            var counter = 1;
            var sw = Stopwatch.StartNew();
            using var writer = new StreamWriter(new FileStream("_puzzle_solving_test.txt", FileMode.Create));

            for (var i = 0; i < files.Length; i++)
            {
                _message = "";
                _currentFile = files[i];
                var puzzleName = files[i].Split('\\')[^1];

                for (var algoI = 0; algoI < 2 /*possible algorithms count*/; algoI++)
                for (var stateI = 0; stateI < 1 /*possible goals count*/; stateI++)
                for (var heuI = 0; heuI < 2 /*possible heuristics count*/; heuI++)
                {
                    var tmpStr = "";
                    var str = "";
                    var solver = new Solver(_currentFile);
                    var info = new SolvedPuzzleInfo();
                    var heuristic = (HeuristicType) heuI;
                    var goalType = (GoalStateType) stateI;
                    var algorithm = (AlgorithmType) algoI;
                    var tabsForHeuType = heuristic switch
                    {
                        HeuristicType.LinearConflicts => "\t",
                        HeuristicType.Manhattan => "\t\t",
                        HeuristicType.Hamming => "\t\t\t",
                        _ => ""
                    };

                    try
                    {
                        info = solver.SolvePuzzle(goalType, heuristic, algorithm);
                    }
                    catch (Exception e)
                    {
                        _message = e.Message;
                    }

                    var testPassed = info.SolvedNode != null;
                    tmpStr += testPassed ? $"Time: {info.TimeThing.ElapsedMilliseconds}ms" : _message;
                    tmpStr = tmpStr == "" ? "TIME LIMIT" : tmpStr;
                    str += $"{puzzleName}\t|\t" +
                           $"{algorithm}" + "\t\t" + "|\t" +
                           $"{goalType}" + (goalType == GoalStateType.Snail ? "\t\t" : "\t") + "|\t" +
                           $"{heuristic}" + tabsForHeuType + "|\t";
                    str += $"{tmpStr}";

                    writer.WriteLine(str);
                    Console.Clear();
                    Console.WriteLine($"Time elapsed: {sw.Elapsed:g}");
                    Console.WriteLine($"Current test {counter++}/{files.Length * 2 * 1 * 2}."); //2 1 2 stands for cycles iterators.
                    Console.WriteLine($"Last test passed at {DateTime.Now:HH:mm:ss}");
                    Console.WriteLine(str);
                }

                writer.WriteLine("");
            }
        }
    }
}