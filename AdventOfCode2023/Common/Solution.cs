namespace AdventOfCode2023.Common;

public enum SolutionPart
{
    PartA,
    PartB
}

public abstract class Solution : ISolution
{
    public string? Input { get; } = null;
    public abstract int Day { get; }
    private string InputPath => $"inputs/day{Day}.txt";

    public string ReadInput() => Input ?? File.ReadAllText(InputPath);

    public List<string> ReadInputLines() =>
        Input?.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList()
        ?? File.ReadAllLines(InputPath).ToList();

    public abstract string Solve(SolutionPart part);

    public Solution()
    {
        
    }
    
    public Solution(string? input)
    {
        Input = input;
    }

    public void PrintAllSolutions()
    {
        Console.WriteLine($"Advent of Code Day {Day} solutions");
        foreach (SolutionPart part in Enum.GetValues(typeof(SolutionPart)))
        {
            Console.WriteLine($"==== {part} ====");
            Console.WriteLine(Solve(part));
        }
    }
}