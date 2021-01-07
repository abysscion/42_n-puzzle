using System;
using System.Collections.Generic;

namespace N_Puzzle
{
    public static class GoalStates
    {
        private static List<int> _generatedZeroFirst;
        private static List<int> _generatedZeroLast;
        private static List<int> _generatedSnail;
        private static int _lastUsedN = -1;

        public static List<int> GetGoalState(GoalStateType goalStateType, int n)
        {
            if (n < 0)
                return null;

            if (n != _lastUsedN)
                _generatedSnail = _generatedZeroFirst = _generatedZeroLast = null;
            _lastUsedN = n;

            switch (goalStateType)
            {
                case GoalStateType.ZeroFirst:
                    if (_generatedZeroFirst == null)
                        _generatedZeroFirst = GetGoalState_ZeroFirst(n);
                    return _generatedZeroFirst;
                case GoalStateType.ZeroLast:
                    if (_generatedZeroLast == null)
                        _generatedZeroLast = GetGoalState_ZeroLast(n);
                    return _generatedZeroLast;
                case GoalStateType.Snail:
                    if (_generatedSnail == null)
                        _generatedSnail = GetGoalState_Snail(n);
                    return _generatedSnail;
                default:
                    throw new ArgumentOutOfRangeException(nameof(goalStateType), goalStateType, null);
            }
        }

        private static List<int> GetGoalState_Snail(int n)
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

        private static List<int> GetGoalState_ZeroFirst(int n)
        {
            var res = new List<int>();

            for (var i = 0; i < n*n; i++)
                res.Add(i);

            return res;
        }

        private static List<int> GetGoalState_ZeroLast(int n)
        {
            var res = new List<int>();

            for (var i = 1; i < n*n; i++)
                res.Add(i);

            res.Add(0);
            return res;
        }
    }
}