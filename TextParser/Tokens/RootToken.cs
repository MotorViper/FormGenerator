using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class RootToken : BaseToken//, ITreeToken
    {
        public RootToken()
        {
            Children = new ChildListToken();
        }

        //public RootToken(ChildListToken children)
        //{
        //    Children = children;
        //    foreach (IToken child in children)
        //        child.Parent = this;
        //}

        //public override IToken FindToken(string text, bool checkChildren = true, bool checkParent = false)
        //{
        //    return Children.FindToken(text, false, false);
        //}

        //public string this[string name]
        //{
        //    get { return FindToken(name, true, true)?.ValueToken?.ToString(); }
        //}

        //public override IToken ValueToken => null;

        public ChildListToken Children { get; private set; }

        //public void AddChild(PairToken child)
        //{
        //    Children.Add(child);
        //    child.Parent = this;
        //}

        //public void AddChildIfNotPresent(PairToken child)
        //{
        //    if (Children.FindToken(child.Key.ToString(), false, false) == null)
        //    {
        //        Children.Add(child);
        //        child.Parent = this;
        //    }
        //}

        public void RemoveChild(IKeyToken child)
        {
            //Children.Remove(child);
        }

        public void RemoveChildren(IKeyToken child)
        {
            //Children.RemoveAll(child);
        }

        //public ITreeToken Clone()
        //{
        //    RootToken root = new RootToken() ;
        //    foreach (PairToken child in Children)
        //        root.Children.Add(child);
        //    return root;
        //}

        public void ReplaceChild(PairToken child)
        {
            //Children.Replace(child);
        }
    }
}
