namespace Verify.Cli;

public static partial class Verifier
{
    public static SettingsTask VerifyFile(
        string path,
        VerifySettings settings,
        string sourceFile) =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path, null, null));

    private static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile)
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
        
        return new(directory, Path.GetFileName(sourceFile), modifiedSettings);
    }
        

    private static SettingsTask Verify(
        VerifySettings? settings,
        string sourceFile,
        Func<InnerVerifier, Task<VerifyResult>> verify)
    {
        return new(
            settings,
            async verifySettings =>
            {
                using var verifier = GetVerifier(verifySettings, sourceFile);
                return await verify(verifier);
            });
    }
}