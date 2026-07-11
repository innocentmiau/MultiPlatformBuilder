# Multi-Platform Builder

Multi-Platform Builder is an editor-only tool that builds the selected platforms
in sequence into a versioned, organized folder structure. Each build lands in its
own per-platform folder under a shared version folder, for example:

```
Builds/
  Build_v0_1_5/
    Build_v0_1_5_Windows/
      MyGame.exe
    Build_v0_1_5_Linux/
      MyGame.x86_64
```

## How it works

The window lists the standalone platforms (Windows, Linux, macOS) and only
enables the ones whose build module is installed. Selected platforms are built
one after another with the scenes enabled in Build Settings. The output paths
are resolved from three templates (parent folder, platform folder, executable
name) that support the `%VERSION%`, `%PLATFORM%` and `%PRODUCT%` placeholders.
Settings are saved per project in `ProjectSettings/MultiPlatformBuilder.asset`.

## Using Multi-Platform Builder

1. Open the window from **Tools > Build > Multi-Platform Builder**.
2. Check the platforms you want to build.
3. (Optional) Change the builds root, or adjust the folder and executable name
   templates. The preview section shows the resolved output path per platform.
4. (Optional) Toggle the options: development build, clean the platform folder
   before building, abort the sequence on the first failure, reveal the output
   folder when done.
5. (Optional) Use the version section to bump the last component of the app
   version (for example `0.1.5` to `0.1.6`).
6. Click **Build Selected**.

## Reading the results

Each result row shows the platform, success or failure, the total build size and
the duration, plus the resolved output path. The **Reveal** button opens that
build's folder in the file browser.

Note: the active build target is left untouched between builds to avoid domain
reloads, so platform specific `#if` code is compiled for the active target, not
the target being built.
