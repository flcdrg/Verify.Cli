namespace Verify.Cli.Tests;

public class FileVerifierTests
{
    [Fact]
    public async Task SameFiles_AreEqual()
    {
        // Arrange
        var file = new FileInfo("examples/same.json");
        await FileVerifier.VerifyFileAsync(file, new VerifyFileOptions());
    }

    [Fact]
    public async Task DifferentFiles_AreNotEqual()
    {
        // Arrange
        var file = new FileInfo("examples/different.json");
        await Assert.ThrowsAnyAsync<Exception>(async () => await FileVerifier.VerifyFileAsync(file, new VerifyFileOptions()));
    }

    [Fact]
    public async Task FileWithInlineDateTime_ScrubsCorrectly()
    {
        // Arrange
        var file = new FileInfo("examples/sameWithDates.json");
        var options = new VerifyFileOptions(ScrubInlineDatetime: "yyyy-MM-dd");
        
        // Act & Assert
        await FileVerifier.VerifyFileAsync(file, options);
    }

    [Fact]
    public async Task FileWithInlinePattern_ScrubsCorrectly()
    {
        // Arrange
        var file = new FileInfo("examples/azure-pipeline-template-expression.html");

        // Pattern includes named groups for prefix and suffix
        // This ensures that the replacement string retains the original quotes around the matched pattern
        var options = new VerifyFileOptions(ScrubInlinePattern: "(?<prefix>\")/_astro/[^\"]+(?<suffix>\")");
        
        // Act & Assert
        await FileVerifier.VerifyFileAsync(file, options);
    }
}