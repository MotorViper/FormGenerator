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
        protected IFieldWriter Builder { get; set; }
        protected string Offset { get; private set; }
        public TokenTreeList Children { get; private set; }
        public virtual int Level { protected get; set; }
        public string Name { get; set; }
        public IToken Parameter { protected get; set; }
        public IField Parent { protected get; set; }
        public TokenTree Selected { get; set; }
        public List<string> Keys { get; set; }

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

        public virtual void AddProperty<T>(string name, T value)
        {
            switch (name)
            {
                case "Debug":
                    break;
                default:
                    _properties[name] = value.ToString();
                    break;
            }
        }

        protected virtual void AddStart(string endOfLine, TokenTree parameters)
        {
            AppendStartOfLine(Level, "<").Append(Name).Append(" ");
            AddProperties(parameters);
            OutputProperties(_properties);
            Builder.Append(">").Append(endOfLine);
        }

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

        /// <summary>
        /// Any properties that should not be processed.
        /// </summary>
        /// <returns>The list of properties to ignore.</returns>
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
                Builder.AddChild(child, Level + 1, parameters, Offset, endOfLine, this, null, Selected, Keys);
        }

        protected IEnumerable<TokenTree> GetSubFields()
        {
            return Children.Where(child => child.Name == "Field");
        }

        protected virtual void AddEnd(string endOfLine)
        {
            AppendStartOfLine(Level, "</").Append(Name).Append(">").Append(endOfLine);
        }
    }
}
