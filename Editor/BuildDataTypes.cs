using System;
using UnityEditor;

namespace BuildTools
{
    public class BuildPlatformDefinition
    {

        private readonly BuildTarget _target;
        private readonly BuildTargetGroup _group;
        private readonly string _friendlyName;
        private readonly string _extension;

        public BuildTarget Target => _target;
        public BuildTargetGroup Group => _group;
        public string FriendlyName => _friendlyName;
        public string Extension => _extension;

        public BuildPlatformDefinition(BuildTarget target, BuildTargetGroup group, string friendlyName, string extension)
        {
            _target = target;
            _group = group;
            _friendlyName = friendlyName;
            _extension = extension;
        }
    }

    public class BuildTemplateContext
    {

        private readonly string _version;
        private readonly string _product;
        private readonly string _platformToken;

        public string Version => _version;
        public string Product => _product;
        public string PlatformToken => _platformToken;

        public BuildTemplateContext(string version, string product, string platformToken)
        {
            _version = version;
            _product = product;
            _platformToken = platformToken;
        }

        public static BuildTemplateContext FromPlayerSettings(string platformToken) =>
            new BuildTemplateContext(PlayerSettings.bundleVersion.Replace('.', '_'), PlayerSettings.productName, platformToken);
    }

    public class BuildResultEntry
    {

        private readonly string _platformName;
        private readonly bool _succeeded;
        private readonly string _outputPath;
        private readonly ulong _totalSizeBytes;
        private readonly TimeSpan _duration;
        private readonly string _message;

        public string PlatformName => _platformName;
        public bool Succeeded => _succeeded;
        public string OutputPath => _outputPath;
        public ulong TotalSizeBytes => _totalSizeBytes;
        public TimeSpan Duration => _duration;
        public string Message => _message;

        public BuildResultEntry(string platformName, bool succeeded, string outputPath, ulong totalSizeBytes, TimeSpan duration, string message)
        {
            _platformName = platformName;
            _succeeded = succeeded;
            _outputPath = outputPath;
            _totalSizeBytes = totalSizeBytes;
            _duration = duration;
            _message = message;
        }
    }

    public static class BuildPlatformCatalog
    {

        public static readonly BuildPlatformDefinition[] STANDALONE_PLATFORMS = new BuildPlatformDefinition[]
        {
            new BuildPlatformDefinition(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone, "Windows", ".exe"),
            new BuildPlatformDefinition(BuildTarget.StandaloneLinux64, BuildTargetGroup.Standalone, "Linux", ".x86_64"),
            new BuildPlatformDefinition(BuildTarget.StandaloneOSX, BuildTargetGroup.Standalone, "macOS", ".app")
        };
    }
}
