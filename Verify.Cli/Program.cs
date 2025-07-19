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

var rootCommand = new RootCommand("Verify CLI");
rootCommand.Options.Add(fileOption);

var parseResult = rootCommand.Parse(args);

rootCommand.SetAction(async (parseResult) =>
{
    var file = parseResult.GetValue(fileOption);

    if (file == null)
    {
        return;
    }

    var settings = new VerifySettings();
    settings.DisableRequireUniquePrefix();

    var result = await Verifier.VerifyFile(file.FullName, settings, file.FullName);
});

return parseResult.Invoke();