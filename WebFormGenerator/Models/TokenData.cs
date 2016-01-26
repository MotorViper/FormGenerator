using System.Collections.Generic;
using TextParser;

namespace WebFormGenerator.Models
{
    /// <summary>
    /// Provides the token data for generating the output.
    /// </summary>
    public class TokenData : IHtmlTokenData
    {
        private TokenTree _mainData;
        private TokenTree _staticData;

        /// <summary>
        /// The default directory any files will be found in.
        /// </summary>
        public string DefaultDirectory { get; set; }

        /// <summary>
        /// Whether the main display data is available.
        /// </summary>
        public bool HasMainData => true;

        /// <summary>
        /// Whether the static data is available.
        /// </summary>
        public bool HasStaticData => true;

        /// <summary>
        /// The main display data.
        /// </summary>
        public TokenTree MainData => _mainData ?? (_mainData = Parser.ParseFile(MainDataFile, DefaultDirectory, "HTML"));

        /// <summary>
        /// The file containing the main data that will be displayed.
        /// </summary>
        public string MainDataFile { get; set; }

        /// <summary>
        /// The static data.
        /// </summary>
        public TokenTree StaticData => _staticData ?? (_staticData = Parser.ParseFile(StaticDataFile, DefaultDirectory, "HTML"));

        /// <summary>
        /// The file containing any static data.
        /// </summary>
        public string StaticDataFile { get; set; }

        /// <summary>
        /// The html string that will be displayed.
        /// </summary>
        public virtual string Html
        {
            get
            {
                List<string> keys = new List<string>();
                foreach (TokenTree child in MainData.Children)
                    keys.Add(child.Name);
                return new HtmlGenerator().GenerateHtml(StaticData, MainData.Children[0], SelectedItem, keys);
            }
        }

        /// <summary>
        /// The item that has been selected for display.
        /// </summary>
        public string SelectedItem { get; set; }
    }
}
