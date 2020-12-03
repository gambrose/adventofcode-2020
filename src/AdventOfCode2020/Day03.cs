using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day03
    {
        [Fact]
        public void Part_1_example()
        {
            var route = Traverse(Example, (3, 1)).ToArray();

            Assert.Equal("..#.##.####", new string(route));

            Assert.Equal(7, route.Count(IsTree));
        }

        [Fact]
        public void Part_1()
        {
            var route = Traverse(Input, (3, 1));

            Assert.Equal(162, route.Count(IsTree));
        }

        [Fact]
        public void Part_2_example()
        {
            var slopes = new[]
            {
                (1, 1),
                (3, 1),
                (5, 1),
                (7, 1),
                (1, 2)
            };

            var routes = slopes.Select(slope => Traverse(Example, slope));

            var trees = routes.Select(route => route.Count(IsTree));

            Assert.Equal(new[] { 2, 7, 3, 4, 2 }, trees);

            Assert.Equal(336, trees.Aggregate((acc, x) => acc * x));
        }

        [Fact]
        public void Part_2()
        {
            var slopes = new[]
            {
                (1, 1),
                (3, 1),
                (5, 1),
                (7, 1),
                (1, 2)
            };

            var routes = slopes.Select(slope => Traverse(Input, slope));

            var trees = routes.Select(route => route.LongCount(IsTree));

            Assert.Equal(3064612320, trees.Aggregate((acc, x) => acc * x));
        }

        private static IEnumerable<char> Traverse(IReadOnlyList<string> map, (int right, int down) vector)
        {
            var x = 0;
            var y = 0;

            while (y < map.Count)
            {
                var line = map[y];

                yield return line[x % line.Length];

                x += vector.right;
                y += vector.down;
            }
        }

        private static bool IsTree(char c) => c == '#';

        private static IReadOnlyList<string> Example { get; } = new[]
        {
            "..##.......",
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

        private static IReadOnlyList<string> Input { get; } = File.ReadAllLines("Day03.input.txt");
    }
}