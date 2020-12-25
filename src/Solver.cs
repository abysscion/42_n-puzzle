using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using N_Puzzle.PriorityQueue;

namespace N_Puzzle
{
    public class Solver
    {
        private readonly List<int> _initialState;
        private readonly List<int> _moves;
        private SolvedPuzzleInfo _info;
        private HeuristicType _heuristicType;
        private List<int> _goalState;
        private int _puzzleSize;
        private int _tmpMaxStates;
        private Stopwatch _sw;

        public Solver(string file)
        {
            if (!File.Exists(file))
                throw new Exception($"file {file} does not exist.");

            _initialState = new List<int>();
            ParseFileContent(GetFileContentWithoutComments(file));
            _moves = new List<int> {1, -1, _puzzleSize, -_puzzleSize};
        }

        public SolvedPuzzleInfo SolvePuzzle(SolvedStateType goalType, HeuristicType heuristicType)
        {
            if (!IsStateSolvable(_initialState, _puzzleSize))
            {
                var solvable = false;
                var zeroIndex = _initialState.IndexOf(0);
                
                foreach (var move in _moves)
                {
                    var neighbour = PuzzleNode.CreateMovedState(_initialState, move, zeroIndex, _puzzleSize);
                    solvable = IsStateSolvable(neighbour, _puzzleSize);
                    if (solvable)
                        break;
                }
                
                if (!solvable)
                    throw new Exception("given puzzle can't be solved to given goal state.");
            }

            var rootNode = new PuzzleNode(null, _initialState, null);
            _goalState = SolvedStates.GetSolvedState(goalType, _puzzleSize);
            _heuristicType = heuristicType;
            _info = new SolvedPuzzleInfo();
            _info.Heuristic = _heuristicType;
            _info.RootNode = rootNode;
            _info.SolvedNode = IDAStarSearch(rootNode, 0) ?? rootNode;
            _info.PuzzleSize = _puzzleSize;
            return _info;
        }

        private static bool IsStateSolvable(List<int> state, int n)
        {
            var inversionsCount = 0;
            var zeroPos = state.IndexOf(0);
            
            for (var i = 0; i < state.Count; i++)
            for (var j = i + 1; j < state.Count; j++)
            {
                if (state[i] == 0 || state[j] == 0)
                    continue;
                if (state[i] > state[j])
                    inversionsCount++;
            }
            
            if (n % 2 != 0)
                return inversionsCount % 2 == 0;
            if (zeroPos % 2 != 0)
                return inversionsCount % 2 == 0;
            return inversionsCount % 2 != 0;
        }
        
        private PuzzleNode IDAStarSearch(PuzzleNode parent, int gScore)
        {
            _sw = Stopwatch.StartNew();
            var threshold = Heuristics.GetScore(parent.State, _goalState, _heuristicType, _puzzleSize);
            PuzzleNode finalNode = null;

            while (true)
            {
                _tmpMaxStates = 0;
                var foundGoalState = SearchHelper(parent, gScore, threshold, out var minF, ref finalNode);
                //TODO: check if it wrong?
                if (_tmpMaxStates > _info.StatesInMemoryAtTheSameTime)
                    _info.StatesInMemoryAtTheSameTime = _tmpMaxStates;
                if (foundGoalState)
                    return finalNode;
                if (threshold > 300 || _sw.ElapsedMilliseconds > 15000)
                    return null;
                threshold = minF;
            }
        }

        private bool SearchHelper(PuzzleNode node, int gScore, int threshold, out int minF, ref PuzzleNode finalNode)
        {
            var fScore = gScore + Heuristics.GetScore(node.State, _goalState, _heuristicType, _puzzleSize);

            minF = fScore;
            
            if (_sw.ElapsedMilliseconds > 15000)
                return false;
            if (fScore > threshold)
                return false;
            if (node.State.SequenceEqual(_goalState))
                return true;

            var min = int.MaxValue;
            var candidates = GetNewCandidates(node, gScore);

            _tmpMaxStates += candidates.Count;
            while (candidates.Count != 0)
            {
                _info.StatesEverSelected++;
                var bestCandidate = candidates.Dequeue();
                var foundGoalState = SearchHelper(bestCandidate, gScore + 1, threshold, out minF, ref finalNode);

                if (foundGoalState)
                {
                    if (finalNode == null)
                    {
                        _info.TurnsCount = gScore + 1;
                        finalNode = bestCandidate;
                    }
                    return true;
                }

                if (minF < min)
                    min = minF;
            }

            return false;
        }

        private SimplePriorityQueue<PuzzleNode> GetNewCandidates(PuzzleNode parent, int gScore)
        {
            var candidates = new SimplePriorityQueue<PuzzleNode>();
            var zeroIndex = parent.State.IndexOf(0);

            // //==========================================================
            // //TODO: remove
            // Console.WriteLine("State before moves");
            // Utilities.PrintListAsTable(parent.State, _puzzleSize);
            // //==========================================================
            
            foreach (var move in _moves)
            {
                var movedState = PuzzleNode.CreateMovedState(parent.State, move, zeroIndex, _puzzleSize);
                var moveStateSource = PuzzleNode.GetMoveStringFromMoveValue(move, _puzzleSize);
                
                if (movedState == null)
                    continue;

                var movedNode = new PuzzleNode(parent, movedState, moveStateSource);
                var hScore = Heuristics.GetScore(movedState, _goalState, _heuristicType, _puzzleSize);
                var fScore = gScore + hScore;

                candidates.Enqueue(movedNode, fScore);
                
                // //==========================================================
                // //TODO: remove
                // var str = "";
                // if (move == 1)
                //     str = "Right ->";
                // else if (move == -1)
                //     str = "Left <-";
                // else if (move == _puzzleSize)
                //     str = "Down V";
                // else
                //     str = "Up ^";
                // Console.WriteLine($"Depth: {gScore}\tMove: {str}");
                // Utilities.PrintListAsTable(movedNode.State, _puzzleSize);
                // //==========================================================
            }

            return candidates;
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
                
                var nums = Regex.Split(fileContent[i], @"\s+");
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