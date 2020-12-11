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
            var rules = grid.Seats().Select(seat => (Rule)new Part1Rule(seat)).ToList();

            Assert.True(ApplyRules(rules));
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

            Assert.True(ApplyRules(rules));
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

            var grid = Parse(Example);
            var rules = grid.Seats().Select(seat => (Rule)new Part2Rule(seat)).ToList();

            foreach (var mutation in mutations)
            {
                Assert.True(ApplyRules(rules));
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

            Assert.Equal(8, seat.VisibleSeats().Count(seat => seat.Occupied));

            grid = Parse(@".............
.L.L.#.#.#.#.
.............".Split(Environment.NewLine));

            seat = grid.Seats().First();

            Assert.Equal(0, seat.VisibleSeats().Count(seat => seat.Occupied));

            grid = Parse(@".##.##.
#.#.#.#
##...##
...L...
##...##
#.#.#.#
.##.##.".Split(Environment.NewLine));

            seat = grid.Seats().First(s => s.Occupied == false);

            Assert.Empty(seat.VisibleSeats());
        }

        [Fact]
        public void Part_2() => Assert.Equal(1862, Part2(Input));

        private static int Part1(ReadOnlyMemory<string> input)
        {
            var grid = Parse(input);

            var rules = grid.Seats().Select(seat => (Rule)new Part1Rule(seat)).ToList();

            while (ApplyRules(rules)) { }

            return grid.Seats().Count(seat => seat.Occupied);
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            var grid = Parse(input);
            var rules = grid.Seats().Select(seat => (Rule)new Part2Rule(seat)).ToList();

            while (ApplyRules(rules)) { }

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

        private static bool ApplyRules(IEnumerable<Rule> rules)
        {
            var toChange = rules.Where(rule => rule.ShouldChange()).ToList();

            foreach (var rule in toChange)
            {
                var seat = rule.Seat;
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

            public IEnumerable<(Position position, char value)> Cells()
            {
                for (var y = 0; y < Height; y++)
                {
                    var row = Row(y);

                    for (int x = 0; x < row.Length; x++)
                    {
                        var cell = row.Span[x];

                        yield return (new Position(x, y), cell);
                    }
                }
            }

            public IEnumerable<Seat> Seats()
            {
                foreach (var (position, value) in Cells())
                {
                    if (value == 'L' || value == '#')
                    {
                        yield return new Seat(this, position);
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
            private static readonly (int, int)[] Directions = new[]
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

            public IEnumerable<Seat> AdjacentSeats()
            {
                foreach (var vector in Directions)
                {
                    foreach (var position in _grid.Traverse(Position, vector).Take(1))
                    {
                        var value = _grid[position];

                        if (value != '.')
                        {
                            yield return new Seat(_grid, position);
                            break;
                        }
                    }
                }
            }

            public IEnumerable<Seat> VisibleSeats()
            {
                foreach (var vector in Directions)
                {
                    foreach (var position in _grid.Traverse(Position, vector))
                    {
                        var value = _grid[position];

                        if (value != '.')
                        {
                            yield return new Seat(_grid, position);
                            break;
                        }
                    }
                }
            }

            public override string ToString()
            {
                return $"{Position} ({_grid[Position]})";
            }
        }

        private interface Rule
        {
            Seat Seat { get; }

            bool ShouldChange();
        }

        private readonly struct Part1Rule : Rule
        {
            public Part1Rule(Seat seat)
            {
                Seat = seat;
                AdjacentSeats = seat.AdjacentSeats().ToArray();
            }

            public Seat Seat { get; }

            public Seat[] AdjacentSeats { get; }

            public bool ShouldChange()
            {
                int adjacentOccupiedSeats = 0;
                foreach (var seat in AdjacentSeats)
                {
                    if (seat.Occupied)
                    {
                        adjacentOccupiedSeats++;
                    }
                }

                if (Seat.Occupied)
                {
                    return adjacentOccupiedSeats >= 4;
                }

                return adjacentOccupiedSeats == 0;
            }
        }

        private readonly struct Part2Rule : Rule
        {
            public Part2Rule(Seat seat)
            {
                Seat = seat;
                VisibleSeats = seat.VisibleSeats().ToArray();
            }

            public Seat Seat { get; }

            public Seat[] VisibleSeats { get; }

            public bool ShouldChange()
            {
                int visibleOccupiedSeats = 0;
                foreach (var seat in VisibleSeats)
                {
                    if (seat.Occupied)
                    {
                        visibleOccupiedSeats++;
                    }
                }

                if (Seat.Occupied)
                {
                    return visibleOccupiedSeats >= 5;
                }

                return visibleOccupiedSeats == 0;
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