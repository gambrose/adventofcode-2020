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
        public void Part_2_example() => Assert.Equal(291, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(33647, Part2(Input));

        [Fact]
        public void infinite_game_prevention_rule_example()
        {
            var player1 = new Queue<int>(new[] { 43, 19 });
            var player2 = new Queue<int>(new[] { 2, 29, 14 });

            var winner = RecursiveCombat(player1, player2);

            Assert.Equal(player1, winner);
        }

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var (player1, player2) = Parse(input);

            var winner = Combat(player1, player2);

            return Score(winner);
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            var (player1, player2) = Parse(input);

            var winner = RecursiveCombat(new Queue<int>(player1), new Queue<int>(player2));

            return Score(winner);
        }

        static Queue<int> RecursiveCombat(Queue<int> player1, Queue<int> player2, Log? log = null)
        {
            log?.StartGame();

            var previous = new HashSet<(Snapshot, Snapshot)>();

            while (true)
            {
                if (player1.Count == 0)
                {
                    log?.EndGame(2, player1, player2);
                    return player2;

                }

                if (player2.Count == 0)
                {
                    log?.EndGame(1, player1, player2);
                    return player1;
                }

                if (!previous.Add((new Snapshot(player1), new Snapshot(player2))))
                {
                    log?.EndGame(1, player1, player2);
                    return player1;
                }

                log?.StartRound(player1, player2);

                Queue<int> winner;
                Queue<int> looser;

                if (player1.Count > player1.Peek() && player2.Count > player2.Peek())
                {
                    log?.StartSubGame();

                    Queue<int> subGamePlayer1 = new(player1.Skip(1).Take(player1.Peek()));
                    Queue<int> subGamePlayer2 = new(player2.Skip(1).Take(player2.Peek()));

                    var subGameWinner = RecursiveCombat(subGamePlayer1, subGamePlayer2, log);

                    (winner, looser) = ReferenceEquals(subGamePlayer1, subGameWinner) ? (player1, player2) : (player2, player1);

                    log?.EndSubGame();
                }
                else
                {
                    (winner, looser) = player1.Peek() > player2.Peek() ? (player1, player2) : (player2, player1);
                }

                log?.EndRound(ReferenceEquals(winner, player1) ? 1 : 2);

                winner.Enqueue(winner.Dequeue());
                winner.Enqueue(looser.Dequeue());
            }
        }

        static Queue<int> Combat(Queue<int> player1, Queue<int> player2)
        {
            while (true)
            {
                if (player1.Count == 0)
                {
                    return player2;

                }

                if (player2.Count == 0)
                {
                    return player1;
                }

                var (winner, looser) = player1.Peek() > player2.Peek() ? (player1, player2) : (player2, player1);

                winner.Enqueue(winner.Dequeue());
                winner.Enqueue(looser.Dequeue());
            }
        }

        static long Score(Queue<int> cards)
        {
            long score = 0;
            var round = cards.Count;
            foreach (var card in cards)
            {
                score += (card * round--);
            }

            return score;
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

        class Snapshot : IEquatable<Snapshot>
        {
            public Snapshot(Queue<int> cards)
            {

                Cards = cards.ToArray();
            }

            public int[] Cards { get; }

            public override string ToString() => string.Join(", ", Cards);

            public override int GetHashCode() => Cards.Length > 0 ? Cards.Aggregate(HashCode.Combine) : 0;

            public override bool Equals(object? obj) => Equals(obj as Snapshot);

            public bool Equals(Snapshot? other) => !ReferenceEquals(null, other) && Cards.SequenceEqual(other.Cards);
        }

        class Log
        {
            private readonly TextWriter _writer;
            private readonly Stack<(int game, int round)> _subGame = new();

            private int _games;

            public Log(TextWriter writer)
            {
                _writer = writer;
            }

            public int Game { get; private set; }

            public int Round { get; private set; }

            public void StartGame()
            {
                Game = ++_games;
                Round = 0;

                _writer.WriteLine($"=== Game {Game} ===");
            }

            public void EndGame(int winner, IEnumerable<int> player1, IEnumerable<int> player2)
            {
                _writer.WriteLine($"The winner of game {Game} is player {winner}!");

                if (_subGame.Count == 0)
                {
                    _writer.WriteLine();
                    _writer.WriteLine();
                    _writer.WriteLine("== Post-game results ==");
                    WriteDecks(player1, player2);
                }
            }

            public void StartRound(Queue<int> player1, Queue<int> player2)
            {
                Round++;

                _writer.WriteLine();
                _writer.WriteLine($"-- Round {Round} (Game {Game}) --");
                WriteDecks(player1, player2);
                _writer.WriteLine($"Player 1 plays: {player1.Peek()}");
                _writer.WriteLine($"Player 2 plays: {player2.Peek()}");
            }

            public void EndRound(int winner) => _writer.WriteLine($"Player {winner} wins round {Round} of game {Game}!");

            public void StartSubGame()
            {
                _writer.WriteLine("Playing a sub-game to determine the winner...");
                _writer.WriteLine();

                _subGame.Push((Game, Round));
            }

            public void EndSubGame()
            {
                var (game, round) = _subGame.Pop();

                Game = game;
                Round = round;

                _writer.WriteLine();
                _writer.WriteLine($"...anyway, back to game {Game}.");
            }

            private void WriteDecks(IEnumerable<int> player1, IEnumerable<int> player2)
            {
                _writer.WriteLine($"Player 1's deck: {string.Join(", ", player1)}");
                _writer.WriteLine($"Player 2's deck: {string.Join(", ", player2)}");
            }
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