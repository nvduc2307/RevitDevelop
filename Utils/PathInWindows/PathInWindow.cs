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
        public static string AppDataRimTFilterRebar
        {
            get => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\RIMT\\Filter\\Rebar";
        }
        public static string AppDataRimTFilter
        {
            get => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\RIMT\\Filter";
        }
        public static string AppDataRimT
        {
            get => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\RIMT";
        }
        public static string AppDataRimTData
        {
            get => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\RIMT\\data";
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
