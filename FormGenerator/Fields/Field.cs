using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class Field
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();
        private int _marginLeft;
        private int _marginTop;
        private string _xlmns = "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"";

        public Field(Field parent, string name, TokenTree data, int level)
        {
            Name = name;
            Children = data?.Children.Clone();
            Level = level;
            Parent = parent;
        }

        protected StringBuilder Builder { get; set; }
        public TokenTreeList Children { get; }
        protected int Level { get; set; }
        private string Name { get; }
        protected string Offset { get; set; }
        protected Field Parent { get; }

        protected virtual void AddStart(string endOfLine, TokenTree parameters)
        {
            AppendStartOfLine(Level, "<").Append(Name).Append(" ");
            AddProperties(parameters);
            OutputProperties();
            AddHeadings();
            Builder.Append(">").Append(endOfLine);
        }

        protected virtual void AddEnd(string endOfLine)
        {
            AppendStartOfLine(Level, "</").Append(Name).Append(">").Append(endOfLine);
        }

        private static string ProcessTokens(IToken value, TokenTree parameters)
        {
            IToken evaluated = value.Evaluate(new TokenTreeList(parameters))[0];
            ExpressionToken expression = evaluated as ExpressionToken;
            if (expression == null)
            {
                StringToken sValue = evaluated as StringToken;
                return (sValue?.Text?.StartsWith("?") ?? false)
                    ? "{Binding Values[" + sValue.Text.Substring(1) + "]}"
                    : evaluated.Text;
            }
            int id = DataConverter.SetFieldData(evaluated);
            return "{Binding Values, Converter={StaticResource DataConverter}, ConverterParameter=" + id + "}";
        }

        protected void AddProperty(TokenTree child, TokenTree parameters)
        {
            AddProperty(child.Name, child.Value, parameters);
        }

        protected virtual void AddProperty(string name, IToken value, TokenTree parameters)
        {
            AddProperty(name, ProcessTokens(value, parameters));
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
                default:
                    _properties[name] = value.ToString();
                    break;
            }
        }

        protected virtual List<string> IgnoredProperties()
        {
            return new List<string> {"Field", "Across"};
        }

        private void OutputProperties()
        {
            foreach (var property in _properties)
                Builder.Append(property.Key.ToCamelCase()).Append("=\"").Append(property.Value).Append("\" ");
        }

        protected virtual void AddProperties(TokenTree parameters)
        {
            _marginTop = 0;
            _marginLeft = 0;
            foreach (var child in Children.Where(child => !IgnoredProperties().Contains(child.Name)))
            {
                if (child.Name == "Inputs")
                    parameters.Replace(child);
                else
                    AddProperty(child, parameters);
            }
            if (_marginLeft != 0 || _marginTop != 0)
                AddProperty("Margin", $"{_marginLeft},{_marginTop},0,0");
            Parent?.AddChildProperties(this, parameters);
        }

        private void AddHeadings()
        {
            if (_xlmns != null && Level == 0)
                Builder.Append(_xlmns);
        }

        protected internal virtual void AddChildProperties(Field field, TokenTree parameters)
        {
        }

        protected StringBuilder AppendStartOfLine(int level, string start)
        {
            for (int i = 0; i < level; i++)
                Builder.Append(Offset);
            return Builder.Append(start);
        }

        protected virtual void AddChildren(TokenTree parameters, string endOfLine)
        {
            IEnumerable<TokenTree> fields = GetSubFields();
            foreach (TokenTree child in fields)
                AddChild(child, Level + 1, parameters, Builder, Offset, endOfLine, this);
        }

        protected IEnumerable<TokenTree> GetSubFields()
        {
            return Children.Where(child => child.Name == "Field");
        }

        public static void AddChild(TokenTree data, int level, TokenTree parameters, StringBuilder sb, string offset, string endOfLine, Field parent = null)
        {
            Field field = FieldFactory.CreateField(data.Value.Text, data, level, parameters, parent);
            field.OutputField(level, parameters, sb, offset, endOfLine);
        }

        private void OutputField(int level, TokenTree parameters, StringBuilder sb, string offset, string endOfLine)
        {
            Level = level;
            Offset = offset;
            Builder = sb;
            AddStart(endOfLine, parameters);
            AddChildren(parameters, endOfLine);
            AddEnd(endOfLine);
        }
    }
}
