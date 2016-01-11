using System.IO;
using Helpers;

namespace FormGenerator.Models
{
    // ReSharper disable once UnusedMember.Global - used by IOC.
    /// <summary>
    /// Class provides standard token data but uses a pregenerated file for the xaml.
    /// </summary>
    public class PregeneratedXamlTokenData : TokenData
    {
        /// <summary>
        /// The xaml string that will be displayed.
        /// </summary>
        public override string Xaml => File.ReadAllText(FileUtils.GetFullFileName("TestFile.xaml", DefaultDirectory));
    }
}
