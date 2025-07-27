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

    [Fact]
    public async Task FileWithInlineRemove_RemovesCorrectly()
    {
        // Arrange
        var file = new FileInfo("examples/withRemovableIds.html");

        // Simple text to remove (not regex) - removes all instances
        var options = new VerifyFileOptions(ScrubInlineRemove: " data-temp-id");
        
        // Act & Assert
        await FileVerifier.VerifyFileAsync(file, options);
    }

    [Fact]
    public async Task FileWithInlineRemove_EmptyPattern_ThrowsException()
    {
        // Arrange
        var file = new FileInfo("examples/same.json");
        var options = new VerifyFileOptions(ScrubInlineRemove: "");
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await FileVerifier.VerifyFileAsync(file, options));
    }

    [Fact]
    public async Task FileWithInlineRemove_NullPattern_DoesNothing()
    {
        // Arrange
        var file = new FileInfo("examples/same.json");
        var options = new VerifyFileOptions(ScrubInlineRemove: null);
        
        // Act & Assert - Should not throw
        await FileVerifier.VerifyFileAsync(file, options);
    }

    [Fact]
    public async Task VerifyFileAsync_NormalVerbosity_ProducesNoOutput()
    {
        // Arrange
        var file = new FileInfo("examples/same.json");
        var options = new VerifyFileOptions(Verbosity: Verbosity.Normal);
        
        // Act & Assert - Should not throw
        await FileVerifier.VerifyFileAsync(file, options);
    }

    [Fact]
    public async Task VerifyFileAsync_DetailedVerbosity_ProducesOutput()
    {
        // Arrange
        var file = new FileInfo("examples/same.json");
        var options = new VerifyFileOptions(Verbosity: Verbosity.Detailed);
        
        // Capture console output
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        try
        {
            // Act
            await FileVerifier.VerifyFileAsync(file, options);
            
            // Assert
            var output = stringWriter.ToString();
            Assert.Contains("Source file path:", output);
            Assert.Contains("Received path:", output);
            Assert.Contains("Verified path:", output);
            Assert.Contains("Files match", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}