using System.Text;

namespace Verify.Cli;

public static class StringScrubber
{
    public static Action<StringBuilder> BuildReplaceStrings([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        return (builder) =>
        {
            var value = builder.ToString();

            var regEx = new System.Text.RegularExpressions.Regex(pattern);

            // find first match
            var match = regEx.Match(value);

            if (!match.Success)
            {
                return;
            }

            var count = 0;
            var replacedStrings = new Dictionary<string, int>();

            // replace all matches
            while (match.Success)
            {
                int index;
                // if the match is already replaced, skip it
                if (replacedStrings.TryGetValue(match.Value, out var n))
                {
                    index = n;
                }
                else
                {
                    // otherwise, add it to the dictionary with the current count
                    index = count++;
                    replacedStrings[match.Value] = index;
                }

                var replacement = $"STRING_{index}";

                if (match.Groups["prefix"].Success)
                {
                    replacement = match.Groups["prefix"].Value + replacement;
                }
                if (match.Groups["suffix"].Success)
                {
                    replacement += match.Groups["suffix"].Value;
                }

                value = value.Replace(match.Value, replacement);
                // find the next match
                match = regEx.Match(value);
            }


            // update builder with the replaced value
            builder.Clear();
            builder.Append(value);
        };
    }
}