namespace Verify.Cli;

public static partial class Verifier
{
    public static SettingsTask VerifyFile(
        string path,
        VerifySettings settings,
        string sourceFile,
        string? overrideFilename = null) =>
        Verify(settings, sourceFile, overrideFilename, _ => _.VerifyFile(path, null, null));

    private static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile, string? overrideFilename = null)
    {
        // Create a copy of settings without the UseDirectory setting for InnerVerifier
        var modifiedSettings = new VerifySettings(settings);
        var customDirectory = settings.Directory;
        
        // Clear the Directory setting to avoid the InnerVerifier exception
        if (customDirectory != null)
        {
            // Reset the directory in the copied settings
            typeof(VerifySettings).GetProperty("Directory")?.SetValue(modifiedSettings, null);
        }
        
        var sourceFileDirectory = Path.GetDirectoryName(sourceFile) ?? throw new InvalidOperationException("Got null for directory of sourceFile");
        
        // If we have a custom directory, use it; otherwise use the source file directory
        var directory = customDirectory ?? sourceFileDirectory;
        
        // Use override filename if provided, otherwise use the source filename with extension
        string filename;
        if (overrideFilename != null)
        {
            var sourceExtension = Path.GetExtension(sourceFile);
            filename = $"{overrideFilename}{sourceExtension}";
        }
        else
        {
            filename = Path.GetFileName(sourceFile);
        }
        
        return new(directory, filename, modifiedSettings);
    }
        

    private static SettingsTask Verify(
        VerifySettings? settings,
        string sourceFile,
        string? overrideFilename,
        Func<InnerVerifier, Task<VerifyResult>> verify)
    {
        return new(
            settings,
            async verifySettings =>
            {
                using var verifier = GetVerifier(verifySettings, sourceFile, overrideFilename);
                return await verify(verifier);
            });
    }
}