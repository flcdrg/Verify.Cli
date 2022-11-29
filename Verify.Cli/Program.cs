using System.CommandLine;
using Verify.Cli;

var fileOption = new Option<FileInfo?>(
    name: "--file",
    description: "The file to verify."
);

fileOption.AddAlias("-f");
fileOption.IsRequired = true;

var rootCommand = new RootCommand("Verify CLI");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(async (file) =>
    {
        if (file == null)
        {
            return;
        }

        // ApplyScrubbers.UseAssembly(null, file.DirectoryName!);

        var settings = new VerifySettings();
        settings.DisableRequireUniquePrefix();

        var result = await Verifier.VerifyFile(file.FullName, settings, sourceFile: file.FullName);
    },
    fileOption);

return await rootCommand.InvokeAsync(args);