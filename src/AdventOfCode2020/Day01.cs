using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode2020
{
    public class Day01
    {
        [Fact]
        public void Part_1_example()
        {
            var numbers = Example();

            var (a, b) = SumOfTwoNumbers(numbers.AsSpan(), 2020);

            Assert.Equal(2020, a + b);

            var answer = a * b;

            Assert.Equal(514579, answer);
        }

        [Fact]
        public async Task Part_1()
        {
            var numbers = await Input();

            var (a, b) = SumOfTwoNumbers(numbers.AsSpan(), 2020);

            Assert.Equal(2020, a + b);
            var answer = a * b;

            Assert.Equal(545379, answer);
        }

        [Fact]
        public void Part_2_example()
        {
            var numbers = Example();

            var (a, b, c) = SumOfThreeNumbers(numbers.AsSpan(), 2020);

            Assert.Equal(2020, a + b + c);

            Assert.Equal(241861950, a * b * c);
        }

        [Fact]
        public async Task Part_2()
        {
            var numbers = await Input();

            var (a, b, c) = SumOfThreeNumbers(numbers.AsSpan(), 2020);

            Assert.Equal(2020, a + b + c);

            Assert.Equal(257778836, a * b * c);
        }

        private static int[] Example() => new[]
        {
            1721,
            979,
            366,
            299,
            675,
            1456
        };

        private static async Task<int[]> Input()
        {
            var lines = await File.ReadAllLinesAsync("Day01.input.txt");

            return lines.Select(int.Parse).ToArray();
        }

        private static (int, int) SumOfTwoNumbers(Span<int> numbers, int total)
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

        private static (int, int, int) SumOfThreeNumbers(Span<int> numbers, int total)
        {
            numbers.Sort();

            while (numbers.Length >= 3)
            {
                if (numbers[0] + numbers[1] + numbers[^1] > total)
                {
                    numbers = numbers.Slice(0, numbers.Length - 1);
                    continue;
                }

                var a = numbers[0];
                var c = numbers[^1];
                var b = total - a - c;

                if (numbers.BinarySearch(b) > 0)
                {
                    return (a, b, c);
                }

                numbers = numbers.Slice(1);
            }

            return default;
        }
    }
}
