using System.Linq;
using TextParser;

namespace FormGenerator.Fields
{
    public static class FieldFactory
    {
        public static Field CreateField(string fieldName, TokenTree data, int level, TokenTree parameters, Field parent)
        {
            Field field;
            switch (fieldName)
            {
                case "ComboBox":
                    field = new ComboBox(parent, data, level);
                    break;
                case "Selector":
                    field = new Selector(parent, data, level);
                    break;
                case "Grid":
                    field = new Grid(parent, data, level);
                    break;
                case "":
                case null:
                    field = new Continuation(parent, data, level);
                    break;
                case "TextBox":
                    field = new TextBox(parent, data, level);
                    break;
                default:
                    TokenTreeList fields = parameters?.GetAll("Field");
                    TokenTree replacement = fields?.FindValue(fieldName);
                    if (replacement != null)
                    {
                        fieldName = replacement["BasedOn"];
                        foreach (TokenTree child in replacement.Children.Where(x => x.Name != "BasedOn"))
                            data.Children.Add(child);
                        field = CreateField(fieldName, data, level, parameters, parent);
                    }
                    else
                    {
                        field = new Field(parent, fieldName, data, level);
                    }
                    break;
            }
            return field;
        }
    }
}
