#nullable  enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day17
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(112, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(301, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(848, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(2424, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var world = Parse(input).ToDictionary(x => x.Position);

            var comparer = new CubePositionComparer();

            var changed = new HashSet<Cube>(world.Values);
            var active = world.Count;

            for (int cycles = 0; cycles < 6; cycles++)
            {
                var shouldChange = new HashSet<Cube>(comparer);

                foreach (var cube in changed)
                {
                    if (cube.ShouldMutate(world))
                    {
                        shouldChange.Add(cube);
                    }

                    foreach (var neighbor in cube.GetNeighbors(world))
                    {
                        if (neighbor.ShouldMutate(world))
                        {
                            shouldChange.Add(neighbor);
                        }
                    }
                }

                foreach (var cube in shouldChange)
                {
                    cube.Active = !cube.Active;

                    if (cube.Active)
                    {
                        active++;
                    }
                    else
                    {
                        active--;
                    }
                }

                changed = shouldChange;
            }

            return active;
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            var world = Parse2(input).ToDictionary(x => x.Position);

            var comparer = new HypercubePositionComparer();

            var changed = new HashSet<Hypercube>(world.Values);
            var active = world.Count;

            for (int cycles = 0; cycles < 6; cycles++)
            {
                var shouldChange = new HashSet<Hypercube>(comparer);

                foreach (var cube in changed)
                {
                    if (cube.ShouldMutate(world))
                    {
                        shouldChange.Add(cube);
                    }

                    foreach (var neighbor in cube.GetNeighbors(world))
                    {
                        if (neighbor.ShouldMutate(world))
                        {
                            shouldChange.Add(neighbor);
                        }
                    }
                }

                foreach (var cube in shouldChange)
                {
                    cube.Active = !cube.Active;

                    if (cube.Active)
                    {
                        active++;
                    }
                    else
                    {
                        active--;
                    }
                }

                changed = shouldChange;
            }

            return active;
        }

        public static IEnumerable<Cube> Parse(ReadOnlyMemory<string> input)
        {
            var size = input.Length;
            var offset = -(size / 2);

            for (int x = 0; x < size; x++)
            {
                var line = input.Span[x];

                for (int y = 0; y < size; y++)
                {
                    var c = line[y];

                    if (c == '#')
                    {
                        var position = new PositionXYZ { X = offset + x, Y = offset + y };
                        yield return new Cube(position)
                        {
                            Active = true
                        };
                    }
                }
            }
        }

        public static IEnumerable<Hypercube> Parse2(ReadOnlyMemory<string> input)
        {
            var size = input.Length;
            var offset = -(size / 2);

            for (int x = 0; x < size; x++)
            {
                var line = input.Span[x];

                for (int y = 0; y < size; y++)
                {
                    var c = line[y];

                    if (c == '#')
                    {
                        var position = new PositionXYZW { X = offset + x, Y = offset + y };
                        yield return new Hypercube(position)
                        {
                            Active = true
                        };
                    }
                }
            }
        }

        public readonly struct PositionXYZ : IEquatable<PositionXYZ>
        {
            public int X { get; init; }
            public int Y { get; init; }
            public int Z { get; init; }

            public override string ToString()
            {
                return $"{X}, {Y}, {Z}";
            }

            public IEnumerable<PositionXYZ> Surrounding()
            {
                int[] diff = { -1, 0, 1 };

                foreach (var z in diff)
                {
                    foreach (var y in diff)
                    {
                        foreach (var x in diff)
                        {
                            var position = new PositionXYZ { X = X + x, Y = Y + y, Z = Z + z };

                            if (!position.Equals(this))
                            {
                                yield return position;
                            }
                        }
                    }
                }
            }

            public bool Equals(PositionXYZ other)
            {
                return X == other.X && Y == other.Y && Z == other.Z;
            }

            public override bool Equals(object? obj)
            {
                return obj is PositionXYZ other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y, Z);
            }
        }

        public class Cube
        {
            private Cube[]? _neighbors;

            public Cube(PositionXYZ position)
            {
                Position = position;
            }

            public PositionXYZ Position { get; }

            public bool Active { get; set; }


            public Cube[] GetNeighbors(Dictionary<PositionXYZ, Cube> world)
            {
                if (_neighbors != null)
                {
                    return _neighbors;
                }


                _neighbors = new Cube[26];

                var i = 0;
                foreach (var position in Position.Surrounding())
                {
                    if (!world.TryGetValue(position, out var cube))
                    {
                        world.Add(position, cube = new Cube(position));
                    }

                    _neighbors[i++] = cube;
                }

                return _neighbors;
            }

            public bool ShouldMutate(Dictionary<PositionXYZ, Cube> world)
            {
                int activeNeighbors = GetNeighbors(world).Count(x => x.Active);

                bool shouldBeActive;
                if (Active)
                {
                    shouldBeActive = activeNeighbors >= 2 & activeNeighbors <= 3;
                }
                else
                {

                    shouldBeActive = activeNeighbors == 3;
                }

                return Active != shouldBeActive;
            }
        }

        public class CubePositionComparer : IEqualityComparer<Cube>
        {
            public bool Equals(Cube x, Cube y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Position.Equals(y.Position);
            }

            public int GetHashCode(Cube obj)
            {
                return obj.Position.GetHashCode();
            }
        }

        public readonly struct PositionXYZW : IEquatable<PositionXYZW>
        {
            public int X { get; init; }
            public int Y { get; init; }
            public int Z { get; init; }
            public int W { get; init; }

            public override string ToString()
            {
                return $"{X}, {Y}, {Z}, {W}";
            }

            public IEnumerable<PositionXYZW> Surrounding()
            {
                int[] diff = { -1, 0, 1 };

                foreach (var w in diff)
                {
                    foreach (var z in diff)
                    {
                        foreach (var y in diff)
                        {
                            foreach (var x in diff)
                            {
                                var position = new PositionXYZW { X = X + x, Y = Y + y, Z = Z + z, W = W + w };

                                if (!position.Equals(this))
                                {
                                    yield return position;
                                }
                            }
                        }
                    }
                }
            }

            public bool Equals(PositionXYZW other)
            {
                return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
            }

            public override bool Equals(object? obj)
            {
                return obj is PositionXYZW other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y, Z, W);
            }
        }

        public class Hypercube
        {
            private Hypercube[]? _neighbors;

            public Hypercube(PositionXYZW position)
            {
                Position = position;
            }

            public PositionXYZW Position { get; }

            public bool Active { get; set; }

            public Hypercube[] GetNeighbors(Dictionary<PositionXYZW, Hypercube> world)
            {
                if (_neighbors != null)
                {
                    return _neighbors;
                }


                _neighbors = new Hypercube[80];

                var i = 0;
                foreach (var position in Position.Surrounding())
                {
                    if (!world.TryGetValue(position, out var cube))
                    {
                        world.Add(position, cube = new Hypercube(position));
                    }

                    _neighbors[i++] = cube;
                }

                return _neighbors;
            }

            public bool ShouldMutate(Dictionary<PositionXYZW, Hypercube> world)
            {
                int activeNeighbors = GetNeighbors(world).Count(x => x.Active);

                bool shouldBeActive;
                if (Active)
                {
                    shouldBeActive = activeNeighbors >= 2 & activeNeighbors <= 3;
                }
                else
                {

                    shouldBeActive = activeNeighbors == 3;
                }

                return Active != shouldBeActive;
            }
        }

        public class HypercubePositionComparer : IEqualityComparer<Hypercube>
        {
            public bool Equals(Hypercube x, Hypercube y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Position.Equals(y.Position);
            }

            public int GetHashCode(Hypercube obj)
            {
                return obj.Position.GetHashCode();
            }
        }

        private static ReadOnlyMemory<string> Example { get; } = @".#.
..#
###".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day17.input.txt").ToArray();
    }
}