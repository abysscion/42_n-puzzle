using System;
using System.Collections.Generic;
using System.IO;

namespace N_Puzzle
{
    public static class Utilities
    {
        public static void ShuffleList(List<int> list)
        {
            var rng = new Random();
            var n = list.Count;
            
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        
        public static void PrintStateAsTable(List<int> state, int size)
        {
            for (var i = 0; i < size; i++)
            {
                for (var j = 0 ; j < size; j++)
                    Console.Write(state[j + i * size] + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        
        public static string GetStateAsString(List<int> state, string delimiter)
        {
            var result = "";

            for (var i = 0; i < state.Count - 1; i++)
            {
                var tile = state[i];
                result += tile + delimiter;
            }

            result += state[^1];
            return result;
        }

        public static void CreateRandomIncorrectPuzzles()
        {
            var rng = new Random();
            //from 3n puzzle to 7n puzzle
            for (var size = 3; size < 8; size++)
            {
                for (var count = 0; count < 50; count++)
                {
                    using var writer = new StreamWriter(new FileStream($"C:\\Born2Code\\C#\\42_n-puzzle\\incorrectPuzzles\\rnd_puzzle_{size}#{count}.txt", FileMode.Create));
                    
                    //create solved puzzle state
                    var puzzle = GoalStates.GetGoalState(GoalStateType.ZeroFirst, size);

                    //shuffle until it's unsolvable
                    do
                    { 
                        ShuffleList(puzzle);
                    } while (IsStateSolvable(puzzle, size, GoalStateType.Snail));

                    //save
                    writer.WriteLine(size);
                    var index = 0;
                    for (var row = 0; row < size; row++)
                    {
                        for (var col = 0; col < size; col++)
                            writer.Write($"{puzzle[index++]}\t");
                        writer.WriteLine();
                    }
                    
                    writer.Close();
                }
            }
        }
        
        public static void CreateRandomPuzzles()
        {
            var rng = new Random();
            //from 3n puzzle to 7n puzzle
            for (var size = 3; size < 8; size++)
            {
                var moves = new [] {1, -1, size, -size};
                for (var count = 0; count < 50; count++)
                {
                    using var writer = new StreamWriter(new FileStream($"C:\\Born2Code\\C#\\42_n-puzzle\\correctPuzzles\\rnd_puzzle_{size}#{count}.txt", FileMode.Create));
                    
                    //create solved puzzle state
                    var puzzle = GoalStates.GetGoalState(GoalStateType.Snail, size);

                    //shuffle with 200 random moves
                    for (var stepsCount = 0; stepsCount < 200; stepsCount++)
                    {
                        var zeroIndex = puzzle.IndexOf(0);
                        var rndI = rng.Next(0, moves.Length);
                        var candidate = PuzzleNode.CreateMovedState(puzzle, moves[rndI], zeroIndex, size);
                        
                        if (candidate == null)
                            continue;
                        puzzle = candidate;
                    }

                    //save
                    writer.WriteLine(size);
                    var index = 0;
                    for (var row = 0; row < size; row++)
                    {
                        for (var col = 0; col < size; col++)
                            writer.Write($"{puzzle[index++]}\t");
                        writer.WriteLine();
                    }
                    
                    writer.Close();
                }
            }
        }

        public static bool IsStateSolvable(List<int> state, int n, GoalStateType goalType)
        {
            if (goalType == GoalStateType.Snail)
                return IsSnailStateSolvable(state, n);
            
            var inversionsCount = 0;
            var zeroPos = state.IndexOf(0);

            for (var i = 0; i < state.Count - 1; i++)
            {
                if (state[i] == 0)
                    continue;
                for (var j = i + 1; j < state.Count; j++)
                {
                    if (state[i] == 0 || state[j] == 0)
                        continue;
                    if (state[i] > state[j])
                        inversionsCount++;
                }
            }

            if (n % 2 != 0)
                return inversionsCount % 2 == 0;
            if (zeroPos % 2 != 0)
                return inversionsCount % 2 == 0;
            return inversionsCount % 2 != 0;
        }
        
        private static bool IsSnailStateSolvable(List<int> state, int n)
        {
            var stateInversions = 0;
            var goalInversions = 0;
            var goalState = GoalStates.GetGoalState(GoalStateType.Snail, n);
            var zeroRowGiven = state.IndexOf(0) / n;
            var zeroRowGoal = goalState.IndexOf(0) / n;

            for (var i = 0; i < state.Count - 1; i++)
            {
                if (state[i] == 0)
                    continue;
                for (var j = i + 1; j < state.Count; j++)
                {
                    if (state[j] == 0)
                        continue;
                    if (state[i] > state[j])
                        stateInversions++;
                }
            }

            for (var i = 0; i < state.Count - 1; i++)
            {
                if (state[i] == 0)
                    continue;
                for (var j = i + 1; j < state.Count; j++)
                {
                    if (goalState[j] == 0)
                        continue;
                    if (goalState[i] > goalState[j])
                        goalInversions++;
                }
            }

            if (n % 2 != 0)
                return stateInversions % 2 == goalInversions % 2;
            else
                return (stateInversions + zeroRowGiven) % 2 == (goalInversions + zeroRowGoal) % 2;
        }
    }
}