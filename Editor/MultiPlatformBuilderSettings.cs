using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MultiPlatformBuilder
{
    [FilePath("ProjectSettings/MultiPlatformBuilder.asset", FilePathAttribute.Location.ProjectFolder)]
    public class MultiPlatformBuilderSettings : ScriptableSingleton<MultiPlatformBuilderSettings>
    {

        [SerializeField] private string buildsRoot = "Builds";
        [SerializeField] private bool rootIsRelative = true;
        [SerializeField] private string parentFolderTemplate = "Build_v%VERSION%";
        [SerializeField] private string platformFolderTemplate = "Build_v%VERSION%_%PLATFORM%";
        [SerializeField] private string executableNameTemplate = "%PRODUCT%";
        [SerializeField] private bool developmentBuild = false;
        [SerializeField] private bool cleanPlatformFolder = true;
        [SerializeField] private bool abortOnFailure = true;
        [SerializeField] private bool revealWhenDone = false;
        [SerializeField] private List<BuildTarget> selectedTargets = new List<BuildTarget>();

        public string BuildsRoot { get => buildsRoot; set => buildsRoot = value; }
        public bool RootIsRelative { get => rootIsRelative; set => rootIsRelative = value; }
        public string ParentFolderTemplate { get => parentFolderTemplate; set => parentFolderTemplate = value; }
        public string PlatformFolderTemplate { get => platformFolderTemplate; set => platformFolderTemplate = value; }
        public string ExecutableNameTemplate { get => executableNameTemplate; set => executableNameTemplate = value; }
        public bool DevelopmentBuild { get => developmentBuild; set => developmentBuild = value; }
        public bool CleanPlatformFolder { get => cleanPlatformFolder; set => cleanPlatformFolder = value; }
        public bool AbortOnFailure { get => abortOnFailure; set => abortOnFailure = value; }
        public bool RevealWhenDone { get => revealWhenDone; set => revealWhenDone = value; }

        public bool IsSelected(BuildTarget target) => selectedTargets.Contains(target);

        public void SetSelected(BuildTarget target, bool selected)
        {
            if (selected && !selectedTargets.Contains(target)) selectedTargets.Add(target);
            if (!selected) selectedTargets.Remove(target);
        }

        public void SaveSettings() => Save(true);
    }
}
