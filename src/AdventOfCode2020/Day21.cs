#nullable  enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode2020
{
    public class Day21
    {
        [Fact]
        public void Part_1_example() => Assert.Equal(5, Part1(Example));

        [Fact]
        public void Part_1() => Assert.Equal(2635, Part1(Input));

        [Fact]
        public void Part_2_example() => Assert.Equal("mxmxvkd,sqjhc,fvjkl", Part2(Example));

        [Fact]
        public void Part_2() => Assert.Equal("xncgqbcp,frkmp,qhqs,qnhjhn,dhsnxr,rzrktx,ntflq,lgnhmx", Part2(Input));

        private static long Part1(ReadOnlyMemory<string> input)
        {
            var foods = Parse(input).ToList();

            var knownAllergens = ResolveAllergens(foods);

            var dangerousIngredients = new HashSet<string>(knownAllergens.Select(x => x.ingredient));

            var count = 0;
            foreach (var (_, ingredients) in foods)
            {
                foreach (var ingredient in ingredients)
                {
                    if (dangerousIngredients.Contains(ingredient) == false)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static string Part2(ReadOnlyMemory<string> input)
        {
            var foods = Parse(input);

            var knownAllergens = ResolveAllergens(foods);

            var dangerousIngredients = knownAllergens.OrderBy(x => x.allergen).Select(x => x.ingredient);

            return string.Join(",", dangerousIngredients);
        }

        private static IEnumerable<(string allergen, string ingredient)> ResolveAllergens(IEnumerable<(string[] allergens, string[] ingredients)> foods)
        {
            Dictionary<string, HashSet<string>> unknownAllergens = new();

            foreach (var (allergens, ingredients) in foods)
            {
                foreach (var allergen in allergens)
                {
                    if (unknownAllergens.TryGetValue(allergen, out var possibleIngredients))
                    {
                        possibleIngredients.IntersectWith(ingredients);
                    }
                    else
                    {
                        unknownAllergens[allergen] = new HashSet<string>(ingredients);
                    }
                }
            }

            while (unknownAllergens.Count > 0)
            {
                var originalCount = unknownAllergens.Count;

                foreach (var (allergen, possibleIngredients) in unknownAllergens)
                {
                    if (possibleIngredients.Count == 1)
                    {
                        var ingredient = possibleIngredients.Single();

                        yield return (allergen, ingredient);


                        unknownAllergens.Remove(allergen);

                        foreach (var ingredients in unknownAllergens.Values)
                        {
                            ingredients.Remove(ingredient);
                        }
                    }
                }

                if (unknownAllergens.Count == originalCount)
                {
                    break;
                }
            }
        }

        private static IEnumerable<(string[] allergens, string[] ingredients)> Parse(ReadOnlyMemory<string> input)
        {
            foreach (string line in input)
            {
                const string pattern = " (contains ";
                var start = line.AsSpan().IndexOf(pattern);

                var allergens = line[(start + pattern.Length)..^1].Trim();
                var ingredients = line[..start].Trim();

                yield return (allergens.Split(", "), ingredients.Split(" "));
            }
        }

        private static ReadOnlyMemory<string> Example { get; } = @"mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
trh fvjkl sbzzf mxmxvkd (contains dairy)
sqjhc fvjkl (contains soy)
sqjhc mxmxvkd sbzzf (contains fish)".SplitLines();

        private static ReadOnlyMemory<string> Input { get; } = File.ReadLines("Day21.input.txt").ToArray();
    }
}