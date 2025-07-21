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

        return VerifyJson(text, settings);
        // end-snippet
    }
}
