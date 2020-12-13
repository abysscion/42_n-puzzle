using System;
using System.Collections.Generic;

namespace N_Puzzle
{
    public class PuzzleNode
    {
        public IReadOnlyList<int> State { get; }
        public PuzzleNode Parent { get; }
        public int GScore { get; }
        public int HScore { get; }
        public int N { get; }

        public PuzzleNode(PuzzleNode parent, List<int> state, SolvedStateType goalType, HeuristicType heuristicType)
        {
            Parent = parent;
            State = state;
            GScore = parent == null ? 0 : parent.GScore;
            N = parent == null ? (int) MathF.Sqrt(state.Count) : parent.N;
            HScore = Heuristics.CalculateHeuristicScore(state, goalType, heuristicType, N);
        }
    }
}