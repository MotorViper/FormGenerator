using TextParser;

namespace Generator
{
    /// <summary>
    /// Interface for classes that provide token data to be displayed.
    /// </summary>
    public interface ITokenData
    {
        /// <summary>
        /// The default directory any files will be found in.
        /// </summary>
        string DefaultDirectory { set; }

        /// <summary>
        /// Whether the main display data is available.
        /// </summary>
        bool HasMainData { get; }

        /// <summary>
        /// Whether the static data is available.
        /// </summary>
        bool HasStaticData { get; }

        /// <summary>
        /// The main display data.
        /// </summary>
        TokenTree MainData { get; }

        /// <summary>
        /// The file containing the main data that will be displayed.
        /// </summary>
        string MainDataFile { set; }

        /// <summary>
        /// The static data.
        /// </summary>
        TokenTree StaticData { get; }

        /// <summary>
        /// The file containing any static data.
        /// </summary>
        string StaticDataFile { set; }
    }
}
