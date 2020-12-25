namespace N_Puzzle
{
    public struct SolvedPuzzleInfo
    {
        public HeuristicType Heuristic;
        public PuzzleNode SolvedNode;
        public PuzzleNode RootNode;
        public int StatesInMemoryAtTheSameTime;
        public int StatesEverSelected;
        public int TurnsCount;
        public int PuzzleSize;
    }
}