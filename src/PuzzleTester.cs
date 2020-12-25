using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ThreadState = System.Threading.ThreadState;

namespace N_Puzzle
{
    public static class PuzzleTester
    {
        private static SolvedPuzzleInfo _info;
        private static SolvedStateType _solvedStateType;
        private static HeuristicType _heuristicType;
        private static string _currentFile;
        private static string _message;
        private static bool _solved;

        public static void TestFiles(string folderPath)
        {
            var files = Directory.GetFiles(folderPath);
            using var writer = new StreamWriter(new FileStream("_checker_results.txt", FileMode.Create));

            for (var i = 0; i < files.Length; i++)
            {
                _currentFile = files[i];
                var puzzleName = files[i].Split('\\')[^1];

                for (var stateI = 0; stateI < 3; stateI++)
                for (var heuI = 0; heuI < 3; heuI++)
                    MakeTest(writer, puzzleName, (SolvedStateType)stateI, (HeuristicType)heuI);
            }
        }

        private static void MakeTest(TextWriter writer, string puzzleName, SolvedStateType solvedStateType,
            HeuristicType heuristicType)
        {
            _solved = false;
            _heuristicType = heuristicType;
            _solvedStateType = solvedStateType;
            _message = "";
            
            var thread = new Thread(ExecuteInBackground);
            thread.Start();
            var sw = Stopwatch.StartNew();

            try
            {
                while (thread.ThreadState == ThreadState.Running && !_solved)
                {
                    ;
                }
            }
            catch (Exception e)
            {
                _message += $"\n[Error in main thread] {e.Message}";
            }
            finally
            {
                thread.Join();
                var export = "";
                export += $"{puzzleName} | {_heuristicType} | {_solvedStateType} | ";
                export += _solved ? $"Time:{sw.ElapsedMilliseconds}ms" : (_message == "" ? "TIME LIMIT" : _message);

                writer.WriteLine(export);
            }
        }

        private static void ExecuteInBackground()
        {
            try
            {
                var solver = new Solver(_currentFile);
                _info = solver.SolvePuzzle(_solvedStateType, _heuristicType);
                _solved = true;
                if (_info.SolvedNode == _info.RootNode)
                    _solved = false;
            }
            catch (Exception e)
            {
                _message = e.Message;
                throw;
            }
        }
    }
}