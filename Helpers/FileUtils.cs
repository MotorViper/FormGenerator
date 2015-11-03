using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class FileUtils
    {
        public static string GetFullFileName(string fileName, string defaultDirectory)
        {
            if (!File.Exists(fileName))
            {
                if (defaultDirectory.EndsWith("\\"))
                    fileName = defaultDirectory + fileName;
                else
                    fileName = defaultDirectory + "\\" + fileName;
            }
            return fileName;
        }
    }
}
