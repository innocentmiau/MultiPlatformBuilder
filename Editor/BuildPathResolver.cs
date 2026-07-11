using System.IO;
using UnityEngine;

namespace MultiPlatformBuilder
{
    public static class BuildPathResolver
    {

        public const string VERSION_TOKEN = "%VERSION%";
        public const string PLATFORM_TOKEN = "%PLATFORM%";
        public const string PRODUCT_TOKEN = "%PRODUCT%";

        public static string ProjectRoot => Path.GetDirectoryName(Application.dataPath).Replace('\\', '/');

        public static string ResolveOutputPath(MultiPlatformBuilderSettings settings, BuildPlatformDefinition platform)
        {
            BuildTemplateContext context = BuildTemplateContext.FromPlayerSettings(platform.FriendlyName);
            string platformFolder = SanitizeSegment(ResolveTemplate(settings.PlatformFolderTemplate, context));
            string executableName = SanitizeSegment(ResolveTemplate(settings.ExecutableNameTemplate, context)) + platform.Extension;
            return Path.GetFullPath(Path.Combine(ResolveParentFolderPath(settings, context), platformFolder, executableName)).Replace('\\', '/');
        }

        public static string ResolveRootPath(MultiPlatformBuilderSettings settings)
        {
            string root = settings.RootIsRelative ? Path.Combine(ProjectRoot, settings.BuildsRoot) : settings.BuildsRoot;
            return Path.GetFullPath(root).Replace('\\', '/');
        }

        public static string ResolveParentFolderPath(MultiPlatformBuilderSettings settings, BuildTemplateContext context)
        {
            string parentFolder = SanitizeSegment(ResolveTemplate(settings.ParentFolderTemplate, context));
            return Path.GetFullPath(Path.Combine(ResolveRootPath(settings), parentFolder)).Replace('\\', '/');
        }

        public static string ResolveTemplate(string template, BuildTemplateContext context) =>
            (template ?? string.Empty)
                .Replace(VERSION_TOKEN, context.Version)
                .Replace(PLATFORM_TOKEN, context.PlatformToken)
                .Replace(PRODUCT_TOKEN, context.Product);

        public static string SanitizeSegment(string segment)
        {
            foreach (char invalid in Path.GetInvalidFileNameChars())
                segment = segment.Replace(invalid, '_');

            return segment;
        }
    }
}
