#nullable  enable
using System;
using System.IO;
using System.Linq;
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

        [Theory]
        [InlineData("(2 * 8)", 16)]
        [InlineData("(2 * 8) + (1 * 8)", 24)]
        [InlineData("1 + 2 * 3 + 4 * 5 + 6", 231)]
        [InlineData("1 + (2 * 3) + (4 * (5 + 6))", 51)]
        [InlineData("2 * 3 + (4 * 5)", 46)]
        [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 1445)]
        [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 669060)]
        [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 23340)]
        public void Part_2_examples(string input, int answer) => Assert.Equal(answer, Part2(new[] { input }));

        [Fact]
        public void Part_2() => Assert.Equal(88500956630893, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            static long Evaluate(ReadOnlySpan<char> expression)
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

            long sum = 0;

            foreach (var line in input)
            {
                sum += Evaluate(line);
            }

            return sum;
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            static long Evaluate(ReadOnlySpan<char> expression)
            {
                expression = expression.Trim();

                if (expression[0] == '(')
                {
                    var count = 1;
                    var enclosed = expression.Slice(1);

                    while (count > 0)
                    {
                        count += enclosed[0] switch
                        {
                            '(' => 1,
                            ')' => -1,
                            _=> 0
                        };

                        enclosed = enclosed.Slice(1);
                    }

                    if (enclosed.IsEmpty)
                    {
                        expression = expression[1..^1];
                    }
                }

                Span<char> operands = stackalloc char[expression.Length];
                var parenthesesCount = 0;
                for (int i = 0; i < expression.Length; i++)
                {
                    parenthesesCount += expression[i] switch
                    {
                        '(' => 1,
                        ')' => -1,
                        _ => 0
                    };

                    if (parenthesesCount == 0)
                    {
                        operands[i] = expression[i] switch
                        {
                            '*' => '*',
                            '+' => '+',
                            _ => '_'
                        };
                    }
                    else
                    {
                        operands[i] = '_';
                    }
                }

                var multiplication = operands.LastIndexOf('*');

                if (multiplication >= 0)
                {
                    var left = expression.Slice(0, multiplication);
                    var right = expression.Slice(multiplication + 1);

                    return Evaluate(left) * Evaluate(right);
                }

                var addition = operands.LastIndexOf('+');

                if (addition >= 0)
                {
                    var left = expression.Slice(0, addition);
                    var right = expression.Slice(addition + 1);

                    return Evaluate(left) + Evaluate(right);
                }

                return long.Parse(expression);
            }

            long sum = 0;

            foreach (var line in input)
            {
                sum += Evaluate(line);
            }

            return sum;
        }
        
        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day18.input.txt").ToArray();
    }
}