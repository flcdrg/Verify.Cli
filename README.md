# Verify.Cli

[![.NET](https://github.com/flcdrg/Verify.Cli/actions/workflows/dotnet.yml/badge.svg)](https://github.com/flcdrg/Verify.Cli/actions/workflows/dotnet.yml)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Cli.svg?label=Verify.Cli)](https://www.nuget.org/packages/Verify.Cli/)
[![Docker Image Version](https://img.shields.io/docker/v/flcdrg/verify-cli?sort=semver&arch=amd64&label=Docker)](https://hub.docker.com/r/flcdrg/verify-cli)

A command-line tool that uses the [Verify](https://github.com/VerifyTests/Verify) library for regular files (without requiring you to create a unit test project).

## Installation

### As a .NET global tool

Install as .NET global tool

```bash
dotnet tool install -g Verify.Cli
```

Once installed, invoke directly via the `verify` command.

### Docker image

An image is published to Docker Hub. Ensure that you create a volume mapping (via the `-v` parameter) so that the container can access the files you wish to verify.

Bash:

```bash
docker run --rm -v $PWD:/tmp flcdrg/verify-cli --file /tmp/examples/same.json
```

PowerShell:

```pwsh
docker run --rm -v ${pwd}:/tmp verify-cli:latest --file /tmp/examples/same.json
```

## Usage

If you run this tool interactively, then you'll get a similar experience that you would using VerifyTests in a .NET unit test project. Verify.Cli will utilise any existing compatible file diff tool to display the diff.

```bash
verify --file <path to file to verify> [--verified-dir <directory>]
```

The first time you use Verify.Cli, it will output the contents of the file.

### Options

- `--file` or `-f`: The file to verify (required)
- `--verified-dir` or `-d`: Directory to store/look for .verified files (optional)
- `--scrub-inline-datetime`: Format for inline date times to scrub, e.g., 'yyyy-MM-ddTHH:mm:ss.fffZ' (optional)
- `--scrub-inline-pattern`: One or more regex patterns to match inline strings for scrubbing. Repeat the option to specify multiple patterns. For example: '"/astro/[^"]+"' or '(?&lt;prefix&gt;")/_astro/[^"]+(?&lt;suffix&gt;")' (optional)
- `--scrub-inline-remove`: One or more text values to remove from the file content (exact match, not regex). Repeat the option to specify multiple values, e.g., 'temp-id-123' (optional)
- `--verbosity`: Set the verbosity level. Options are: quiet, minimal, normal, detailed, diagnostic (optional, default: normal)

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

With date time scrubbing:

```pwsh
verify --file C:\tmp\log.txt --scrub-inline-datetime "yyyy-MM-ddTHH:mm:ss.fffZ"
```

This will scrub inline date times matching the specified format from the file content before verification.

With pattern scrubbing:

```pwsh
verify --file C:\tmp\output.html --scrub-inline-pattern "\"/astro/[^\"]+\""
```

This will scrub inline strings matching the regex pattern (e.g., dynamic asset paths like "/astro/chunk-123.js") from the file content before verification.

With multiple pattern scrubbers (repeat the option):

```pwsh
verify --file C:\tmp\output.html --scrub-inline-pattern "\"/astro/[^\"]+\"" --scrub-inline-pattern "(?<prefix>\")/_astro/[^\"]+(?<suffix>\")"
```

This applies both patterns in order.

With pattern scrubbing using named groups to preserve parts:

```pwsh
verify --file C:\tmp\output.html --scrub-inline-pattern "(?<prefix>\")/_astro/[^\"]+(?<suffix>\")"
```

This will replace the dynamic part while preserving the prefix and suffix (e.g., "/_astro/chunk-123.js" becomes the preserved prefix and suffix).

With text removal:

```pwsh
verify --file C:\tmp\output.html --scrub-inline-remove "data-temp-id"
```

This will remove all instances of the text "data-temp-id" from the file content before verification.

With multiple text removals (repeat the option):

```pwsh
verify --file C:\tmp\output.html --scrub-inline-remove "data-temp-id" --scrub-inline-remove "temp-session"
```

This removes both values.

With verbosity control:

```pwsh
verify --file C:\tmp\example.txt --verbosity quiet
```

This will run with minimal output (quiet mode). Available verbosity levels are:

- `quiet`: Minimal output, only errors and critical information
- `minimal`: Basic output with essential information
- `normal`: Standard output (default)
- `detailed`: More detailed output including additional information
- `diagnostic`: Full diagnostic output for troubleshooting

You can combine options:

```pwsh
verify --file C:\tmp\log.txt --verified-dir C:\MyVerifiedFiles --scrub-inline-datetime "yyyy-MM-dd HH:mm:ss" --scrub-inline-pattern "id=\"[^\"]+\"" --scrub-inline-pattern "data-asset=\"[^\"]+\"" --scrub-inline-remove "temp-session" --verbosity detailed
```

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
