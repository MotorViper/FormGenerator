﻿using System.Text;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class TextBox : Field
    {
        public TextBox(Field parent, TokenTree data, int level, StringBuilder builder) : base(parent, "TextBox", data, level, builder)
        {
        }

        protected override void AddProperty(string name, IToken value, TokenTreeList parameters)
        {
            switch (name)
            {
                case "Content":
                    name = "Text";
                    break;
            }
            base.AddProperty(name, value, parameters);
        }
    }
}
