using TextParser;

namespace VTTUtilities
{
    public class TestInputData : IInputData
    {
        /// <summary>
        /// The key into the main token tree of the data that is being displayed.
        /// </summary>
        public string DataName => "Not Used";

        /// <summary>
        /// The default directory any files will be found in.
        /// </summary>
        public string DefaultDirectory => @"C:\Development\Projects\FormGenerator\Data\";

        /// <summary>
        /// The name of the file containing the unprocessed data to be displayed.
        /// </summary>
        public string MainDataFile => "Fields.vtt";

        /// <summary>
        /// The name of the file containing the processing options.
        /// </summary>
        public string OptionsFile => "Options.vtt";

        /// <summary>
        /// The name of the file containing any static data that will be used to generate the display data.
        /// </summary>
        public string StaticDataFile => "Not Used";
    }
}
