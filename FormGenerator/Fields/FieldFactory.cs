using System.Linq;
using System.Text;
using TextParser;

namespace FormGenerator.Fields
{
    public static class FieldFactory
    {
        public static Field CreateField(string fieldName, TokenTree data, int level, TokenTree parameters, Field parent, StringBuilder builder)
        {
            Field field;
            switch (fieldName)
            {
                case "ComboBox":
                    field = new ComboBox(parent, data, level, builder);
                    break;
                case "CheckBox":
                    field = new CheckBox(parent, data, level, builder);
                    break;
                case "Selector":
                    field = new Selector(parent, data, level, builder);
                    break;
                case "Grid":
                    field = new Grid(parent, data, level, builder);
                    break;
                case "":
                case null:
                    field = new Continuation(parent, data, level, builder);
                    break;
                case "Table":
                    field = new Table(parent, data, level, builder);
                    break;
                case "TextBox":
                    field = new TextBox(parent, data, level, builder);
                    break;
                default:
                    TokenTreeList fields = parameters?.GetAll("Field");
                    TokenTree replacement = fields?.FindValue(fieldName);
                    if (replacement != null)
                    {
                        fieldName = replacement["BasedOn"];
                        foreach (TokenTree child in replacement.Children.Where(x => x.Name != "BasedOn"))
                        {
                            if (child.Name == "Field")
                                data.Children.Add(child);
                            else
                                data.Children.AddIfMissing(child);
                        }
                        field = CreateField(fieldName, data, level, parameters, parent, builder);
                    }
                    else
                    {
                        field = new Field(parent, fieldName, data, level, builder);
                    }
                    break;
            }
            return field;
        }
    }
}
