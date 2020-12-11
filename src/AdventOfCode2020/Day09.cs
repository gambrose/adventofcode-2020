using System;
using System.IO;
using System.Linq;
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
        public void Part_2_example() => Assert.Equal(62, Part2(Example, 5));

        [Fact]
        public void Part_2() => Assert.Equal(54142584, Part2(Input));

        private static long Part1(ReadOnlyMemory<long> input, int preamble = 25)
        {
            var numbers = input.Span;

            for (int i = preamble; i < numbers.Length - preamble; i++)
            {
                var previous = numbers.Slice(i - preamble, preamble);
                var total = numbers[i];
                var answer = SumOfTwoNumbers(previous, total);

                if (answer is (0, 0))
                {
                    return total;
                }
            }

            return 0;
        }

        private static long Part2(ReadOnlyMemory<long> input, int preamble = 25)
        {
            var numbers = input.Span;

            var total = Part1(input, preamble);

            var range = ContiguousSetThatSumTo(numbers, total).ToArray();

            if (range.Length > 2)
            {
                return range.Min() + range.Max();
            }

            return default;
        }

        private static (long, long) SumOfTwoNumbers(ReadOnlySpan<long> numbers, long total)
        {
            Span<long> sorted = stackalloc long[numbers.Length];
            numbers.CopyTo(sorted);
            sorted.Sort();

            while (sorted.Length >= 2)
            {
                var sum = sorted[0] + sorted[^1];
                if (sum > total)
                {
                    sorted = sorted.Slice(0, sorted.Length - 1);

                }
                else if (sum < total)
                {
                    sorted = sorted.Slice(1);
                }
                else
                {
                    return (sorted[0], sorted[^1]);
                }
            }

            return default;
        }

        private static ReadOnlySpan<long> ContiguousSetThatSumTo(ReadOnlySpan<long> numbers, long total)
        {
            int start = 0;
            var end = 2;

            long windowTotal = numbers[0] + numbers[1];

            while (end < numbers.Length)
            {
                if (windowTotal > total)
                {
                    windowTotal -= numbers[start++];
                }

                if (windowTotal < total)
                {
                    windowTotal += numbers[end++];
                }

                if (windowTotal == total)
                {
                    return numbers[start..end];
                }
            }

            return ReadOnlySpan<long>.Empty;
        }

        private static ReadOnlyMemory<long> Example { get; } = @"35
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
576".SplitLines().Select(long.Parse).ToArray();

        private static ReadOnlyMemory<long> Input { get; } = File.ReadLines("Day09.input.txt").Select(long.Parse).ToArray();
    }
}