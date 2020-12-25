using System;

namespace N_Puzzle
{
    internal static class OptionsParser
    {
        private static bool _heuristicFlagSet;
        private static bool _goalFlagSet;
        
        public static SolvedStateType GoalFlag { get; private set; }
        public static HeuristicType HeuristicFlag { get; private set; }
        public static bool TableStepFlag { get; private set; }
        public static bool FFlag { get; private set; }

        public static void Parse(string[] opts)
        { 
            for (var i = 0; i < opts.Length - 1; i++)
            {
                var opt = opts[i];
                
                if (CheckForGoalFlag(opt))
                    continue;
                if (CheckForHeuristicFlag(opt))
                    continue;
                
                switch (opt)
                {
                    case "-ts":
                        if (TableStepFlag)
                            throw new Exception("-ts flag is already set.");
                        TableStepFlag = true;
                        break;
                    case "-f":
                        if (FFlag)
                            throw new Exception("-f flag is already set.");
                        FFlag = true;
                        break;
                    default:
                        throw new Exception("unknown flag provided: " + opt);
                }
            }
        }

        private static bool CheckForHeuristicFlag(string opt)
        {
            if (!opt.StartsWith("-heuristic:"))
                return false;
            
            if (_heuristicFlagSet)
                throw new Exception("-heuristic flag is already set.");
            var heuristic = opt.Substring(11);
            if (heuristic.Length < 1)
                throw new Exception($"unknown heuristic type provided: {heuristic}");
            if (heuristic.Equals("Manhattan"))
                HeuristicFlag = HeuristicType.Manhattan;
            else if (heuristic.Equals("LinearConflicts"))
                HeuristicFlag = HeuristicType.LinearConflicts;
            else if (heuristic.Equals("Hamming"))
                HeuristicFlag = HeuristicType.Hamming;
            else
                throw new Exception($"unknown heuristic type provided: {heuristic}");

            _heuristicFlagSet = true;
            return true;
        }

        private static bool CheckForGoalFlag(string opt)
        {
            if (!opt.StartsWith("-goal:"))
                return false;
            
            if (_goalFlagSet)
                throw new Exception("-goal flag is already set.");
            var goal = opt.Substring(6);
            if (goal.Length < 1)
                throw new Exception($"unknown goal type provided: {goal}");
            if (goal.Equals("ZeroFirst"))
                GoalFlag = SolvedStateType.ZeroFirst;
            else if (goal.Equals("ZeroLast"))
                GoalFlag = SolvedStateType.ZeroLast;
            else if (goal.Equals("Snail"))
                GoalFlag = SolvedStateType.Snail;
            else
                throw new Exception($"unknown goal type provided: {goal}");

            _goalFlagSet = true;
            return true;
        }
    }
}
