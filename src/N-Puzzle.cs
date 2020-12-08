using System;
using System.Globalization;

namespace N_Puzzle
{
    internal static class NPuzzleSolver
    {
        private static void Main(string[] args)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);

                if (args.Length < 1)
                    PrintUsage();
                else
                    ProceedArguments(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error] " + (e.Message.Length != 0 ? e.Message : e.ToString()));
            }
        }

        private static void ProceedArguments(string[] args)
        {
            OptionsParser.Parse(args);
            var solver = new Solver(args[^1]);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("N-Puzzle [options] <path/to/file/puzzle.txt>");
            Console.WriteLine("options:\n" +
                              "\t-s\t\t- option 1\n" +
                              "\t-r\t\t- option 2");
        }
    }
}