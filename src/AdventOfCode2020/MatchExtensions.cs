#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode2020
{
    public static class MatchExtensions
    {
        public static bool TryCapture(this Match match, [NotNullWhen(true)] out string? groupValue1, [NotNullWhen(true)] out string? groupValue2)
        {
            if (match is { Success: true, Groups: { Count: > 2 } })
            {
                groupValue1 = match.Groups[1].Value;
                groupValue2 = match.Groups[2].Value;
                return true;
            }

            groupValue1 = null;
            groupValue2 = null;
            return false;
        }

        public static bool TryCapture(this Match match, [NotNullWhen(true)] out string? groupValue1, [NotNullWhen(true)] out string? groupValue2, [NotNullWhen(true)] out string? groupValue3)
        {
            if (match is { Success: true, Groups: { Count: > 3 } })
            {
                groupValue1 = match.Groups[1].Value;
                groupValue2 = match.Groups[2].Value;
                groupValue3 = match.Groups[3].Value;
                return true;
            }

            groupValue1 = null;
            groupValue2 = null;
            groupValue3 = null;
            return false;
        }
    }
}