using System;

namespace AdventOfCode2020
{
    public static class SpanExtensions
    {
        public static TAccumulate Aggregate<TSource, TAccumulate>(this ReadOnlySpan<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            TAccumulate result = seed;
            foreach (TSource element in source)
            {
                result = func(result, element);
            }

            return result;
        }
    }
}