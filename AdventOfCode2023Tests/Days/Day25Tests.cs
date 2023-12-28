using AdventOfCode2023.Common;
using AdventOfCode2023.Days.Day25;

namespace AdventOfCode2023Tests.Days;

public class Day25Tests
{
    private const string TestDataSimple =
        """
        a: b
        b: c
        c: d
        d: e
        e: f
        f: a
        c: x
        x: d
        """;
    
    private const string TestData =
        """
        jqt: rhn xhk nvd
        rsh: frs pzl lsr
        xhk: hfx
        cmg: qnr nvd lhk bvb
        rhn: xhk bvb hfx
        bvb: xhk hfx
        pzl: lsr hfx nvd
        qnr: nvd
        ntq: jqt hfx bvb xhk
        nvd: lhk
        lsr: lhk
        rzs: qnr cmg lsr rsh
        frs: qnr lhk lsr
        """;

    [TestCase(TestData, ExpectedResult = "54")]
    public string PartA(string input)
    {
        return new Day25Solution(input).Solve(SolutionPart.PartA);
    }

    [TestCase(TestDataSimple, ExpectedResult = "10")]
    public string PartASimple(string input)
    {
        var day = new Day25Solution(input);
        var wiring = day.Parse();
        return wiring.MinCutCompoundProduct(2).ToString();
    }
}