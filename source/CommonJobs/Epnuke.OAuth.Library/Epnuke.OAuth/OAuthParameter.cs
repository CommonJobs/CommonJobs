namespace Epnuke.OAuth
{
    internal class OAuthParameter
    {
        private readonly string _value;
        private readonly OAuthParamaterSource _source;

        public OAuthParameter(string value, OAuthParamaterSource source)
        {
            _value = value;
            _source = source;
        }

        public string Value { get { return _value; } }
        public OAuthParamaterSource Source { get { return _source;  } }

    }
}