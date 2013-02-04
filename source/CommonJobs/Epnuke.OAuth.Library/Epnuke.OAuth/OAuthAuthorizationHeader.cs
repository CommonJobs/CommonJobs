using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Epnuke.OAuth
{
    public class OAuthAuthorizationHeader
    {

        private string _content;

        public const string HeaderName = "Authorization";

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(_content); }
        }

        public string Content
        {
            get { return _content; }
        }

        public OAuthAuthorizationHeader(string content)
        {
            _content = content;
        }

        public OAuthAuthorizationHeader(IDictionary<string, string> content)
        {
            var sb = new StringBuilder();
            var nkeys = content.Keys.Count;
            var idx = 1;
            var encoder = new OAuthEncoder();
            sb.Append("OAuth ");
            foreach (var key in content.Keys)
            {
                sb.Append(string.Format(@"{0}=""{1}""", encoder.Encode(key),  encoder.Encode(content[key])));
                if (idx < nkeys) sb.Append(", ");
                idx++;
            }

            _content = sb.ToString();
        }

        public void AppendOAuthValue (string oAuthValueName, string oAuthValueData)
        {
            _content += string.Format(@", {0}=""{1}""", oAuthValueName, oAuthValueData);
        }

        public IDictionary<string, string> ParseEncoded()
        {
            return Parse(true);
        }


        public IDictionary<string, string> Parse()
        {
            return Parse(false);
        }


        private IDictionary<string, string> Parse(bool encoded)
        {
            var oAuthParameters = new Dictionary<string, string>();
            var encoder = new OAuthEncoder();
            if (_content == null) return null;
            var contentTrimmed = _content.Trim();
            if (!contentTrimmed.StartsWith("OAuth")) return null;
            contentTrimmed = contentTrimmed.Substring(5); // Eliminamos "OAuth" del principio

            var tokens = contentTrimmed.Split(new[] { "\"," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                var splittedToken = token.Replace("\"", string.Empty).Trim();
                var sepIdx = splittedToken.IndexOf('=');
                if (sepIdx > -1)
                {
                    var paramName = splittedToken.Substring(0, sepIdx);
                    var paramValue = splittedToken.Substring(sepIdx + 1);
                    if (paramName.StartsWith("oauth_"))
                    {
                        oAuthParameters.Add(encoded ? encoder.Decode(paramName) : paramName, encoded ? encoder.Decode(paramValue) : paramValue);
                    }
                }
            }

            return oAuthParameters;

        }

    }
}