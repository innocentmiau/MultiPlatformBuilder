namespace BuildTools
{
    public static class BuildVersionUtility
    {

        // Bumps only the last numeric part, e.g. 0.1.137 -> 0.1.138. Bigger version
        // changes are intentionally left to the user in Player Settings.
        public static string BumpLastComponent(string version)
        {
            if (string.IsNullOrEmpty(version)) return null;

            int lastDotIndex = version.LastIndexOf('.');
            string lastPart = version.Substring(lastDotIndex + 1);
            if (!int.TryParse(lastPart, out int number)) return null;

            return version.Substring(0, lastDotIndex + 1) + (number + 1);
        }
    }
}
