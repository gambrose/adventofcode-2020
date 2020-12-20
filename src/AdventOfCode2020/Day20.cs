#nullable  enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public void Part_2_example() => Assert.Equal(273, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(2495, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var tiles = Parse(input).ToDictionary(x => x.ID);

            Dictionary<Boarder, int> edgeCount = new();

            foreach (var (_, tile) in tiles)
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

            var corners = new List<int>();
            foreach (var (id, tile) in tiles)
            {
                if (tile.Edges().Select(e => edgeCount[e]).Count(n => n == 1) == 2)
                {
                    corners.Add(id);
                }
            }

            return corners.Aggregate(1L, (acc, id) => acc * id);
        }

        private static int Part2(ReadOnlyMemory<string> input)
        {
            Dictionary<int, Tile> tiles = Parse(input).ToDictionary(x => x.ID);

            Dictionary<Boarder, List<int>> edges = new();

            foreach (var (id, tile) in tiles)
            {
                foreach (var edge in tile.Edges())
                {
                    if (!edges.TryGetValue(edge, out var ids))
                    {
                        edges[edge] = ids = new List<int>(2);
                    }

                    ids.Add(id);

                    var flipped = edge.Flip();
                    if (!edges.TryGetValue(flipped, out ids))
                    {
                        edges[flipped] = ids = new List<int>(2);
                    }

                    ids.Add(id);
                }
            }

            var gridSize = (int)Math.Sqrt(tiles.Count);

            var corners = new List<int>();
            var edgeTiles = 0;
            var middleTiles = 0;
            foreach (var (id, tile) in tiles)
            {
                var numberOfMatchingEdges = tile.Edges().Select(e => edges[e]).Count(ids => ids.Count == 2);
                switch (numberOfMatchingEdges)
                {
                    case 2:
                        corners.Add(id);
                        break;
                    case 3:
                        edgeTiles++;
                        break;
                    case 4:
                        middleTiles++;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            Debug.Assert(corners.Count == 4);
            Debug.Assert(edgeTiles == 4 * (gridSize - 2));

            var grid = new Tile[gridSize, gridSize];

            // Starting in one corner
            {
                var corner = tiles[corners[0]];

                // To make same as example
                corner = corner.FlipHorizontal();

                // Flip it so it will match
                while (edges[corner.LeftBoarder].Count != 1 || edges[corner.TopBoarder].Count != 1)
                {
                    corner = corner.Rotate();
                }

                grid[0, 0] = corner;
            }

            // Fill in the top edge
            for (int x = 1; x < gridSize; x++)
            {
                var left = grid[0, x - 1];

                var right = tiles[edges[left.RightBoarder].Single(id => id != left.ID)];

                while (left.RightBoarder.MatchesInADirection(right.LeftBoarder) is false)
                {
                    right = right.Rotate();
                }

                if (left.RightBoarder.Equals(right.LeftBoarder) is false)
                {
                    right = right.FlipHorizontal();
                    Debug.Assert(left.RightBoarder.Equals(right.LeftBoarder));
                }

                if (edges[right.TopBoarder].Count != 1)
                {
                    right = right.Rotate().Rotate().FlipVertical();
                    Debug.Assert(left.RightBoarder.Equals(right.LeftBoarder));
                    Debug.Assert(edges[right.TopBoarder].Count == 1);
                }

                grid[0, x] = right;
            }

            // Fill the left edge
            for (int y = 1; y < gridSize; y++)
            {
                var upper = grid[y - 1, 0];
                Debug.Assert(edges[upper.LeftBoarder].Count == 1);

                var lower = tiles[edges[upper.BottomBoarder].Single(id => id != upper.ID)];

                while (upper.BottomBoarder.MatchesInADirection(lower.TopBoarder) is false)
                {
                    lower = lower.Rotate();
                }

                if (upper.BottomBoarder.Equals(lower.TopBoarder) is false)
                {
                    lower = lower.FlipVertical();
                    Debug.Assert(upper.BottomBoarder.Equals(lower.TopBoarder));
                }

                Debug.Assert(edges[lower.LeftBoarder].Count == 1);

                grid[y, 0] = lower;
            }

            // Fill the rest
            for (int x = 1; x < gridSize; x++)
            {
                for (int y = 1; y < gridSize; y++)
                {
                    var upper = grid[y - 1, x];
                    var left = grid[y, x - 1];

                    var right = edges[left.RightBoarder].Single(id => id != left.ID);
                    var lower = edges[upper.BottomBoarder].Single(id => id != upper.ID);

                    Debug.Assert(right == lower);

                    var tile = tiles[right];

                    while (left.RightBoarder.MatchesInADirection(tile.LeftBoarder) is false)
                    {
                        tile = tile.Rotate();
                    }

                    if (left.RightBoarder.Equals(tile.LeftBoarder) is false)
                    {
                        tile = tile.FlipHorizontal();
                        Debug.Assert(left.RightBoarder.Equals(tile.LeftBoarder));
                    }

                    Debug.Assert(upper.BottomBoarder.Equals(tile.TopBoarder));

                    grid[y, x] = tile;
                }
            }

            var imageSize = gridSize * 8;
            var image = new char[imageSize, imageSize];

            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    var tileImage = grid[y, x].Image;

                    var xOffset = x * 8;
                    var yOffset = y * 8;

                    for (var i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            image[yOffset + i, xOffset + j] = tileImage[i, j];
                        }
                    }
                }
            }

            var bar = RenderGrid(grid);
            var foo = image.Render();

            var monsters = MarkSeaMonsters(image);

            //if (monsters == 0)
            //{
            //    var rotated = image.Rotate();
            //    monsters = MarkSeaMonsters(rotated);

            //    if (monsters == 0)
            //    {
            //        rotated = image.Rotate();
            //        monsters = MarkSeaMonsters(rotated);
            //    }
            //}

            if (monsters == 0)
            {
                var flipped = image.FlipHorizontal();
                monsters = MarkSeaMonsters(flipped);

                if (monsters > 0)
                {
                    image = flipped;
                }
            }

            var foo2 = image.Render();

            var roughness = 0;
            for (int i = 0; i < imageSize; i++)
            {
                for (int j = 0; j < imageSize; j++)
                {
                    if (image[i, j] == '#')
                    {
                        roughness++;
                    }
                }
            }


            return roughness;
        }

        private static int MarkSeaMonsters(char[,] image)
        {
            var imageSize = image.GetLength(0);
            
            var monsters = 0;
            for (int i = 0; i < imageSize - 18; i++)
            {
                for (int j = 0; j < imageSize - 3; j++)
                {
                    if (TryMarkSeaMonsterAt(i, j, image))
                    {
                        monsters++;
                    }
                }
            }

            return monsters;
        }

        private static bool TryMarkSeaMonsterAt(int x, int y, char[,] image)
        {
            var monster =
                image[x, y + 1] == '#' &&
                image[x + 5, y + 1] == '#' &&
                image[x + 6, y + 1] == '#' &&
                image[x + 11, y + 1] == '#' &&
                image[x + 12, y + 1] == '#' &&
                image[x + 17, y + 1] == '#' &&
                image[x + 18, y + 1] == '#' &&
                image[x + 19, y + 1] == '#' &&
                image[x + 18, y] == '#' &&
                image[x + 1, y + 2] == '#' &&
                image[x + 4, y + 2] == '#' &&
                image[x + 7, y + 2] == '#' &&
                image[x + 10, y + 2] == '#' &&
                image[x + 13, y + 2] == '#' &&
                image[x + 16, y + 2] == '#';


            if (monster)
            {
                image[x + 18, y] = 'O';

                image[x, y + 1] = 'O';
                image[x + 5, y + 1] = 'O';
                image[x + 6, y + 1] = 'O';
                image[x + 11, y + 1] = 'O';
                image[x + 12, y + 1] = 'O';
                image[x + 17, y + 1] = 'O';
                image[x + 18, y + 1] = 'O';
                image[x + 19, y + 1] = 'O';

                image[x + 1, y + 2] = 'O';
                image[x + 4, y + 2] = 'O';
                image[x + 7, y + 2] = 'O';
                image[x + 10, y + 2] = 'O';
                image[x + 13, y + 2] = 'O';
                image[x + 16, y + 2] = 'O';
            }

            return monster;
        }

        private static IEnumerable<Tile> Parse(ReadOnlyMemory<string> input)
        {
            foreach (var tileInput in input.Split(string.Empty))
            {
                var id = int.Parse(tileInput.Span[0].AsSpan().TrimStart("Tile ").TrimEnd(":"));

                yield return new Tile(id, tileInput.Slice(1).Span);
            }
        }

        [Fact]
        public void Boarder_tests()
        {
            var boarder = new Boarder(new[] { true, false, true, false, true, false, true, false, true, false });

            Assert.Equal("#.#.#.#.#.", boarder.ToString());
            Assert.Equal(".#.#.#.#.#", boarder.Flip().ToString());

            Assert.True(boarder.MatchesInADirection(boarder));
            Assert.True(boarder.MatchesInADirection(boarder.Flip()));

            Assert.True(boarder.Equals(boarder));
            Assert.True(boarder.Equals(boarder.Flip().Flip()));
        }

        [Fact]
        public void Tile_can_be_rotated_and_flipped()
        {
            var tile = Parse(Example).First();

            void IsSame(Tile transformed)
            {
                var foo = RenderGrid(new[,] { { tile, transformed } });

                Assert.Equal(tile.TopBoarder, transformed.TopBoarder);
                Assert.Equal(tile.LeftBoarder, transformed.LeftBoarder);
                Assert.Equal(tile.BottomBoarder, transformed.BottomBoarder);
                Assert.Equal(tile.RightBoarder, transformed.RightBoarder);
            }

            IsSame(tile.Rotate().Rotate().Rotate().Rotate());
            IsSame(tile.FlipHorizontal().FlipHorizontal());
            IsSame(tile.FlipVertical().FlipVertical());


            var foo = RenderGrid(new[,]
            {
                { tile, tile.Rotate() },
                { tile.Rotate(), tile.Rotate().Rotate() },
                { tile.Rotate().Rotate(), tile.Rotate().Rotate().FlipHorizontal() },
                { tile.Rotate().Rotate().FlipHorizontal(), tile.Rotate().Rotate().FlipHorizontal().FlipVertical() }
            });

            IsSame(tile.Rotate().Rotate().FlipHorizontal().FlipVertical());
        }

        [Fact]
        public void Tile_image_tests()
        {
            var tile = Parse(Example).First();

            Assert.Equal("#..#....\n...##..#\n###.#...\n#.##.###\n#...#.##\n#.#.#..#\n.#....#.\n##...#.#\n", tile.RenderImage());

            Assert.Equal("##...#.#\n.#....#.\n#.#.#..#\n#...#.##\n#.##.###\n###.#...\n...##..#\n#..#....\n", tile.FlipHorizontal().RenderImage());

            Assert.Equal("....#..#\n#..##...\n...#.###\n###.##.#\n##.#...#\n#..#.#.#\n.#....#.\n#.#...##\n", tile.FlipVertical().RenderImage());

            Assert.Equal("#.####.#\n##...#..\n..#.##..\n....#.##\n..##.##.\n#...#...\n.#.##...\n#.###.#.\n", tile.Rotate().RenderImage());
        }

        private readonly struct Tile
        {
            public Tile(int id, ReadOnlySpan<string> input)
            {
                ID = id;

                Span<bool> top = stackalloc bool[10];
                Span<bool> right = stackalloc bool[10];
                Span<bool> bottom = stackalloc bool[10];
                Span<bool> left = stackalloc bool[10];

                for (var index = 0; index < 10; index++)
                {
                    top[index] = Parse(input[0][index]);
                    right[index] = Parse(input[index][9]);
                    bottom[index] = Parse(input[9][index]);
                    left[index] = Parse(input[index][0]);
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

                Image = new char[8, 8];

                var imageLines = input.Slice(1, 8);

                for (int y = 0; y < imageLines.Length; y++)
                {
                    var imageLine = imageLines[y].AsSpan().Slice(1, 8);

                    for (int x = 0; x < imageLine.Length; x++)
                    {
                        Image[y, x] = imageLine[x];
                    }
                }
            }

            private Tile(int id, char[,] image, Boarder top, Boarder right, Boarder bottom, Boarder left)
            {
                ID = id;
                Image = image;
                TopBoarder = top;
                RightBoarder = right;
                BottomBoarder = bottom;
                LeftBoarder = left;
            }

            public int ID { get; }

            public char[,] Image { get; }

            public Boarder TopBoarder { get; }
            public Boarder RightBoarder { get; }
            public Boarder BottomBoarder { get; }
            public Boarder LeftBoarder { get; }

            public IEnumerable<Boarder> Edges()
            {
                yield return TopBoarder;
                yield return LeftBoarder;
                yield return BottomBoarder;
                yield return RightBoarder;
            }

            public string RenderImage()
            {
                Span<char> image = stackalloc char[8 * 8 + 8];

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        image[x * 8 + x + y] = Image[x, y];
                    }

                    image[x * 8 + x + 8] = '\n';
                }

                return new string(image);
            }

            public Tile Rotate()
            {
                var image = new char[8, 8];

                for (int x = 0; x < 8; ++x)
                {
                    for (int y = 0; y < 8; ++y)
                    {
                        image[x, y] = Image[8 - y - 1, x];
                    }
                }

                return new(ID, image, LeftBoarder.Flip(), TopBoarder, RightBoarder.Flip(), BottomBoarder);
            }

            public Tile FlipVertical()
            {
                var image = new char[8, 8];

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        image[x, y] = Image[x, 7 - y];
                    }
                }

                return new(ID, image, TopBoarder.Flip(), LeftBoarder, BottomBoarder.Flip(), RightBoarder);
            }

            public Tile FlipHorizontal()
            {
                var image = new char[8, 8];

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        image[x, y] = Image[7 - x, y];
                    }
                }

                return new(ID, image, BottomBoarder, RightBoarder.Flip(), TopBoarder, LeftBoarder.Flip());
            }

            public override string ToString() => $"Tile {ID}";
        }

        private readonly struct Boarder : IEquatable<Boarder>
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

            private Boarder(int value)
            {
                _value = value;
            }

            public Boarder Flip()
            {
                int forward = _value;
                int reverse = 0;

                for (var index = 0; index < 10; index++)
                {
                    reverse <<= 1;
                    reverse += (forward & 1);
                    forward >>= 1;
                }

                return new Boarder(reverse);
            }

            public bool MatchesInADirection(Boarder other)
            {
                return _value == other._value || _value == other.Flip()._value;
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

            public bool Equals(Boarder other)
            {
                return _value == other._value;
            }

            public override bool Equals(object? obj)
            {
                return obj is Boarder other && Equals(other);
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }


        private static string RenderGrid(Tile[,] image)
        {
            var height = image.GetLength(0);
            var width = image.GetLength(1);

            var render = new StringBuilder();
            Span<char> boarder = stackalloc char[10];

            for (int y = 0; y < height; y++)
            {
                if (y != 0)
                {
                    render.AppendLine();
                }

                for (int x = 0; x < width; x++)
                {
                    if (x != 0)
                    {
                        render.Append(" ");
                    }


                    render.Append(image[y, x].TopBoarder.Render(boarder));
                }

                render.AppendLine();

                for (int i = 1; i < 9; i++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x != 0)
                        {
                            render.Append(" ");
                        }


                        render.Append(image[y, x].LeftBoarder.Render(boarder)[i]);

                        var tileImage = image[y, x].Image;

                        for (int j = 0; j < 8; j++)
                        {
                            render.Append(tileImage[i - 1, j]);
                        }

                        //render.Append("        ");

                        render.Append(image[y, x].RightBoarder.Render(boarder)[i]);
                    }

                    render.AppendLine();
                }

                for (int x = 0; x < width; x++)
                {
                    if (x != 0)
                    {
                        render.Append(" ");
                    }

                    render.Append(image[y, x].BottomBoarder.Render(boarder));
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