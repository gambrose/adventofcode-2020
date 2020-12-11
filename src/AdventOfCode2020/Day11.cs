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
            private readonly Memory<char> _cells;

            public Grid(int width, int height)
            {
                Width = width;
                Height = height;
                _cells = new char[width * height];
            }

            public int Width { get; }

            public int Height { get; }

            public Memory<char> Row(int y) => _cells.Slice(y * Width, Width);

            public char this[Position position]
            {
                get => Row(position.Y).Span[position.X];
                set => Row(position.Y).Span[position.X] = value;
            }

            public IEnumerable<Position> Traverse(Position start, (int x, int y) directionVector)
            {
                var (x, y) = start;

                while (true)
                {
                    x += directionVector.x;
                    y += directionVector.y;

                    if (x < 0 || x >= Width)
                    {
                        yield break;
                    }

                    if (y < 0 || y >= Height)
                    {
                        yield break;
                    }

                    yield return new Position(x, y);
                }
            }

            public IEnumerable<Seat> Seats()
            {
                for (var y = 0; y < Height; y++)
                {
                    var row = Row(y);

                    for (int x = 0; x < row.Length; x++)
                    {
                        var cell = row.Span[x];

                        if (cell == 'L' || cell == '#')
                        {
                            yield return new Seat(this, new Position(x, y));
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

        private readonly struct Position
        {
            public Position(int x, int y)
            {
                if (x < 0) throw new ArgumentOutOfRangeException(nameof(x));
                if (y < 0) throw new ArgumentOutOfRangeException(nameof(y));
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }

            public void Deconstruct(out int x, out int y)
            {
                x = X;
                y = Y;
            }

            public override string ToString()
            {
                return $"{X}, {Y}";
            }
        }

        private readonly struct Seat
        {
            private readonly Grid _grid;

            public Seat(Grid grid, Position position)
            {
                _grid = grid;
                Position = position;
            }

            public Position Position { get; }

            public bool Occupied
            {
                get => _grid[Position] == '#';
                set => _grid[Position] = value ? '#' : 'L';
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
                foreach (var position in _grid.Traverse(Position, vector))
                {
                    yield return _grid[position];
                }
            }

            private void Surroundings(Span<char> surroundings)
            {
                var (x, y) = Position;

                bool left = x > 0;
                bool right = x + 1 < _grid.Width;
                Range gridRange = (left ? x - 1 : x)..(right ? x + 2 : x + 1);
                Range surroundingRange = (left ? 0 : 1)..(right ? 3 : 2);

                surroundings.Fill(' ');

                var row = surroundings.Slice(0, 3);
                if (y > 0)
                {
                    _grid.Row(y - 1)[gridRange].Span.CopyTo(row[surroundingRange]);
                }

                row = surroundings.Slice(3, 3);
                _grid.Row(y)[gridRange].Span.CopyTo(row[surroundingRange]);

                row = surroundings.Slice(6, 3);
                if (y + 1 < _grid.Height)
                {
                    _grid.Row(y + 1)[gridRange].Span.CopyTo(row[surroundingRange]);
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