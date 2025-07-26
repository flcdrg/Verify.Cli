using Verify.Cli;
using Verify.Other.Tests.examples;
using Verifier = VerifyXunit.Verifier;

namespace Verify.Other.Tests;

public class Tests
{
    [Fact]
    public Task TestDotNetPocoClass()
    {
        var person = ClassBeingTested.FindPerson();
        return Verifier.Verify(person);
    }

    [Fact]
    public Task TestFile()
    {
        return VerifyFile("data.json");
    }

    [Fact]
    public Task TestJson()
    {
        return VerifyJson(File.ReadAllTextAsync("data.json", TestContext.Current.CancellationToken));
    }

    [Fact]
    public Task TestJson2()
    {
        // begin-snippet: verify-csharp-json
        var json = new
        {
            id = Guid.NewGuid(),
            time = DateTimeOffset.Now,
            name = "David Gardiner",
            currentUser = Environment.UserName
        };

        var text = System.Text.Json.JsonSerializer.Serialize(json);

        var settings = new VerifySettings();
        settings.ScrubUserName();

        // Ensure received and verified files have .json suffix (otherwise it will be .txt)
        settings.UseStrictJson();

        return VerifyJson(text, settings);
        // end-snippet
    }

    [Fact]
    public Task FileWithScrubber()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(StringScrubber.BuildReplaceStrings("(?<prefix>\")/_astro/[^\"]+(?<suffix>\")"));

        return VerifyFile("azure-pipeline-template-expression.html", settings);
    }

    [Fact]
    public Task FileWithRemoveScrubber()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(StringScrubber.BuildRemoveText("temp-123"));

        return VerifyJson("""
        {
          "name": "test",
          "id": "temp-123",
          "value": 42
        }
        """, settings);
    }
}
