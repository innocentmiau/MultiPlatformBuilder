using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace MultiPlatformBuilder
{
    public static class BuildRunner
    {

        // IsBuildTargetSupported is public API in Unity 6, so the reflection fallback is not needed.
        public static bool IsSupported(BuildPlatformDefinition platform) =>
            BuildPipeline.IsBuildTargetSupported(platform.Group, platform.Target);

        public static string[] EnabledScenePaths() =>
            EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();

        public static BuildResultEntry BuildPlatform(MultiPlatformBuilderSettings settings, BuildPlatformDefinition platform, string outputPath)
        {
            if (settings.CleanPlatformFolder) CleanPlatformFolder(settings, platform, outputPath);

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            BuildPlayerOptions playerOptions = new BuildPlayerOptions
            {
                scenes = EnabledScenePaths(),
                locationPathName = outputPath,
                target = platform.Target,
                targetGroup = platform.Group,
                options = settings.DevelopmentBuild ? BuildOptions.Development : BuildOptions.None
            };

            // The active build target is intentionally left untouched to avoid a domain reload
            // between builds. Caveat: scripting defines are evaluated for the ACTIVE target, so
            // platform specific #if code may not match the target actually being built here.
            try
            {
                BuildReport report = BuildPipeline.BuildPlayer(playerOptions);
                BuildSummary summary = report.summary;
                bool succeeded = summary.result == BuildResult.Succeeded;
                string message = succeeded ? string.Empty : $"{summary.totalErrors} error(s)";
                return new BuildResultEntry(platform.FriendlyName, succeeded, outputPath, summary.totalSize, summary.totalTime, message);
            }
            catch (Exception exception)
            {
                return new BuildResultEntry(platform.FriendlyName, false, outputPath, 0UL, TimeSpan.Zero, exception.Message);
            }
        }

        // Deletes only this platform's own output folder so other platforms survive a partial rebuild.
        private static void CleanPlatformFolder(MultiPlatformBuilderSettings settings, BuildPlatformDefinition platform, string outputPath)
        {
            string platformFolder = Path.GetDirectoryName(outputPath)?.Replace('\\', '/');
            if (string.IsNullOrEmpty(platformFolder) || !Directory.Exists(platformFolder)) return;

            BuildTemplateContext context = BuildTemplateContext.FromPlayerSettings(platform.FriendlyName);
            string rootPath = BuildPathResolver.ResolveRootPath(settings);
            string parentPath = BuildPathResolver.ResolveParentFolderPath(settings, context);

            // Never delete the builds root or the shared parent folder, e.g. when a template resolves to empty.
            bool sharedFolder = platformFolder == rootPath || platformFolder == parentPath || !platformFolder.StartsWith(rootPath);
            if (sharedFolder) return;

            Directory.Delete(platformFolder, true);
        }
    }
}
