#nullable  enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode2020
{
    public class Day19
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(2, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(210, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal(default, Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal(default, Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var rules = new Dictionary<int, string>();
            var messages = new List<string>();

            foreach (string line in input)
            {
                var ruleSeparator = line.IndexOf(':');
                if (ruleSeparator >= 0)
                {
                    var ruleNumber = int.Parse(line.AsSpan().Slice(0, ruleSeparator));

                    var rule = line.AsSpan().Slice(ruleSeparator + 1).Trim();

                    rules.Add(ruleNumber, rule.ToString());
                }
                else if (line.Length > 0)
                {
                    messages.Add(line);
                }
            }

            string BuildRule(int number)
            {
                var rule = rules[number];

                if (rule.StartsWith('"'))
                {
                    return rule.Substring(1, 1);
                }

                var r = new StringBuilder();

                if (rule.IndexOf('|') > 0)
                {
                    r.Append("(?<r");
                    r.Append(number);
                    r.Append(">");
                    
                    foreach (var option in rule.Split("|"))
                    {
                        if (r.Length > 0)
                        {
                            r.Append("|");
                        }

                        r.Append("(");

                        foreach (var n in option.Trim().Split(" ").Select(int.Parse))
                        {
                            r.Append(BuildRule(n));
                        }

                        r.Append(")");
                    }

                    r.Append(")");
}
                else
                {
                    r.Append("(?<r");
                    r.Append(number);
                    r.Append(">");

                    foreach (var n in rule.Trim().Split(" ").Select(int.Parse))
                    {
                        r.Append(BuildRule(n));
                    }

                    r.Append(")");
                }

                return r.ToString();
            }

            var ruleZero = BuildRule(0);
            var regexZero = new Regex($"^{ruleZero}$", RegexOptions.Compiled);

            return messages.Count(regexZero.IsMatch);
        }

        private static long Part2(ReadOnlyMemory<string> input)
        {
            return default;
        }

        private static ReadOnlyMemory<string> Example { get; } = @"0: 4 1 5
1: 2 3 | 3 2
2: 4 4 | 5 5
3: 4 5 | 5 4
4: ""a""
5: ""b""

ababbb
bababa
abbbab
aaabbb
aaaabbb".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day19.input.txt").ToArray();
    }
}