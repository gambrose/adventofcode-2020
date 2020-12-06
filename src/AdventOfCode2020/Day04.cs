using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode2020
{
    public class Day04
    {
        [Fact]
        public void Part_1_example()
        {
            var passports = ParsePassports(Example).ToList();

            Assert.Equal(4, passports.Count);

            Assert.Equal(2, passports.Count(IsCompletePassport));
        }

        [Fact]
        public void Part_1()
        {
            var passports = ParsePassports(Input);

            Assert.Equal(230, passports.Count(IsCompletePassport));
        }

        [Fact]
        public void Part_2_example()
        {
            var passports = ParsePassports(Example).ToList();

            Assert.Equal(2, passports.Where(IsCompletePassport).Count(IsValidPassport));
        }

        [Fact]
        public void Part_2()
        {
            var passports = ParsePassports(Input);

            Assert.Equal(156, passports.Where(IsCompletePassport).Count(IsValidPassport));
        }

        [Theory]
        [MemberData(nameof(ExampleFields))]
        public void Example_field_cases(KeyValuePair<string, string> pair, bool isValid)
        {
            Assert.Equal(isValid, IsValidField(pair));
        }

        public static TheoryData<KeyValuePair<string, string>, bool> ExampleFields
        {
            get
            {
                var examples = new[]
                {
                    "byr valid:   2002",
                    "byr invalid: 2003",
                    "hgt valid:   60in",
                    "hgt valid:   190cm",
                    "hgt invalid: 190in",
                    "hgt invalid: 190",
                    "hcl valid:   #123abc",
                    "hcl invalid: #123abz",
                    "hcl invalid: 123abc",
                    "ecl valid:   brn",
                    "ecl invalid: wat",
                    "pid valid:   000000001",
                    "pid invalid: 0123456789"
                };

                var examplePattern = new Regex(@"^(\w+) (valid|invalid): +(.+)$");

                var data = new TheoryData<KeyValuePair<string, string>, bool>();

                foreach (var example in examples)
                {
                    if (examplePattern.Match(example).TryCapture(out var key, out var expected, out var value))
                    {
                        data.Add(KeyValuePair.Create(key, value), expected == "valid");
                    }
                }

                return data;
            }
        }

        private static readonly Regex KeyValuePattern = new Regex(@"(\S+):(\S+)", RegexOptions.Compiled);

        private static IEnumerable<IDictionary<string, string>> ParsePassports(ReadOnlyMemory<string> input)
        {
            foreach (var group in input.Split(string.Empty))
            {
                var passport = new Dictionary<string, string>();

                foreach (var line in group)
                {
                    foreach (Match match in KeyValuePattern.Matches(line))
                    {
                        if (match.TryCapture(out var key, out var value))
                        {
                            passport.Add(key, value);
                        }
                    }
                }

                yield return passport;
            }
        }

        private static bool IsCompletePassport(IDictionary<string, string> passport)
        {
            var fields = new SortedSet<string>
            {
                "byr", // Birth Year
                "iyr", // (Issue Year)
                "eyr", // (Expiration Year)
                "hgt", // (Height)
                "hcl", // (Hair Color)
                "ecl", // (Eye Color)
                "pid", // (Passport ID)
              // "cid", 
            };

            return fields.IsSubsetOf(passport.Keys);
        }

        private static bool IsValidPassport(IDictionary<string, string> passport)
        {
            return passport.All(IsValidField);
        }

        private static bool IsValidField(KeyValuePair<string, string> pair)
        {
            var (key, value) = pair;

            return key switch
            {
                //byr(Birth Year) - four digits; at least 1920 and at most 2002.
                "byr" => value.Length == 4 && int.TryParse(value, out var year) && year >= 1920 && year <= 2002,
                //iyr(Issue Year) - four digits; at least 2010 and at most 2020.
                "iyr" => value.Length == 4 && int.TryParse(value, out var year) && year >= 2010 && year <= 2020,
                //eyr(Expiration Year) - four digits; at least 2020 and at most 2030.
                "eyr" => value.Length == 4 && int.TryParse(value, out var year) && year >= 2020 && year <= 2030,
                //hgt(Height) - a number followed by either cm or in:
                "hgt" => Regex.Match(value, @"^(\d+)(cm|in)$").TryCapture(out var digits, out var unit)
                         && int.TryParse(digits, out var number)
                         && unit switch
                         {
                             //If cm, the number must be at least 150 and at most 193.
                             "cm" => number >= 150 && number <= 193,
                             //If in, the number must be at least 59 and at most 76.
                             "in" => number >= 59 && number <= 76,
                             _ => false
                         },
                //hcl(Hair Color) - a # followed by exactly six characters 0-9 or a-f.
                "hcl" => Regex.IsMatch(value, "^#[0-9a-f]{6}$"),
                //ecl(Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
                "ecl" => Regex.IsMatch(value, "^(amb|blu|brn|gry|grn|hzl|oth)$"),
                //pid(Passport ID) - a nine - digit number, including leading zeroes.
                "pid" => Regex.IsMatch(value, @"^\d{9}$"),
                //cid(Country ID) - ignored, missing or not.
                "cid" => true,
                _ => false
            };
        }

        private static ReadOnlyMemory<string> Example { get; } = new[]
        {
            "ecl:gry pid:860033327 eyr:2020 hcl:#fffffd",
            "byr:1937 iyr:2017 cid:147 hgt:183cm",
            "",
            "iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884",
            "hcl:#cfa07d byr:1929",
            "",
            "hcl:#ae17e1 iyr:2013",
            "eyr:2024",
            "ecl:brn pid:760753108 byr:1931",
            "hgt:179cm",
            "",
            "hcl:#cfa07d eyr:2025 pid:166559648",
            "iyr:2011 ecl:brn hgt:59in"
        };

        private static ReadOnlyMemory<string> Input { get; } = File.ReadAllLines("Day04.input.txt");
    }
}