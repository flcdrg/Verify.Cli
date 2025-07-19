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

var rootCommand = new RootCommand("Verify CLI");
rootCommand.Options.Add(fileOption);
rootCommand.Options.Add(verifiedDirOption);

var parseResult = rootCommand.Parse(args);

rootCommand.SetAction(async (parseResult) =>
{
    var file = parseResult.GetValue(fileOption);
    var verifiedDir = parseResult.GetValue(verifiedDirOption);

    if (file == null)
    {
        return;
    }

    var settings = new VerifySettings();
    settings.DisableRequireUniquePrefix();

    if (verifiedDir != null)
    {
        settings.UseDirectory(verifiedDir.FullName);
    }

    var result = await Verifier.VerifyFile(file.FullName, settings, file.FullName);
});

return parseResult.Invoke();