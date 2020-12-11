using System;
using System.Collections.Generic;

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
    }
}