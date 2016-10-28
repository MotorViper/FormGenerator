namespace TextParser
{
    /// <summary>
    /// Interface used to specify the input files.
    /// </summary>
    public interface IInputData
    {
        string DataName { get; }

        /// <summary>
        /// The default directory any files will be found in.
        /// </summary>
        string DefaultDirectory { get; }

        string MainDataFile { get; }

        string OptionsFile { get; }

        /// <summary>
        /// The file containing any static data.
        /// </summary>
        string StaticDataFile { get; }
    }
}
