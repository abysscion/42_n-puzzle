using System.Collections.Generic;

namespace N_Puzzle
{
    public static class SolvedStates
    {
        public static List<int> GetSolvedStates_Snail(int n)
        {
            var matrix = new int[n, n];
            for (int step = 0, a = 0; step < n / 2; step++)
            {
                var size = n - step * 2 - 1;
                for (var i = 0; i < 4 * size; i++)
                {
                    var chunk = i / size;
                    var chunkIndex = i % size;
                    var chunkOffset = n - step - 1;

                    if (chunk == 0)
                        matrix[step, chunkIndex + step] = a + 1;
                    else if (chunk == 1)
                        matrix[chunkIndex + step, chunkOffset] = a + 1;
                    else if (chunk == 2)
                        matrix[chunkOffset, chunkOffset - chunkIndex] = a + 1;
                    else if (chunk == 3) matrix[chunkOffset - chunkIndex, step] = a + 1;

                    a++;
                }
            }

            if (n % 2 == 0)
                matrix[n / 2, n / 2 - 1] = 0;
            var res = new List<int>();
            for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
                res.Add(matrix[i, j]);
            return res;
        }

        public static List<int> GetSolvedStates_ZeroFirst(int n)
        {
            var res = new List<int>();

            for (var i = 0; i < n*n; i++)
                res.Add(i);

            return res;
        }
        
        public static List<int> GetSolvedStates_ZeroLast(int n)
        {
            var res = new List<int>();

            for (var i = 1; i < n*n; i++)
                res.Add(i);

            res.Add(0);
            return res;
        }
    }
}