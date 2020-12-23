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
        public void Part_2_example() => Assert.Equal(default, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));


        private static string Part1(ReadOnlyMemory<string> input, int iterations = 100)
        {
            var cups = Parse(input);

            var circle = new CupCircle(cups);

            var current = circle.Start;

            using var log = new StreamWriter(File.OpenWrite("log.txt"));

            for (int i = 0; i < iterations; i++)
            {
                log.WriteLine($"-- move {i + 1} --");

                log.Write("cups: ");

                foreach (var cup in circle.From(circle.Start))
                {
                    log.Write(cup == current ? '(' : ' ');

                    log.Write(cup);
                    log.Write(cup == current ? ')' : ' ');
                }
                log.WriteLine();

                var pickup = new int[3];

                pickup[0] = circle.Next(current);
                pickup[1] = circle.Next(pickup[0]);
                pickup[2] = circle.Next(pickup[1]);

                log.Write("pick up: ");
                log.Write(string.Join(", ", pickup));
                log.WriteLine();

                var destination = current;

                do
                {
                    destination--;

                    if (destination == 0)
                    {
                        destination = 9;
                    }
                }
                while (pickup.Contains(destination));

                log.Write($"destination: {destination}");
                log.WriteLine();

                log.WriteLine();

                circle.Link(current, circle.Next(pickup[2]));
                circle.Link(pickup[2], circle.Next(destination));
                circle.Link(destination, pickup[0]);

                current = circle.Next(current);
            }

            return string.Join("", circle.From(1).Skip(1));
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }

        class CupCircle
        {
            private readonly int[] _offsets = new int[9];

            public CupCircle(int[] cups)
            {
                Start = cups[0];

                var previous = Start;
                for (var i = 1; i < cups.Length; i++)
                {
                    var current = cups[i];

                    _offsets[previous - 1] = current - previous;

                    previous = current;
                }

                _offsets[previous - 1] = Start - previous;
            }

            public int Start { get; }

            public int Count => _offsets.Length;

            public int Next(int cup) => cup + _offsets[cup - 1];

            public void Link(int left, int right)
            {
                _offsets[left - 1] = right - left;
            }

            public IEnumerable<int> From(int start)
            {
                yield return start;

                var cup = start;

                for (int i = 1; i < Count; i++)
                {
                    yield return cup = Next(cup);
                }
            }

            public override string ToString()
            {
                return string.Join(", ", From(Start));
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