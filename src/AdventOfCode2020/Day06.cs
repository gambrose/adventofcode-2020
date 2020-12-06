using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day06
    {
        [Fact]
        public void Part_1_example()
        {
            Assert.Equal(11, ParsePassports(Example).Sum(x => x.Count));
        }

        [Fact]
        public void Part_1()
        {
            Assert.Equal(6947, ParsePassports(Input).Sum(x => x.Count));
        }

        [Fact]
        public void Part_2_example()
        {
            Assert.Equal(6, ParsePassports2(Example).Sum(x => x.Count));
        }

        [Fact]
        public void Part_2()
        {
            Assert.Equal(3398, ParsePassports2(Input).Sum(x => x.Count));
        }

        private static IEnumerable<ISet<char>> ParsePassports(IReadOnlyList<string> input)
        {
            var correctAnswers = new HashSet<char>();

            foreach (var line in input)
            {
                if (line == string.Empty && correctAnswers.Count > 0)
                {
                    yield return correctAnswers;

                    correctAnswers = new HashSet<char>();
                    continue;
                }

                foreach (var c in line)
                {
                    if (Char.IsLetter(c))
                    {
                        correctAnswers.Add(c);
                    }
                }
            }

            if (correctAnswers.Count > 0)
            {
                yield return correctAnswers;
            }
        }

        private static IEnumerable<ISet<char>> ParsePassports2(IReadOnlyList<string> input)
        {
            HashSet<char> groupAnswers = null;

            foreach (var line in input)
            {
                if (line == string.Empty)
                {
                    if (groupAnswers?.Count > 0)
                    {
                        yield return groupAnswers;
                    }

                    groupAnswers = null;
                    continue;
                }

                var lineAnswers = new HashSet<char>(line);

                groupAnswers ??= lineAnswers;

                groupAnswers.IntersectWith(lineAnswers);
            }

            if (groupAnswers?.Count > 0)
            {
                yield return groupAnswers;
            }
        }

        public static IReadOnlyList<string> Example = @"abc

a
b
c

ab
ac

a
a
a
a

b".Split(Environment.NewLine);


        private static IReadOnlyList<string> Input { get; } = File.ReadAllLines("Day06.input.txt");
    }
}