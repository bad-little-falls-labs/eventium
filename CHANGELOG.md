# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

- (no unreleased changes yet)

## [0.2.0] - 2025-12-30 (Baseline)

### Added

- SimulationRunner with pause/resume/step/seek and real-time pacing
- SnapshotBuffer with arbitrary-time seek (restore nearest snapshot, replay forward)
- MemoryPack-based cloning for snapshots; faster than JSON
- Performance benchmarking infrastructure with BenchmarkDotNet
- DocFX API docs scaffolding and GitHub Pages deployment
- Issue and PR templates, CODEOWNERS, CONTRIBUTING, SECURITY policy

### Changed

- Enabled XML documentation file generation for public APIs
- Real-time pacing honors TimeScale changes frame-by-frame
- Pause/resume no longer causes time jumps; wall-clock deltas ignored while paused

### Fixed

- Seek comparisons now use domain tolerance (1e-9) instead of double.Epsilon
