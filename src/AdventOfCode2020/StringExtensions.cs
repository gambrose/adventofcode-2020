using System;

namespace AdventOfCode2020
{
    public static class StringExtensions
    {
        public static string[] SplitLines(this string s) => s.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
    }
}