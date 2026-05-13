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
    string[]? ScrubInlinePatterns = null,
    string[]? ScrubInlineRemoves = null,
    Verbosity Verbosity = Verbosity.Normal,
    string? OverrideFilename = null);

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

        if (options.ScrubInlinePatterns != null)
        {
            foreach (var pattern in options.ScrubInlinePatterns)
            {
                if (pattern is null)
                {
                    continue;
                }
                settings.AddScrubber(StringScrubber.BuildReplaceStrings(pattern));
            }
        }

        if (options.ScrubInlineRemoves != null)
        {
            foreach (var text in options.ScrubInlineRemoves)
            {
                // Allow BuildRemoveText to validate empties and throw as per existing behavior
                settings.AddScrubber(StringScrubber.BuildRemoveText(text));
            }
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
            var extension = Path.GetExtension(fullName);
            
            // When override filename is provided, it's treated as the base name (without extension)
            // and the extension is appended to it before InnerVerifier processes it
            string baseName;
            if (options.OverrideFilename != null)
            {
                baseName = $"{options.OverrideFilename}{extension}";
            }
            else
            {
                baseName = Path.GetFileName(fullName);
            }
            
            // InnerVerifier adds .received and .verified to the base name
            var receivedPath = Path.Combine(directory, $"{baseName}.received.json");
            var verifiedPath = Path.Combine(directory, $"{baseName}.verified.json");
            
            Console.WriteLine($"Received path: {receivedPath}");
            Console.WriteLine($"Verified path: {verifiedPath}");
        }
        
        try
        {
            await Verifier.VerifyFile(fullName, settings, file.FullName, options.OverrideFilename);
            
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