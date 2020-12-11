using System;
using System.Diagnostics;
using System.IO;
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
        public void Part_2_example()
        {
            Assert.Equal(8, Part1(FixedExample));

            Assert.Equal(8, Part2(BrokenExample));
        }

        [Fact]
        public void Part_2() => Assert.Equal(2251, Part2(Input));

        private static int Part1(ReadOnlyMemory<string> input)
        {
            TryRun(input, out var accumulator);

            return accumulator;
        }

        private static int Part2(ReadOnlyMemory<string> input)
        {
            var lines = input.ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                var (operation, argument) = Parse(input.Span[i]);

                switch (operation)
                {
                    case "jmp":
                        lines[i] = $"nop {argument}";
                        break;
                    case "nop":
                        lines[i] = $"jmp {argument}";
                        break;
                }

                if (TryRun(lines, out var answer))
                {
                    return answer;
                }

                lines[i] = line;
            }

            return default;
        }

        private static bool TryRun(ReadOnlyMemory<string> input, out int accumulator)
        {
            accumulator = 0;

            var visited = new bool[input.Length];

            var i = 0;

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

                if (i >= input.Length)
                {
                    return true;
                }
            }

            return false;
        }

        private static (string operation, int argument) Parse(string line)
        {
            Debug.Assert(line.Length > 4);

            var argument = int.Parse(line.AsSpan(4));

            switch (line[0])
            {
                case 'a' when line.StartsWith("acc "):
                    return ("acc", argument);
                case 'j' when line.StartsWith("jmp "):
                    return ("jmp", argument);
                case 'n' when line.StartsWith("nop "):
                    return ("nop", argument);
                default:
                    throw new ArgumentException();
            }
        }

        private static ReadOnlyMemory<string> Example { get; } = @"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6".SplitLines();

        private static ReadOnlyMemory<string> BrokenExample { get; } = @"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6".SplitLines();

        private static ReadOnlyMemory<string> FixedExample { get; } = @"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
nop -4
acc +6".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadAllLines("Day08.input.txt");
    }
}