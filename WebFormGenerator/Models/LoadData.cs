using System.ComponentModel.DataAnnotations;
using Helpers;

namespace WebFormGenerator.Models
{
    public class LoadData
    {
        private readonly IHtmlTokenData _data = IOCContainer.Instance.Resolve<IHtmlTokenData>();

        /// <summary>
        /// Loads the data into the program.
        /// </summary>
        /// <param name="dataName">The main data item name.</param>
        /// <param name="directory">The default directory for files.</param>
        /// <param name="file">The main data file name.</param>
        /// <param name="staticDataFile">The static data file name.</param>
        public LoadData(string dataName, string directory, string file, string staticDataFile)
        {
            DataName = dataName;
            Directory = directory;
            File = file;
            StaticDataFile = staticDataFile;
            _data.DefaultDirectory = directory;
            _data.MainDataFile = file;
            _data.StaticDataFile = staticDataFile;
            _data.SelectedItem = dataName;
        }

        /// <summary>
        /// The data that will be displayed.
        /// </summary>
        public string Data => _data.Html;

        /// <summary>
        /// The main data item name.
        /// </summary>
        [Display(Name = "Data Name")]
        public string DataName { get; set; }

        /// <summary>
        /// The default directory for files.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The main data file name.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// The static data file name.
        /// </summary>
        [Display(Name = "Static Data File")]
        public string StaticDataFile { get; set; }
    }
}
