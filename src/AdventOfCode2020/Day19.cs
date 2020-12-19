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
        public void Part_2_example()
        {
            Assert.Equal(3, Part1(Example2));

            Assert.Equal(12, Part2(Example2));
        }

        [Fact]
        public void Part_2() => Assert.Equal(422, Part2(Input));

        [Fact]
        public void Rule_building()
        {
            var (rules, _) = Parse(Example);

            Assert.Equal("a", BuildRule(rules, 4));
            Assert.Equal("(?:aa|bb)", BuildRule(rules, 2));
            Assert.Equal("(?:(?:aa|bb)(?:ab|ba)|(?:ab|ba)(?:aa|bb))", BuildRule(rules, 1));
            Assert.Equal("(?:a(?:(?:aa|bb)(?:ab|ba)|(?:ab|ba)(?:aa|bb))b)", BuildRule(rules, 0));
        }

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var (rules, messages) = Parse(input);

            var ruleZero = BuildRule(rules, 0);

            var regexZero = new Regex($"^{ruleZero}$", RegexOptions.Compiled);

            return messages.Count(regexZero.IsMatch);
        }

        private static int Part2(ReadOnlyMemory<string> input)
        {
            var (rules, messages) = Parse(input);
            
            var rule42 = BuildRule(rules, 42);
            var rule8 = $"{rule42}+";

            var rule31 = BuildRule(rules, 31);
            var rule11 = $"(?<g42>{rule42})+(?<g31-g42>{rule31})+";

            var rule0 = $"^{rule8}{rule11}$";
            var regex0 = new Regex($"^{rule0}$", RegexOptions.Compiled);

            return messages.Count(regex0.IsMatch);
        }

        private static string BuildRule(Dictionary<int, string> rules, int number)
        {
            var rule = rules[number];

            if (rule.StartsWith('"'))
            {
                return rule.Substring(1, 1);
            }

            var r = new StringBuilder();

            if (rule.IndexOf('|') > 0)
            {
                r.Append("(?:");

                foreach (var option in rule.Split("|"))
                {
                    if (r.Length > 3)
                    {
                        r.Append("|");
                    }

                    foreach (var n in option.Trim().Split(" ").Select(int.Parse))
                    {
                        r.Append(BuildRule(rules, n));
                    }
                }

                r.Append(")");
            }
            else
            {
                r.Append("(?:");

                foreach (var n in rule.Trim().Split(" ").Select(int.Parse))
                {
                    r.Append(BuildRule(rules, n));
                }

                r.Append(")");
            }

            return r.ToString();
        }
        private static (Dictionary<int, string> rules, IEnumerable<string> messages) Parse(ReadOnlyMemory<string> input)
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

            return (rules, messages);
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

        private static ReadOnlyMemory<string> Example2 { get; } = @"42: 9 14 | 10 1
9: 14 27 | 1 26
10: 23 14 | 28 1
1: ""a""
11: 42 31
5: 1 14 | 15 1
19: 14 1 | 14 14
12: 24 14 | 19 1
16: 15 1 | 14 14
31: 14 17 | 1 13
6: 14 14 | 1 14
2: 1 24 | 14 4
0: 8 11
13: 14 3 | 1 12
15: 1 | 14
17: 14 2 | 1 7
23: 25 1 | 22 14
28: 16 1
4: 1 1
20: 14 14 | 1 15
3: 5 14 | 16 1
27: 1 6 | 14 18
14: ""b""
21: 14 1 | 1 14
25: 1 1 | 1 14
22: 14 14
8: 42
26: 14 22 | 1 20
18: 15 15
7: 14 5 | 1 21
24: 14 1

abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa
bbabbbbaabaabba
babbbbaabbbbbabbbbbbaabaaabaaa
aaabbbbbbaaaabaababaabababbabaaabbababababaaa
bbbbbbbaaaabbbbaaabbabaaa
bbbababbbbaaaaaaaabbababaaababaabab
ababaaaaaabaaab
ababaaaaabbbaba
baabbaaaabbaaaababbaababb
abbbbabbbbaaaababbbbbbaaaababb
aaaaabbaabaaaaababaa
aaaabbaaaabbaaa
aaaabbaabbaaaaaaabbbabbbaaabbaabaaa
babaaabbbaaabaababbaabababaaab
aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day19.input.txt").ToArray();
    }
}