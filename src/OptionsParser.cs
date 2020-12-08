using System;

namespace N_Puzzle
{
    internal static class OptionsParser
    {
        public static bool SFlag { get; private set; }
        public static bool RFlag { get; private set; }
        public static bool FFlag { get; private set; }

        public static void Parse(string[] opts)
        {
            SFlag = false;
            RFlag = false;

            for (var i = 0; i < opts.Length - 1; i++)
            {
                var opt = opts[i];
                switch (opt)
                {
                    case "-s":
                        if (SFlag)
                            throw new Exception("-s flag is already set.");
                        SFlag = true;
                        break;
                    case "-f":
                        if (FFlag)
                            throw new Exception("-f flag is already set.");
                        FFlag = true;
                        break;
                    case "-r":
                        if (RFlag)
                            throw new Exception("-r flag is already set.");
                        RFlag = true;
                        break;
                    default:
                        throw new Exception("unknown flag provided: " + opt);
                }
            }
        }
    }
}
