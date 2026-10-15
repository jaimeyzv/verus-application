namespace Verus.Infrastructure.Relevance;

internal static class TextSimilarity
{
    public static string Normalize(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var buffer = new char[value.Length];
        var length = 0;
        foreach (var character in value)
        {
            if (char.IsLetterOrDigit(character))
                buffer[length++] = char.ToLowerInvariant(character);
        }

        return new string(buffer, 0, length);
    }

    public static double Ratio(string left, string right)
    {
        if (left.Length == 0 && right.Length == 0)
            return 1.0;

        var distance = LevenshteinDistance(left, right);
        var longest = Math.Max(left.Length, right.Length);
        return longest == 0 ? 0.0 : 1.0 - ((double)distance / longest);
    }

    private static int LevenshteinDistance(string left, string right)
    {
        var previous = new int[right.Length + 1];
        var current = new int[right.Length + 1];

        for (var j = 0; j <= right.Length; j++)
            previous[j] = j;

        for (var i = 1; i <= left.Length; i++)
        {
            current[0] = i;
            for (var j = 1; j <= right.Length; j++)
            {
                var cost = left[i - 1] == right[j - 1] ? 0 : 1;
                current[j] = Math.Min(
                    Math.Min(current[j - 1] + 1, previous[j] + 1),
                    previous[j - 1] + cost);
            }

            (previous, current) = (current, previous);
        }

        return previous[right.Length];
    }
}
