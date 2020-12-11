using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var rules = new Part1Rules(grid);

            Assert.True(rules.Apply());
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

            Assert.True(rules.Apply());
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
            var rules = new Part2Rules(grid);

            foreach (var mutation in mutations)
            {
                Assert.True(rules.Apply());
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
...#.....".SplitLines());

            var seat = grid.Seats().First(s => s.Occupied == false);

            Assert.Equal(8, seat.VisibleSeats().Count(seat => seat.Occupied));

            grid = Parse(@".............
.L.L.#.#.#.#.
.............".SplitLines());

            seat = grid.Seats().First();

            Assert.Equal(0, seat.VisibleSeats().Count(seat => seat.Occupied));

            grid = Parse(@".##.##.
#.#.#.#
##...##
...L...
##...##
#.#.#.#
.##.##.".SplitLines());

            seat = grid.Seats().First(s => s.Occupied == false);

            Assert.Empty(seat.VisibleSeats());
        }

        [Fact]
        public void Part_2() => Assert.Equal(1862, Part2(Input));

        private static int Part1(ReadOnlyMemory<string> input)
        {
            var grid = Parse(input);

            var rules = new Part1Rules(grid);

            while (rules.Apply()) { }

            return grid.Seats().Count(seat => seat.Occupied);
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            var grid = Parse(input);

            var rules = new Part2Rules(grid);

            while (rules.Apply()) { }

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
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _cells.Span[position.Y * Width + position.X];
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set => _cells.Span[position.Y * Width + position.X] = value;
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
            private static readonly (int, int)[] Directions = {
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
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _grid[Position] == '#';
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        private class Part1Rules
        {
            private readonly (Seat seat, Seat[] adjacentSeats)[] _seats;
            private readonly bool[] _changes;

            public Part1Rules(Grid grid)
            {
                _seats = grid.Seats().Select(seat =>
                {
                    var adjacentSeats = seat.AdjacentSeats().ToArray();
                    return (seat, adjacentSeats);
                }).ToArray();
                _changes = new bool[_seats.Length];
            }

            public bool Apply()
            {
                static bool ShouldChange(Seat seat, Seat[] adjacentSeats)
                {
                    int adjacentOccupiedSeats = 0;
                    foreach (var adjacentSeat in adjacentSeats)
                    {
                        if (adjacentSeat.Occupied)
                        {
                            adjacentOccupiedSeats++;
                        }
                    }

                    if (seat.Occupied)
                    {
                        return adjacentOccupiedSeats >= 4;
                    }

                    return adjacentOccupiedSeats == 0;
                }

                for (var i = 0; i < _seats.Length; i++)
                {
                    var (seat, adjacentSeats) = _seats[i];
                    _changes[i] = ShouldChange(seat, adjacentSeats);
                }

                int changes = 0;
                for (var i = 0; i < _seats.Length; i++)
                {
                    if (_changes[i])
                    {
                        var (seat, _) = _seats[i];
                        seat.Occupied = !seat.Occupied;
                        changes++;
                    }
                }

                return changes > 0;
            }
        }

        private class Part2Rules
        {
            private readonly (Seat seat, Seat[] visibleSeats)[] _seats;
            private readonly bool[] _changes;

            public Part2Rules(Grid grid)
            {
                _seats = grid.Seats().Select(seat =>
                {
                    var visibleSeats = seat.VisibleSeats().ToArray();
                    return (seat, visibleSeats);
                }).ToArray();
                _changes = new bool[_seats.Length];
            }

            public bool Apply()
            {
                static bool ShouldChange(Seat seat, Seat[] visibleSeats)
                {
                    int visibleOccupiedSeats = 0;
                    foreach (var visibleSeat in visibleSeats)
                    {
                        if (visibleSeat.Occupied)
                        {
                            visibleOccupiedSeats++;
                        }
                    }

                    if (seat.Occupied)
                    {
                        return visibleOccupiedSeats >= 5;
                    }

                    return visibleOccupiedSeats == 0;
                }

                for (var i = 0; i < _seats.Length; i++)
                {
                    var (seat, visibleSeats) = _seats[i];
                    _changes[i] = ShouldChange(seat, visibleSeats);
                }

                int changes = 0;
                for (var i = 0; i < _seats.Length; i++)
                {
                    if (_changes[i])
                    {
                        var (seat, _) = _seats[i];
                        seat.Occupied = !seat.Occupied;
                        changes++;
                    }
                }

                return changes > 0;
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
L.LLLLL.LL".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day11.input.txt").ToArray();
    }
}