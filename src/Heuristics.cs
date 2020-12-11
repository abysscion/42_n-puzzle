using System;
using System.Collections.Generic;

namespace N_Puzzle
{
    public static class Heuristics
    {
        // number of tiles out of correct place
        public static int GetHammingScore(List<int> puzzle, List<int> solvedPuzzle)
        {
            var score = 0;
            
            for (var i = 0; i < puzzle.Count; i++)
                if (puzzle[i] != 0 && puzzle[i] != solvedPuzzle[i])
                    score++;
            
            return score;
        }
        
        // sum of distances from current tile to goal tile
        public static int GetManhattanScore(List<int> puzzle, List<int> solvedPuzzle, int n)
        {
            var sum = 0;

            for (var i = 0; i < puzzle.Count; i++)
            {
                if (puzzle[i] == 0 || puzzle[i] == solvedPuzzle[i])
                    continue;
                
                var pieceIndex = solvedPuzzle.IndexOf(puzzle[i]);
                sum += Math.Abs(i / n - pieceIndex / n) + Math.Abs(i % n - pieceIndex % n);
            }

            return sum;
        }

        /*
            Manhattan distances + Linear conflicts count * 2
            
            T1 and T2 are the same line
            T1 in correct line, T2 in correct line
            T1 is to the right of T2                 (T2 . . T1)
            T1_goal is to the left of T2_goal        (T1_goal . . T2 . . T1 .. T2_goal)
            
            (c) https://cse.sc.edu/~mgv/csce580sp15/gradPres/HanssonMayerYung1992.pdf
        */
        public static int GetLinearConflictsScore(List<int> puzzle, List<int> solvedPuzzle, int n)
        {
            var conflictsCount = 0;
            var puzzleSize = n * n;

            for (var i = 0; i < puzzleSize; i++)
            {
                // if tile (T2) in correct column, get its goal index (T2_goal)
                if (IsTileInCorrectColumn(puzzle, solvedPuzzle, n, i, out var t2ColGoal))
                {
                    // check every tile (T1) to the down
                    for (var tmp = i + n; tmp < puzzleSize; tmp += n)
                    {
                        // if tile (T1) in correct column, get its goal index (T1_goal) AND
                        // if T2_goal > T1_goal they are in conflict 
                        if (IsTileInCorrectColumn(puzzle, solvedPuzzle, n, tmp, out var t1ColGoal) &&
                            t2ColGoal > t1ColGoal)
                            conflictsCount++;
                    }
                }
                // if tile (T2) in correct row, get its goal index (T2_goal)
                if (IsTileInCorrectRow(puzzle, solvedPuzzle, n, i, out var t2RowGoal))
                {
                    // check every tile (T1) to the right
                    for (var tmp = i + 1; tmp % n > 0; tmp++)
                    {
                        // if tile (T1) in correct row, get its goal index (T1_goal) AND
                        // if T2_goal > T1_goal they are in conflict 
                        if (IsTileInCorrectRow(puzzle, solvedPuzzle, n, tmp, out var t1RowGoal) &&
                            t2RowGoal > t1RowGoal)
                            conflictsCount++;
                    }
                }
            }

            return GetManhattanScore(puzzle, solvedPuzzle, n) + conflictsCount * 2;
        }
        
        private static bool IsTileInCorrectRow(List<int> puzzle, List<int> solvedPuzzle, int n, int tileIndex,
            out int goalIndex)
        {
            if (puzzle[tileIndex] == solvedPuzzle[tileIndex])
            {
                goalIndex = tileIndex;
                return true;
            }

            // let's assume, that tile already in correct row, then
            //    if we will iterate through columns, we will hit goalIndex at some point
            
            // checking right --->
            for (var i = tileIndex + 1; i % n > 0; i++)
            {
                if (puzzle[tileIndex] != solvedPuzzle[i])
                    continue;
                goalIndex = i;
                return true;
            }

            // checking left <---
            for (var i = tileIndex - 1; i % n >= 0; i--)
            {
                if (puzzle[tileIndex] != solvedPuzzle[i])
                    continue;
                goalIndex = i;
                return true;
            }
            
            goalIndex = 0;
            return false;
        }

        private static bool IsTileInCorrectColumn(List<int> puzzle, List<int> solvedPuzzle, int n, int tileIndex,
            out int goalIndex)
        {
            if (puzzle[tileIndex] == solvedPuzzle[tileIndex])
            {
                goalIndex = tileIndex;
                return true;
            }

            // let's assume, that tile already in correct column, then
            //    if we will iterate through rows, we will hit goalIndex at some point
            
            // checking down v
            for (var i = tileIndex + n; i < n * n; i += n)
            {
                if (puzzle[tileIndex] != solvedPuzzle[i])
                    continue;
                goalIndex = i;
                return true;
            }

            // checking up ^
            for (var i = tileIndex - n; i >= 0; i -= n)
            {
                if (puzzle[tileIndex] != solvedPuzzle[i])
                    continue;
                goalIndex = i;
                return true;
            }

            goalIndex = 0;
            return false;
        }
    }
}