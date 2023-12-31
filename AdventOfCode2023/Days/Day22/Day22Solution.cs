﻿using System.Data;
using System.Net;
using AdventOfCode2023.Common;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Days.Day22;

public class Day22Solution : Solution
{
    public override int Day => 22;

    public Day22Solution()
    {
    }

    public Day22Solution(string input) : base(input)
    {
    }

    public override string Solve(SolutionPart part)
    {
        return part switch
        {
            SolutionPart.PartA => Parse().CountRemovableBlocks(false).ToString(),
            SolutionPart.PartB => Parse().CountRemovableBlocks(true).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    private BlockTower Parse()
        => new(ReadInputLines()
            .Select(l => l.Split('~'))
            .Select(s => s.Select(v => v.Split(',').Select(int.Parse).ToList()).ToList())
            .Select(i =>
                new Block(new Vec3(i[0][0], i[0][1], i[0][2]), new Vec3(i[1][0], i[1][1], i[1][2]))
            ).ToList());

    private class BlockTower
    {
        public List<Block> Blocks { get; }

        public BlockTower(List<Block> blocks, bool sort = true)
        {
            Blocks = blocks;

            if (sort)
            {
                // Sort blocks in ascending Z order
                Blocks.Sort((a, b) => a.GetMinZ.CompareTo(b.GetMinZ));
            }
        }

        public int Fall()
        {
            var heightMap = new DictionaryWithDefault<Vec2, int>(0);

            int fallen = 0;
            foreach (var block in Blocks)
            {
                var vectors = block.Vectors.Select(v => v.GetXY()).ToArray();
                var maxZ = vectors.Max(v => heightMap[v]);

                if (block.GetMinZ > maxZ + 1)
                {
                    block.MoveToZ(maxZ + 1);
                    
                    fallen++;
                }
                
                var newMax = block.GetMaxZ;
                foreach (var v in vectors)
                {
                    heightMap[v] = newMax;
                }
            }

            return fallen;
        }

        public int CountRemovableBlocks(bool chain)
        {
            // First, let all blocks fall into place
            Fall();

            // Then remove a block and check if we can move the blocks down now
            int safeBlocks = 0;
            foreach (var block in Blocks)
            {
                var subTower = new BlockTower(
                    Blocks.Where(b => b != block).Select(b => b.Clone()).ToList(), false);
                var fallen = subTower.Fall();
                if (chain)
                {
                    safeBlocks += fallen;
                }
                else
                {
                    safeBlocks += fallen <= 0 ? 1 : 0;
                }
            }

            return safeBlocks;
        }
    }

    private class Block
    {
        public Vec3 Start { get; private set; }
        public Vec3 End { get; private set; }

        public Block(Vec3 start, Vec3 end)
        {
            Start = start;
            End = end;
        }

        public void MoveToZ(int z)
        {
            var zDiff = z - Start.Z;
            Start = new Vec3(Start.X, Start.Y, Start.Z + zDiff);
            End = new Vec3(End.X, End.Y, End.Z + zDiff);
        }

        public Block Clone() => new(Start, End);

        public IEnumerable<Vec3> Vectors => Start | End;

        public int GetMinZ => (int) Math.Min(Start.Z, End.Z);
        public int GetMaxZ => (int) Math.Max(Start.Z, End.Z);

        public bool Contains(Vec3 vec) => Vectors.Contains(vec);
    }
}