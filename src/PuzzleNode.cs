using System;
using System.Collections.Generic;

namespace N_Puzzle
{
    public class PuzzleNode
    {
        public List<int> State { get; }
        public int GScore { get; }
        public int FScore { get; }
        public int HScore { get; }
        private PuzzleNode Parent { get; }
        private string MoveSource { get; }

        public PuzzleNode(PuzzleNode parent, List<int> state, string moveSource, int gScore, int hScore)
        {
            Parent = parent;
            State = state;
            MoveSource = moveSource;
            GScore = gScore;
            HScore = hScore;
            FScore = GScore + HScore;
        }

        public static List<int> CreateMovedState(List<int> state, int move, int zeroIndex, int puzzleSize)
        {
            if (!IsAbleToCreateMovedState(move, zeroIndex, puzzleSize, out var newTileIndex))
                return null;

            var tmpValue = state[newTileIndex];
            var movedState = new List<int>(puzzleSize * puzzleSize);

            for (var i = 0; i < state.Count; i++)
                movedState.Add(state[i]);

            movedState[newTileIndex] = 0;
            movedState[zeroIndex] = tmpValue;

            return movedState;
        }

        public static string GetMoveStringFromMoveValue(int move, int puzzleSize)
        {
            switch (move)
            {
                case -1:
                    return "Left";
                case 1:
                    return "Right";
                default:
                {
                    if (move == puzzleSize)
                        return "Down";
                    if (move == -puzzleSize)
                        return "Up";
                    throw new Exception($"wrong value {move} provided for method {nameof(GetMoveStringFromMoveValue)}");
                }
            }
        }

        public static List<string> GetStatesSequenceToNodeAsStrings(PuzzleNode node)
        {
            var lst = new List<string>();

            var tmp = node;
            do
            {
                lst.Add(tmp.MoveSource + "\t" + Utilities.GetStateAsString(tmp.State, "|"));
                tmp = tmp.Parent;
            } while (tmp?.Parent != null);

            lst.Reverse();
            return lst;
        }

        public static List<List<int>> GetStatesSequenceToNode(PuzzleNode node)
        {
            var lst = new List<List<int>>();

            var tmp = node;
            do
            {
                lst.Add(tmp.State);
                tmp = tmp.Parent;
            } while (tmp?.Parent != null);

            lst.Reverse();
            return lst;
        }

        private static bool IsAbleToCreateMovedState(int move, int zeroIndex, int puzzleSize, out int newTileIndex)
        {
            newTileIndex = 0;

            if (zeroIndex % puzzleSize == 0 && move == -1)
                return false;
            if (zeroIndex % puzzleSize == puzzleSize - 1 && move == 1)
                return false;
            newTileIndex = zeroIndex + move;
            if (newTileIndex < 0 || newTileIndex >= puzzleSize * puzzleSize)
                return false;

            return true;
        }
    }
}