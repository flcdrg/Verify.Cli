namespace Verify.Cli;

public static partial class Verifier
{
    static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile) =>
        new(
            sourceFile,
            settings
        );

    static SettingsTask Verify(
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