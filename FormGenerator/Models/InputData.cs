using System.Configuration;
using Helpers;
using TextParser;

namespace FormGenerator.Models
{
    /// <summary>
    /// Specifies the locations of the files that will be used by the application.
    /// </summary>
    public class InputData : IInputData
    {
        public string MainDataFile => ConfigurationManager.AppSettings.Get("DataFile");

        /// <summary>
        /// The file containing any static data.
        /// </summary>
        public string StaticDataFile => ConfigurationManager.AppSettings.Get("StaticData");

        /// <summary>
        /// The default directory any files will be found in.
        /// </summary>
        public string DefaultDirectory => ConfigurationManager.AppSettings.Get("DataDirectory");

        public string DataName => ConfigurationManager.AppSettings.Get("DataName");

        public string OptionsFile
        {
            get
            {
                string optionsFile = ConfigurationManager.AppSettings.Get("OptionsFile");
                if (optionsFile != null && !optionsFile.Contains(":"))
                    optionsFile = FileUtils.GetFullFileName(optionsFile, DefaultDirectory);
                return optionsFile;
            }
        }
    }
}
