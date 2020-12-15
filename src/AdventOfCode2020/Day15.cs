using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day15
    {
        [Fact]
        public void Part_1_example()
        {
            var numbers = Play(new[] { 0, 3, 6 }).Take(10).ToArray();

            Assert.Equal(new[] { 0, 3, 6, 0, 3, 3, 1, 0, 4, 0 }, numbers);
        }

        [Theory]
        [InlineData("0,3,6", 436)]
        [InlineData("1,3,2", 1)]
        [InlineData("2,1,3", 10)]
        [InlineData("2,3,1", 78)]
        [InlineData("3,2,1", 438)]
        [InlineData("3,1,2", 1836)]
        public void Part_1_examples(string startingNumbers, int answer) => Assert.Equal(answer, Part1(startingNumbers));

        [Fact]
        public void Part_1() => Assert.Equal(536, Part1(Input));

        [Fact]
        public void Part_2_example()
        {
            Assert.Equal(default, Part2(""));
        }

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static long Part1(string input)
        {
            var startingNumbers = input.Split(",").Select(int.Parse).ToArray();

            return Play(startingNumbers).Take(2020).Last();
        }

        private static long Part2(string input)
        {
            return default;
        }

        private static IEnumerable<int> Play(int[] startingNumbers)
        {
            var said = new Dictionary<int, (int turn, int diff)>();

            var turn = 0;
            var lastNumber = 0;

            foreach (var num in startingNumbers)
            {
                turn++;

                yield return lastNumber = num;

                said.TryGetValue(lastNumber, out var info);
                said[lastNumber] = (turn, info.turn == 0 ? 0 : turn - info.turn);
            }

            while (true)
            {
                turn++;

                if (said.TryGetValue(lastNumber, out var info) is false)
                {
                    yield return lastNumber = 0;
                }
                else
                {
                    yield return lastNumber = info.diff;
                }

                said.TryGetValue(lastNumber, out info);
                said[lastNumber] = (turn, info.turn == 0 ? 0 : turn - info.turn);
            }
        }

        private static string Input { get; } = File.ReadLines("Day15.input.txt").First();
    }
}