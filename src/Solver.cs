using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace N_Puzzle
{
    public class Solver
    {
        private HashSet<int> _pieces;
        private int _puzzleSize;
        
        public Solver(string file)
        {
            if (!File.Exists(file))
                throw new Exception($"file {file} does not exist.");
            
            _pieces = new HashSet<int>();
            ParseFileContent(GetFileContentWithoutComments(file));
        }

        private void ParseFileContent(List<string> fileContent)
        {
            if (!Regex.IsMatch(fileContent[0], @"^\d$"))
                throw new Exception("first line of config should contain only 1 integer digit (size of puzzle).");
            
            _puzzleSize = int.Parse(fileContent[0]);
            
            if (fileContent.Count - 1 != _puzzleSize)
                throw new Exception($"invalid puzzle pieces lines ({fileContent.Count - 1}), while should be ({_puzzleSize}).");
            for (var i = 1; i < fileContent.Count; i++)
            {
                if (Regex.IsMatch(fileContent[i], @"[^\d\s]"))
                    throw new Exception("puzzle config lines (except comments) should contain only positive integer digits.");
                
                var nums = fileContent[i].Split(' ');
                
                if (nums.Length != _puzzleSize)
                    throw new Exception($"invalid puzzle pieces count in a row ({nums.Length}), while should be ({_puzzleSize}).");
                foreach (var num in nums)
                {
                    var tmp = int.Parse(num);
                    
                    if (_pieces.Contains(tmp))
                        throw new Exception($"puzzle should not contain same pieces: {tmp}.");
                    _pieces.Add(tmp);
                }
            }
        }
        
        private List<string> GetFileContentWithoutComments(string file)
        {
            using var fs = new FileStream(file, FileMode.Open);
            using var reader = new StreamReader(fs);
            var line = "";
            var fileContent = new List<string>();
            
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith('#'))
                    continue;
                line = Regex.Replace(line, @"#.+", "");
                line = line.Trim();
                if (line.Length != 0)
                    fileContent.Add(line);
            }

            return fileContent;
        }
    }
}