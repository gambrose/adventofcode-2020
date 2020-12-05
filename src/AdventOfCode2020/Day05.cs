using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day05
    {
        [Fact]
        public void Part_1()
        {
            Assert.Equal(806, Input.Max(SeatId));
        }

        [Fact]
        public void Part_2()
        {
            var seats = new bool[128 * 8];

            foreach (var boardingPass in Input)
            {
                seats[SeatId(boardingPass)] = true;
            }

            var freeSeats = new List<int>();

            for (int id = 1; id < seats.Length - 1; id++)
            {
                if (seats[id] == false && seats[id - 1] && seats[id + 1])
                {
                    freeSeats.Add(id);
                }
            }

            Assert.Equal(new[] { 562 }, freeSeats);
        }

        [Theory]
        [InlineData("FBFBBFFRLR", 44)]
        [InlineData("FFFBBBFRRR", 14)]
        [InlineData("BBFFBBFRLL", 102)]
        [InlineData("BFFFBBFRRR", 70)]
        public void Get_row_from_boarding_pass(string boardingPass, int row)
        {
            Assert.Equal(row, Row(boardingPass));
        }

        [Theory]
        [InlineData("FBFBBFFRLR", 5)]
        [InlineData("FFFBBBFRRR", 7)]
        [InlineData("BBFFBBFRLL", 4)]
        [InlineData("BFFFBBFRRR", 7)]
        public void Get_column_from_boarding_pass(string boardingPass, int column)
        {
            Assert.Equal(column, Column(boardingPass));
        }

        [Theory]
        [InlineData("FBFBBFFRLR", 357)]
        [InlineData("FFFBBBFRRR", 119)]
        [InlineData("BBFFBBFRLL", 820)]
        [InlineData("BFFFBBFRRR", 567)]
        public void Get_seat_ID_from_boarding_pass(string boardingPass, int seatId)
        {
            Assert.Equal(seatId, SeatId(boardingPass));
        }

        private static int Row(string boardingPass)
        {
            var row = 0;
            foreach (var c in boardingPass.AsSpan(0, 7))
            {
                row = (row << 1) | c switch
                {
                    'F' => 0,
                    'B' => 1,
                    _ => throw new ArgumentException()
                };
            }

            return row;
        }

        private static int Column(string boardingPass)
        {
            var column = 0;
            foreach (var c in boardingPass.AsSpan(7, 3))
            {
                column = (column << 1) | c switch
                {
                    'L' => 0,
                    'R' => 1,
                    _ => throw new ArgumentException()
                };
            }

            return column;
        }

        private static int SeatId(string boardingPass) => Row(boardingPass) * 8 + Column(boardingPass);

        private static IReadOnlyList<string> Input { get; } = File.ReadAllLines("Day05.input.txt");
    }
}