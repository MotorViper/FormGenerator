using FormGenerator.Tools;
using TextParser;

namespace FormGenerator.Models
{
    /// <summary>
    /// Provides the token data for generating the output.
    /// </summary>
    public class TokenData : IXamlTokenData
    {
        private TokenTree _mainData;
        private TokenTree _staticData;

        /// <summary>
        /// The file containing the main data that will be displayed.
        /// </summary>
        public string MainDataFile { get; set; }

        /// <summary>
        /// The file containing any static data.
        /// </summary>
        public string StaticDataFile { get; set; }

        /// <summary>
        /// The default directory any files will be found in.
        /// </summary>
        public string DefaultDirectory { get; set; }

        /// <summary>
        /// The main display data.
        /// </summary>
        public TokenTree MainData => _mainData ?? (_mainData = Parser.ParseFile(MainDataFile, DefaultDirectory, "XML"));

        /// <summary>
        /// Whether the main display data is available.
        /// </summary>
        public bool HasMainData => !string.IsNullOrWhiteSpace(DefaultDirectory) && !string.IsNullOrWhiteSpace(MainDataFile);

        /// <summary>
        /// The static data.
        /// </summary>
        public TokenTree StaticData => _staticData ?? (_staticData = Parser.ParseFile(StaticDataFile, DefaultDirectory, "XML"));

        /// <summary>
        /// Whether the static data is available.
        /// </summary>
        public bool HasStaticData => !string.IsNullOrWhiteSpace(DefaultDirectory) && !string.IsNullOrWhiteSpace(StaticDataFile);

        /// <summary>
        /// The xaml string that will be displayed.
        /// </summary>
        public virtual string Xaml => new XamlGenerator().GenerateXaml(StaticData);
    }
}
