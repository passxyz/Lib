namespace PassXYZLib
{
    // All the code in this file is included in all platforms.
    public partial class PxEnvironment
    {
#if ANDROID || IOS || MACCATALYST || WINDOWS
        public static partial string GetRoot();
#else
        public static string GetRoot()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
#endif
    }
}
