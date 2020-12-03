using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode2020
{
    public class Day03
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

            var slopes = new[]
            {
                (1, 1),
                (3, 1),
                (5, 1),
                (7, 1),
                (1, 2)
            };

            var routes = slopes.Select(slope => Navigate(lines, slope));

            var trees = routes.Select(route => route.Count(c => c == 'X'));

            Assert.Equal(new[] { 2, 7, 3, 4, 2 }, trees);
            
            Assert.Equal(336, trees.Aggregate((acc, x) => acc * x));
        }

        [Fact]
        public async Task Part_2()
        {
            var lines = await Input();

            var slopes = new[]
            {
                (1, 1),
                (3, 1),
                (5, 1),
                (7, 1),
                (1, 2)
            };

            var routes = slopes.Select(slope => Navigate(lines, slope));

            var trees = routes.Select(route => route.LongCount(c => c == 'X'));
            
            Assert.Equal(3064612320, trees.Aggregate((acc, x) => acc * x));
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
            return File.ReadAllLinesAsync("Day03.input.txt");
        }
    }
}