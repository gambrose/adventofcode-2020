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
                var remaining = ParseLiteral(expression, out var left).TrimStart();

                long value = 0;

                if (left[0] == '(' && left[^1] == ')')
                {
                    value = Evaluate(left[1..^1]);
                }
                else
                {
                    value = long.Parse(left);
                }

                while (!remaining.IsEmpty)
                {
                    var opp = remaining[0];

                    remaining = ParseLiteral(remaining.Slice(1), out var right).TrimStart();

                    long rightValue;
                    if (right[0] == '(' && right[^1] == ')')
                    {
                        rightValue = Evaluate(right[1..^1]);
                    }
                    else
                    {
                        rightValue = long.Parse(right);
                    }

                    value = opp switch
                    {
                        '+' => value + rightValue,
                        '*' => value * rightValue,
                        _ => throw new NotImplementedException()
                    };
                }

                return value;
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
                            _ => 0
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

        private static ReadOnlySpan<char> ParseLiteral(ReadOnlySpan<char> expression, out ReadOnlySpan<char> value)
        {
            expression = expression.TrimStart();

            if (expression[0] == '(')
            {
                var count = 1;
                for (var i = 1; i < expression.Length; i++)
                {
                    var c = expression[i];
                    count += c switch
                    {
                        '(' => 1,
                        ')' => -1,
                        _ => 0
                    };

                    if (count == 0)
                    {
                        value = expression.Slice(0, i + 1);
                        expression = expression.Slice(value.Length);
                        return expression;
                    }
                }
            }
            else
            {
                for (var i = 0; i < expression.Length; i++)
                {
                    var c = expression[i];

                    if (!char.IsDigit(c))
                    {
                        value = expression.Slice(0, i);
                        expression = expression.Slice(value.Length);
                        return expression;
                    }
                }
            }

            value = expression;
            return ReadOnlySpan<char>.Empty;
        }

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day18.input.txt").ToArray();
    }
}