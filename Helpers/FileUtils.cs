using System.IO;

namespace Helpers
{
    /// <summary>
    /// Class for file helper methods.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Constructs the full file name from a file name and a default directory.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="defaultDirectory">The default directory.</param>
        /// <returns></returns>
        public static string GetFullFileName(string fileName, string defaultDirectory)
        {
            if (fileName != null && !File.Exists(fileName))
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
