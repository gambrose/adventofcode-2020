using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode2020
{
    public class Day3
    {
        [Fact]
        public void Part_1_example()
        {
            var lines = Example();

            var route = Navigate(lines, (3, 1));

            Assert.Equal(".OXOXXOXXXX", route);

            Assert.Equal(7, route.Count(c => c == 'X'));
        }

        [Fact]
        public async Task Part_1()
        {
            var lines = await Input();

            var route = Navigate(lines, (3, 1));

            Assert.Equal(162, route.Count(c => c == 'X'));
        }

        [Fact]
        public void Part_2_example()
        {
            var lines = Example();

            throw new NotImplementedException();
        }

        [Fact]
        public async Task Part_2()
        {
            var lines = await Input();

            throw new NotImplementedException();
        }

        private static string Navigate(string[] map, (int right, int down) vector)
        {
            var route = new StringBuilder();

            var x = 0;
            var y = 0;

            while (y < map.Length)
            {
                var line = map[y];

                route.Append(line[x % line.Length]);

                x += vector.right;
                y += vector.down;
            }

            route.Replace('.', 'O', 1, route.Length - 1);
            route.Replace('#', 'X', 1, route.Length - 1);

            return route.ToString();
        }

        private static string[] Example() => new[]
        {
            @"..##.......",
            "#...#...#..",
            ".#....#..#.",
            "..#.#...#.#",
            ".#...##..#.",
            "..#.##.....",
            ".#.#.#....#",
            ".#........#",
            "#.##...#...",
            "#...##....#",
            ".#..#...#.#"
        };

        private static Task<string[]> Input()
        {
            return File.ReadAllLinesAsync("Day3-input.txt");
        }
    }
}