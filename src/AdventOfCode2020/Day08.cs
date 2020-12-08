using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode2020
{
    public class Day08
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(5, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(2014, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(0, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(0, Part2(Input));

        private static int Part1(ReadOnlyMemory<string> input)
        {
            int accumulator = 0;

            var visited = new bool[input.Length];

            int i = 0;

            while (visited[i] == false)
            {
                visited[i] = true;
                var (operation, argument) = Parse(input.Span[i]);

                switch (operation)
                {
                    case "acc":
                        accumulator += argument;
                        i++;
                        break;
                    case "jmp":
                        i += argument;
                        break;
                    case "nop":
                        i++;
                        break;
                }
            }

            return accumulator;
        }

        private static int Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }

        private static (string operation, int argument) Parse(string line)
        {
            var parts = line.Split(" ");

            return (parts[0], int.Parse(parts[1]));
        }

        private static ReadOnlyMemory<string> Example { get; } = @"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6".Split(Environment.NewLine);

        private static ReadOnlyMemory<string> Input { get; } = File.ReadAllLines("Day08.input.txt");
    }
}