namespace PassXYZLib
{
    // All the code in this file is only included on Mac Catalyst.
    public partial class PxEnvironment
    {
        public static partial string GetRoot()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
    }
}
