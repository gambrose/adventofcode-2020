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

        private static string Part1(string input, int iterations = 100)
        {
            var cups = Parse(input);

            var circle = new CupCircle(cups);

            Run(circle, iterations);

            return string.Join("", circle.From(1).Skip(1));
        }

        private static long Part2(string input)
        {
            var cups = Parse(input);

            var circle = new CupCircle(cups, 1000000);

            Run(circle, 10000000);

            var star1 = circle.Next(1);
            var star2 = circle.Next(star1);

            return (long)star1 * (long)star2;
        }

        private static void Run(CupCircle circle, int iterations)
        {
            var current = circle.Start;

            for (int i = 0; i < iterations; i++)
            {
                var pickup1 = circle.Next(current);
                var pickup2 = circle.Next(pickup1);
                var pickup3 = circle.Next(pickup2);

                var destination = current;

                do
                {
                    destination--;

                    if (destination == 0)
                    {
                        destination = circle.Max;
                    }
                }
                while (destination == pickup1 || destination == pickup2 || destination == pickup3);

                circle.Link(current, circle.Next(pickup3));
                circle.Link(pickup3, circle.Next(destination));
                circle.Link(destination, pickup1);

                current = circle.Next(current);
            }
        }

        class CupCircle
        {
            private readonly int[] _next;

            public CupCircle(int[] cups, int size = 9)
            {
                _next = new int[size];

                for (var i = 0; i < _next.Length; i++)
                {
                    _next[i] = i + 2;
                }

                Start = cups[0];

                for (var i = 1; i < cups.Length; i++)
                {
                    Link(cups[i - 1], cups[i]);
                }

                if (size > cups.Length)
                {
                    Link(size, Start);
                    Link(cups[^1], cups.Length + 1);
                }
                else
                {
                    Link(cups[^1], cups[0]);
                }
            }

            public int Start { get; }

            public int Max => _next.Length;

            public int Next(int cup) => _next[cup - 1];

            public void Link(int left, int right)
            {
                _next[left - 1] = right;
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

        static int[] Parse(string input)
        {
            var txt = input.AsSpan().Trim();

            var cups = new int[txt.Length];
            for (int i = 0; i < txt.Length; i++)
            {
                cups[i] = int.Parse(txt.Slice(i, 1));
            }

            return cups;
        }

        private static string Example { get; } = "389125467";

        private static string Input { get; } = File.ReadAllText("Day23.input.txt");
    }
}