using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode2020
{
    public class Day07
    {
        [Fact]
        public void Part_1_example()
        {
            Assert.Equal(4, CanContain(ParseRules(Example), "shiny gold").Count);
        }

        [Fact]
        public void Part_1()
        {
            Assert.Equal(224, CanContain(ParseRules(Input), "shiny gold").Count);
        }

        [Fact]
        public void Part_2_example1()
        {
            Assert.Equal(7, MustContain(ParseRules(Example), "dark olive").Values.Sum());
            Assert.Equal(11, MustContain(ParseRules(Example), "vibrant plum").Values.Sum());
            Assert.Equal(32, MustContain(ParseRules(Example), "shiny gold").Values.Sum());
        }

        [Fact]
        public void Part_2_example2()
        {
            Assert.Equal(126, MustContain(ParseRules(Example2), "shiny gold").Values.Sum());
        }

        [Fact]
        public void Part_2()
        {
            Assert.Equal(1488, MustContain(ParseRules(Input), "shiny gold").Values.Sum());
        }

        private static Dictionary<string, Dictionary<string, int>> ParseRules(IReadOnlyList<string> input)
        {
            var rules = new Dictionary<string, Dictionary<string, int>>();

            foreach (var line in input)
            {
                var parts = line.Split(" bags contain ");

                var key = parts[0];

                var contents = new Dictionary<string, int>();

                foreach (Match match in Regex.Matches(parts[1], @"(\d+) ([\w ]+) bag"))
                {
                    if (match.TryCapture(out var number, out var colour) && int.TryParse(number, out var x))
                    {
                        contents.Add(colour, x);
                    }
                }

                rules.Add(key, contents);
            }

            return rules;
        }

        private static HashSet<string> CanContain(Dictionary<string, Dictionary<string, int>> rules, string colour)
        {
            var canContain = new HashSet<string>();

            foreach (var rule in rules)
            {
                if (rule.Value.ContainsKey(colour))
                {
                    canContain.Add(rule.Key);
                }
            }

            foreach (var s in canContain.ToArray())
            {
                canContain.UnionWith(CanContain(rules, s));
            }

            return canContain;
        }

        private static Dictionary<string, int> MustContain(Dictionary<string, Dictionary<string, int>> rules, string colour)
        {
            var mustContain = new Dictionary<string, int>();

            if (rules.TryGetValue(colour, out var requrements))
            {
                foreach (var keyValuePair in requrements)
                {
                    var n = keyValuePair.Value;

                    var children = MustContain(rules, keyValuePair.Key);
                    if (children.Any())
                    {
                        n += n * children.Values.Sum();
                    }

                    mustContain[keyValuePair.Key] = n;
                }
            }

            return mustContain;
        }


        public static IReadOnlyList<string> Example = @"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.".SplitLines();

        public static IReadOnlyList<string> Example2 = @"shiny gold bags contain 2 dark red bags.
dark red bags contain 2 dark orange bags.
dark orange bags contain 2 dark yellow bags.
dark yellow bags contain 2 dark green bags.
dark green bags contain 2 dark blue bags.
dark blue bags contain 2 dark violet bags.
dark violet bags contain no other bags.".SplitLines();


        private static IReadOnlyList<string> Input { get; } = File.ReadAllLines("Day07.input.txt");
    }
}