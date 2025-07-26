namespace Verify.Cli;

public record VerifyFileOptions(
    DirectoryInfo? VerifiedDir = null,
    string? ScrubInlineDatetime = null,
    string? ScrubInlinePattern = null,
    string? ScrubInlineRemove = null);

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
        
        await Verifier.VerifyFile(fullName, settings, file.FullName);
    }
}