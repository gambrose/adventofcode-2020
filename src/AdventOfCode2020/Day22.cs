#nullable  enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

            var (winner, _) = RecursiveCombat(player1, player2);

            Assert.Equal(1, winner);
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

            var (_, deck) = RecursiveCombat(new Queue<int>(player1), new Queue<int>(player2));

            return Score(deck);
        }

        static (int winner, Deck winningDeck) RecursiveCombat(IEnumerable<int> player1, IEnumerable<int> player2, Log? log = null)
        {
            log?.StartGame();

            var previous = new HashSet<(Deck, Deck)>();

            var deck1 = new Deck(player1);
            var deck2 = new Deck(player2);

            while (true)
            {
                if (deck1.Count == 0)
                {
                    log?.EndGame(2, deck1, deck2);
                    return (2, deck2);
                }

                if (deck2.Count == 0)
                {
                    log?.EndGame(1, deck1, deck2);
                    return (1, deck1);
                }

                if (!previous.Add((deck1, deck2)))
                {
                    log?.EndGame(1, deck1, deck2);
                    return (1, deck1);
                }

                log?.StartRound(deck1, deck2);

                int winner;

                if (deck1.Count > deck1.Peek() && deck2.Count > deck2.Peek())
                {
                    log?.StartSubGame();

                    (winner, _) = RecursiveCombat(deck1.Skip(1).Take(deck1.Peek()), deck2.Skip(1).Take(deck2.Peek()), log);

                    log?.EndSubGame();
                }
                else
                {
                    winner = deck1.Peek() > deck2.Peek() ? 1 : 2;
                }

                log?.EndRound(winner);

                if (winner == 1)
                {
                    deck1 = deck1.Win(deck1.Peek(), deck2.Peek());
                    deck2 = deck2.Loose();
                }
                else
                {
                    deck2 = deck2.Win(deck2.Peek(), deck1.Peek());
                    deck1 = deck1.Loose();
                }
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

        static long Score(IReadOnlyCollection<int> cards)
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

        readonly struct Deck : IReadOnlyCollection<int>, IEquatable<Deck>
        {
            private readonly int[] _cards;
            private readonly int _start;

            public Deck(IEnumerable<int> cards)
            {
                _cards = cards.ToArray();
                _start = 0;
                Count = _cards.Length;
            }

            private Deck(int[] cards, int start, int count)
            {
                _cards = cards;
                _start = start;
                Count = count;
            }

            public ReadOnlyMemory<int> AsMemory() => _cards.AsMemory(_start, Count);

            public ReadOnlySpan<int> AsSpan() => _cards.AsSpan(_start, Count);

            public int Count { get; }

            public int Peek() => _cards[_start];

            public Deck Loose() => new(_cards, _start + 1, Count - 1);

            public Deck Win(int card1, int card2)
            {
                var cards = _cards;
                var start = _start + 1;
                var length = Count + 1;

                if (start + length > _cards.Length)
                {
                    cards = new int[length * 2];
                    start = 0;

                    _cards.AsSpan(_start + 1, Count - 1).CopyTo(cards);
                }

                Debug.Assert(cards[start + Count - 1] == 0);
                Debug.Assert(cards[start + Count] == 0);

                cards[start + Count - 1] = card1;
                cards[start + Count] = card2;

                return new(cards, start, length);
            }

            public override string ToString() => string.Join(", ", AsMemory());

            public override int GetHashCode()
            {
                var hashCode = 0;
                foreach (var card in AsSpan())
                {
                    hashCode += card;
                    hashCode <<= 1;
                }

                return hashCode;
            }

            public override bool Equals(object? obj) => obj is Deck other && Equals(other);

            public bool Equals(Deck other) => AsSpan().SequenceEqual(other.AsSpan());

            public IEnumerator<int> GetEnumerator()
            {
                foreach (var card in AsMemory())
                {
                    yield return card;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
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

            public void StartRound(Deck player1, Deck player2)
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