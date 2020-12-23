#nullable  enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day23
    {
        [Fact]
        public void Part_1_example()
        {
            Assert.Equal("92658374", Part1(Example, 10));
            Assert.Equal("67384529", Part1(Example));
        }

        [Fact]
        public void Part_1() => Assert.Equal("24798635", Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(149245887792, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(12757828710, Part2(Input));

        private static string Part1(ReadOnlyMemory<string> input, int iterations = 100)
        {
            var cups = Parse(input);

            var circle = new CupCircle(cups);

            Run(circle, iterations);

            return string.Join("", circle.From(1).Skip(1));
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            var cups = Parse(input);

            var circle = new CupCircle(cups, 1000000);

            var foo = circle.From(circle.Start).Take(20).ToList();

            foo = circle.From(1000000 - 4).Take(20).ToList();

            Run(circle, 10000000);


            var star1 = circle.Next(1);
            var star2 = circle.Next(star1);

            return (long)star1 * (long)star2;
        }

        private static void Run(CupCircle circle, int iterations)
        {
            var current = circle.Start;

            var pickup = new int[3];

            for (int i = 0; i < iterations; i++)
            {
                circle.CopyTo(pickup, circle.Next(current));

                var destination = current;

                do
                {
                    destination--;

                    if (destination == 0)
                    {
                        destination = circle.Max;
                    }
                }
                while (pickup.Contains(destination));

                circle.Link(current, circle.Next(pickup[2]));
                circle.Link(pickup[2], circle.Next(destination));
                circle.Link(destination, pickup[0]);

                current = circle.Next(current);
            }
        }

        class CupCircle
        {
            private readonly int[] _offsets;

            public CupCircle(int[] cups, int size = 9)
            {
                _offsets = new int[size];
                Start = cups[0];

                var previous = Start;
                for (var i = 1; i < cups.Length; i++)
                {
                    var current = cups[i];

                    Link(previous, current);

                    previous = current;
                }

                if (size == 9)
                {
                    Link(previous, Start);
                }
                else
                {
                    Link(previous, cups.Length + 1);
                    Link(size, Start);
                }
            }

            public int Start { get; }

            public int Max => _offsets.Length;

            public int Next(int cup) => cup + _offsets[cup - 1] + 1;

            public void Link(int left, int right)
            {
                _offsets[left - 1] = right - left - 1;
            }

            public void CopyTo(Span<int> destination, int start)
            {
                destination[0] = start;

                for (int i = 1; i < destination.Length; i++)
                {
                    destination[i] = Next(destination[i - 1]);
                }
            }

            public IEnumerable<int> From(int start)
            {
                yield return start;

                var cup = start;

                for (int i = 1; i < Max; i++)
                {
                    yield return cup = Next(cup);
                }
            }
        }

        static int[] Parse(ReadOnlyMemory<string> input)
        {
            return input.Span[0].Select(c => int.Parse(new[] { c })).ToArray();
        }

        private static ReadOnlyMemory<string> Example { get; } = new[] { "389125467" };

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day23.input.txt").ToArray();
    }
}