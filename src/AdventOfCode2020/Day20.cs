#nullable  enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode2020
{
    public class Day20
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(20899048083289, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(21599955909991, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(default, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var tiles = Parse(input).ToList();

            Dictionary<Boarder, int> edgeCount = new Dictionary<Boarder, int>();

            foreach (var tile in tiles)
            {
                foreach (var edge in tile.Edges())
                {
                    edgeCount.TryGetValue(edge, out var count);
                    edgeCount[edge] = count + 1;

                    var flipped = edge.Flip();
                    edgeCount.TryGetValue(flipped, out count);
                    edgeCount[flipped] = count + 1;
                }
            }

            var corners = new List<Tile>();
            foreach (var tile in tiles)
            {
                if (tile.Edges().Select(e => edgeCount[e]).Count(n => n == 1) == 2)
                {
                    corners.Add(tile);
                }
            }

            return corners.Aggregate(1L, (acc, title) => acc * title.ID);
        }

        private static int Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }

        private static IEnumerable<Tile> Parse(ReadOnlyMemory<string> input)
        {
            foreach (var tileInput in input.Split(string.Empty))
            {
                var id = int.Parse(tileInput.Span[0].AsSpan().TrimStart("Tile ").TrimEnd(":"));

                yield return new Tile(id, tileInput.Slice(1).Span);
            }
        }

        private readonly struct Tile
        {
            public Tile(int id, ReadOnlySpan<string> image)
            {
                ID = id;

                Span<bool> top = stackalloc bool[10];
                Span<bool> left = stackalloc bool[10];
                Span<bool> bottom = stackalloc bool[10];
                Span<bool> right = stackalloc bool[10];

                for (var index = 0; index < 10; index++)
                {
                    top[index] = Parse(image[0][index]);
                    left[index] = Parse(image[index][0]);
                    bottom[index] = Parse(image[9][index]);
                    right[index] = Parse(image[index][9]);
                }

                TopBoarder = new Boarder(top);
                LeftBoarder = new Boarder(left);
                BottomBoarder = new Boarder(bottom);
                RightBoarder = new Boarder(right);

                static bool Parse(char c) => c switch
                {
                    '#' => true,
                    '.' => false
                };
            }

            private Tile(int id, Boarder top, Boarder left, Boarder bottom, Boarder right)
            {
                ID = id;

                TopBoarder = top;
                LeftBoarder = left;
                BottomBoarder = bottom;
                RightBoarder = right;
            }

            public int ID { get; }

            public Boarder TopBoarder { get; }
            public Boarder LeftBoarder { get; }
            public Boarder BottomBoarder { get; }
            public Boarder RightBoarder { get; }

            public IEnumerable<Boarder> Edges()
            {
                yield return TopBoarder;
                yield return LeftBoarder;
                yield return BottomBoarder;
                yield return RightBoarder;
            }

            public Tile Rotate()
            {
                return new(ID, LeftBoarder, TopBoarder.Flip(), RightBoarder, BottomBoarder.Flip());
            }

            public override string ToString() => $"Tile {ID}";
        }

        private readonly struct Boarder
        {
            private readonly int _value;

            public Boarder(ReadOnlySpan<bool> values)
            {
                _value = 0;
                for (var index = 0; index < 10; index++)
                {
                    _value <<= 1;
                    _value += values[index] ? 1 : 0;
                }
            }

            public Boarder Flip()
            {
                int boarder = _value;
                Span<bool> values = stackalloc bool[10];

                for (var index = 9; index >= 0; index--)
                {
                    values[index] = (boarder & 1) == 1;
                    boarder >>= 1;
                }

                values.Reverse();

                return new Boarder(values);
            }

            public Span<char> Render(Span<char> writeBuffer)
            {
                int boarder = _value;
                writeBuffer = writeBuffer.Slice(0, 10);
                for (var index = 9; index >= 0; index--)
                {
                    writeBuffer[index] = (boarder & 1) switch
                    {
                        1 => '#',
                        0 => '.'
                    };
                    boarder >>= 1;
                }

                return writeBuffer;
            }

            public override string ToString()
            {
                Span<char> boarder = stackalloc char[10];
                Render(boarder);

                return new string(boarder);
            }
        }


        private static string RenderGrid(Tile[,] image)
        {
            var imageSize = image.GetLength(0);

            var render = new StringBuilder();
            Span<char> boarder = stackalloc char[10];

            for (int y = 0; y < imageSize; y++)
            {
                if (y != 0)
                {
                    render.AppendLine();
                }

                for (int x = 0; x < imageSize; x++)
                {
                    if (x != 0)
                    {
                        render.Append(" ");
                    }


                    render.Append(image[x, y].TopBoarder.Render(boarder));
                }

                render.AppendLine();

                for (int i = 1; i < 9; i++)
                {
                    for (int x = 0; x < imageSize; x++)
                    {
                        if (x != 0)
                        {
                            render.Append(" ");
                        }

                        render.Append(image[x, y].LeftBoarder.Render(boarder)[i]);
                        render.Append("        ");
                        render.Append(image[x, y].RightBoarder.Render(boarder)[i]);
                    }

                    render.AppendLine();
                }

                for (int x = 0; x < imageSize; x++)
                {
                    if (x != 0)
                    {
                        render.Append(" ");
                    }

                    render.Append(image[x, y].BottomBoarder.Render(boarder));
                }

                render.AppendLine();
            }

            return render.ToString();
        }

        private static ReadOnlyMemory<string> Example { get; } = @"Tile 2311:
..##.#..#.
##..#.....
#...##..#.
####.#...#
##.##.###.
##...#.###
.#.#.#..##
..#....#..
###...#.#.
..###..###

Tile 1951:
#.##...##.
#.####...#
.....#..##
#...######
.##.#....#
.###.#####
###.##.##.
.###....#.
..#.#..#.#
#...##.#..

Tile 1171:
####...##.
#..##.#..#
##.#..#.#.
.###.####.
..###.####
.##....##.
.#...####.
#.##.####.
####..#...
.....##...

Tile 1427:
###.##.#..
.#..#.##..
.#.##.#..#
#.#.#.##.#
....#...##
...##..##.
...#.#####
.#.####.#.
..#..###.#
..##.#..#.

Tile 1489:
##.#.#....
..##...#..
.##..##...
..#...#...
#####...#.
#..#.#.#.#
...#.#.#..
##.#...##.
..##.##.##
###.##.#..

Tile 2473:
#....####.
#..#.##...
#.##..#...
######.#.#
.#...#.#.#
.#########
.###.#..#.
########.#
##...##.#.
..###.#.#.

Tile 2971:
..#.#....#
#...###...
#.#.###...
##.##..#..
.#####..##
.#..####.#
#..#.#..#.
..####.###
..#.#.###.
...#.#.#.#

Tile 2729:
...#.#.#.#
####.#....
..#.#.....
....#..#.#
.##..##.#.
.#.####...
####.#.#..
##.####...
##..#.##..
#.##...##.

Tile 3079:
#.#.#####.
.#..######
..#.......
######....
####.#..#.
.#...#.##.
#.#####.##
..#.###...
..#.......
..#.###...".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day20.input.txt").ToArray();
    }
}