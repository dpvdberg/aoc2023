using System.Collections.Specialized;
using System.Text;
using AdventOfCode2023.Common;
using Microsoft.VisualBasic.CompilerServices;

namespace AdventOfCode2023.Days.Day15;

using Boxes = Dictionary<int, OrderedDictionary>;

public class Day15Solution : Solution
{
    public override int Day => 15;


    public Day15Solution()
    {
    }

    public Day15Solution(string input) : base(input)
    {
    }

    private List<Step> Parse() => ReadInput().Split(',').Select(s => new Step(s)).ToList();


    private Boxes ApplySteps(List<Step> steps)
    {
        Boxes boxes = new();
        foreach (var step in steps)
        {
            var box = step.LabelHash();
            if (!boxes.ContainsKey(box))
            {
                boxes[box] = new OrderedDictionary();
            }

            if (step.IsAssignment)
            {
                // Put lens in box, order is preserved automagically (by using OrderedDictionary)
                boxes[box][step.Label] = step.FocalLength;
            }
            else
            {
                // Remove lens from box
                boxes[box].Remove(step.Label);
            }
        }

        return boxes;
    }

    private static int FocusingPower(Boxes boxes)
    {
        var s = 0;
        foreach (var (key, box) in boxes)
        {
            for (var lensIndex = 0; lensIndex < box.Count; lensIndex++)
            {
                var focalLength = (int) box[lensIndex]!;

                s += (key + 1) * (lensIndex + 1) * focalLength;
            }
        }

        return s;
    }

    private class Step
    {
        private readonly string _step;

        public bool IsAssignment { get; }
        public string Label { get; }
        public int FocalLength { get; } = -1;

        public Step(string step)
        {
            _step = step;

            if (step.Last() == '-')
            {
                IsAssignment = false;
                Label = step.Remove(step.Length - 1);
            }
            else if (step.Contains('='))
            {
                IsAssignment = true;
                var split = step.Split('=');
                Label = split[0];
                FocalLength = int.Parse(split[1]);
            }
            else
            {
                throw new ArgumentException($"Illegal step '{step}'");
            }
        }

        public int LabelHash() => Hash(Label);

        public int StepHash() => Hash(_step);

        private static int Hash(string s)
        {
            var h = 0;
            foreach (var c in Encoding.ASCII.GetBytes(s))
            {
                h += c;
                h *= 17;
                h %= 256;
            }

            return h;
        }
    }

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => Parse().Sum(s => s.StepHash()).ToString(),
            SolutionPart.PartB => FocusingPower(ApplySteps(Parse())).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }
}