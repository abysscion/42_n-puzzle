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
        private const long InfoPrintDelay = 250; //250ms
        private readonly List<int> _moves;
        private SolvedPuzzleInfo _info;
        private HeuristicType _heuristicType;
        private AlgorithmType _algorithmType;
        private Stopwatch _sw;
        private List<int> _goalState;
        private long _lastInfoPrintTime;
        private int _tmpMaxStates;
        private int _timeLimit;
        
        public List<int> InitialState { get; }
        public int PuzzleSize { get; private set; }

        public Solver(string file)
        {
            if (!File.Exists(file))
                throw new Exception($"file {file} does not exist.");

            InitialState = new List<int>();
            ParseFileContent(GetFileContentWithoutComments(file));
            _moves = new List<int> {1, -1, PuzzleSize, -PuzzleSize};
        }

        public SolvedPuzzleInfo SolvePuzzle(GoalStateType goalType, HeuristicType heuristicType,
                                            AlgorithmType algorithmType, int timeLimit = 10000)
        {
            if (!Utilities.IsStateSolvable(InitialState, PuzzleSize, goalType))
            {
                var solvable = false;
                var zeroIndex = InitialState.IndexOf(0);

                //Additional check for neighbour positions created via empty tile moves. For a strange cases.
                foreach (var move in _moves)
                {
                    var neighbour = PuzzleNode.CreateMovedState(InitialState, move, zeroIndex, PuzzleSize);
                    if (neighbour == null)
                        continue;
                    solvable = Utilities.IsStateSolvable(neighbour, PuzzleSize, goalType);
                    if (solvable)
                        break;
                }

                if (!solvable)
                    throw new Exception("given puzzle can't be solved to given goal state.");
            }

            _timeLimit = timeLimit;
            _algorithmType = algorithmType;
            _goalState = GoalStates.GetGoalState(goalType, PuzzleSize);
            _heuristicType = heuristicType;
            _lastInfoPrintTime = 0;
            var goalState = GoalStates.GetGoalState(goalType, PuzzleSize);
            var initialHScore = Heuristics.GetScore(InitialState, goalState, heuristicType, PuzzleSize);
            var rootNode = new PuzzleNode(null, InitialState, null, 0, initialHScore);
            _info = new SolvedPuzzleInfo {Heuristic = _heuristicType, RootNode = rootNode, PuzzleSize = PuzzleSize};
            //============== search entry point =================
            if (algorithmType == AlgorithmType.Astar)
                _info.SolvedNode = AStarSearch(rootNode);
            else if (algorithmType == AlgorithmType.IDAstar)
                _info.SolvedNode = IDAStarSearch(rootNode);
            else
                throw new Exception("unknown algorithm type provided!");
            //===================================================
            _sw.Stop();
            _info.TimeThing = _sw;
            return _info;
        }

        private PuzzleNode AStarSearch(PuzzleNode parent)
        {
            _sw = Stopwatch.StartNew();
            var fScore = Heuristics.GetScore(parent.State, _goalState, _heuristicType, PuzzleSize); //g = 0
            var openSet = new SimplePriorityQueue<PuzzleNode>();
            var closedSet = new List<PuzzleNode>();

            openSet.Enqueue(parent, fScore);
            while (openSet.Count != 0)
            {
                if (_timeLimit > 0 && _sw.ElapsedMilliseconds > _timeLimit)
                    return null;
                var currentNode = openSet.Dequeue();
                if (currentNode.State.SequenceEqual(_goalState))
                {
                    _info.TurnsCount = currentNode.GScore;
                    return currentNode;
                }
                
                PrintInfoIfNeeded(currentNode.GScore, currentNode.HScore);

                closedSet.Add(currentNode);
                var candidates = GetNewCandidates(currentNode, currentNode.GScore + 1);
                foreach (var candidate in candidates)
                {
                    if (!closedSet.Contains(candidate) && !openSet.Contains(candidate))
                        openSet.Enqueue(candidate, candidate.FScore);
                    else
                    {
                        if (candidate.FScore <= (currentNode.GScore + 1) + candidate.HScore)
                            continue;
                        if (!closedSet.Contains(candidate))
                            continue;
                        closedSet.Remove(candidate);
                        openSet.Enqueue(candidate, candidate.FScore);
                    }
                }
                if (_info.StatesInMemoryAtTheSameTime < openSet.Count + closedSet.Count)
                    _info.StatesInMemoryAtTheSameTime = openSet.Count + closedSet.Count;
                _info.StatesEverSelected = closedSet.Count;
            }

            return null;
        }

        private PuzzleNode IDAStarSearch(PuzzleNode parent)
        {
            _sw = Stopwatch.StartNew();
            var threshold = Heuristics.GetScore(parent.State, _goalState, _heuristicType, PuzzleSize);
            PuzzleNode finalNode = null;

            while (true)
            {
                _tmpMaxStates = 0;

                var tmpF = SearchHelper(parent, 0, threshold, ref finalNode);

                if (_tmpMaxStates > _info.StatesInMemoryAtTheSameTime)
                    _info.StatesInMemoryAtTheSameTime = _tmpMaxStates;
                if (finalNode != null)
                    return finalNode;
                if (tmpF > 666) //too deep search
                    return null;
                if (tmpF == -1) //time limit
                    return null;
                
                threshold = tmpF;
            }
        }

        //helper method for IDA star search. returns minimum f_score encountered, such as f_score > threshold 
        private int SearchHelper(PuzzleNode node, int gScore, int threshold, ref PuzzleNode finalNode)
        {
            var hScore = Heuristics.GetScore(node.State, _goalState, _heuristicType, PuzzleSize);
            var fScore = gScore + hScore;
            var nextThreshold = int.MaxValue;
            
            if (_timeLimit > 0 && _sw.ElapsedMilliseconds > _timeLimit)
                return -1;
            //encountered greater f score => restarting search with new threshold
            if (fScore > threshold)
                return fScore;
            //returning of fscore is not necessary if we found finalNode, btw
            if (finalNode == null)
            {
                if (node.State.SequenceEqual(_goalState))
                {
                    finalNode = node;
                    _info.TurnsCount = gScore + 1;
                    return int.MaxValue;
                }
            }
            else
                return int.MaxValue;

            //create new candidates aka possible states after empty tile move
            var candidates = GetNewCandidates(node, gScore);

            //info managing
            _tmpMaxStates += candidates.Count;
            PrintInfoIfNeeded(gScore, hScore);

            //run search for every candidate
            while (candidates.Count != 0)
            {
                _info.StatesEverSelected++;
                var bestCandidate = candidates.Dequeue();
                //going down to the tree, to the BEST candidate found by f score of every moved state
                var tmpF = SearchHelper(bestCandidate, gScore + 1, threshold, ref finalNode);
                if (finalNode != null)
                    break;
                // save new minimum f_score encountered, such as f_score > threshold 
                if (tmpF < nextThreshold)
                    nextThreshold = tmpF;
            }

            return nextThreshold;
        }

        //returns NEWLY created queue of NEWLY created nodes, made by possible moves from position of empty tile
        //more priority given to lesser f score values
        private SimplePriorityQueue<PuzzleNode> GetNewCandidates(PuzzleNode parent, int gScore)
        {
            var candidates = new SimplePriorityQueue<PuzzleNode>();
            var zeroIndex = parent.State.IndexOf(0);

            foreach (var move in _moves)
            {
                var movedState = PuzzleNode.CreateMovedState(parent.State, move, zeroIndex, PuzzleSize);
                var moveStateSource = PuzzleNode.GetMoveStringFromMoveValue(move, PuzzleSize);

                if (movedState == null)
                    continue;

                var hScore = Heuristics.GetScore(movedState, _goalState, _heuristicType, PuzzleSize);
                var movedNode = new PuzzleNode(parent, movedState, moveStateSource, gScore, hScore);

                candidates.Enqueue(movedNode, movedNode.FScore);
            }

            return candidates;
        }

        private void PrintInfoIfNeeded(int gScore, int hScore)
        {
            //print ongoing solving info if needed (eats performance)
            if (!OptionsParser.PrintSolvingInfo)
                return;
            if (_sw.ElapsedMilliseconds <= _lastInfoPrintTime + InfoPrintDelay)
                return;
            
            _lastInfoPrintTime = _sw.ElapsedMilliseconds;
            Console.Clear();
            Console.WriteLine("*SOLVING IS GOING ON*");
            Console.WriteLine($"Current depth: {gScore}");
            Console.WriteLine($"Heuristic score for last node: {hScore}");
            Console.WriteLine($"Total nodes traversed: {_info.StatesEverSelected}");
            if (_algorithmType == AlgorithmType.IDAstar)
            {
                Console.WriteLine($"Current nodes in memory: {_tmpMaxStates}");
                Console.WriteLine($"Maximum nodes in memory after reaching minimum f: {_info.StatesInMemoryAtTheSameTime}");
            }
            else
                Console.WriteLine($"Current nodes in memory: {_info.StatesInMemoryAtTheSameTime}");
            Console.WriteLine($"Time elapsed: {_sw.Elapsed.Minutes}:{_sw.Elapsed.Seconds}:{_sw.Elapsed.Milliseconds}");
        }
        
        private void ParseFileContent(List<string> fileContent)
        {
            if (!Regex.IsMatch(fileContent[0], @"^\d$"))
                throw new Exception("first line of config should contain only 1 integer digit (size of puzzle).");

            PuzzleSize = int.Parse(fileContent[0]);

            if (fileContent.Count - 1 != PuzzleSize)
                throw new Exception(
                    $"invalid puzzle pieces lines ({fileContent.Count - 1}), while should be ({PuzzleSize}).");
            for (var i = 1; i < fileContent.Count; i++)
            {
                if (Regex.IsMatch(fileContent[i], @"[^\d\s]"))
                    throw new Exception(
                        "puzzle config lines (except comments) should contain only positive integer digits.");

                var nums = Regex.Split(fileContent[i], @"\s+");
                if (nums.Length != PuzzleSize)
                    throw new Exception(
                        $"invalid puzzle pieces count in a row ({nums.Length}), while should be ({PuzzleSize}).");

                foreach (var num in nums)
                {
                    var tmp = int.Parse(num);

                    if (InitialState.Contains(tmp))
                        throw new Exception($"puzzle should not contain same pieces: {tmp}.");
                    InitialState.Add(tmp);
                }
            }
            if (InitialState.IndexOf(0) == -1)
                throw new Exception("puzzle should contain empty piece.");
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