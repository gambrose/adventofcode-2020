using System;
using System.Collections.Generic;
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
        public void Part_2_first_example() => Assert.Equal(8, Part2(Example));

        [Fact]
        public void Part_2_second_example() => Assert.Equal(19208, Part2(Example2));

        [Fact]
        public void Part_2() => Assert.Equal(3454189699072, Part2(Input));

        private static int Part1(ReadOnlyMemory<int> input)
        {
            var diffCounts = Diffs(input).GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            return diffCounts[1] * diffCounts[3];
        }

        private static long Part2(ReadOnlyMemory<int> input)
        {
            Span<int> adapters = new int[input.Length + 2];
            input.Span.CopyTo(adapters.Slice(1));
            adapters[1..^1].Sort();
            adapters[^1] = adapters[^2] + 3;

            var root = Variations(new Dictionary<int, Node>(), adapters);

            return root.Possibilities;
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

        private static Node Variations(Dictionary<int, Node> nodes, ReadOnlySpan<int> adapters)
        {
            var adapter = adapters[0];

            if (nodes.TryGetValue(adapter, out var existing))
            {
                return existing;
            }

            const int maxDiff = 3;

            Node node;
            if (adapters.Length > 1)
            {
                var variations = new List<Node>(maxDiff);

                var children = adapters.Slice(1);
                while (children.Length > 0 && children[0] - adapter <= maxDiff)
                {
                    variations.Add(Variations(nodes, children));
                    children = children.Slice(1);
                }

                node = new Node(adapter, variations);
            }
            else
            {
                node = new Node(adapter);
            }

            nodes.Add(node.Adapter, node);

            return node;
        }

        public readonly struct Node
        {
            public int Adapter { get; }

            public Node[] Variations { get; }

            public long Possibilities { get; }

            public Node(int adapter)
            {
                Adapter = adapter;
                Variations = Array.Empty<Node>();
                Possibilities = 1;
            }

            public Node(int adapter, IEnumerable<Node> variations)
            {
                Adapter = adapter;
                Variations = variations.ToArray();
                Possibilities = Variations.Length == 0 ? 1 : Variations.Sum(n => n.Possibilities);
            }
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
4".SplitLines().Select(int.Parse).ToArray();

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
3".SplitLines().Select(int.Parse).ToArray();

        private static ReadOnlyMemory<int> Input { get; } = File.ReadLines("Day10.input.txt").Select(int.Parse).ToArray();
    }
}