using System;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day01
    {
        [Fact]
        public void Part_1_example()
        {
            var numbers = Example;

            var (a, b) = SumOfTwoNumbers(numbers.AsSpan(), 2020);

            Assert.Equal(2020, a + b);

            var answer = a * b;

            Assert.Equal(514579, answer);
        }

        [Fact]
        public void Part_1()
        {
            var numbers = Input;

            var (a, b) = SumOfTwoNumbers(numbers.AsSpan(), 2020);

            Assert.Equal(2020, a + b);
            var answer = a * b;

            Assert.Equal(545379, answer);
        }

        [Fact]
        public void Part_2_example()
        {
            var numbers = Example;

            var (a, b, c) = SumOfThreeNumbers(numbers.AsSpan(), 2020);

            Assert.Equal(2020, a + b + c);

            Assert.Equal(241861950, a * b * c);
        }

        [Fact]
        public void Part_2()
        {
            var numbers = Input;

            var (a, b, c) = SumOfThreeNumbers(numbers.AsSpan(), 2020);

            Assert.Equal(2020, a + b + c);

            Assert.Equal(257778836, a * b * c);
        }

        private static (int, int) SumOfTwoNumbers(ReadOnlySpan<int> numbers, int total)
        {
            Span<bool> seen = stackalloc bool[total];

            foreach (var number in numbers)
            {
                if (seen[total - number])
                {
                    return (total - number, number);
                }

                seen[number] = true;
            }

            return default;
        }

        private static (int, int, int) SumOfThreeNumbers(ReadOnlySpan<int> numbers, int total)
        {
            Span<int> sorted = numbers.ToArray();
            sorted.Sort();

            while (sorted.Length >= 3)
            {
                if (sorted[0] + sorted[1] + sorted[^1] > total)
                {
                    sorted = sorted.Slice(0, sorted.Length - 1);
                    continue;
                }

                var a = sorted[0];
                var c = sorted[^1];
                var b = total - a - c;

                if (sorted.BinarySearch(b) > 0)
                {
                    return (a, b, c);
                }

                sorted = sorted.Slice(1);
            }

            return default;
        }

        private static int[] Example { get; } = {
            1721,
            979,
            366,
            299,
            675,
            1456
        };

        private static int[] Input { get; } = File.ReadLines("Day01.input.txt").Select(int.Parse).ToArray();
    }
}
