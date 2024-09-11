using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Utils.StringInWindow
{
    public static class TextFileMethods
    {
        public static List<string> ReadFile(string fileDirection, string fileName)
        {
            var list = new List<string>();
            var path = Path.Combine(fileDirection, fileName);
            try
            {
                list = !File.Exists(path)
                    ? throw new Exception("didnt exist")
                    : File.ReadAllLines(path).Where(x => !string.IsNullOrEmpty(x)).ToList();
            }
            catch (Exception)
            {

            }
            return list;
        }
        public static void WriteFile(string path, List<string> texts)
        {
            try
            {
                if (!File.Exists(path))
                {
                    using (FileStream fs = File.Create(path))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("");
                        fs.Write(info, 0, info.Length);
                    }
                    File.WriteAllLines(path, texts);
                }
                else
                {
                    File.WriteAllLines(path, texts);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("保存失敗", "通知", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
