using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace AdventOfCode2020
{
    public class Day16
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(71, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(25059, Part1(Input));

        [Fact]
        public void Part_2_example()
        {
            Assert.Equal(default, Part2(Example));
        }

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var rules = new List<Rule>();
            var tickets = new List<string>();

            foreach (string line in input)
            {
                if (line.Contains('-'))
                {
                    rules.Add(Rule.Parse(line));
                }
                else if (line.Contains(','))
                {
                    tickets.Add(line);
                }
            }

            // Ignore your ticket
            tickets.RemoveAt(0);

            var invalidValues = new List<int>();

            foreach (var ticket in tickets)
            {
                var numbers = ticket.Split(',').Select(int.Parse);

                foreach (var number in numbers)
                {
                    if (!rules.Any(rule => rule.Test(number)))
                    {
                        invalidValues.Add(number);
                        break;
                    }
                }
            }

            return invalidValues.Sum();
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }

        private class Rule
        {
            private Rule(string name, Range range1, Range range2)
            {
                Name = name;
                Range1 = range1;
                Range2 = range2;
            }

            public string Name { get; }
            public Range Range1 { get; }
            public Range Range2 { get; }

            public bool Test(int number)
            {
                return Range1.Test(number) || Range2.Test(number);
            }

            public static Rule Parse(string line)
            {
                var name = line.Substring(0, line.IndexOf(':'));

                var parts = line.Substring(name.Length).Split(' ');

                var range1 = Range.Parse(parts[1]);
                var range2 = Range.Parse(parts[3]);

                return new Rule(name, range1, range2);
            }
        }

        private class Range
        {
            private Range(int low, int high)
            {
                Low = low;
                High = high;
            }

            public int Low { get; }
            public int High { get; }

            public bool Test(int number)
            {
                return number >= Low && number <= High;
            }

            public static Range Parse(ReadOnlySpan<char> range)
            {
                var seperator = range.IndexOf('-');

                int low = int.Parse(range.Slice(0, seperator));
                int high = int.Parse(range.Slice(seperator + 1));

                return new Range(low, high);
            }
        }

        private static ReadOnlyMemory<string> Example { get; } = @"class: 1-3 or 5-7
row: 6-11 or 33-44
seat: 13-40 or 45-50

your ticket:
7,1,14

nearby tickets:
7,3,47
40,4,50
55,2,20
38,6,12".SplitLines();


        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day16.input.txt").ToArray();
    }
}