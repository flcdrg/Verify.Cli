using System.CommandLine;
using Verify.Cli;

var fileOption = new Option<FileInfo?>(
    name: "--file"
)
{
    Description = "The file to verify",
};

fileOption.Aliases.Add("-f");
fileOption.Required = true;

var verifiedDirOption = new Option<DirectoryInfo?>(
    name: "--verified-dir"
)
{
    Description = "Directory to store/look for .verified files",
};
verifiedDirOption.Aliases.Add("-d");

var scrubInlineDateTime = new Option<string?>(
    name: "--scrub-inline-datetime")
{
    Description = "Format for inline date times to scrub, e.g., 'yyyy-MM-ddTHH:mm:ss.fffZ'.",
};

var scrubInlinePattern = new Option<string?>(
    name: "--scrub-inline-pattern")
{
    Description = "Regex pattern to match inline strings for scrubbing, e.g., '\"/astro/[^\"]+\"', or '(?<prefix>\")/_astro/[^\"]+(?<suffix>\")'.",
};

var rootCommand = new RootCommand("Verify CLI");
rootCommand.Options.Add(fileOption);
rootCommand.Options.Add(verifiedDirOption);
rootCommand.Options.Add(scrubInlineDateTime);
rootCommand.Options.Add(scrubInlinePattern);

var parseResult = rootCommand.Parse(args);

rootCommand.SetAction(async (innerParseResult) =>
{
    var file = innerParseResult.GetValue(fileOption);
    var options = new VerifyFileOptions(
        VerifiedDir: innerParseResult.GetValue(verifiedDirOption),
        ScrubInlineDatetime: innerParseResult.GetValue(scrubInlineDateTime),
        ScrubInlinePattern: innerParseResult.GetValue(scrubInlinePattern)
    );

    if (file == null)
    {
        return;
    }

    await FileVerifier.VerifyFileAsync(file, options);
});

return parseResult.Invoke();