using System.Linq;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class ChildListToken //: ListToken<PairToken>
    {
        //public void Replace(PairToken token)
        //{
        //    Value.RemoveAll(x => x.Key == token.Key);
        //    Value.Add(token);
        //}

        //public override IToken FindToken(string text, bool checkChildren, bool checkParent)
        //{
        //    ListToken list = new ListToken();
        //    IToken found;
        //    foreach (PairToken child in Value)
        //    {
        //        found = child.FindToken(text, false, false);
        //        if (found != null)
        //            list.Value.Add(found);
        //    }
        //    found = list.Value.Count == 0 ? null : (list.Value.Count == 1 ? list.Value[0] : list);
        //    if (found == null && checkParent)
        //        found = Parent.FindToken(text, false, false);
        //    return found;
        //}

        //public void Remove(IKeyToken key)
        //{
        //    PairToken found = Value.Where(x => x.Key == key).FirstOrDefault();
        //    if (found != null)
        //        Value.Remove(found);
        //}

        //public void RemoveAll(IKeyToken key)
        //{
        //    Value.RemoveAll(x => x.Key == key);
        //}
    }
}
