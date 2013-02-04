using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epnuke.OAuth
{
    public class OAuthTokenPair
    {
        private readonly string _oauthToken;
        private readonly string _oauthTokenSecret;

        public OAuthTokenPair(string token, string secret)
        {
            _oauthToken = token;
            _oauthTokenSecret = secret;
        }

        public string Token { get { return _oauthToken; } }
        public string Secret { get { return _oauthTokenSecret; } }

    }
}
