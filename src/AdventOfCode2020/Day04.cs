using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Xunit;

namespace AdventOfCode2020
{
    public class Day04
    {
        [Fact]
        public void Part_1_example()
        {
            var passports = ParsePassports(Example).ToList();

            Assert.Equal(4, passports.Count());

            Assert.Equal(2, passports.Count(IsValidPassport));
        }

        [Fact]
        public void Part_1()
        {
            var passports = ParsePassports(Input);

            Assert.Equal(230, passports.Count(IsValidPassport));
        }

        [Fact]
        public void Part_2_example()
        {
        }

        [Fact]
        public void Part_2()
        {
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

        private static bool IsValidPassport(IDictionary<string, string> passport)
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