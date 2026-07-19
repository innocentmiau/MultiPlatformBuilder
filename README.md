# Multi-Platform Builder

Editor-only Unity tool that builds the selected platforms in sequence into a
versioned, organized folder structure under `Builds/`, with **one folder per
platform**.

- Platform checkboxes that only enable platforms whose build module is installed
- Customizable folder-name templates with `%VERSION%`, `%PLATFORM%` and `%PRODUCT%` placeholders
- Per-build results with total size and duration
- Settings saved per project

## Requirements

- Unity **6.3** (`6000.3`) or newer
- Editor-only: no runtime code or dependencies

## Installation (Git URL)

In Unity, open **Window > Package Manager**, click **+ > Add package from git URL**,
and paste:

```
https://github.com/innocentmiau/MultiPlatformBuilder.git
```

To pin a specific release, append the tag:

```
https://github.com/innocentmiau/MultiPlatformBuilder.git#v1.0.0
```

Alternatively, add it directly to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.andreleandrodev.multiplatformbuilder": "https://github.com/innocentmiau/MultiPlatformBuilder.git#v1.0.0"
  }
}
```

## Usage

1. Open **Tools > Build > Multi-Platform Builder**.
2. Check the platforms you want to build.
3. Adjust the folder and executable name templates if needed, using the
   `%VERSION%`, `%PLATFORM%` and `%PRODUCT%` placeholders. The preview shows the
   resolved output path for each selected platform.
4. Click **Build Selected** and wait for the builds to finish. Each result shows
   status, size and duration, with a **Reveal** button to open the output folder.

## License

[MIT](LICENSE.md) (c) Andre Leandro
