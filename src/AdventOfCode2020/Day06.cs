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
            Assert.Equal(11, AnyoneAnswered(Example).Sum(x => x.Count));
        }

        [Fact]
        public void Part_1()
        {
            Assert.Equal(6947, AnyoneAnswered(Input).Sum(x => x.Count));
        }

        [Fact]
        public void Part_2_example()
        {
            Assert.Equal(6, EveryoneAnswered(Example).Sum(x => x.Count));
        }

        [Fact]
        public void Part_2()
        {
            Assert.Equal(3398, EveryoneAnswered(Input).Sum(x => x.Count));
        }

        private static IEnumerable<ISet<char>> AnyoneAnswered(ReadOnlyMemory<string> input)
        {
            foreach (var group in input.Split(string.Empty))
            {
                var correctAnswers = new HashSet<char>();

                foreach (var line in group)
                {
                    correctAnswers.UnionWith(line);
                }

                yield return correctAnswers;
            }
        }

        private static IEnumerable<ISet<char>> EveryoneAnswered(ReadOnlyMemory<string> input)
        {
            foreach (var group in input.Split(string.Empty))
            {
                HashSet<char> groupAnswers = null;

                foreach (var line in group)
                {
                    groupAnswers ??= new HashSet<char>(line);
                    groupAnswers.IntersectWith(line);
                }

                yield return groupAnswers;
            }
        }


        public static ReadOnlyMemory<string> Example = @"abc

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


        private static ReadOnlyMemory<string> Input { get; } = File.ReadAllLines("Day06.input.txt");
    }
}