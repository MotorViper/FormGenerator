using System;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class TreeToken : PairToken//, ITreeToken
    {
        //public static TreeToken Create(string text, ChildListToken children = null, bool ignoreErrors = false)
        //{
        //    PairToken token = PairToken.Create(text, ignoreErrors);
        //    return new TreeToken(token.Key, token.Value, children);
        //}

        //public TreeToken(IKeyToken key = null, IToken value = null, ChildListToken children = null) : base(key, value)
        //{
        //    _children = children;
        //    if (children != null)
        //        foreach (IToken child in children)
        //            child.Parent = this;
        //    else
        //        _children = new ChildListToken();
        //}

        //private readonly ChildListToken _children;

        //public void AddChild(PairToken child)
        //{
        //    _children.Add(child);
        //    child.Parent = this;
        //}

        //public override IToken FindToken(string text, bool checkChildren = true, bool checkParent = true)
        //{
        //    IToken found;
        //    if (text.Contains("."))
        //    {
        //        string[] bits = text.Split(new[] { '.' }, 2);
        //        found = base.FindToken(bits[0], false, false);
        //        found = found == this ? _children.FindToken(bits[1], false, false) : found?.FindToken(bits[1], false, false);
        //    }
        //    else
        //    {
        //        found = base.FindToken(text, false, false);
        //        if (found != null && found != this)
        //        {
        //            ChildListToken allValues = new ChildListToken();
        //            if (found is TreeToken foundList)
        //                foreach (TreeToken token in foundList._children)
        //                    allValues.Add(token);
        //            foreach (TreeToken token in _children)
        //                allValues.Replace(token);
        //            found = new TreeToken(new StringToken(text), Value, allValues);
        //        }
        //    }

        //    if (found == null && checkChildren)
        //        found = _children.FindToken(text, false, false);

        //    if (found == null && checkParent)
        //        found = Parent.FindToken(text, false, checkParent);

        //    return found;
        //}

        //public override void WalkTree(Action<string, string> walker, string prefix = null)
        //{
        //    base.WalkTree(walker, prefix);
        //    foreach (PairToken item in _children)
        //        item.WalkTree(walker, "\t" + prefix);
        //}

        //public ITreeToken Clone()
        //{
        //    TreeToken root = new TreeToken(Key, Value) { Parent = Parent };
        //    foreach (PairToken child in _children)
        //        root._children.Add(child);
        //    return root;
        //}

        //public void ReplaceChild(PairToken child)
        //{
        //    _children.Replace(child);
        //}

        //public TreeToken SubstituteParameters(TreeToken tree)
        //{
        //    return new TreeToken(Key.SubstituteParameters(tree), Value.SubstituteParameters(tree), _children.SubstituteParameters(tree));
        //}
    }
}
