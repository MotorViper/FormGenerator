﻿namespace TextParser
{
    /// <summary>
    /// Interface used to specify the input files.
    /// </summary>
    public interface IInputData
    {
        /// <summary>
        /// The key into the main token tree of the data that is being displayed.
        /// </summary>
        string DataName { get; }

        /// <summary>
        /// The default directory any files will be found in.
        /// </summary>
        string DefaultDirectory { get; }

        /// <summary>
        /// The name of the file containing the unprocessed data to be displayed.
        /// </summary>
        string MainDataFile { get; }

        /// <summary>
        /// The name of the file containing the processing options.
        /// </summary>
        string OptionsFile { get; }

        /// <summary>
        /// The name of the file containing any static data that will be used to generate the display data.
        /// </summary>
        string StaticDataFile { get; }
    }
}
