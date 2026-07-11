using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BuildTools
{
    public class MultiPlatformBuilderWindow : EditorWindow
    {

        private const float BROWSE_BUTTON_WIDTH = 70f;
        private const float INCREASE_BUTTON_WIDTH = 150f;
        private const float REVEAL_BUTTON_WIDTH = 60f;
        private const float BUILD_BUTTON_HEIGHT = 30f;

        private readonly List<BuildResultEntry> _results = new List<BuildResultEntry>();
        private Vector2 _scrollPosition;

        [MenuItem("Tools/Build/Multi-Platform Builder")]
        private static void Open()
        {
            MultiPlatformBuilderWindow window = GetWindow<MultiPlatformBuilderWindow>("Multi-Platform Builder");
            window.minSize = new Vector2(440f, 480f);
        }

        private void OnGUI()
        {
            MultiPlatformBuilderSettings settings = MultiPlatformBuilderSettings.instance;
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUI.BeginChangeCheck();
            DrawPlatformSection(settings);
            EditorGUILayout.Space();
            DrawOutputSection(settings);
            EditorGUILayout.Space();
            DrawOptionsSection(settings);
            if (EditorGUI.EndChangeCheck()) settings.SaveSettings();

            EditorGUILayout.Space();
            DrawVersionSection();
            EditorGUILayout.Space();
            DrawPreviewSection(settings);
            EditorGUILayout.Space();
            DrawBuildSection(settings);
            EditorGUILayout.Space();
            DrawResultsSection();

            EditorGUILayout.EndScrollView();
        }

        private static void DrawPlatformSection(MultiPlatformBuilderSettings settings)
        {
            EditorGUILayout.LabelField("Platforms", EditorStyles.boldLabel);

            foreach (BuildPlatformDefinition platform in BuildPlatformCatalog.STANDALONE_PLATFORMS)
            {
                bool supported = BuildRunner.IsSupported(platform);
                using (new EditorGUI.DisabledScope(!supported))
                {
                    string label = supported ? platform.FriendlyName : $"{platform.FriendlyName} (module not installed)";
                    bool selected = supported && settings.IsSelected(platform.Target);
                    bool toggled = EditorGUILayout.ToggleLeft(label, selected);
                    if (toggled != selected) settings.SetSelected(platform.Target, toggled);
                }
            }
        }

        private static void DrawOutputSection(MultiPlatformBuilderSettings settings)
        {
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            settings.BuildsRoot = EditorGUILayout.TextField("Builds Root", settings.BuildsRoot);
            if (GUILayout.Button("Browse...", GUILayout.Width(BROWSE_BUTTON_WIDTH))) BrowseForRoot(settings);
            EditorGUILayout.EndHorizontal();

            settings.RootIsRelative = EditorGUILayout.Toggle("Relative To Project", settings.RootIsRelative);
            settings.ParentFolderTemplate = EditorGUILayout.TextField("Parent Folder", settings.ParentFolderTemplate);
            settings.PlatformFolderTemplate = EditorGUILayout.TextField("Platform Folder", settings.PlatformFolderTemplate);
            settings.ExecutableNameTemplate = EditorGUILayout.TextField("Executable Name", settings.ExecutableNameTemplate);
            EditorGUILayout.LabelField("Placeholders: %VERSION%, %PLATFORM%, %PRODUCT%", EditorStyles.miniLabel);
        }

        private static void DrawOptionsSection(MultiPlatformBuilderSettings settings)
        {
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            settings.DevelopmentBuild = EditorGUILayout.Toggle("Development Build", settings.DevelopmentBuild);
            settings.CleanPlatformFolder = EditorGUILayout.Toggle(new GUIContent("Clean Before Build", "Deletes only this platform's output folder before building it, other platform folders are untouched."), settings.CleanPlatformFolder);
            settings.AbortOnFailure = EditorGUILayout.Toggle("Abort On Failure", settings.AbortOnFailure);
            settings.RevealWhenDone = EditorGUILayout.Toggle("Reveal When Done", settings.RevealWhenDone);
        }

        private static void DrawVersionSection()
        {
            EditorGUILayout.LabelField("Version", EditorStyles.boldLabel);

            string currentVersion = PlayerSettings.bundleVersion;
            string bumpedVersion = BuildVersionUtility.BumpLastComponent(currentVersion);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"App Version: {currentVersion}");
            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(bumpedVersion)))
            {
                if (GUILayout.Button($"Increase to {bumpedVersion ?? "?"}", GUILayout.Width(INCREASE_BUTTON_WIDTH)))
                    ApplyBumpedVersion(bumpedVersion);
            }
            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(bumpedVersion))
                EditorGUILayout.HelpBox("The last version part is not a number, edit the version in Player Settings instead.", MessageType.Info);
        }

        private static void ApplyBumpedVersion(string newVersion)
        {
            PlayerSettings.bundleVersion = newVersion;

            // Flushes the dirty PlayerSettings to ProjectSettings.asset right away instead of
            // waiting for the next editor wide save.
            AssetDatabase.SaveAssets();
        }

        private static void DrawPreviewSection(MultiPlatformBuilderSettings settings)
        {
            List<BuildPlatformDefinition> selected = SelectedPlatforms(settings);
            if (selected.Count == 0) return;

            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            foreach (BuildPlatformDefinition platform in selected)
                EditorGUILayout.LabelField(BuildPathResolver.ResolveOutputPath(settings, platform), EditorStyles.miniLabel);
        }

        private void DrawBuildSection(MultiPlatformBuilderSettings settings)
        {
            bool hasScenes = BuildRunner.EnabledScenePaths().Length > 0;
            if (!hasScenes) EditorGUILayout.HelpBox("No enabled scenes in Build Settings. Add scenes before building.", MessageType.Warning);

            List<BuildPlatformDefinition> selected = SelectedPlatforms(settings);
            using (new EditorGUI.DisabledScope(!hasScenes || selected.Count == 0))
            {
                // Deferred so the build does not run inside the OnGUI callstack.
                if (GUILayout.Button("Build Selected", GUILayout.Height(BUILD_BUTTON_HEIGHT)))
                    EditorApplication.delayCall += RunSelectedBuilds;
            }
        }

        private void DrawResultsSection()
        {
            if (_results.Count == 0) return;

            EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);

            foreach (BuildResultEntry entry in _results)
            {
                EditorGUILayout.BeginHorizontal();
                string status = entry.Succeeded ? "Success" : $"Failed ({entry.Message})";
                EditorGUILayout.LabelField($"{entry.PlatformName}: {status} | {FormatSize(entry.TotalSizeBytes)} | {entry.Duration.TotalSeconds:F1}s");
                if (GUILayout.Button("Reveal", GUILayout.Width(REVEAL_BUTTON_WIDTH))) EditorUtility.RevealInFinder(entry.OutputPath);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField(entry.OutputPath, EditorStyles.miniLabel);
            }
        }

        private void RunSelectedBuilds()
        {
            MultiPlatformBuilderSettings settings = MultiPlatformBuilderSettings.instance;
            List<BuildPlatformDefinition> selected = SelectedPlatforms(settings);
            if (selected.Count == 0) return;

            _results.Clear();

            try
            {
                for (int i = 0; i < selected.Count; i++)
                {
                    BuildPlatformDefinition platform = selected[i];
                    EditorUtility.DisplayProgressBar("Multi-Platform Builder", $"Building {platform.FriendlyName} ({i + 1}/{selected.Count})", (float)i / selected.Count);

                    string outputPath = BuildPathResolver.ResolveOutputPath(settings, platform);
                    BuildResultEntry result = BuildRunner.BuildPlatform(settings, platform, outputPath);
                    _results.Add(result);

                    if (!result.Succeeded && settings.AbortOnFailure) break;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (settings.RevealWhenDone) RevealParentFolder();
            Repaint();
        }

        private void RevealParentFolder()
        {
            string outputPath = _results.FirstOrDefault(entry => entry.Succeeded)?.OutputPath;
            if (string.IsNullOrEmpty(outputPath)) return;

            // Opens the shared Build_version folder that contains every per platform folder.
            string parentFolder = Path.GetDirectoryName(Path.GetDirectoryName(outputPath));
            if (!string.IsNullOrEmpty(parentFolder)) EditorUtility.OpenWithDefaultApp(parentFolder);
        }

        private static void BrowseForRoot(MultiPlatformBuilderSettings settings)
        {
            string picked = EditorUtility.OpenFolderPanel("Select Builds Root", BuildPathResolver.ProjectRoot, string.Empty);
            if (string.IsNullOrEmpty(picked)) return;

            picked = picked.Replace('\\', '/');
            string projectRoot = BuildPathResolver.ProjectRoot;
            bool underProject = picked.StartsWith(projectRoot);

            // Keep the stored path relative only when it actually lives under the project.
            settings.RootIsRelative = settings.RootIsRelative && underProject;
            settings.BuildsRoot = settings.RootIsRelative ? RelativeToProject(picked, projectRoot) : picked;
            settings.SaveSettings();
        }

        private static string RelativeToProject(string absolutePath, string projectRoot)
        {
            string relative = absolutePath.Substring(projectRoot.Length).TrimStart('/');
            return string.IsNullOrEmpty(relative) ? "." : relative;
        }

        private static List<BuildPlatformDefinition> SelectedPlatforms(MultiPlatformBuilderSettings settings)
        {
            List<BuildPlatformDefinition> selected = new List<BuildPlatformDefinition>();

            foreach (BuildPlatformDefinition platform in BuildPlatformCatalog.STANDALONE_PLATFORMS)
                if (settings.IsSelected(platform.Target) && BuildRunner.IsSupported(platform))
                    selected.Add(platform);

            return selected;
        }

        private static string FormatSize(ulong bytes)
        {
            if (bytes >= 1073741824UL) return $"{bytes / 1073741824f:F2} GB";
            if (bytes >= 1048576UL) return $"{bytes / 1048576f:F1} MB";
            if (bytes >= 1024UL) return $"{bytes / 1024f:F1} KB";
            return $"{bytes} B";
        }
    }
}
