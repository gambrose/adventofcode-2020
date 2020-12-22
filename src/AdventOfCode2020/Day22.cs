#nullable  enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day22
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(306, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(31308, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(default, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var (player1, player2) = Parse(input);

            while (player1.Count != 0 && player2.Count != 0)
            {
                var (winner, looser) = player1.Peek() > player2.Peek() ? (player1, player2) : (player2, player1);

                winner.Enqueue(winner.Dequeue());
                winner.Enqueue(looser.Dequeue());
            }

            {
                var winner = player1.Count > 0 ? player1 : player2;

                long score = 0;
                var round = winner.Count;
                foreach (var card in winner)
                {
                    score += (card * round--);
                }

                return score;
            }
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }

        static (Queue<int> player1, Queue<int> player2) Parse(ReadOnlyMemory<string> input)
        {
            Queue<int> player1 = new();
            Queue<int> player2 = new();

            var dealTo = player1;

            foreach (var line in input)
            {
                if (int.TryParse(line, out var card))
                {
                    dealTo.Enqueue(card);
                }
                else if (line == string.Empty)
                {
                    dealTo = player2;
                }
            }

            return (player1, player2);
        }


        private static ReadOnlyMemory<string> Example { get; } = @"Player 1:
9
2
6
3
1

Player 2:
5
8
4
7
10".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day22.input.txt").ToArray();
    }
}