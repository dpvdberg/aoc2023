using System.Data;
using System.Text.RegularExpressions;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Days.Day19;

public class Day19SolutionNaive : Solution
{
    public Day19SolutionNaive()
    {
    }

    public Day19SolutionNaive(string input) : base(input)
    {
    }

    public override int Day => 19;

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => Parse().SumAcceptedRatings().ToString(),
            SolutionPart.PartB => "Definitely not going to work with this approach...",
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private SortSystem Parse()
    {
        var input = ReadInput();
        var sep = input.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);
        var rawWorkflows = sep[0];
        var rawParts = sep[1];

        var workflows = new Dictionary<string, Workflow>();
        var parts = new List<Part>();

        foreach (var wf in rawWorkflows.Split("\r\n"))
        {
            var split = wf.Split('{');
            var rules = split[1].Trim('}').Split(',');
            workflows[split[0]] = new Workflow(rules.Select(r => new Rule(r)).ToList());
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
        public SortSystem(Dictionary<string, Workflow> workflows, List<Part> parts)
        {
            Workflows = workflows;
            Parts = parts;
        }

        public Dictionary<string, Workflow> Workflows { get; }
        public List<Part> Parts { get; }
        private Dictionary<Part, bool> Sort()
        {
            var d = new Dictionary<Part, bool>();
            foreach (var part in Parts)
            {
                var result = RuleResult.Forward("in");
                while (result.Applicable && result.ForwardRule != "")
                {
                    var rule = Workflows[result.ForwardRule];
                    result = rule.Apply(part);
                }

                d[part] = result.Accepted;
            }

            return d;
        }

        public long SumAcceptedRatings()
            => Sort().Where(d => d.Value).Sum(p => p.Key.RatingSum);
    }

    private class Workflow
    {
        public Workflow(List<Rule> rules)
        {
            Rules = rules;
        }

        public List<Rule> Rules { get; }

        public RuleResult Apply(Part part)
        {
            foreach (var rule in Rules)
            {
                var result = rule.Apply(part);
                if (result.Applicable)
                {
                    return result;
                }
            }

            return RuleResult.NotApplicable();
        }
    }


    private class Rule
    {
        private RuleType _ruleType;
        private string _forwardRule = "";
        private Rule? _conditionRule = null;
        private Func<Part, bool>? _ruleFunc = null;

        private RuleResult ApplyCondition(Part part)
        {
            if (_ruleFunc!(part))
            {
                return _conditionRule!.Apply(part);
            }

            return RuleResult.NotApplicable();
        }

        public Rule(string rawRule)
        {
            if (rawRule == "R")
            {
                _ruleType = RuleType.Reject;
            }
            else if (rawRule == "A")
            {
                _ruleType = RuleType.Accept;
            }
            else if (Regex.IsMatch(rawRule, @"^\w+$"))
            {
                _ruleType = RuleType.Forward;
                _forwardRule = rawRule;
            }
            else
            {
                _ruleType = RuleType.Condition;
                var split = rawRule.Split('<', '>');
                var ratingType = split[0][0];
                var rightSplit = split[1].Split(':');
                var ratingValue = int.Parse(rightSplit[0]);
                _conditionRule = new Rule(rightSplit[1]);

                if (rawRule.Contains("<"))
                {
                    _ruleFunc = part => part.Rating[ratingType] < ratingValue;
                }
                else if (rawRule.Contains(">"))
                {
                    _ruleFunc = part => part.Rating[ratingType] > ratingValue;
                }
                else
                {
                    throw new ArgumentException($"Unknown rule: {rawRule}");
                }
            }
        }

        public RuleResult Apply(Part part)
        {
            return _ruleType switch
            {
                RuleType.Condition => ApplyCondition(part),
                RuleType.Forward => RuleResult.Forward(_forwardRule),
                RuleType.Accept => RuleResult.Accept(),
                RuleType.Reject => RuleResult.Reject(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private enum RuleType
        {
            Condition,
            Forward,
            Accept,
            Reject
        }
    }

    private class RuleResult
    {
        private RuleResult(bool applicable, bool accepted, string forwardRule)
        {
            Applicable = applicable;
            Accepted = accepted;
            ForwardRule = forwardRule;
        }

        public static RuleResult NotApplicable() => new(false, false, "");
        public static RuleResult Accept() => new(true, true, "");
        public static RuleResult Reject() => new(true, false, "");
        public static RuleResult Forward(string rule) => new(true, false, rule);

        public bool Applicable { get; }
        public bool Accepted { get; }
        public string ForwardRule { get; }
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