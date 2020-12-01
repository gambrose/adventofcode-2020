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
        public void Example()
        {
            var numbers = new[]
            {
                1721,
                979,
                366,
                299,
                675,
                1456
            };

            var (a, b) = SumOfTwoNumbers(numbers.AsSpan(), 2020);

            Assert.Equal(2020, a + b);

            var answer = a * b;

            Assert.Equal(514579, answer);
        }

        [Fact]
        public async Task Part1()
        {
            var lines = await File.ReadAllLinesAsync("input.txt");

            var numbers = lines.Select(int.Parse).ToArray();

            var (a, b) = SumOfTwoNumbers(numbers.AsSpan(), 2020);

            Assert.Equal(2020, a + b);
            var answer = a * b;

            Assert.Equal(545379, answer);
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
    }
}
