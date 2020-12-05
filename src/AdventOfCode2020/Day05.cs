using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Assert.Equal(806, Input.Max(boardingPass => GetSeatId(GetRow(boardingPass), GetColumn(boardingPass))));
        }

        [Fact]
        public void Part_2()
        {
            var seats = new bool[128 * 8];

            foreach (var boardingPass in Input)
            {
                seats[GetSeatId(GetRow(boardingPass), GetColumn(boardingPass))] = true;
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
        public void Get_row(string boardingPass, int row)
        {
            Assert.Equal(row, GetRow(boardingPass));
        }

        [Theory]
        [InlineData("FBFBBFFRLR", 5)]
        [InlineData("FFFBBBFRRR", 7)]
        [InlineData("BBFFBBFRLL", 4)]
        [InlineData("BFFFBBFRRR", 7)]
        public void Get_column(string boardingPass, int column)
        {
            Assert.Equal(column, GetColumn(boardingPass));
        }

        [Theory]
        [InlineData(44, 5, 357)]
        [InlineData(14, 7, 119)]
        [InlineData(102, 4, 820)]
        [InlineData(70, 7, 567)]
        public void Get_seat_ID(int row, int column, int seatId)
        {
            Assert.Equal(seatId, GetSeatId(row, column));
        }

        private static int GetRow(string boardingPass)
        {
            return BinarySearch(boardingPass.AsSpan(0, 7), 'F', 'B');
        }

        private static int GetColumn(string boardingPass)
        {
            return BinarySearch(boardingPass.AsSpan(7, 3), 'L', 'R');
        }

        private static int GetSeatId(int row, int column)
        {
            return (row * 8) + column;
        }

        private static int BinarySearch(ReadOnlySpan<char> search, char lower, char upper)
        {
            int min = 0;
            int max = (1 << search.Length) - 1;

            while (!search.IsEmpty)
            {
                var half = ((max - min) >> 1) + 1;

                if (search[0] == lower)
                {
                    max -= half;

                }
                else if (search[0] == upper)
                {
                    min += half;
                }
                else
                {
                    throw new ArgumentException();
                }

                search = search.Slice(1);
            }

            Debug.Assert(min == max);

            return min;
        }

        private static IReadOnlyList<string> Input { get; } = File.ReadAllLines("Day05.input.txt");
    }
}