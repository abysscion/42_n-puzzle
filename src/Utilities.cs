using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace N_Puzzle
{
    public class Utilities
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

        public static void CreateRandomPuzzles()
        {
            for (var size = 3; size < 6; size++)
            {
                var moves = new [] {1, -1, size, -size};
                for (var count = 0; count < 20; count++)
                {
                    using var writer = new StreamWriter(new FileStream($"C:\\Born2Code\\C#\\42_n-puzzle\\puzzles\\rnd_puzzle_{size}#{count}.txt", FileMode.Create));
                    
                    //create puzzle
                    var puzzle = new List<int>();
                    for (var i = 0; i < size * size; i++)
                        puzzle.Add(i);

                    //shuffle
                    for (var stepsCount = 0; stepsCount < 100; stepsCount++)
                    {
                        var zeroIndex = puzzle.IndexOf(0);
                        var newMoves = moves.ToList();
                        ShuffleList(newMoves);
                        for (var i = 0; i < newMoves.Count; i++)
                        {
                            var tmpMove = PuzzleNode.CreateMovedState(puzzle, newMoves[i], zeroIndex, size);
                            if (tmpMove == null)
                                continue;
                            puzzle = tmpMove;
                            break;
                        }
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
    }
}