using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextParser.Functions;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class ListToken : TypeToken<List<IToken>>, IReversibleToken, ITokenWithLength, IEnumerable<IToken>, IContainerToken
    {
        public ListToken() : base(new List<IToken>())
        {
        }

        public ListToken(IToken value) : this()
        {
            Value.Add(value);
        }

        public ListToken(IToken value, IToken value2) : this(value)
        {
            Value.Add(value2);
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        /// <returns>The number of elements contained in the collection</returns>
        public int Count => Value.Count;

        /// <summary>
        /// Whether this is token contains an expression.
        /// </summary>
        public override bool IsExpression => Value.Any(token => token.IsExpression);

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public IToken this[int index]
        {
            get { return Value[index]; }
            set { Value[index] = value; }
        }

        public override string ToString()
        {
            if (Value.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (IToken token in Value)
                sb.Append(token.ToString()).Append("|");
            string text = sb.ToString();
            return "(" + text.Substring(0, text.Length - 1) + ")";
        }

        IntToken ITokenWithLength.Count()
        {
            return new IntToken(Value.Count());
        }

        public IToken Reverse()
        {
            ListToken reverse = new ListToken();
            reverse.Value.AddRange(Value);
            reverse.Value.Reverse();
            return reverse;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IToken> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IToken token)
        {
            Value.Add(token);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>true if item is found in the collection, otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the collection.</param>
        public bool Contains(IToken item)
        {
            return Value.Contains(item);
        }

        public override IToken ConvertToDouble(TokenTreeList substitutions, bool isFinal)
        {
            ListToken result = new ListToken();
            foreach (IToken token in Value)
                result.Add(new DoubleFunction().Perform(token, substitutions, isFinal));
            return result;
        }

        public override IToken ConvertToInt(TokenTreeList substitutions, bool isFinal)
        {
            ListToken result = new ListToken();
            foreach (IToken token in Value)
                result.Add(new IntFunction().Perform(token, substitutions, isFinal));
            return result;
        }

        /// <summary>
        /// Converts the token to a boolean.
        /// </summary>
        public override bool ToBool()
        {
            if (Value.Count == 1)
                return Value[0].ToBool();
            throw new Exception("Could not convert ListToken");
        }

        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        public override int ToInt()
        {
            if (Value.Count == 1)
                return Value[0].ToInt();
            throw new Exception("Could not convert ListToken");
        }

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        public override double ToDouble()
        {
            if (Value.Count == 1)
                return Value[0].ToDouble();
            throw new Exception("Could not convert ListToken");
        }

        /// <summary>
        /// Evaluates the token.
        /// </summary>
        /// <param name="parameters">The parameters to use for substitutions.</param>
        /// <param name="isFinal">Whether this is a final parse.</param>
        /// <returns></returns>
        protected override IToken Process(TokenTreeList parameters, bool isFinal)
        {
            ListToken list = new ListToken();
            foreach (IToken token in Value)
            {
                IToken item = token.Evaluate(parameters, isFinal);
                if (!item.IsExpression || !isFinal)
                    list.Value.Add(item);
            }
            return list;
        }

        /// <summary>
        /// Converts the token to a list of tokens if possible and required.
        /// </summary>
        /// <returns>The list of tokens or the original token.</returns>
        public override IToken Flatten()
        {
            ListToken newList = new ListToken();
            foreach (IToken item in Value)
            {
                IToken flattened = item.Flatten();
                if (flattened is ListToken list)
                {
                    if (list.Count == 1)
                        newList.Value.Add(list.Value[0]);
                    else if (list.Count > 1)
                        newList.Value.AddRange(list.Value);
                }
                else if (!(flattened is NullToken))
                {
                    newList.Value.Add(flattened);
                }
            }
            return newList;
        }

        public override IToken SubstituteParameters(TokenTree parameters)
        {
            ListToken list = new ListToken();
            foreach (IToken token in Value)
                list.Value.Add(token.SubstituteParameters(parameters));
            return list;
        }

        public override void ModifyParameters(UserFunction function)
        {
            foreach (IToken token in Value)
                token.ModifyParameters(function);
        }

        /// <summary>
        /// Whether the token contains the input text.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <returns>True if the current token contains the input text.</returns>
        public override bool Contains(string text)
        {
            return Value.Any(token => token.Contains(text));
        }
    }
    //public class ListToken<T> : TypeToken<List<T>>, IReversibleToken, ITokenWithLength, IEnumerable<T>//, IContainerToken
    //    where T : IToken
    //{
    //    public ListToken() : base(new List<T>())
    //    {
    //    }

    //    public ListToken(T value) : this()
    //    {
    //        Value.Add(value);
    //    }

    //    public ListToken(T value, T value2) : this(value)
    //    {
    //        Value.Add(value2);
    //    }

    //    /// <summary>
    //    /// Gets the number of elements contained in the collection.
    //    /// </summary>
    //    /// <returns>The number of elements contained in the collection</returns>
    //    public int Count => Value.Count;

    //    /// <summary>
    //    /// Whether this token contains an expression.
    //    /// </summary>
    //    public override bool IsExpression => Value.Any(token => token.IsExpression);

    //    /// <summary>
    //    /// Gets or sets the element at the specified index.
    //    /// </summary>
    //    /// <returns>The element at the specified index.</returns>
    //    /// <param name="index">The zero-based index of the element to get or set.</param>
    //    public T this[int index]
    //    {
    //        get { return Value[index]; }
    //        set { Value[index] = value; }
    //    }

    //    public override string ToString()
    //    {
    //        if (Value.Count == 0)
    //            return "";

    //        StringBuilder sb = new StringBuilder();
    //        foreach (IToken token in Value)
    //            sb.Append(token.ToString()).Append("|");
    //        string text = sb.ToString();
    //        return "(" + text.Substring(0, text.Length - 1) + ")";
    //    }

    //    IntToken ITokenWithLength.Count()
    //    {
    //        return new IntToken(Value.Count());
    //    }

    //    public IToken Reverse()
    //    {
    //        ListToken<T> reverse = new ListToken<T>();
    //        reverse.Value.AddRange(Value);
    //        reverse.Value.Reverse();
    //        return reverse;
    //    }

    //    /// <summary>
    //    /// Returns an enumerator that iterates through the collection.
    //    /// </summary>
    //    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return Value.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    public void Add(T token)
    //    {
    //        Value.Add(token);
    //    }

    //    /// <summary>
    //    /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
    //    /// </summary>
    //    /// <returns>true if item is found in the collection, otherwise, false.
    //    /// </returns>
    //    /// <param name="item">The object to locate in the collection.</param>
    //    public bool Contains(T item)
    //    {
    //        return Value.Contains(item);
    //    }

    //    public override IToken ConvertToDouble(TokenTreeList substitutions, bool isFinal)
    //    {
    //        ListToken result = new ListToken();
    //        foreach (IToken token in Value)
    //            result.Add(new DoubleFunction().Perform(token, substitutions, isFinal));
    //        return result;
    //    }

    //    public override IToken ConvertToInt(TokenTreeList substitutions, bool isFinal)
    //    {
    //        ListToken result = new ListToken();
    //        foreach (IToken token in Value)
    //            result.Add(new IntFunction().Perform(token, substitutions, isFinal));
    //        return result;
    //    }

    //    /// <summary>
    //    /// Converts the token to a boolean.
    //    /// </summary>
    //    public override bool ToBool()
    //    {
    //        if (Value.Count == 1)
    //            return Value[0].ToBool();
    //        throw new Exception("Could not convert ListToken");
    //    }

    //    /// <summary>
    //    /// Converts the token to an integer.
    //    /// </summary>
    //    public override int ToInt()
    //    {
    //        if (Value.Count == 1)
    //            return Value[0].ToInt();
    //        throw new Exception("Could not convert ListToken");
    //    }

    //    /// <summary>
    //    /// Converts the token to a double.
    //    /// </summary>
    //    public override double ToDouble()
    //    {
    //        if (Value.Count == 1)
    //            return Value[0].ToDouble();
    //        throw new Exception("Could not convert ListToken");
    //    }

    //    /// <summary>
    //    /// Evaluates the token.
    //    /// </summary>
    //    /// <param name="parameters">The parameters to use for substitutions.</param>
    //    /// <param name="isFinal">Whether this is a final parse.</param>
    //    /// <returns></returns>
    //    public override IToken Evaluate(TokenTreeList parameters, bool isFinal)
    //    {
    //        ListToken list = new ListToken();
    //        foreach (T token in Value)
    //        {
    //            IToken item = token.Evaluate(parameters, isFinal);
    //            if (!item.IsExpression || !isFinal)
    //                list.Value.Add(item);
    //        }
    //        return list;
    //    }

    //    //public override IToken FindToken(string text, bool checkChildren)
    //    //{
    //    //    ListToken list = new ListToken();
    //    //    foreach (IToken child in Value)
    //    //    {
    //    //        IToken found = child.FindToken(text, false);
    //    //        if (found != null)
    //    //            list.Value.Add(found);
    //    //    }
    //    //    return list.Value.Count == 0 ? null : (list.Value.Count == 1 ? list.Value[0] : list);
    //    //}

    //    /// <summary>
    //    /// Converts the token to a list of tokens if possible and required.
    //    /// </summary>
    //    /// <returns>The list of tokens or the original token.</returns>
    //    public override IToken Flatten()
    //    {
    //        ListToken newList = new ListToken();
    //        foreach (T item in Value)
    //        {
    //            IToken flattened = item.Flatten();
    //            if (flattened is ListToken<T> list)
    //                newList.Value.AddRange(list.Value.ConvertAll<IToken>(x => x));
    //            else if (!(flattened == null))
    //                newList.Value.Add(flattened);
    //        }
    //        return newList;
    //    }

    //    //public bool Contains(IToken token)
    //    //{
    //    //    return Value.Contains((T)token);

    //    //}

    //    public override IToken SubstituteParameters(TokenTree parameters)
    //    {
    //        ListToken list = new ListToken();
    //        foreach (IToken token in Value)
    //            list.Value.Add(token.SubstituteParameters(parameters));
    //        return list;
    //    }

    //    /// <summary>
    //    /// Whether the token contains the input text.
    //    /// </summary>
    //    /// <param name="text">The input text.</param>
    //    /// <returns>True if the current token contains the input text.</returns>
    //    public override bool Contains(string text)
    //    {
    //        return Value.Any(token => token.Contains(text));
    //    }
    //}

    //public class ListToken : ListToken<IToken>
    //{
    //    public ListToken() : base()
    //    {
    //    }

    //    public ListToken(IToken value) : base(value)
    //    {
    //    }

    //    public ListToken(IToken value, IToken value2) : base(value, value2)
    //    {
    //    }
    //}
}
