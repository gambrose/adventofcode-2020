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
        [Theory]
        [InlineData("FFFBBBFRRR", 14, 7, 119)]
        [InlineData("BBFFBBFRLL", 102, 4, 820)]
        [InlineData("BFFFBBFRRR", 70, 7, 567)]
        public void Part_1_example(string boardingPass, int row, int column, int seatId)
        {
            Assert.Equal(row, GetRow(boardingPass));
            Assert.Equal(column, GetColumn(boardingPass));
            Assert.Equal(seatId, GetSeatId(row, column));
        }

        [Fact]
        public void Part_1()
        {
            Assert.Equal(806, Input.Max(boardingPass => GetSeatId(GetRow(boardingPass), GetColumn(boardingPass))));
        }

        [Fact]
        public void Part_2_example()
        {

        }

        [Fact]
        public void Part_2()
        {

        }

        [Fact]
        public void Get_row()
        {
            Assert.Equal(44, GetRow("FBFBBFFRLR"));
        }

        [Fact]
        public void Get_column()
        {
            Assert.Equal(5, GetColumn("FBFBBFFRLR"));
        }


        [Fact]
        public void Get_seat_ID()
        {
            Assert.Equal(357, GetSeatId(44, 5));
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