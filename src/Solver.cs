using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace N_Puzzle
{
    public class Solver
    {
        private readonly List<int> _initialState;
        private List<int> _moves;
        private int _puzzleSize;

        public Solver(string file)
        {
            if (!File.Exists(file))
                throw new Exception($"file {file} does not exist.");
            
            _initialState = new List<int>();
            _moves = new List<int> {1, -1, _puzzleSize, -_puzzleSize};
            ParseFileContent(GetFileContentWithoutComments(file));
        }

        public static bool IsPuzzleSolvable(List<int> state, SolvedStateType goalType, int n)
        {
            var inversionsCount = 0;
            var goalState = SolvedStates.GetSolvedState(goalType, n);
            
            for (var i = 0; i < n * n - 1; i++)
            for (var j = i + 1; j < n * n; j++)
            {
                if (goalState.IndexOf(state[i]) > goalState.IndexOf(state[j]))
                    inversionsCount++;
            }

            var pIndex = state.IndexOf(0);
            var spIndex = goalState.IndexOf(0);
            var distance = Math.Abs(pIndex / n - spIndex / n) + Math.Abs(pIndex % n - spIndex % n);

            if (distance % 2 == 0 && inversionsCount % 2 == 0)
                return true;
            if (distance % 2 != 0 && inversionsCount % 2 != 0)
                return true;
            return false;
        }

        public void SolvePuzzle(SolvedStateType goalType, HeuristicType heuristicType)
        {
            if (!IsPuzzleSolvable(_initialState, goalType, _puzzleSize))
                throw new Exception($"given puzzle can't be solved to given goal state.");

            var x = IDAStarSearch(new PuzzleNode(null, _initialState, goalType, heuristicType));
        }

        public PuzzleNode IDAStarSearch(PuzzleNode root)
        {
            //TODO: ready to make IDA star
            return root;
        }
        
        private void ParseFileContent(List<string> fileContent)
        {
            if (!Regex.IsMatch(fileContent[0], @"^\d$"))
                throw new Exception("first line of config should contain only 1 integer digit (size of puzzle).");
            
            _puzzleSize = int.Parse(fileContent[0]);

            if (fileContent.Count - 1 != _puzzleSize)
                throw new Exception($"invalid puzzle pieces lines ({fileContent.Count - 1}), while should be ({_puzzleSize}).");
            for (var i = 1; i < fileContent.Count; i++)
            {
                if (Regex.IsMatch(fileContent[i], @"[^\d\s]"))
                    throw new Exception("puzzle config lines (except comments) should contain only positive integer digits.");
                
                var nums = fileContent[i].Split(' ');
                if (nums.Length != _puzzleSize)
                    throw new Exception($"invalid puzzle pieces count in a row ({nums.Length}), while should be ({_puzzleSize}).");
                
                foreach (var num in nums)
                {
                    var tmp = int.Parse(num);

                    if (_initialState.Contains(tmp))
                        throw new Exception($"puzzle should not contain same pieces: {tmp}.");
                    _initialState.Add(tmp);
                }
            }
        }
        
        private List<string> GetFileContentWithoutComments(string file)
        {
            using var fs = new FileStream(file, FileMode.Open);
            using var reader = new StreamReader(fs);
            string line;
            var fileContent = new List<string>();
            
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith('#'))
                    continue;
                line = Regex.Replace(line, @"#.+", "");
                line = line.Trim();
                if (line.Length != 0)
                    fileContent.Add(line);
            }

            return fileContent;
        }
    }
}