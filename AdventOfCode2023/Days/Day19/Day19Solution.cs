using System.Data;
using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Days.Day19;

public class Day19Solution : Solution
{
    public Day19Solution()
    {
    }

    public Day19Solution(string input) : base(input)
    {
    }

    public override int Day => 19;

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => Parse().GetAcceptedParts().Sum(p => p.RatingSum).ToString(),
            SolutionPart.PartB => Parse().GetAcceptingConditionChains().Sum(SortSystem.GetAcceptingCombinations).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private SortSystem Parse()
    {
        var input = ReadInput();
        var sep = input.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);
        var rawWorkflows = sep[0];
        var rawParts = sep[1];

        var workflows = new List<Workflow>();
        var parts = new List<Part>();

        foreach (var wf in rawWorkflows.Split("\r\n"))
        {
            var split = wf.Split('{');
            var rules = split[1].Trim('}').Split(',');
            workflows.Add(new Workflow(split[0], rules.Select(r => new Rule(r)).ToList()));
        }

        foreach (var wf in workflows)
        {
            wf.Connect(workflows);
        }

        foreach (var p in rawParts.Split("\r\n"))
        {
            var ratings = p.Trim('{', '}')
                .Split(',')
                .Select(r => r.Split('='))
                .ToDictionary(r => r[0][0], r => int.Parse(r[1]));
            parts.Add(new Part(ratings));
        }

        return new SortSystem(workflows, parts);
    }

    private class SortSystem
    {
        public SortSystem(List<Workflow> workflows, List<Part> parts)
        {
            Workflows = workflows;
            Parts = parts;
        }

        public List<Workflow> Workflows { get; }
        public List<Part> Parts { get; }

        /// <summary>
        /// Given a list of conditions, count the number of possible parts that could be accepted
        /// </summary>
        /// <param name="conditions">List of conditions</param>
        public static long GetAcceptingCombinations(List<ConditionData> conditions)
        {
            var d = new Dictionary<char, Vector2>();
            foreach (var l in "xmas".ToCharArray())
            {
                d[l] = new Vector2(1, 4000);
            }

            foreach (var condition in conditions)
            {
                var orig = d[condition.RatingName];
                d[condition.RatingName] =
                    condition.LessThan
                        ? orig with { Y = Math.Min(orig.Y, condition.RatingValue - 1) }
                        : orig with { X = Math.Max(orig.X, condition.RatingValue + 1) };
            }

            return d.Values.Aggregate(1L, (i, v) => i * (long)(v.Y - v.X + 1));
        }

        /// <summary>
        /// Get all accepted parts.
        /// </summary>
        /// <returns></returns>
        public List<Part> GetAcceptedParts()
        {
            // For part A, get the rules that are accepted for some chain of conditions.
            var chains = GetAcceptingConditionChains();
            return Parts.Where(p =>
                chains.Any(chain => chain.All(cond => cond.Accepts(p)))
            ).ToList();
        }

        /// <summary>
        /// Get all accepting chains of conditions.
        /// </summary>
        /// <returns></returns>
        public List<List<ConditionData>> GetAcceptingConditionChains()
        {
            var start = Workflows.FirstOrDefault(w => w.Label == "in")!;
            var chains = start.GetAcceptingConditionChains();

            chains.Sort((c1, c2) => c1.Count.CompareTo(c2.Count));

            return chains;
        }
    }

    private class Workflow
    {
        public Workflow(string label, List<Rule> rules)
        {
            Label = label;
            Rules = rules;
        }

        public string Label { get; }
        public List<Rule> Rules { get; }

        public void Connect(List<Workflow> workflows)
        {
            foreach (var rule in Rules)
            {
                rule.Connect(workflows);
            }
        }

        public List<List<ConditionData>> GetAcceptingConditionChains()
        {
            var negatedConditions = new List<ConditionData>();
            var l = new List<List<ConditionData>>();
            foreach (var rule in Rules)
            {
                // Get the accepting condition chains for this rule
                var acceptingChains = rule.GetAcceptingConditionChains();
                if (acceptingChains != null)
                {
                    // If we found any chains, then prepend it with the negated conditions in this workflow
                    l.AddRange(acceptingChains
                        .Select(c => negatedConditions.Concat(c).ToList()).ToList());
                }

                if (rule.Condition != null)
                {
                    // For upcoming rules in this workflow, we add the negated condition of this rule to the chain
                    negatedConditions.Add(rule.Condition.Negated());
                }
            }

            return l;
        }
    }


    private class Rule
    {
        public string ReferencedWorkflowString { get; } = "";
        public ConditionData? Condition { get; } = null;
        public RuleType RuleType { get; }

        private Workflow? ReferencedWorkflow { get; set; } = null;

        public Rule(string rawRule)
        {
            if (rawRule == "R")
            {
                RuleType = RuleType.Reject;
            }
            else if (rawRule == "A")
            {
                RuleType = RuleType.Accept;
            }
            else if (Regex.IsMatch(rawRule, @"^\w+$"))
            {
                RuleType = RuleType.Forward;
                ReferencedWorkflowString = rawRule;
            }
            else
            {
                RuleType = RuleType.Condition;
                // Do some ugly parsing...
                var split = rawRule.Split('<', '>');
                var ratingType = split[0][0];
                var rightSplit = split[1].Split(':');
                var ratingValue = int.Parse(rightSplit[0]);
                var conditionRule = new Rule(rightSplit[1]);

                Condition = new ConditionData(
                    conditionRule,
                    rawRule.Contains('<'),
                    ratingType,
                    ratingValue
                );
            }
        }

        public void Connect(List<Workflow> workflows)
        {
            if (ReferencedWorkflowString != "")
            {
                ReferencedWorkflow = workflows.FirstOrDefault(w => w.Label == ReferencedWorkflowString);
            }

            Condition?.Rule.Connect(workflows);
        }

        public List<List<ConditionData>>? GetAcceptingConditionChains()
        {
            switch (RuleType)
            {
                case RuleType.Condition:
                    // Get the accepting chains if we were to accept the condition.
                    var chains = Condition!.Rule.GetAcceptingConditionChains();
                    if (chains == null)
                    {
                        return null;
                    }
                    
                    // Return those chains and prepend the condition
                    chains.ForEach(c => c.Insert(0, Condition));
                    return chains;
                case RuleType.Forward:
                    return ReferencedWorkflow!.GetAcceptingConditionChains();
                case RuleType.Accept:
                    return new List<List<ConditionData>> { new() };
                case RuleType.Reject:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private class ConditionData
    {
        public ConditionData(Rule rule, bool lessThan, char ratingName, int ratingValue)
        {
            Rule = rule;
            LessThan = lessThan;
            RatingName = ratingName;
            RatingValue = ratingValue;
        }

        public Rule Rule { get; }
        public bool LessThan { get; }
        public char RatingName { get; }
        public int RatingValue { get; }

        public ConditionData Negated()
            => new(Rule, !LessThan, RatingName, LessThan ? RatingValue - 1 : RatingValue + 1);

        public bool Accepts(Part part) => LessThan
            ? part.Rating[RatingName] < RatingValue
            : part.Rating[RatingName] > RatingValue;
    }

    private enum RuleType
    {
        Condition,
        Forward,
        Accept,
        Reject
    }

    private class Part
    {
        public Part(Dictionary<char, int> rating)
        {
            Rating = rating;
        }

        public Dictionary<char, int> Rating { get; }

        public int RatingSum => Rating.Sum(e => e.Value);
    }
}