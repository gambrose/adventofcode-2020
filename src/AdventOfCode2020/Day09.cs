using System;
using System.IO;
using Xunit;

namespace AdventOfCode2020
{
    public class Day09
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(127, Part1(Example, 5));

        [Fact]
        public void Part_1() => Assert.Equal(375054920, Part1(Input));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input, int preamble = 25)
        {
            var numbers = Array.ConvertAll(input.ToArray(), long.Parse).AsSpan();

            for (int i = preamble; i < numbers.Length - preamble; i++)
            {
                var previous = numbers.Slice(i - preamble, preamble).ToArray();
                var total = numbers[i];
                var answer = SumOfTwoNumbers(previous, total);

                if (answer is (0, 0))
                {
                    return total;
                }
            }

            return 0;
        }

        private static int Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }

        private static (long, long) SumOfTwoNumbers(Span<long> numbers, long total)
        {
            numbers.Sort();

            while (numbers.Length >= 2)
            {
                var sum = numbers[0] + numbers[^1];
                if (sum > total)
                {
                    numbers = numbers.Slice(0, numbers.Length - 1);

                }
                else if (sum < total)
                {
                    numbers = numbers.Slice(1);
                }
                else
                {
                    return (numbers[0], numbers[^1]);
                }
            }

            return default;
        }

        private static ReadOnlyMemory<string> Example { get; } = @"35
20
15
25
47
40
62
55
65
95
102
117
150
182
127
219
299
277
309
576".Split(Environment.NewLine);

        private static ReadOnlyMemory<string> Input { get; } = File.ReadAllLines("Day09.input.txt");
    }
}