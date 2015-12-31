using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TextParser;

namespace WebFormGenerator.Models
{
    public class LoadData
    {
        private readonly HtmlGenerator _generator = new HtmlGenerator();

        public LoadData(string dataName, string directory, string file, string staticDataFile)
        {
            DataName = dataName;
            Directory = directory;
            File = file;
            StaticDataFile = staticDataFile;
            AllValues = Parser.ParseFile(File, Directory);
            Keys = new List<string>();
            foreach (TokenTree child in AllValues.Children)
                Keys.Add(child.Name);
            StaticData = Parser.ParseFile(StaticDataFile, Directory, "HTML");
        }

        [Display(Name = "Data Name")]
        public string DataName { get; set; }

        public string Directory { get; set; }

        public string File { get; set; }

        [Display(Name = "Static Data File")]
        public string StaticDataFile { get; set; }

        public TokenTree StaticData { get; set; }

        public string Data
        {
            get
            {
                string data = _generator.GenerateHtml(StaticData, AllValues.Children[0], DataName);
                return data;
            }
        }

        public List<string> Keys { get; set; }

        public TokenTree AllValues { get; set; }
    }
}