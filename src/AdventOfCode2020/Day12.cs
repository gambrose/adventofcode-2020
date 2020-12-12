using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day12
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(25, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(default, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(default, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static int Part1(ReadOnlyMemory<string> input)
        {
            var ship = new Ship();

            foreach (var line in input)
            {
                var (action, value) = Parse(line);

                ship.Move(action, value);
            }

            return Math.Abs(ship.EastWestPosition) + Math.Abs(ship.NorthSouthPosition);
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }

        private class Ship
        {
            public int EastWestPosition { get; private set; }

            public int NorthSouthPosition { get; private set; }

            public int Direction { get; private set; } = 90;

            public void Move(char action, int value)
            {
                switch (action)
                {
                    //Action N means to move north by the given value.
                    case 'N':
                        NorthSouthPosition += value;
                        return;
                    //Action S means to move south by the given value.
                    case 'S':
                        NorthSouthPosition -= value;
                        return;
                    //Action E means to move east by the given value.
                    case 'E':
                        EastWestPosition += value;
                        return;
                    //Action W means to move west by the given value.
                    case 'W':
                        EastWestPosition -= value;
                        return;
                    //Action L means to turn left the given number of degrees.
                    case 'L':
                        Move('R', 360 - value);
                        return;
                    //Action R means to turn right the given number of degrees.
                    case 'R':
                        Direction = (Direction + value) % 360;
                        return;
                    //Action F means to move forward by the given value in the direction the ship is currently facing.
                    case 'F':
                        var direction = Direction switch
                        {
                            0 => 'N',
                            90 => 'E',
                            180 => 'S',
                            270 => 'W',
                            _ => throw new NotImplementedException()
                        };
                        Move(direction, value);
                        return;
                }
            }
        }

        private static (char action, int value) Parse(string line)
        {
            Debug.Assert(line.Length > 1);

            var action = line[0];

            var value = int.Parse(line.AsSpan(1));

            return (action, value);
        }

        private static ReadOnlyMemory<string> Example { get; } = @"F10
N3
F7
R90
F11".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day12.input.txt").ToArray();
    }
}