using System.Collections.Generic;
using System.Text;
using TextParser.Tokens;

namespace TextParser
{
    public class TokenList : List<IToken>
    {
        public TokenList(IToken token)
        {
            Add(token);
        }

        public TokenList()
        {
        }

        public int Sum()
        {
            int sum = 0;
            foreach (IToken token in this)
                sum += token.Convert<int>();
            return sum;
        }

        public double DSum()
        {
            double sum = 0;
            foreach (IToken token in this)
                sum += token.Convert<double>();
            return sum;
        }

        public string Aggregate()
        {
            Dictionary<string, int> found = new Dictionary<string, int>();
            foreach (IToken child in this)
            {
                int count;
                string value = child.Text;
                found[value] = found.TryGetValue(value, out count) ? ++count : 1;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in found)
                sb.Append(item.Key).Append("(").Append(item.Value).Append(")/");
            return sb.ToString().TrimEnd('/');
        }
    }
}
