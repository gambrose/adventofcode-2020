using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode2020
{
    public class Day11
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(37, Part1(Example));

        [Fact]
        public void Part_1_example_mutations()
        {
            var grid = Parse(Example);

            Assert.True(ApplyPart1Rules(grid));
            Assert.Equal(@"#.##.##.##
#######.##
#.#.#..#..
####.##.##
#.##.##.##
#.#####.##
..#.#.....
##########
#.######.#
#.#####.##", grid.ToString());

            Assert.True(ApplyPart1Rules(grid));
            Assert.Equal(@"#.LL.L#.##
#LLLLLL.L#
L.L.L..L..
#LLL.LL.L#
#.LL.LL.LL
#.LLLL#.##
..L.L.....
#LLLLLLLL#
#.LLLLLL.L
#.#LLLL.##", grid.ToString());
        }

        [Fact]
        public void Part_1() => Assert.Equal(2093, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(26, Part2(Example));

        [Fact]
        public void Part_2_example_mutations()
        {
            var grid = Parse(Example);

            string[] mutations =
            {
                @"#.##.##.##
#######.##
#.#.#..#..
####.##.##
#.##.##.##
#.#####.##
..#.#.....
##########
#.######.#
#.#####.##",
                @"#.LL.LL.L#
#LLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLL#
#.LLLLLL.L
#.LLLLL.L#",
                @"#.L#.##.L#
#L#####.LL
L.#.#..#..
##L#.##.##
#.##.#L.##
#.#####.#L
..#.#.....
LLL####LL#
#.L#####.L
#.L####.L#",
                @"#.L#.L#.L#
#LLLLLL.LL
L.L.L..#..
##LL.LL.L#
L.LL.LL.L#
#.LLLLL.LL
..L.L.....
LLLLLLLLL#
#.LLLLL#.L
#.L#LL#.L#",
                @"#.L#.L#.L#
#LLLLLL.LL
L.L.L..#..
##L#.#L.L#
L.L#.#L.L#
#.L####.LL
..#.#.....
LLL###LLL#
#.LLLLL#.L
#.L#LL#.L#",
                @"#.L#.L#.L#
#LLLLLL.LL
L.L.L..#..
##L#.#L.L#
L.L#.LL.L#
#.LLLL#.LL
..#.L.....
LLL###LLL#
#.LLLLL#.L
#.L#LL#.L#"
            };

            foreach (var mutation in mutations)
            {
                Assert.True(ApplyPart2Rules(grid));
                Assert.Equal(mutation, grid.ToString());
            }
        }

        [Fact]
        public void Part_2_seat_examples()
        {
            var grid = Parse(@".......#.
...#.....
.#.......
.........
..#L....#
....#....
.........
#........
...#.....".Split(Environment.NewLine));

            var seat = grid.Seats().First(s => s.Occupied == false);

            Assert.Equal(8, seat.OccupiedVisibleSeats());

            grid = Parse(@".............
.L.L.#.#.#.#.
.............".Split(Environment.NewLine));

            seat = grid.Seats().First();

            Assert.Equal(0, seat.OccupiedVisibleSeats());

            grid = Parse(@".##.##.
#.#.#.#
##...##
...L...
##...##
#.#.#.#
.##.##.".Split(Environment.NewLine));

            seat = grid.Seats().First(s => s.Occupied == false);

            Assert.Equal(0, seat.OccupiedVisibleSeats());
        }

        [Fact]
        public void Part_2() => Assert.Equal(1862, Part2(Input));

        private static int Part1(ReadOnlyMemory<string> input)
        {
            var grid = Parse(input);

            while (ApplyPart1Rules(grid)) { }

            return grid.Seats().Count(seat => seat.Occupied);
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            var grid = Parse(input);

            while (ApplyPart2Rules(grid)) { }

            return grid.Seats().Count(seat => seat.Occupied);
        }

        private static Grid Parse(ReadOnlyMemory<string> input)
        {
            var grid = new Grid(input.Span[0].Length, input.Length);

            for (var y = 0; y < input.Span.Length; y++)
            {
                var line = input.Span[y];
                line.AsSpan().CopyTo(grid.Row(y).Span);
            }

            return grid;
        }

        private static bool ApplyPart1Rules(Grid grid)
        {
            static bool ShouldChange(Seat seat)
            {
                if (seat.Occupied)
                {
                    return seat.OccupiedAdjacentSeats >= 4;
                }
                else
                {
                    return seat.OccupiedAdjacentSeats == 0;
                }
            }

            var toChange = grid.Seats().Where(ShouldChange).ToList();

            foreach (var seat in toChange)
            {
                seat.Occupied = !seat.Occupied;
            }

            return toChange.Count > 0;
        }

        private static bool ApplyPart2Rules(Grid grid)
        {
            static bool ShouldChange(Seat seat)
            {
                if (seat.Occupied)
                {
                    return seat.OccupiedVisibleSeats() >= 5;
                }
                else
                {
                    return seat.OccupiedVisibleSeats() == 0;
                }
            }

            var toChange = grid.Seats().Where(ShouldChange).ToList();

            foreach (var seat in toChange)
            {
                seat.Occupied = !seat.Occupied;
            }

            return toChange.Count > 0;
        }

        private readonly struct Grid
        {
            private readonly Memory<char> map;

            public Grid(int width, int height)
            {
                Width = width;
                Height = height;
                map = new char[width * height];
            }

            public int Width { get; }

            public int Height { get; }

            public Memory<char> Row(int y) => map.Slice(y * Width, Width);

            public char this[int x, int y]
            {
                get => Row(y).Span[x];
                set => Row(y).Span[x] = value;
            }

            public void CopyTo(Grid grid)
            {
                map.CopyTo(grid.map);
            }

            public IEnumerable<Seat> Seats()
            {
                for (var y = 0; y < Height; y++)
                {
                    var row = Row(y);

                    for (int x = 0; x < row.Length; x++)
                    {
                        var position = row.Span[x];

                        if (position == 'L' || position == '#')
                        {
                            yield return new Seat(this, x, y);
                        }
                    }
                }
            }

            public override string ToString()
            {
                var txt = new StringBuilder();

                txt.Append(Row(0));

                for (var y = 1; y < Height; y++)
                {
                    txt.AppendLine();
                    txt.Append(Row(y));
                }

                return txt.ToString();
            }
        }

        private readonly struct Seat
        {
            private readonly Grid map;

            public Seat(Grid map, int x, int y)
            {
                this.map = map;
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }

            public bool Occupied
            {
                get => map[X, Y] == '#';
                set => map[X, Y] = value ? '#' : 'L';
            }

            public int OccupiedAdjacentSeats
            {
                get
                {
                    Span<char> surroundings = stackalloc char[9];

                    Surroundings(surroundings);

                    surroundings[4] = 'O';

                    int count = 0;
                    int index;
                    while ((index = surroundings.IndexOf('#')) >= 0)
                    {
                        count++;
                        surroundings = surroundings.Slice(index + 1);
                    }

                    return count;
                }
            }

            public int OccupiedVisibleSeats()
            {
                var directions = new[]
                {
                    (-1, -1),
                    (0, -1),
                    (1, -1),
                    (1, 0),
                    (1, 1),
                    (0, 1),
                    (-1, 1),
                    (-1, 0)
                };

                int count = 0;
                foreach (var vector in directions)
                {
                    var seat = (Traverse(vector).FirstOrDefault(cell => cell != '.'));

                    if (seat == '#')
                    {
                        count++;
                    }
                }


                return count;
            }

            private IEnumerable<char> Traverse((int x, int y) vector)
            {
                var (x, y) = (X, Y);

                while (true)
                {
                    x += vector.x;
                    y += vector.y;

                    if (x < 0 || x >= map.Width)
                    {
                        yield break;
                    }

                    if (y < 0 || y >= map.Height)
                    {
                        yield break;
                    }

                    yield return map[x, y];
                }
            }

            private void Surroundings(Span<char> surroundings)
            {
                bool left = X > 0;
                bool right = X + 1 < map.Width;
                Range gridRange = (left ? X - 1 : X)..(right ? X + 2 : X + 1);
                Range surroundingRange = (left ? 0 : 1)..(right ? 3 : 2);

                surroundings.Fill(' ');

                var row = surroundings.Slice(0, 3);
                if (Y > 0)
                {
                    map.Row(Y - 1)[gridRange].Span.CopyTo(row[surroundingRange]);
                }

                row = surroundings.Slice(3, 3);
                map.Row(Y)[gridRange].Span.CopyTo(row[surroundingRange]);

                row = surroundings.Slice(6, 3);
                if (Y + 1 < map.Height)
                {
                    map.Row(Y + 1)[gridRange].Span.CopyTo(row[surroundingRange]);
                }
            }

            public override string ToString()
            {
                Span<char> surroundings = stackalloc char[9];

                Surroundings(surroundings);

                return new string(surroundings);
            }
        }

        private static ReadOnlyMemory<string> Example { get; } = @"L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL".Split(Environment.NewLine).ToArray();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day11.input.txt").ToArray();
    }
}