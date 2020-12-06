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
        public void Part_2()
        {

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