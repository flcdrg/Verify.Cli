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
        if (string.IsNullOrEmpty(pattern))
            throw new ArgumentException("Value cannot be null or empty.", nameof(pattern));

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

    /// <summary>
    /// Builds a scrubber action that removes all occurrences of the given text from a <see cref="StringBuilder"/>.
    /// Uses simple string matching rather than regex for better performance and simplicity.
    /// </summary>
    /// <param name="textToRemove">The exact text to remove from the content.</param>
    /// <returns>
    /// An <see cref="Action{StringBuilder}"/> that performs the removal in the provided <see cref="StringBuilder"/>.
    /// </returns>
    public static Action<StringBuilder> BuildRemoveText(string textToRemove)
    {
        if (string.IsNullOrEmpty(textToRemove))
            throw new ArgumentException("Value cannot be null or empty.", nameof(textToRemove));

        return (builder) =>
        {
            // Get the content as a span to avoid string allocation during search
            var originalLength = builder.Length;
            if (originalLength == 0)
                return;

            // For small builders, work directly with spans for better performance
            if (originalLength < 1024)
            {
                var value = builder.ToString();
                var valueSpan = value.AsSpan();
                
                if (valueSpan.IndexOf(textToRemove.AsSpan(), StringComparison.Ordinal) == -1)
                    return;

                builder.Clear();
                var searchStart = 0;
                
                while (searchStart < valueSpan.Length)
                {
                    var foundIndex = valueSpan[searchStart..].IndexOf(textToRemove.AsSpan(), StringComparison.Ordinal);
                    if (foundIndex == -1)
                    {
                        // No more matches, append the rest
                        builder.Append(valueSpan[searchStart..]);
                        break;
                    }
                    
                    // Append text before the match
                    var actualIndex = searchStart + foundIndex;
                    if (actualIndex > searchStart)
                    {
                        builder.Append(valueSpan.Slice(searchStart, actualIndex - searchStart));
                    }
                    
                    // Skip the matched text and continue searching
                    searchStart = actualIndex + textToRemove.Length;
                }
            }
            else
            {
                // For larger builders, use StringBuilder with span optimizations
                var value = builder.ToString();
                var valueSpan = value.AsSpan();
                
                if (valueSpan.IndexOf(textToRemove.AsSpan(), StringComparison.Ordinal) == -1)
                    return;

                var result = new StringBuilder(builder.Length);
                var lastIndex = 0;

                while (lastIndex < valueSpan.Length)
                {
                    var searchIndex = valueSpan[lastIndex..].IndexOf(textToRemove.AsSpan(), StringComparison.Ordinal);
                    if (searchIndex == -1)
                    {
                        // No more matches, append the rest
                        result.Append(valueSpan[lastIndex..]);
                        break;
                    }

                    var actualIndex = lastIndex + searchIndex;
                    
                    // Append text before the match
                    if (actualIndex > lastIndex)
                    {
                        result.Append(valueSpan.Slice(lastIndex, actualIndex - lastIndex));
                    }

                    // Skip the matched text (don't append it - this removes it)
                    lastIndex = actualIndex + textToRemove.Length;
                }

                builder.Clear();
                builder.Append(result);
            }
        };
    }
}