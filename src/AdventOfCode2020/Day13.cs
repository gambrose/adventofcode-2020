using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day13
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(295, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(3966, Part1(Input));

        [Fact]
        public void Part_2_example()
        {
            Assert.Equal(1068781, Part2(Example));
        }

        [Fact]
        public void Part_2() => Assert.Equal(800177252346225, Part2(Input));

        private static int Part1(ReadOnlyMemory<string> input)
        {
            var timestamp = int.Parse(input.Span[0]);

            var buses = input.Span[1].Split(',').Where(bus => bus != "x").Select(int.Parse).ToArray();

            int busId = 0;
            int departs = timestamp;

            while (busId == 0)
            {
                busId = buses.FirstOrDefault(bus => departs % bus == 0);
                departs++;
            }

            var wait = departs - timestamp - 1;

            return busId * wait;
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            var buses = input.Span[1].Split(',')
                .Select((id, offset) => (id, offset))
                .Where(bus => bus.id != "x")
                .Select(bus => (id: int.Parse(bus.id!), bus.offset))
                .ToArray();

            long timestamp = 0;
            long step = 1;

            var search = buses.AsSpan();

            while (search.Length > 0)
            {
                var (id, offset) = search[0];

                while (true)
                {
                    timestamp += step;

                    if ((timestamp + offset) % id == 0)
                    {
                        break;
                    }
                }

                step *= id;
                search = search.Slice(1);
            }

            return timestamp;
        }

        private static ReadOnlyMemory<string> Example { get; } = @"939
7,13,x,x,59,x,31,19".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day13.input.txt").ToArray();
    }
}