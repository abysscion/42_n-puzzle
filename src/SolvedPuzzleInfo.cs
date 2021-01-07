using System.Diagnostics;

namespace N_Puzzle
{
    public struct SolvedPuzzleInfo
    {
        public HeuristicType Heuristic;
        public PuzzleNode SolvedNode;
        public PuzzleNode RootNode;
        public Stopwatch TimeThing;
        public int StatesInMemoryAtTheSameTime;
        public int StatesEverSelected;
        public int TurnsCount;
        public int PuzzleSize;
    }
}