# Contributing to Eventium

Thanks for your interest in contributing!

## Development Workflow

1. Fork and create a feature branch
2. Run pre-commit hooks locally
3. Add tests and ensure coverage does not decrease
4. Submit a PR with a clear description

## Commit Style

Use Conventional Commits:

- `feat(scope): description`
- `fix(scope): description`
- `docs(scope): description`
- `test(scope): description`
- `chore(scope): description`

## Pull Request Checks

- CI passes (build, tests)
- Code coverage maintained or improved
- No new security vulnerabilities
- At least one approval required

## Local Setup

```bash
# Restore tools and install husky
dotnet tool restore
dotnet husky install
# Build and test
dotnet build
dotnet test
```

## Code Style

- Enforced via `.editorconfig`
- Use file-scoped namespaces
- XML docs required on public APIs

## Release Process

- Semantic versioning (`MAJOR.MINOR.PATCH`)
- Automated NuGet publish on tag push

## Questions

Open an issue or start a discussion.
