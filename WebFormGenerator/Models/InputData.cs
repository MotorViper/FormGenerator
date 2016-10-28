using TextParser;

namespace WebFormGenerator.Models
{
    /// <summary>
    /// Provides the files that make up the display data.
    /// </summary>
    public class InputData : IInputData
    {
        public InputData(string dataName, string directory, string file, string staticDataFile, string optionsFile)
        {
            DefaultDirectory = directory;
            MainDataFile = file;
            StaticDataFile = staticDataFile;
            OptionsFile = optionsFile;
            DataName = dataName;
        }

        public string MainDataFile { get; }

        /// <summary>
        /// The file containing any static data.
        /// </summary>
        public string StaticDataFile { get; }

        /// <summary>
        /// The default directory any files will be found in.
        /// </summary>
        public string DefaultDirectory { get; }

        public string DataName { get; }
        public string OptionsFile { get; }
    }
}
