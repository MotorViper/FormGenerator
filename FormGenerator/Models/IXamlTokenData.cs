using Generator;

namespace FormGenerator.Models
{
    /// <summary>
    /// Interface for classes that provide token data and xaml output.
    /// </summary>
    public interface IXamlTokenData : ITokenData
    {
        /// <summary>
        /// The xaml string that will be displayed.
        /// </summary>
        string Xaml { get; }
    }
}
