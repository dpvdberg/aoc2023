using System.Diagnostics;
using System.IO.Compression;
using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day20;

public class Day20Solution : Solution
{
    public override int Day => 20;

    public Day20Solution()
    {
    }

    public Day20Solution(string input) : base(input)
    {
    }

    public override string Solve(SolutionPart part)
    {
        var config = Parse();
        
        if (part == SolutionPart.PartA)
        {
            // Push the button 1000 times.
            var total = 1000;

            // Press it for the remaining times
            config = Parse();
            for (int i = 0; i < total; i++)
            {
                config.PushButtonOnce();
            }

            var low = config.Modules.Sum(m => (long)m.LowPulsesReceived);
            var high = config.Modules.Sum(m => (long)m.HighPulsesReceived);

            return (low * high).ToString();
        }
        else
        {
            var rx = config.Modules.FirstOrDefault(m => m.Name == "rx")!;

            return config.CountButtonPressesUntilLow(rx).ToString();
        }
    }

    private ModuleConfiguration Parse()
    {
        var modules = new List<Module>();
        var moduleNames = new Dictionary<string, Module>();
        var moduleOutputs = new Dictionary<Module, List<string>>();
        foreach (var line in ReadInputLines())
        {
            var split = line.Split(" -> ");
            var origin = split[0].Trim();
            var outputs = split[1]
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => s != "")
                .ToList();

            Module module;
            if (origin == "broadcaster")
            {
                module = new BroadcastModule(origin);
                
                // Also create button
                var button = new ButtonModule("button");
                button.AddOutput(module);
                modules.Add(button);
            }
            else if (origin.StartsWith('%'))
            {
                module = new FlipFlopModule(origin.Trim('%'));
            }
            else if (origin.StartsWith('&'))
            {
                module = new ConjunctionModule(origin.Trim('&'));
            }
            else
            {
                throw new InvalidOperationException($"Module '{origin}' has unknown prefix");
            }

            modules.Add(module);
            moduleOutputs[module] = outputs;
            moduleNames[module.Name] = module;
        }
        
        // Connect
        foreach (var item in moduleOutputs)
        {
            foreach (var outputName in item.Value)
            {
                if (!moduleNames.ContainsKey(outputName))
                {
                    // Create sink module
                    var sink = new SinkModule(outputName);
                    modules.Add(sink);
                    moduleNames[outputName] = sink;
                }
                
                item.Key.AddOutput(moduleNames[outputName]);
                moduleNames[outputName].AddInput(item.Key);
            }
        }

        return new ModuleConfiguration(modules);
    }

    private class ModuleConfiguration
    {
        public ButtonModule ButtonModule { get; }
        public ModuleConfiguration(List<Module> modules)
        {
            Modules = modules;

            var button = Modules.FirstOrDefault(m => m is ButtonModule);
            ButtonModule = (ButtonModule) button! ?? throw new ArgumentException("No button found in modules");
        }

        private bool HasCycled() => Modules.All(m => m.IsReset);
        
        public void PushButtonOnce(Action? afterPulse = null)
        {
            var queue = new Queue<Module>();
            queue.Enqueue(ButtonModule);
            
            do
            {
                var module = queue.Dequeue();
                var outputs = module.Pulse();
                foreach (var output in outputs)
                {
                    if (output.ShouldPulse)
                    {
                        queue.Enqueue(output);
                    }
                }

                afterPulse?.Invoke();
            } while (queue.Count > 0 && !HasCycled());
        }

        public List<Module> Modules { get; }

        public long CountButtonPressesUntilLow(Module rx)
        {
            // It appears the input of rx is a single conjugation module, so that makes life easier...
            var conjugationParent = (ConjunctionModule) rx.Input.FirstOrDefault()!;
            var conjugationInputCycleSize = conjugationParent.Input
                .ToDictionary(i => i, i => 0);
            
            int index = 0;
            while (conjugationInputCycleSize.Values.Any(c => c == 0))
            {
                var actionIndex = index;
                PushButtonOnce(afterPulse: () =>
                {
                    foreach (var input in conjugationInputCycleSize.Keys)
                    {
                        if (conjugationParent.Memory[input] && conjugationInputCycleSize[input] == 0)
                        {
                            conjugationInputCycleSize[input] = actionIndex + 1;
                        }
                    }
                });
                
                index++;
            }

            return conjugationInputCycleSize
                .Values
                .Select(v => (long) v)
                .LeastCommonMultiple();
        }
    }

    private class SinkModule : Module
    {
        public override bool IsReset => true;

        public SinkModule(string name) : base(name)
        {
        }
    }

    private class ButtonModule : Module
    {
        public override bool ReceivePulse(Module inputModule, bool high)
        {
            throw new InvalidOperationException("Button module cannot receive a pulse");
        }

        public override List<Module> Pulse()
        {
            base.Pulse();
            if (Output is not [BroadcastModule])
            {
                throw new InvalidOperationException("Button should be directly connected to broadcast module");
            }
            
            return Output.Where(o => o.ReceivePulse(this, false)).ToList();
        }

        public ButtonModule(string name) : base(name)
        {
        }
    }

    private class BroadcastModule : Module
    {
        private bool _lastPulseHigh;

        public override bool ReceivePulse(Module inputModule, bool high)
        {
            base.ReceivePulse(inputModule, high);
            ShouldPulse = true;
            _lastPulseHigh = high;

            return true;
        }

        public override List<Module> Pulse()
        {
            base.Pulse();
            
            return Output.Where(o => o.ReceivePulse(this, _lastPulseHigh)).ToList();
        }

        public BroadcastModule(string name) : base(name)
        {
        }
    }

    private class FlipFlopModule : Module
    {
        public bool IsOn { get; private set; }

        public override bool IsReset => !ShouldPulse && !IsOn;

        public override bool ReceivePulse(Module inputModule, bool high)
        {
            base.ReceivePulse(inputModule, high);
            if (!high)
            {
                IsOn = !IsOn;
                ShouldPulse = true;
                return true;
            }

            return false;
        }

        public override List<Module> Pulse()
        {
            base.Pulse();
            return Output.Where(o => o.ReceivePulse(this, IsOn)).ToList();
        }

        public FlipFlopModule(string name) : base(name)
        {
        }
    }

    private class ConjunctionModule : Module
    {
        public Dictionary<Module, bool> Memory { get; } = new();

        public override bool IsReset => !ShouldPulse && Memory.Values.All(b => !b);

        public override bool ReceivePulse(Module inputModule, bool high)
        {
            base.ReceivePulse(inputModule, high);
            ShouldPulse = true;
            Memory[inputModule] = high;

            return true;
        }

        public override List<Module> Pulse()
        {
            base.Pulse();

            var high = !Memory.Values.All(b => b);
            return Output.Where(o => o.ReceivePulse(this, high)).ToList();
        }

        public override void AddInput(Module module)
        {
            base.AddInput(module);
            Memory[module] = false;
        }

        public ConjunctionModule(string name) : base(name)
        {
        }
    }

    private abstract class Module
    {
        public virtual bool IsReset => !ShouldPulse;
        public string Name { get; }
        public List<Module> Input { get; } = new();
        protected List<Module> Output { get; } = new();
        public int LowPulsesReceived { get; set; } = 0;
        public int HighPulsesReceived { get; set; } = 0;
        public bool ShouldPulse { get; set; }

        public Module(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Receive and process a pulse.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="high"></param>
        /// <returns>true if this node should be queued for pulsing</returns>
        public virtual bool ReceivePulse(Module parent, bool high)
        {
            // PrintReceived(parent, high);
            if (high)
            {
                HighPulsesReceived++;
            }
            else
            {
                LowPulsesReceived++;
            }

            return false;
        }

        /// <summary>
        /// Pulse this module.
        /// </summary>
        /// <returns>List of outputs that are should be queued for pulsing</returns>
        public virtual List<Module> Pulse()
        {
            ShouldPulse = false;
            return Output;
        }

        public virtual void AddInput(Module module)
        {
            Input.Add(module);
        }

        public virtual void AddOutput(Module module)
        {
            Output.Add(module);
        }

        private void PrintReceived(Module parent, bool high)
        {
            var highLow = high ? "high" : "low";
            Console.WriteLine($"{parent.Name} -{highLow}-> {Name}");
            Debug.WriteLine($"{parent.Name} -{highLow}-> {Name}");
        }
    }
}