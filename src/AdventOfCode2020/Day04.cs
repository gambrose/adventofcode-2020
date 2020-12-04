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
                var examples = new[] {"byr valid:   2002",
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
                                 "pid invalid: 0123456789"};

                var data = new TheoryData<KeyValuePair<string, string>, bool>();

                foreach (var example in examples)
                {
                    var match = Regex.Match(example, @"^(\w+)\W+(valid|invalid): +(.+)$");

                    bool isValid = match.Groups[2].Value == "valid";

                    data.Add(new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[3].Value), isValid);
                }

                return data;
            }
        }

        private static IEnumerable<IDictionary<string, string>> ParsePassports(IReadOnlyList<string> input)
        {
            var passport = new Dictionary<string, string>();

            foreach (var line in input)
            {
                if (line == string.Empty && passport.Count > 0)
                {
                    yield return passport;

                    passport = new Dictionary<string, string>();
                    continue;
                }

                foreach (var part in line.Split(" "))
                {
                    var pair = part.Split(":");

                    passport.Add(pair[0], pair[1]);
                }
            }

            if (passport.Count > 0)
            {
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
              //  "cid", 
            };

            return fields.IsSubsetOf(passport.Keys);
        }

        private static bool IsValidPassport(IDictionary<string, string> passport)
        {
            return passport.All(IsValidField);
        }

        private static bool IsValidField(KeyValuePair<string, string> pair)
        {
            static bool ValidHeight(string value)
            {
                //hgt(Height) - a number followed by either cm or in:
                var match = Regex.Match(value, @"^(\d+)(cm|in)$");

                if (!match.Success)
                {
                    return false;
                }

                var number = int.Parse(match.Groups[1].Value);
                return match.Groups[2].Value switch
                {
                    //If cm, the number must be at least 150 and at most 193.
                    "cm" => number >= 150 && number <= 193,
                    //If in, the number must be at least 59 and at most 76.
                    "in" => number >= 59 && number <= 76
                };
            }

            var (key, value) = pair;
            return key switch
            {
                //byr(Birth Year) - four digits; at least 1920 and at most 2002.
                "byr" => value.Length == 4 && int.Parse(value) >= 1920 && int.Parse(value) <= 2002,
                //iyr(Issue Year) - four digits; at least 2010 and at most 2020.
                "iyr" => value.Length == 4 && int.Parse(value) >= 2010 && int.Parse(value) <= 2020,
                //eyr(Expiration Year) - four digits; at least 2020 and at most 2030.
                "eyr" => value.Length == 4 && int.Parse(value) >= 2020 && int.Parse(value) <= 2030,

                "hgt" => ValidHeight(value),

                //hcl(Hair Color) - a # followed by exactly six characters 0-9 or a-f.
                "hcl" => Regex.IsMatch(value, "^#[0-9a-f]{6}$"),
                //ecl(Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
                "ecl" => Regex.IsMatch(value, "^(amb|blu|brn|gry|grn|hzl|oth)$"),
                //pid(Passport ID) - a nine - digit number, including leading zeroes.
                "pid" => Regex.IsMatch(value, @"^\d{9}$"),
                //cid(Country ID) - ignored, missing or not.
                _ => true
            };
        }

        private static IReadOnlyList<string> Example { get; } = new string[]
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

        private static IReadOnlyList<string> Input { get; } = File.ReadAllLines("Day04.input.txt");
    }
}