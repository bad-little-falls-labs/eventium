# DevOps & CI/CD Roadmap

This document outlines the plan for introducing DevOps practices, CI/CD pipelines, and pre-commit automation for the Eventium project.

## Overview

| Phase | Focus                      | Timeline |
| ----- | -------------------------- | -------- |
| 1     | Local Developer Experience | Week 1   |
| 2     | GitHub Actions CI          | Week 1-2 |
| 3     | Quality Gates              | Week 2-3 |
| 4     | Release Automation         | Week 3-4 |
| 5     | Advanced Practices         | Ongoing  |

---

## Phase 1: Local Developer Experience

### 1.1 Pre-commit Hooks

Set up Git hooks to catch issues before code reaches the repository.

**Tools:**

- Husky.Net - Git hooks for .NET projects
- Or shell-based hooks in `.git/hooks/`

**Pre-commit checks:**

```sh
# .husky/pre-commit
#!/bin/sh
dotnet format --verify-no-changes
dotnet build --no-restore
dotnet test --no-build --verbosity quiet
```

**Checks to run:**

- [ ] Code formatting verification (`dotnet format --verify-no-changes`)
- [ ] Build succeeds (`dotnet build`)
- [ ] All tests pass (`dotnet test`)
- [ ] No compiler warnings (`dotnet build -warnaserror`)

### 1.2 EditorConfig

Create `.editorconfig` for consistent code style across IDEs:

- Indentation (spaces vs tabs)
- Line endings
- Charset
- C# specific rules (naming conventions, using directives)

### 1.3 Directory.Build.props

Centralize build configuration:

```xml
<Project>
  <PropertyGroup>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <AnalysisLevel>latest-recommended</AnalysisLevel>
  </PropertyGroup>
</Project>
```

### 1.4 Development Scripts

Create helper scripts in `/scripts` or use a `Makefile`:

- `build.sh` / `build.ps1`
- `test.sh` / `test.ps1`
- `format.sh` / `format.ps1`

---

## Phase 2: GitHub Actions CI

### 2.1 Basic CI Pipeline

**File:** `.github/workflows/ci.yml`

```yaml
name: CI

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "9.0.x"
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal
```

### 2.2 Matrix Builds

Test across multiple platforms and .NET versions:

```yaml
strategy:
  matrix:
    os: [ubuntu-latest, windows-latest, macos-latest]
    dotnet-version: ["8.0.x", "9.0.x"]
```

### 2.3 Caching

Speed up builds with dependency caching:

```yaml
- uses: actions/cache@v4
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
```

---

## Phase 3: Quality Gates

### 3.1 Code Coverage

**Tools:**

- Coverlet (already included with xUnit template)
- ReportGenerator for HTML reports
- Codecov or Coveralls for tracking

**CI Step:**

```yaml
- name: Test with Coverage
  run: dotnet test --collect:"XPlat Code Coverage"
- name: Upload Coverage
  uses: codecov/codecov-action@v4
```

**Coverage Targets:**

| Component     | Minimum | Target |
| ------------- | ------- | ------ |
| Eventium.Core | 70%     | 85%    |
| Overall       | 60%     | 80%    |

### 3.2 Static Analysis

**Tools:**

- .NET Analyzers (built-in)
- StyleCop.Analyzers
- SonarCloud (optional, free for open source)

**Configuration:**

```xml
<PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
```

### 3.3 Security Scanning

**Tools:**

- `dotnet list package --vulnerable` - Check for vulnerable dependencies
- GitHub Dependabot - Automated dependency updates
- CodeQL - Static application security testing (SAST)

**Dependabot Config:** `.github/dependabot.yml`

```yaml
version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "weekly"
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "weekly"
```

### 3.4 PR Checks

Enforce quality on pull requests:

- [ ] All CI checks must pass
- [ ] Code coverage must not decrease
- [ ] No new security vulnerabilities
- [ ] At least one approval required

---

## Phase 4: Release Automation

### 4.1 Versioning Strategy

**Semantic Versioning:** `MAJOR.MINOR.PATCH`

- MAJOR: Breaking changes
- MINOR: New features (backward compatible)
- PATCH: Bug fixes

**Tools:**

- MinVer or GitVersion for automatic versioning from Git tags

### 4.2 NuGet Publishing

**File:** `.github/workflows/release.yml`

```yaml
name: Release

on:
  push:
    tags:
      - "v*"

jobs:
  publish:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "9.0.x"
      - name: Pack
        run: dotnet pack -c Release -o ./artifacts
      - name: Publish to NuGet
        run: dotnet nuget push ./artifacts/*.nupkg -k ${{ secrets.NUGET_API_KEY }} -s https://api.nuget.org/v3/index.json
```

### 4.3 GitHub Releases

Automatically create releases with changelogs:

```yaml
- name: Create Release
  uses: softprops/action-gh-release@v1
  with:
    files: ./artifacts/*.nupkg
    generate_release_notes: true
```

### 4.4 Changelog Generation

**Tools:**

- Conventional Commits for commit messages
- Auto-generate CHANGELOG.md from commits

**Commit Format:**

```text
type(scope): description # e.g., feat(auth): add password reset

feat: new feature
fix: bug fix
docs: documentation
test: tests
refactor: code refactoring
chore: maintenance
```

---

## Phase 5: Advanced Practices

### 5.1 Branch Protection Rules

Configure on GitHub:

- Require status checks before merging
- Require branches to be up to date
- Require signed commits (optional)
- Restrict who can push to main

### 5.2 Environment-based Deployments

For future web/API scenarios:

- Development → Staging → Production
- Manual approval gates for production
- Rollback capabilities

### 5.3 Performance Benchmarks

**Status:** ✅ **Complete**

**Implementation:**

- ✅ BenchmarkDotNet 0.14.0 integrated
- ✅ EventQueue benchmarks (Enqueue, Dequeue, combined operations)
- ✅ SimulationEngine benchmarks (various scenarios)
- ✅ World benchmarks (entity/component operations)
- ✅ Memory diagnostics enabled ([MemoryDiagnoser])
- ✅ Parameterized tests (100, 1000, 10000 scale)
- ✅ GitHub Actions workflow for automated benchmark runs
- ✅ PR comments with benchmark results

**Baseline Performance (Apple M1 Max):**

| Component  | Scale  | Operation       | Mean Time | Allocation |
| ---------- | ------ | --------------- | --------- | ---------- |
| EventQueue | 100    | Enqueue         | 840 ns    | 6.09 KB    |
| EventQueue | 100    | Enqueue+Dequeue | 2.96 µs   | 6.09 KB    |
| EventQueue | 1,000  | Enqueue         | 8.61 µs   | 48.16 KB   |
| EventQueue | 1,000  | Enqueue+Dequeue | 74.4 µs   | 48.16 KB   |
| EventQueue | 10,000 | Enqueue         | 273 µs    | 768 KB     |
| EventQueue | 10,000 | Enqueue+Dequeue | 1.41 ms   | 768 KB     |

**Files:** `benchmarks/Eventium.Benchmarks/`, `.github/workflows/benchmarks.yml`

**Usage:**

```bash
cd benchmarks/Eventium.Benchmarks
dotnet run -c Release
```

### 5.4 Documentation Generation

**Tools:**

- DocFX for API documentation
- GitHub Pages for hosting

### 5.5 Container Support (Future)

If needed for deployment:

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Eventium.dll"]
```

---

## Implementation Checklist

### Immediate (This Sprint)

- [x] Create `.editorconfig`
- [x] Create `Directory.Build.props`
- [x] Set up basic GitHub Actions CI workflow
- [x] Configure Dependabot

### Short-term (Next 2 Weeks)

- [x] Add pre-commit hooks with Husky.Net
- [x] Configure code coverage reporting
- [x] Add static analysis (StyleCop)
- [x] Set up branch protection rules

### Medium-term (Next Month)

- [x] Implement release workflow
- [x] Set up NuGet publishing
- [x] Add security scanning (CodeQL)
- [x] Create contribution guidelines (CONTRIBUTING.md)

### Long-term (Ongoing)

- [ ] Performance benchmarking
- [ ] API documentation generation
- [ ] Expand to multi-platform testing

---

## Repository Structure (Target)

```text
eventium/
├── .editorconfig
├── .gitignore
├── .github/
│   ├── dependabot.yml
│   ├── CODEOWNERS
│   └── workflows/
│       ├── ci.yml
│       ├── release.yml
│       └── codeql.yml
├── Directory.Build.props
├── Eventium.sln
├── src/
│   ├── Eventium.Core/
│   └── Eventium.Scenarios/
├── tests/
│   └── Eventium.Core.Tests/
├── benchmarks/
│   └── Eventium.Benchmarks/
├── docs/
├── scripts/
│   ├── build.sh
│   └── test.sh
├── CHANGELOG.md
├── CONTRIBUTING.md
├── DESIGN_CONSIDERATIONS.md
├── DEVOPS_ROADMAP.md
├── LICENSE
├── README.md
└── ROADMAP.md
```

---

## Success Metrics

| Metric                    | Current | Target           |
| ------------------------- | ------- | ---------------- |
| CI Build Time             | N/A     | < 5 min          |
| Test Coverage             | ~0%     | > 80%            |
| Time to First PR Feedback | Manual  | < 10 min         |
| Dependency Freshness      | Manual  | Automated weekly |
| Security Vulnerabilities  | Unknown | 0 critical/high  |

---

## References

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET CI/CD Best Practices](https://docs.microsoft.com/en-us/dotnet/devops/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [Husky.Net](https://alirezanet.github.io/Husky.Net/)
