namespace Verify.Cli;

public enum Verbosity
{
    Quiet,
    Minimal,
    Normal,
    Detailed,
    Diagnostic
}

public record VerifyFileOptions(
    DirectoryInfo? VerifiedDir = null,
    string? ScrubInlineDatetime = null,
    string? ScrubInlinePattern = null,
    string? ScrubInlineRemove = null,
    Verbosity Verbosity = Verbosity.Normal);

public static class FileVerifier
{
    public static async Task VerifyFileAsync(FileInfo file, VerifyFileOptions options)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (options == null) throw new ArgumentNullException(nameof(options));

        var fullName = file.FullName;

        var settings = new VerifySettings();
        settings.DisableRequireUniquePrefix();

        if (options.ScrubInlineDatetime != null)
        {
            settings.ScrubInlineDateTimes(options.ScrubInlineDatetime);
        }

        if (options.ScrubInlinePattern != null)
        {
            settings.AddScrubber(StringScrubber.BuildReplaceStrings(options.ScrubInlinePattern));
        }

        if (options.ScrubInlineRemove != null)
        {
            settings.AddScrubber(StringScrubber.BuildRemoveText(options.ScrubInlineRemove));
        }

        VerifierSettings.OmitContentFromException();

        if (options.VerifiedDir != null)
        {
            settings.UseDirectory(options.VerifiedDir.FullName);
        }

        // Output detailed information if verbosity is detailed or diagnostic
        if (options.Verbosity >= Verbosity.Detailed)
        {
            Console.WriteLine($"Source file path: {fullName}");
            
            // Calculate received and verified paths to match Verify library naming convention
            var directory = options.VerifiedDir?.FullName ?? Path.GetDirectoryName(fullName)!;
            var fileName = Path.GetFileNameWithoutExtension(fullName);
            var extension = Path.GetExtension(fullName);
            
            var receivedPath = Path.Combine(directory, $"{fileName}.received{extension}");
            var verifiedPath = Path.Combine(directory, $"{fileName}.verified{extension}");
            
            Console.WriteLine($"Received path: {receivedPath}");
            Console.WriteLine($"Verified path: {verifiedPath}");
        }
        
        try
        {
            await Verifier.VerifyFile(fullName, settings, file.FullName);
            
            // If we reach here, verification succeeded (files match)
            if (options.Verbosity >= Verbosity.Detailed)
            {
                Console.WriteLine("Files match");
            }
        }
        catch (Exception)
        {
            // Verification failed, let the exception bubble up
            throw;
        }
    }
}