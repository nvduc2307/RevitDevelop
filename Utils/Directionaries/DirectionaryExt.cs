﻿using HcBimUtils.MoreLinq;
using System.IO;
using System.Text;

namespace Utils.Directionaries
{
    public static class DirectionaryExt
    {
        public static void CreateDirectory(this string pathFile)
        {
            var pathItems = pathFile.Split('\\');
            var fileName = pathItems.LastOrDefault();
            var dir = pathItems.Slice(0, pathItems.Count() - 1).Aggregate((a, b) => $"{a}\\{b}");
            var isFileExisted = File.Exists(pathFile);
            var isExisted = Directory.Exists(dir);
            if (!isExisted) Directory.CreateDirectory(dir);

            if (!isFileExisted)
            {
                using (FileStream fs = File.Create(pathFile))
                {
                    char[] value = "[]".ToCharArray();
                    fs.Write(Encoding.UTF8.GetBytes(value), 0, value.Length);
                }
            }
        }
    }
}
