using System.Reflection;
using System.IO;

namespace Utils.PathInWindows
{
    internal class PathInWindow
    {
        public static string PathData
        {
            get
            {
                return AssemblyDirectory + "\\Resources\\data";
            }
        }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public static string AppDataRimT
        {
            get => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\RIMT";
        }
        public static string AppDataDirectory
        {
            get
            {
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\";
            }
        }
    }
}
