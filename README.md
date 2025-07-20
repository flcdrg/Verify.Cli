# Verify.Cli

[![.NET](https://github.com/flcdrg/Verify.Cli/actions/workflows/dotnet.yml/badge.svg)](https://github.com/flcdrg/Verify.Cli/actions/workflows/dotnet.yml)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Cli.svg?label=Verify.Cli)](https://www.nuget.org/packages/Verify.Cli/)

A command-line tool that uses the [Verify](https://github.com/VerifyTests/Verify) library for regular files (without requiring you to create a unit test project).

## Installation

Install as .NET global tool

```bash
dotnet tool install -g Verify.Cli
```

Once installed, invoke directly via the `verify` command.

## Usage

If you run this tool interactively, then you'll get a similar experience that you would using VerifyTests in a .NET unit test project. Verify.Cli will utilise any existing compatible file diff tool to display the diff.

```bash
verify --file <path to file to verify> [--verified-dir <directory>]
```

The first time you use Verify.Cli, it will output the contents of the file.

### Options

- `--file` or `-f`: The file to verify (required)
- `--verified-dir` or `-d`: Directory to store/look for .verified files (optional)

### Examples

Basic usage:

```pwsh
verify --file C:\tmp\example.txt
```

With custom verified files directory:

```pwsh
verify --file C:\tmp\example.txt --verified-dir C:\MyVerifiedFiles
```

This will look for the verified file at `C:\MyVerifiedFiles\example.txt.verified.txt` instead of the default location next to the source file.

When the files match, the tool exits with code 0 and produces no output.

```text
Unhandled exception: VerifyException: Directory: C:\tmp
New:
  - Received: example.txt.received.txt
    Verified: example.txt.verified.txt

FileContent:

New:

Received: example.txt.received.txt
This is
a text file.
```

Your diff tool of choice (if found by the [Verify's DiffEngine library](https://github.com/VerifyTests/DiffEngine#supported-tools)) can then be used to compare to the verified file (if it exists), or create it (if the first time).

If the verified file matches the received file, then there is no output (and the exit code is zero).

If the received file is different from the verified file, then a diff will be shown in the console, a non-zero exit code will be returned, and if in an interactive environment with a supported diff tool, then that tool will be launched.

```text
Unhandled exception: VerifyException: Directory: C:\tmp
NotEqual:
  - Received: example.txt.received.txt
    Verified: example.txt.verified.txt

FileContent:

NotEqual:

Received: example.txt.received.txt
This is
an updated text file.

Verified: example.txt.verified.txt
This is
a text file.
```

Example with Beyond Compare launched:

<!-- Use full URL so image is rendered on nuget.org package page too -->
![Screenshot of using Beyond Compare to see differences in files](https://raw.githubusercontent.com/flcdrg/Verify.Cli/refs/heads/main/beyond-compare.png)
