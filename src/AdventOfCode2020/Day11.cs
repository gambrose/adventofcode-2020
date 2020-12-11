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

            Assert.True(ApplyRule(grid));
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

            Assert.True(ApplyRule(grid));
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
        public void Part_2_example() => Assert.Equal(default, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static int Part1(ReadOnlyMemory<string> input)
        {
            var grid = Parse(input);

            var visualisation = new List<string> { grid.ToString() };

            while (ApplyRule(grid))
            {
                visualisation.Add(grid.ToString());
            }

            return grid.Seats().Count(seat => seat.Occupied);
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            return default;
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

        private static bool ApplyRule(Grid grid)
        {
            var toChange = grid.Seats().Where(ShouldChange).ToList();

            foreach (var seat in toChange)
            {
                seat.Occupied = !seat.Occupied;
            }

            return toChange.Count > 0;
        }

        private static bool ShouldChange(Seat seat)
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