using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode2020
{
    public class Day2
    {
        [Fact]
        public void Part_1_example()
        {
            var lines = Example();

            Assert.Equal(2, lines.Count(IsValidSledRentalPassword));
        }

        [Fact]
        public async Task Part_1()
        {
            var lines = await Input();

            Assert.Equal(538, lines.Count(IsValidSledRentalPassword));
        }

        [Fact]
        public void Part_2_example()
        {
            var lines = Example();

            Assert.Equal(1, lines.Count(IsTobogganPassword));
        }

        [Fact]
        public async Task Part_2()
        {
            var lines = await Input();

            Assert.Equal(489, lines.Count(IsTobogganPassword));
        }

        private static bool IsValidSledRentalPassword(string line)
        {
            var parts = line.Split(' ');

            var range = parts[0].Split('-').Select(int.Parse).ToArray();
            var letter = parts[1][0];
            var password = parts[2];

            var count = password.Count(x => x == letter);

            return (count >= range[0] && count <= range[1]);
        }

        private static bool IsTobogganPassword(string line)
        {
            var parts = line.Split(' ');

            var indexes = parts[0].Split('-').Select(int.Parse).ToArray();
            var letter = parts[1][0];
            var password = parts[2];

            var matches = indexes.Count(i => password[i - 1] == letter);

            return matches == 1;
        }

        private static string[] Example() => new[]
        {
            "1-3 a: abcde",
            "1-3 b: cdefg",
            "2-9 c: ccccccccc"
        };

        private static Task<string[]> Input()
        {
            return File.ReadAllLinesAsync("Day2-input.txt");
        }
    }
}