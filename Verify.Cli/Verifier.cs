namespace Verify.Cli;

public static partial class Verifier
{
    public static SettingsTask VerifyFile(
        string path,
        VerifySettings settings,
        string sourceFile) =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path, null));

    private static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile) =>
        new(
            sourceFile,
            settings
        );

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