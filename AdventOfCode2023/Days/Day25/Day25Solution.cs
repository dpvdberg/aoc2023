using System.Collections;
using System.Net;
using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day25;

public class Day25Solution : Solution
{
    public override int Day => 25;

    public Day25Solution()
    {
    }

    public Day25Solution(string input) : base(input)
    {
    }

    public Wiring Parse()
    {
        var components = new Dictionary<string, Component>();

        foreach (var line in ReadInputLines())
        {
            var split = line.Split(':');
            var baseComp = split[0];

            if (!components.ContainsKey(baseComp))
            {
                components[baseComp] = new Component(baseComp);
            }

            var connected = split[1].Trim().Split(' ').Select(n => n.Trim());
            foreach (var comp in connected)
            {
                if (!components.ContainsKey(comp))
                {
                    components[comp] = new Component(comp);
                }

                components[baseComp].Connected.Add(components[comp]);
                components[comp].Connected.Add(components[baseComp]);
            }
        }

        return new Wiring(components.Values.ToList());
    }

    private class ContractedComponent : Component
    {
        public Component Left { get; }

        public Component Right { get; }

        public override int GetSize()
        {
            return Left.GetSize() + Right.GetSize();
        }

        public ContractedComponent(Component a, Component b) : base($"({a}, {b})")
        {
            Left = a;
            Right = b;
            Connected.AddRange(a.Connected.Where(c => c != b));
            Connected.AddRange(b.Connected.Where(c => c != a));
        }

        public override Component Clone()
        {
            return new ContractedComponent(Left.Clone(), Right.Clone());
        }
    }

    public class Component
    {
        public string Name { get; }

        public List<Component> Connected { get; set; } = new();

        public virtual int GetSize()
        {
            return 1;
        }

        public Component(string name)
        {
            Name = name;
        }

        public virtual Component Clone()
        {
            return new Component(Name);
        }
        
        public override string ToString()
        {
            return Name;
        }
    }

    public class ComponentList
    {
        public List<Component> Components { get; }

        public ComponentList(List<Component> components)
        {
            Components = components;
        }

        public ComponentList DeepClone()
        {
            var c = Components.ToDictionary(
                c => c.Name,
                c => (c, c.Clone()));

            foreach (var (oldComp, newComp) in c.Values)
            {
                newComp.Connected = oldComp.Connected.Select(con => c[con.Name].Item2).ToList();
            }

            return new ComponentList(c.Values.Select(t => t.Item2).ToList());
        }

        public Component this[int next] => Components[next];

        public int Count => Components.Count;
    }

    public class Wiring
    {
        public ComponentList CompList { get; }

        public Wiring(List<Component> compList)
        {
            CompList = new ComponentList(compList);
        }

        private readonly Random _random = new(42);

        public ComponentList Contract(int k, ComponentList? list = null)
        {
            list ??= CompList.DeepClone();

            while (list.Components.Count > k)
            {
                var c1 = list[_random.Next(list.Count)];
                var c2 = c1.Connected[_random.Next(c1.Connected.Count)];
                var contracted = new ContractedComponent(c1, c2);

                foreach (var edge in c1.Connected.Concat(c2.Connected).Distinct())
                {
                    if (edge == c1 || edge == c2)
                        continue;
                    
                    var count = edge.Connected.Count(co => co == c1 || co == c2);
                    edge.Connected = edge.Connected.Where(co => co != c1 & co != c2)
                        .Concat(new List<Component> { contracted }.Repeat(count - 1)).ToList();
                }

                list.Components.Remove(c1);
                list.Components.Remove(c2);
                list.Components.Add(contracted);
            }

            return list;
        }

        public int MinCutCompoundProduct(int cutSize)
        {
            ComponentList minCut;
            int cut;
            do
            {
                minCut = MinCut(null, cutSize);
                cut = minCut.Components[0].Connected.Count;
            } while (cut != cutSize);

            return minCut.Components.Aggregate(1, (i, c) => i * c.GetSize());
        }

        /// <summary>
        /// Karger–Stein algorithm to produce min cut.
        /// </summary>
        /// <param name="components">Component list</param>
        /// <param name="knownCutSize">Optionally, a cut size to stop searching</param>
        /// <returns></returns>
        public ComponentList MinCut(ComponentList? components = null, int? knownCutSize = null)
        {
            components ??= CompList.DeepClone();

            if (components.Count <= 6)
            {
                return Contract(2, components);
            }
            else
            {
                var t = (int)Math.Ceiling(1 + components.Count / Math.Sqrt(2));
                var g1 = Contract(t, components.DeepClone());
                var g2 = Contract(t, components.DeepClone());

                var g1Cut = MinCut(g1, knownCutSize);
                // Get cut size, note that the edges are bidirectional
                // so number of edges on component 0 is equal to number of edges on component 1
                var e1 = g1Cut.Components[0].Connected.Count;

                if (e1 <= knownCutSize)
                {
                    return g1Cut;
                }
                
                var g2Cut = MinCut(g2, knownCutSize);
                var e2 = g2Cut.Components[0].Connected.Count;

                return e1 < e2 ? g1Cut : g2Cut;
            }
        }
    }

    public override string Solve(SolutionPart part)
    {
        var p = Parse();
        return p.MinCutCompoundProduct(3).ToString();
    }
}