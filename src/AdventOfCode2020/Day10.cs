using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day10
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(35, Part1(Example));

        [Fact]
        public void Part_1_example_2() => Assert.Equal(220, Part1(Example2));

        [Fact]
        public void Part_1() => Assert.Equal(1820, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(default, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static int Part1(ReadOnlyMemory<int> input)
        {
            var foo = Diffs(input).GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            return foo[1] * foo[3];
        }

        private static int Part2(ReadOnlyMemory<int> input)
        {
            return default;
        }

        private static IEnumerable<int> Diffs(ReadOnlyMemory<int> input)
        {
            var adapters = input.ToArray();

            Array.Sort(adapters);

            var previous = 0;
            foreach (var adapter in adapters)
            {
                yield return adapter - previous;
                previous = adapter;

            }

            yield return 3;
        }

        private static ReadOnlyMemory<int> Example { get; } = @"16
10
15
5
1
11
7
19
6
12
4".Split(Environment.NewLine).Select(int.Parse).ToArray();

        private static ReadOnlyMemory<int> Example2 { get; } = @"28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3".Split(Environment.NewLine).Select(int.Parse).ToArray();

        private static ReadOnlyMemory<int> Input { get; } = File.ReadLines("Day10.input.txt").Select(int.Parse).ToArray();
    }
}