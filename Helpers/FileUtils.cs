using System.IO;

namespace Helpers
{
    public static class FileUtils
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
