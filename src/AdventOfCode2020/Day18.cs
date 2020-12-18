#nullable  enable
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace AdventOfCode2020
{
    public class Day18
    {
        [Theory]
        [InlineData("3 + 8", 11)]
        [InlineData("2 * 8", 16)]
        [InlineData("1 + 2 * 3 + 4 * 5 + 6", 71)]
        [InlineData("1 + (2 * 3) + (4 * (5 + 6))", 51)]
        [InlineData("2 * 3 + (4 * 5)", 26)]
        [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 437)]
        [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 12240)]
        [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632)]
        public void Part_1_examples(string input, int answer) => Assert.Equal(answer, Part1(new[] { input }));

        [Fact]
        public void Part_1() => Assert.Equal(13976444272545, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(default, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            long sum = 0;

            foreach (var line in input)
            {
                sum += Evaluate(line);
            }

            return sum;
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }


        private static long Evaluate(ReadOnlySpan<char> expression)
        {
            expression = expression.Trim();

            long? right = null;

            if (expression[^1] == ')')
            {
                var count = 1;
                int start = expression.Length - 1;

                while (count > 0)
                {
                    start = expression.Slice(0, start).LastIndexOfAny('(', ')');

                    count += expression[start] switch
                    {
                        '(' => -1,
                        ')' => 1
                    };
                }

                right = Evaluate(expression[(start + 1)..^1]);

                expression = expression[..start];
            }

            var indexOfLastOperator = expression.LastIndexOfAny('+', '*');

            if (indexOfLastOperator < 0)
            {
                return right ?? long.Parse(expression);
            }

            var left = Evaluate(expression.Slice(0, indexOfLastOperator));
            right ??= long.Parse(expression.Slice(indexOfLastOperator + 1));

            return expression[indexOfLastOperator] switch
            {
                '+' => left + right.Value,
                '*' => left * right.Value,
                _ => throw new NotImplementedException()
            };
        }


        private static ReadOnlyMemory<string> Example { get; } = @".#.
..#
###".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day18.input.txt").ToArray();
    }
}