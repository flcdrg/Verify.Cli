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

var scrubInlineRemove = new Option<string?>(
    name: "--scrub-inline-remove")
{
    Description = "Text to match and remove from the file content, e.g., 'temp-id-123'.",
};

var rootCommand = new RootCommand("Verify CLI")
{
    Description = "A command-line tool to verify files against expected content."
};
rootCommand.Options.Add(fileOption);
rootCommand.Options.Add(verifiedDirOption);
rootCommand.Options.Add(scrubInlineDateTime);
rootCommand.Options.Add(scrubInlinePattern);
rootCommand.Options.Add(scrubInlineRemove);

var parseResult = rootCommand.Parse(args);

rootCommand.SetAction(async (innerParseResult) =>
{
    var file = innerParseResult.GetValue(fileOption);
    var options = new VerifyFileOptions(
        VerifiedDir: innerParseResult.GetValue(verifiedDirOption),
        ScrubInlineDatetime: innerParseResult.GetValue(scrubInlineDateTime),
        ScrubInlinePattern: innerParseResult.GetValue(scrubInlinePattern),
        ScrubInlineRemove: innerParseResult.GetValue(scrubInlineRemove)
    );

    if (file == null)
    {
        return;
    }

    await FileVerifier.VerifyFileAsync(file, options);
});

return parseResult.Invoke();