
namespace SewerScavenger.Models
{
    //Global Variables
    public static class VersionGlobals
    {
        public const int VersionDataMajor = 1;
        public const int VersionDataMinor = 1;

        public const int VersionCodeMajor = 1;
        public const int VersionCodeMinor = 1;

       public static string GetCodeVersion()
        {
            return VersionCodeMajor + "." + VersionCodeMinor;
        }

        public static string GetDataVersion()
        {
            return VersionCodeMajor + "." + VersionCodeMinor;
        }

        public static string GetCombinedVersion()
        {
            return "Version: " + GetCodeVersion() + " Data: " + GetDataVersion();
        }
    }
}
