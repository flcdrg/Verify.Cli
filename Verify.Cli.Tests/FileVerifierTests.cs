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
}