# Changelog

All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-07-11

### Added

- Initial release.
- Editor window (**Tools > Build > Multi-Platform Builder**) that builds selected platforms in sequence.
- Platform checkboxes limited to installed build modules (Windows, Linux, macOS).
- Versioned, organized output structure under `Builds/` with per-platform folders.
- Folder and executable name templates with `%VERSION%`, `%PLATFORM%` and `%PRODUCT%` placeholders.
- Per-build results with total size and duration, plus a Reveal button.
- Version bump helper for the last version component.
- Settings saved per project in `ProjectSettings/MultiPlatformBuilder.asset`.
