using System.Collections.Generic;
using System.Linq;
using TextParser;
using TextParser.Tokens;

namespace Generator
{
    public class TokenTreeProperty : IProperty
    {
        private readonly TokenTree _data;
        private readonly TokenTreeList _parameters;
        private IToken _evaluated;

        public TokenTreeProperty(TokenTree data, TokenTree parameters)
        {
            _data = data;
            _parameters = new TokenTreeList(parameters);
        }

        private IToken Evaluated => _evaluated ?? (_evaluated = _data.Value.Evaluate(_parameters, false));

        public string Name => _data.Name;

        public bool IsList => Evaluated is ListToken;
        public IValue Value => new TokenTreeValue(Evaluated);

        public IList<IValue> Values => (Evaluated as ListToken)?.Tokens.Select(x => (IValue)new TokenTreeValue(x)).ToList();
    }
}
