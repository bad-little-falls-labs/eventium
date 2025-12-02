# Eventium API Documentation

This folder hosts the DocFX configuration to generate API documentation for Eventium.

## Prerequisites

- .NET 9 SDK
- DocFX (install via `dotnet tool install -g docfx` or see <https://dotnet.github.io/docfx/>)

## Generate API Metadata

```bash
cd /Users/jimcorrell/Development/neho/eventium/docs
# Generate API YAML from projects
docfx metadata
```

## Build the Site

```bash
# Build static site into `docs/_site`
docfx build
```

## Preview Locally

```bash
# Serve the generated site on http://localhost:8080
docfx serve _site
```

## Structure

- `docfx.json` — DocFX configuration
- `api/` — Generated API YAML (do not edit manually)
- `articles/` — Hand-authored guides and overviews
- `_site/` — Generated static website

## Notes

- If `docfx` is not found, ensure your PATH includes the dotnet tools: `export PATH="$HOME/.dotnet/tools:$PATH"`.
- API documentation is generated from XML comments in the source. Ensure public members have XML docs.
