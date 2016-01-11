using Generator;

namespace WebFormGenerator.Models
{
    /// <summary>
    /// Interface for classes that provide token data and html output.
    /// </summary>
    public interface IHtmlTokenData : ITokenData
    {
        /// <summary>
        /// The html string that will be displayed.
        /// </summary>
        string Html { get; }

        /// <summary>
        /// The item that has been selected for display.EF
        /// </summary>
        string SelectedItem { set; }
    }
}
