using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day1;
using AdventOfCode2023.Days.Day2;
using AdventOfCode2023.Days.Day3;

var days = new List<Solution>()
{
    new Day1Solution(),
    new Day2Solution(),
    new Day3Solution(),
};

days.ForEach(s => s.PrintAllSolutions());