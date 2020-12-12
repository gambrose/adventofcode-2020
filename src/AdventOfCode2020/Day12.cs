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
        public void Part_1() => Assert.Equal(1533, Part1(Input));

        [Fact]
        public void Part_2_example()
        {
            var ship = new Ship2();
            Assert.Equal((0, 0), (ship.Position.EastWestPosition, ship.Position.NorthSouthPosition));
            Assert.Equal((10, 1), (ship.Waypoint.EastWestPosition, ship.Waypoint.NorthSouthPosition));

            ship.Move('F', 10);
            Assert.Equal((100, 10), (ship.Position.EastWestPosition, ship.Position.NorthSouthPosition));
            Assert.Equal((10, 1), (ship.Waypoint.EastWestPosition, ship.Waypoint.NorthSouthPosition));

            ship.Move('N', 3);
            Assert.Equal((100, 10), (ship.Position.EastWestPosition, ship.Position.NorthSouthPosition));
            Assert.Equal((10, 4), (ship.Waypoint.EastWestPosition, ship.Waypoint.NorthSouthPosition));

            ship.Move('F', 7);
            Assert.Equal((170, 38), (ship.Position.EastWestPosition, ship.Position.NorthSouthPosition));
            Assert.Equal((10, 4), (ship.Waypoint.EastWestPosition, ship.Waypoint.NorthSouthPosition));

            ship.Move('R', 90);
            Assert.Equal((170, 38), (ship.Position.EastWestPosition, ship.Position.NorthSouthPosition));
            Assert.Equal((4, -10), (ship.Waypoint.EastWestPosition, ship.Waypoint.NorthSouthPosition));

            ship.Move('F', 11);
            Assert.Equal((214, -72), (ship.Position.EastWestPosition, ship.Position.NorthSouthPosition));
            Assert.Equal((4, -10), (ship.Waypoint.EastWestPosition, ship.Waypoint.NorthSouthPosition));

            Assert.Equal(286, Part2(Example));
        }

        [Fact]
        public void Part_2() => Assert.Equal(25235, Part2(Input));

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
            var ship = new Ship2();

            foreach (var line in input)
            {
                var (action, value) = Parse(line);

                ship.Move(action, value);
            }

            return Math.Abs(ship.Position.EastWestPosition) + Math.Abs(ship.Position.NorthSouthPosition);
        }

        private class Ship
        {
            public int EastWestPosition { get; private set; }

            public int NorthSouthPosition { get; private set; }

            public int Direction { get; private set; } = 90;

            public Ship Move(char action, int value)
            {
                switch (action)
                {
                    //Action N means to move north by the given value.
                    case 'N':
                        NorthSouthPosition += value;
                        return this;
                    //Action S means to move south by the given value.
                    case 'S':
                        NorthSouthPosition -= value;
                        return this;
                    //Action E means to move east by the given value.
                    case 'E':
                        EastWestPosition += value;
                        return this;
                    //Action W means to move west by the given value.
                    case 'W':
                        EastWestPosition -= value;
                        return this;
                    //Action L means to turn left the given number of degrees.
                    case 'L':
                        return Move('R', 360 - value);
                    //Action R means to turn right the given number of degrees.
                    case 'R':
                        Direction = (Direction + value) % 360;
                        return this;
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
                        return Move(direction, value);
                }

                throw new NotImplementedException();
            }
        }

        private class Position
        {
            public int EastWestPosition { get; set; }

            public int NorthSouthPosition { get; set; }

            public Position Move(char action, int value)
            {
                switch (action)
                {
                    //Action N means to move north by the given value.
                    case 'N':
                        NorthSouthPosition += value;
                        return this;
                    //Action S means to move south by the given value.
                    case 'S':
                        NorthSouthPosition -= value;
                        return this;
                    //Action E means to move east by the given value.
                    case 'E':
                        EastWestPosition += value;
                        return this;
                    //Action W means to move west by the given value.
                    case 'W':
                        EastWestPosition -= value;
                        return this;
                }

                throw new NotImplementedException();
            }
        }

        private class Ship2
        {
            public Position Waypoint { get; } = new Position { EastWestPosition = 10, NorthSouthPosition = 1 };

            public Position Position { get; } = new Position();

            public void Move(char action, int value)
            {
                switch (action)
                {
                    case 'N':
                    case 'S':
                    case 'E':
                    case 'W':
                        Waypoint.Move(action, value);
                        return;
                    // Action L means to rotate the waypoint around the ship left (counter-clockwise) the given number of degrees.
                    case 'L':
                        Move('R', 360 - value);
                        return;
                    // Action R means to rotate the waypoint around the ship right (clockwise) the given number of degrees.
                    case 'R':
                        if (value == 90)
                        {
                            var (east, north) = (Waypoint.EastWestPosition, Waypoint.NorthSouthPosition);
                            Waypoint.NorthSouthPosition = -east;
                            Waypoint.EastWestPosition = north;
                        }
                        else if (value > 90)
                        {
                            Move('R', 90);
                            Move('R', value - 90);
                        }

                        return;
                    // Action F means to move forward to the waypoint a number of times equal to the given value.
                    case 'F':
                        Position.Move('N', Waypoint.NorthSouthPosition * value);
                        Position.Move('E', Waypoint.EastWestPosition * value);
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