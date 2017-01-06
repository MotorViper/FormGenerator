using System;
using System.Windows;
using System.Windows.Media;

namespace VTTUtilities
{
    public class VttTextOptions
    {
        private static readonly BrushConverter s_brushConverter = new BrushConverter();
        private static readonly FontFamilyConverter s_familyConverter = new FontFamilyConverter();
        private static readonly FontStretchConverter s_stretchConverter = new FontStretchConverter();
        private static readonly FontStyleConverter s_styleConverter = new FontStyleConverter();
        private static readonly FontWeightConverter s_weightConverter = new FontWeightConverter();

        public VttTextOptions()
        {
            Colour = null;
            Decorations = null;
            Family = null;
            Size = 0;
            Stretch = FontStretches.Medium;
            Style = FontStyles.Normal;
            Weight = FontWeights.Normal;
        }

        public VttTextOptions(string options) : this()
        {
            string[] parts = options.Split('|');
            foreach (string bit in parts)
                Add(bit);
        }

        public Brush Colour { get; set; }
        public TextDecorationCollection Decorations { get; set; }
        public FontFamily Family { get; set; }
        public double Size { get; set; }
        public FontStretch Stretch { get; set; }
        public FontStyle Style { get; set; }
        public FontWeight Weight { get; set; }

        public void Add(string option)
        {
            string part = option.Trim();
            bool converted = ConvertColour(part) || ConvertDecoration(part) || ConvertWeight(part) ||
                             ConvertSize(part) || ConvertStretch(part) || ConvertStyle(part) || ConvertFamily(part);
            if (!converted)
                throw new Exception($"Could not convert formatting option {part}");
        }

        /// <summary>
        /// Converts a string into a font size.
        /// </summary>
        /// <param name="part">The string to convert.</param>
        /// <returns>Whether the string could be converted to a font size.</returns>
        private bool ConvertSize(string part)
        {
            double value;
            if (double.TryParse(part, out value))
            {
                Size = value;
                return true;
            }
            return false;
        }

        private bool ConvertWeight(string part)
        {
            try
            {
                object value = s_weightConverter.ConvertFromString(part);
                if (value != null)
                    Weight = (FontWeight)value;
                return value != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ConvertStyle(string part)
        {
            try
            {
                object value = s_styleConverter.ConvertFromString(part);
                if (value != null)
                    Style = (FontStyle)value;
                return value != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ConvertStretch(string part)
        {
            try
            {
                object value = s_stretchConverter.ConvertFromString(part);
                if (value != null)
                    Stretch = (FontStretch)value;
                return value != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ConvertFamily(string part)
        {
            try
            {
                Family = (FontFamily)s_familyConverter.ConvertFromString(part);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ConvertDecoration(string part)
        {
            try
            {
                TextDecorationCollection decorations = TextDecorationCollectionConverter.ConvertFromString(part);
                if (decorations != null && decorations.Count > 0)
                {
                    if (Decorations == null)
                        Decorations = decorations;
                    else
                        Decorations.Add(decorations);
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        private bool ConvertColour(string part)
        {
            try
            {
                Colour = (Brush)s_brushConverter.ConvertFromString(part);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
