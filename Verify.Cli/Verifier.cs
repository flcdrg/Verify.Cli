namespace Verify.Cli;

public static partial class Verifier
{
    /// <summary>
    /// Verifies the contents of <param name="path"/>.
    /// </summary>
    public static SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        object? info = null,
        string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path, info));

}