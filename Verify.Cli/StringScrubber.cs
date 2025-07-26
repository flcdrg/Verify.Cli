using System.Text;

namespace Verify.Cli;

public static class StringScrubber
{
    /// <summary>
    /// Builds a scrubber action that replaces all matches of the given regex pattern in a <see cref="StringBuilder"/> with unique replacement strings.
    /// Each match is replaced with a string in the format <c>STRING_n</c>, where <c>n</c> is a unique index for each distinct match.
    /// If the regex pattern contains named groups "prefix" and/or "suffix", their values are prepended/appended to the replacement string.
    /// </summary>
    /// <param name="pattern">A regex pattern to match strings for replacement. Named groups "prefix" and "suffix" are supported.</param>
    /// <returns>
    /// An <see cref="Action{StringBuilder}"/> that performs the replacement in the provided <see cref="StringBuilder"/>.
    /// </returns>
    public static Action<StringBuilder> BuildReplaceStrings([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        return (builder) =>
        {
            var value = builder.ToString();
            var regEx = new System.Text.RegularExpressions.Regex(pattern);
            var matches = regEx.Matches(value);

            if (matches.Count == 0)
                return;

            var replacedStrings = new Dictionary<string, int>();
            var count = 0;
            var result = new StringBuilder();
            var lastIndex = 0;

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (!replacedStrings.TryGetValue(match.Value, out var index))
                {
                    index = count++;
                    replacedStrings[match.Value] = index;
                }

                // Append text before the match
                result.Append(value.AsSpan(lastIndex, match.Index - lastIndex));

                var replacement = $"STRING_{index}";
                if (match.Groups["prefix"].Success)
                    replacement = match.Groups["prefix"].Value + replacement;
                if (match.Groups["suffix"].Success)
                    replacement += match.Groups["suffix"].Value;

                result.Append(replacement);
                lastIndex = match.Index + match.Length;
            }

            // Append remaining text
            result.Append(value.AsSpan(lastIndex));

            builder.Clear();
            builder.Append(result);
        };
    }
}