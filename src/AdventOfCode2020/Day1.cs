using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode2020
{
    public class Day1
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
            var lines = await File.ReadAllLinesAsync("input.txt");

            return lines.Select(int.Parse).ToArray();
        }

        private static (int, int) SumOfTwoNumbers(Span<int> numbers, int total)
        {
            numbers.Sort();

            var min = numbers[0];
            var max = numbers[^1];

            while (true)
            {
                while (min + max > total)
                {
                    numbers = numbers.Slice(0, numbers.Length - 1);
                    max = numbers[^1];
                }

                if (min + max < total)
                {
                    numbers = numbers.Slice(1);
                    min = numbers[0];
                }
                else
                {
                    break;
                }
            }

            return (min, max);
        }

        private static (int, int, int) SumOfThreeNumbers(Span<int> numbers, int total)
        {
            numbers.Sort();

            while (numbers.Length > 3)
            {
                if (numbers[0] + numbers[1] + numbers[^1] > total)
                {
                    numbers = numbers.Slice(0, numbers.Length - 1);
                }
                else if (!numbers.Contains(total - numbers[^1] - numbers[0]))
                {
                    numbers = numbers.Slice(1);
                }
                else
                {
                    break;
                }
            }

            return (numbers[0], numbers[1], numbers[^1]);
        }
    }
}
