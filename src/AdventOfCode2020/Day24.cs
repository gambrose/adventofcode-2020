#nullable  enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AdventOfCode2020
{
    public class Day24
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(10, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(277, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(default, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var blackTiles = new HashSet<Hex>();

            foreach (var line in input)
            {
                var tile = Parse(line).Aggregate(Hex.Add);

                // Flip to black
                if (blackTiles.Add(tile) is false)
                {
                    // Already black so flip back to white
                    blackTiles.Remove(tile);
                }
            }

            return blackTiles.Count;
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }

        [Theory]
        [InlineData("e")]
        [InlineData("w")]
        [InlineData("ne")]
        [InlineData("nw")]
        [InlineData("se")]
        [InlineData("sw")]
        [InlineData("esenee")]
        public void Can_parse_lines(string line)
        {
            Assert.Equal(line, string.Join("", Parse(line)));
        }

        [Theory]
        [InlineData("nwwswee", "origin")]
        [InlineData("ee", "e2")]
        [InlineData("nwne", "n2")]
        [InlineData("sese", "s2e")]
        public void Moves(string moves, string result) => Assert.Equal(result, Parse(moves).Aggregate(Hex.Add).ToString());

        readonly struct Hex
        {
            public static readonly Hex East = new(1, 0);
            public static readonly Hex West = new(-1, 0);
            public static readonly Hex NorthWest = new(0, -1);
            public static readonly Hex NorthEast = new(1, -1);
            public static readonly Hex SouthWest = new(-1, 1);
            public static readonly Hex SouthEast = new(0, 1);

            public Hex(int q, int r)
            {
                Q = q;
                R = r;
            }

            public int Q { get; }
            public int R { get; }

            public override string ToString()
            {
                if (R == 0 && Q == 0)
                {
                    return "origin";
                }

                var east = Q + (R - (R & 1)) / 2;

                if (R % 2 != 0 && east >= 0)
                {
                    east++;
                }

                var south = R;

                return new StringBuilder()
                    .Append(south switch
                    {
                        0 => "",
                        1 => "s",
                        > 1 => $"s{south}",
                        -1 => "n",
                        < -1 => $"n{-south}"
                    })
                    .Append(east switch
                    {
                        0 => "",
                        1 => "e",
                        > 1 => $"e{east}",
                        -1 => "w",
                        < -1 => $"w{-east}"
                    }).ToString();
            }

            public static Hex operator +(Hex a, Hex b) => Add(a, b);

            public static Hex Add(Hex a, Hex b) => new(a.Q + b.Q, a.R + b.R);
        }

        static IEnumerable<Hex> Parse(string line)
        {
            var remaining = line.AsMemory().Trim();

            static char Read(ref ReadOnlyMemory<char> remaining)
            {
                var c = remaining.Span[0];
                remaining = remaining.Slice(1);
                return c;
            }

            while (remaining.IsEmpty is false)
            {
                yield return Read(ref remaining) switch
                {
                    'n' => Read(ref remaining) switch
                    {
                        'e' => Hex.NorthEast,
                        'w' => Hex.NorthWest
                    },
                    's' => Read(ref remaining) switch
                    {
                        'e' => Hex.SouthEast,
                        'w' => Hex.SouthWest
                    },
                    'e' => Hex.East,
                    'w' => Hex.West
                };
            }
        }

        private static ReadOnlyMemory<string> Example { get; } = @"sesenwnenenewseeswwswswwnenewsewsw
neeenesenwnwwswnenewnwwsewnenwseswesw
seswneswswsenwwnwse
nwnwneseeswswnenewneswwnewseswneseene
swweswneswnenwsewnwneneseenw
eesenwseswswnenwswnwnwsewwnwsene
sewnenenenesenwsewnenwwwse
wenwwweseeeweswwwnwwe
wsweesenenewnwwnwsenewsenwwsesesenwne
neeswseenwwswnwswswnw
nenwswwsewswnenenewsenwsenwnesesenew
enewnwewneswsewnwswenweswnenwsenwsw
sweneswneswneneenwnewenewwneswswnese
swwesenesewenwneswnwwneseswwne
enesenwswwswneneswsenwnewswseenwsese
wnwnesenesenenwwnenwsewesewsesesew
nenewswnwewswnenesenwnesewesw
eneswnwswnwsenenwnwnwwseeswneewsenese
neswnwewnwnwseenwseesewsenwsweewe
wseweeenwnesenwwwswnew".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day24.input.txt").ToArray();
    }
}