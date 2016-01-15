using TextParser;

namespace Generator
{
    /// <summary>
    /// Property specified using token data.
    /// </summary>
    public class TokenTreeProperty : IProperty
    {
        private readonly TokenTree _data;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Token data.</param>
        public TokenTreeProperty(TokenTree data)
        {
            _data = data;
        }

        /// <summary>
        /// The property name.
        /// </summary>
        public string Name => _data.Name;

        /// <summary>
        /// The property value.
        /// </summary>
        public IValue Value => new TokenTreeValue(_data.Value);
    }
}
