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

            Assert.Equal(2, NumberOfValidPasswords(lines));
        }

        [Fact]
        public async Task Part_1()
        {
            var lines = await Input();

            Assert.Equal(538, NumberOfValidPasswords(lines));
        }

        private static int NumberOfValidPasswords(string[] lines)
        {
            int valid = 0;
            foreach (var line in lines)
            {
                var parts = line.Split(' ');

                var range = parts[0].Split('-').Select(int.Parse).ToArray();
                var letter = parts[1][0];
                var password = parts[2];

                var count = password.Count(x => x == letter);

                if (count >= range[0] && count <= range[1])
                {
                    valid++;
                }
            }

            return valid;
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