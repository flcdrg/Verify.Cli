# Instructions

This is a .NET Tool application based on [Verify](https://github.com/VerifyTests/Verify), via the NuGet package [Verify](https://www.nuget.org/packages/Verify).

Avoid making things up and do not hallucinate.

It requires the latest .NET 9 SDK to build and run.

The main application is located in `Verify.Cli`, which is a .NET 9 console application. It uses the [`System.CommandLine` library](https://learn.microsoft.com/en-us/dotnet/standard/commandline/?WT.mc_id=DOP-MVP-5001655) to parse command-line arguments and options.

Integration tests are located in `Verify.Cli.Tests`, which is a .NET 9 test project.

Other additional tests to validate both the Verify library and the CLI tool are located in `Verify.Other.Tests`, which is a .NET 9 test project.

All Markdown files should comply with Markdownlint rules.

All parameters for the command-line tool should be documented in the `README.md` file, including examples of usage.
