using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day14
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(165, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(11926135976176, Part1(Input));

        [Fact]
        public void Part_2_example()
        {
            Assert.Equal(208, Part2(Example2));
        }

        [Fact]
        public void Part_2() => Assert.Equal(4330547254348, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var memory = new Dictionary<long, long>();
            Mask mask = default;

            foreach (string line in input)
            {
                if (line.StartsWith("mask"))
                {
                    mask = ParseMask(line);
                }
                else
                {
                    var index = long.Parse(line.AsSpan()[4..line.IndexOf(']')]);
                    var value = long.Parse(line.AsSpan()[line.LastIndexOf(' ')..]);

                    var updatedValue = mask.Apply(value);

                    memory[index] = updatedValue;
                }
            }

            return memory.Values.Sum();
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            var memory = new Dictionary<long, long>();
            var masks = new List<Mask>();

            foreach (string line in input)
            {
                if (line.StartsWith("mask"))
                {
                    masks = ParseMask2(line);
                }
                else
                {
                    var index = long.Parse(line.AsSpan()[4..line.IndexOf(']')]);
                    var value = long.Parse(line.AsSpan()[line.LastIndexOf(' ')..]);

                    foreach (var mask in masks)
                    {
                        var updatedIndex = mask.Apply(index);

                        memory[updatedIndex] = value;
                    }
                }
            }

            return memory.Values.Sum();
        }

        private static Mask ParseMask(string line)
        {
            var mask = line.AsSpan()[(line.LastIndexOf(' ') + 1)..];

            return new Mask(mask);
        }

        private static List<Mask> ParseMask2(string line)
        {
            var mask = line.AsSpan()[(line.LastIndexOf(' ') + 1)..];

            var numberOfX = 0;
            foreach (var c in mask)
            {
                if (c == 'X')
                {
                    numberOfX++;
                }
            }

            var masks = new List<Memory<char>>(numberOfX)
            {
                new char[mask.Length]
            };

            for (var index = 0; index < mask.Length; index++)
            {
                var c = mask[index];

                switch (c)
                {
                    case '1':
                        for (var i = 0; i < masks.Count; i++)
                        {
                            masks[i].Span[index] = '1';
                        }

                        break;
                    case '0':
                        for (var i = 0; i < masks.Count; i++)
                        {
                            masks[i].Span[index] = 'X';
                        }

                        break;
                    case 'X':
                        {
                            int i = 0;
                            var count = masks.Count;
                            for (; i < count; i++)
                            {
                                var m1 = masks[i];
                                var m2 = new char[m1.Length];
                                m1.CopyTo(m2);
                                masks.Add(m2);
                            }

                            i = 0;
                            for (; i < count; i++)
                            {
                                masks[i].Span[index] = '0';
                            }

                            count = masks.Count;
                            for (; i < count; i++)
                            {
                                masks[i].Span[index] = '1';
                            }
                        }
                        break;
                }

            }

            return masks.Select(x=> new Mask(x.Span)).ToList();
        }

        private readonly struct Mask
        {
            private readonly long _ones;
            private readonly long _zeros;

            public Mask(ReadOnlySpan<char> mask)
            {
                _ones = 0;
                _zeros = 0;

                for (var index = 0; index < mask.Length; index++)
                {
                    var c = mask[index];

                    switch (c)
                    {
                        case '1':
                            _ones += 1;
                            break;
                        case '0':
                            _zeros += 1;
                            break;
                    }

                    _ones <<= 1;
                    _zeros <<= 1;
                }

                _ones >>= 1;
                _zeros >>= 1;
            }

            public long Apply(long value)
            {
                var updatedValue = value | _ones;
                updatedValue ^= (value & _zeros);

                return updatedValue;
            }
        }

        private static ReadOnlyMemory<string> Example { get; } = @"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0".SplitLines();

        private static ReadOnlyMemory<string> Example2 { get; } = @"mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day14.input.txt").ToArray();
    }
}
