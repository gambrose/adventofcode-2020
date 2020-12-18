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
            static long Reduce(Span<long> values, Span<char> operations)
            {
                var left = values[0];
                for (int i = 0; i < operations.Length; i++)
                {
                    var right = values[i + 1];

                    left = operations[i] switch
                    {
                        '+' => left + right,
                        '*' => left * right
                    };
                }

                return left;
            }

            long sum = 0;

            foreach (var line in input)
            {
                ReadOnlySpan<char> expression = line.AsSpan();
                sum += Evaluate(ref expression, Reduce);
            }

            return sum;
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            static long Reduce(Span<long> values, Span<char> operations)
            {
                for (int i = 0; i < operations.Length; i++)
                {
                    if (operations[i] == '+')
                    {
                        var left = values[i];
                        var right = values[i + 1];

                        values[i] = 1;
                        values[i + 1] = left + right;
                    }
                }

                long total = 1;

                foreach (var value in values)
                {
                    total *= value;
                }

                return total;
            }

            long sum = 0;

            foreach (var line in input)
            {
                ReadOnlySpan<char> expression = line.AsSpan();
                sum += Evaluate(ref expression, Reduce);
            }

            return sum;
        }

        delegate long Reducer(Span<long> values, Span<char> operations);

        private static long Evaluate(ref ReadOnlySpan<char> expression, Reducer reducer)
        {
            var remaining = expression;

            remaining = remaining.TrimStart();

            Span<char> operationStack = stackalloc char[5];
            int operations = 0;
            Span<long> valueStack = stackalloc long[operationStack.Length + 1];
            int values = 0;

            while (remaining.Length > 0)
            {
                var peek = remaining[0];

                if (peek == '+' || peek == '*')
                {
                    operationStack[operations++] = peek;
                    remaining = remaining.Slice(1).TrimStart();
                }
                else if (peek == '(')
                {
                    remaining = remaining.Slice(1);
                    valueStack[values++] = Evaluate(ref remaining, reducer);
                    remaining = remaining.TrimStart();
                }
                else if (peek == ')')
                {
                    remaining = remaining.Slice(1).TrimStart();
                    break;
                }
                else if (char.IsDigit(peek))
                {
                    // Consume number
                    var i = 1;
                    for (; i < remaining.Length; i++)
                    {
                        if (!char.IsDigit(remaining[i]))
                        {
                            break;
                        }
                    }

                    var number = remaining.Slice(0, i);

                    valueStack[values++] = long.Parse(number);
                    remaining = remaining.Slice(number.Length).TrimStart();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            expression = remaining;

            return values switch
            {
                1 => valueStack[0],
                > 1 => reducer(valueStack.Slice(0, values), operationStack.Slice(0, operations)),
                _ => default
            };
        }

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day18.input.txt").ToArray();
    }
}