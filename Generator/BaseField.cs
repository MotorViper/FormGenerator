using System;
using System.Collections.Generic;
using System.Linq;
using TextParser;
using TextParser.Tokens;

namespace Generator
{
    public abstract class BaseField : IField
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();
        private int _marginLeft;
        private int _marginTop;
        public IFieldWriter Builder { get; set; }
        protected string Offset { get; private set; }
        public TokenTreeList Children { get; private set; }
        public virtual int Level { protected get; set; }
        public string Name { private get; set; }
        public IToken Parameter { protected get; set; }
        public IField Parent { protected get; set; }
        public TokenTree Selected { get; set; }

        public virtual TokenTree Data
        {
            set { Children = value?.Children.Clone(); }
        }


        public void OutputField(IFieldWriter builder, int level, TokenTree parameters, string offset, string endOfLine)
        {
            Builder = builder;
            Level = level;
            Offset = offset;
            AddStart(endOfLine, parameters);
            AddChildren(parameters, endOfLine);
            AddEnd(endOfLine);
        }

        public virtual void AddChildProperties(IField field)
        {
        }

        public void AddProperty<T>(string name, T value)
        {
            switch (name)
            {
                case "Align":
                case "ContentAlign":
                    string verticalAlignment;
                    string horizontalAlignment;
                    if (name == "Align")
                    {
                        verticalAlignment = "VerticalAlignment";
                        horizontalAlignment = "HorizontalAlignment";
                    }
                    else
                    {
                        verticalAlignment = "VerticalContentAlignment";
                        horizontalAlignment = "HorizontalContentAlignment";
                    }

                    string alignment = value.ToString();
                    if (alignment.StartsWith("Top"))
                    {
                        AddProperty(verticalAlignment, "Top");
                        alignment = alignment.Substring(3);
                    }
                    else if (alignment.StartsWith("Bottom"))
                    {
                        AddProperty(verticalAlignment, "Bottom");
                        alignment = alignment.Substring(6);
                    }
                    else if (alignment.StartsWith("Center") || alignment.StartsWith("Centre"))
                    {
                        AddProperty(verticalAlignment, "Center");
                        alignment = alignment.Substring(6);
                    }
                    else if (alignment.StartsWith("Middle"))
                    {
                        AddProperty(verticalAlignment, "Center");
                        if (alignment != "Middle")
                            alignment = alignment.Substring(6);
                    }
                    else if (alignment == "Fill")
                    {
                        AddProperty(verticalAlignment, "Stretch");
                        alignment = "Stretch";
                    }
                    else
                    {
                        AddProperty(verticalAlignment, "Center");
                    }

                    switch (alignment)
                    {
                        case "Right":
                        case "Left":
                            AddProperty(horizontalAlignment, alignment);
                            break;
                        case "":
                        case "Stretch":
                            AddProperty(horizontalAlignment, "Stretch");
                            break;
                        case "Centre":
                        case "Center":
                        case "Middle":
                            AddProperty(horizontalAlignment, "Center");
                            break;
                        default:
                            throw new Exception($"Unrecognized alignment {alignment}");
                    }
                    break;
                case "ShiftUp":
                    _marginTop = -Convert.ToInt32(value);
                    break;
                case "ShiftRight":
                    _marginLeft = Convert.ToInt32(value);
                    break;
                case "Invert":
                    AddProperty("Background", "Black");
                    AddProperty("Foreground", "White");
                    break;
                case "Debug":
                    break;
                default:
                    _properties[name] = value.ToString();
                    break;
            }
        }

        public virtual void AddStart(string endOfLine, TokenTree parameters)
        {
            AppendStartOfLine(Level, "<").Append(Name).Append(" ");
            AddProperties(parameters);
            OutputProperties(_properties);
            AddHeadings();
            Builder.Append(">").Append(endOfLine);
        }

        protected abstract void AddHeadings();

        protected abstract void OutputProperties(Dictionary<string, string> properties);

        protected virtual void AddProperties(TokenTree parameters)
        {
            _marginTop = 0;
            _marginLeft = 0;

            bool parametersRemoved = false;

            foreach (TokenTree child in Children.Where(child => !IgnoredProperties().Contains(child.Name)))
            {
                if (IsParameter(child.Name))
                {
                    // This is nasty but until I add scoping to function parameters it will have to do.
                    if (!parametersRemoved)
                    {
                        parametersRemoved = true;
                        List<string> toRemove = parameters.Children.Select(x => x.Name).Where(IsParameter).ToList();
                        foreach (string name in toRemove)
                            parameters.Remove(name);
                    }
                    parameters.Children.Add(child);
                }
                else
                {
                    TokenTreeList list = new TokenTreeList(parameters);
                    if (Selected != null)
                        list.Add(Selected);
                    AddProperty(child, list);
                }
            }
            if (_marginLeft != 0 || _marginTop != 0)
                AddProperty("Margin", $"{_marginLeft},{_marginTop},0,0");
            Parent?.AddChildProperties(this);
        }

        protected void AddProperty(TokenTree child, TokenTreeList parameters)
        {
            AddProperty(child.Name, child.Value, parameters);
        }

        protected virtual void AddProperty(string name, IToken value, TokenTreeList parameters)
        {
            AddProperty(name, ProcessTokens(value, parameters));
        }

        protected abstract string ProcessTokens(IToken value, TokenTreeList parameters);

        protected static bool IsParameter(string name)
        {
            if (!name.StartsWith("P"))
                return false;
            int index;
            string sIndex = name.Substring(1);
            return int.TryParse(sIndex, out index) && index.ToString() == sIndex;
        }

        protected virtual List<string> IgnoredProperties()
        {
            return new List<string> {"Field", "Across", "Over", "Columns", "Rows", "Header"};
        }

        protected IFieldWriter AppendStartOfLine(int level, string start)
        {
            for (int i = 0; i < level; i++)
                Builder.Append(Offset);
            return Builder.Append(start);
        }

        protected virtual void AddChildren(TokenTree parameters, string endOfLine)
        {
            IEnumerable<TokenTree> fields = GetSubFields();
            foreach (TokenTree child in fields)
                Builder.AddChild(child, Level + 1, parameters, Offset, endOfLine, this, null, Selected);
        }

        protected IEnumerable<TokenTree> GetSubFields()
        {
            return Children.Where(child => child.Name == "Field");
        }

        public virtual void AddEnd(string endOfLine)
        {
            AppendStartOfLine(Level, "</").Append(Name).Append(">").Append(endOfLine);
        }
    }
}
