using System.IO;
using Helpers;

namespace WebFormGenerator.Models
{
    // ReSharper disable once UnusedMember.Global - used by IOC.
    /// <summary>
    /// Class provides standard token data but uses a pregenerated file for the html.
    /// </summary>
    public class PregeneratedHtmlTokenData : TokenData
    {
        /// <summary>
        /// The html string that will be displayed.
        /// </summary>
        public override string Html => File.ReadAllText(FileUtils.GetFullFileName("TestData.html", DefaultDirectory));
    }
}
