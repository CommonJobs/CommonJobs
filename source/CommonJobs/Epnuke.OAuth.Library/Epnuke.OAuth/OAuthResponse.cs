using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Epnuke.OAuth.Extensions;

namespace Epnuke.OAuth
{
    public class OAuthResponse : DynamicObject
    {

        private readonly IDictionary<string, string> _dictionary;
        private readonly string _rawData;
        private string _contentType;

        public OAuthResponse(OAuthRequest request, bool parseResponse)
        {
            var wresponse = request.GetWebResponse();
            _rawData = ReadAllData(wresponse);
            if (parseResponse)
            {
                _dictionary = Parse(_rawData);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string value;
            result = _dictionary.TryGetValue(binder.Name, out value) ? (object)value : null;
            return true;
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return Parsed ? _dictionary.Keys : Enumerable.Empty<string>();
            }
        }

        public IEnumerable<string> Values
        {
            get
            {
                return Parsed ? _dictionary.Values : Enumerable.Empty<string>();
            }
        }

        public string RawData { get { return _rawData; } }

        public string ContentType { get { return _contentType; } }

        public bool Parsed { get { return _dictionary != null; } }

        public string this[string key]
        {
            get { return Parsed && _dictionary.ContainsKey(key) ? _dictionary[key] : null; }
        }

        private string ReadAllData(WebResponse wresponse)
        {

            _contentType = wresponse.ContentType;
            string responseData = null;
            using (var sr = new StreamReader(wresponse.GetResponseStream()))
            {
                responseData = sr.ReadToEnd();
            }
            wresponse.Close();
            return responseData;
        }

        private static IDictionary<string, string> Parse(string content)
        {
            var data = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(content)) return data;

            var unescapedContent = Uri.UnescapeDataString(content);
            var values = unescapedContent.Split('&');
            foreach (var value in values)
            {
                var sepIdx = value.IndexOf('=');
                if (value.EndsWith("="))
                {
                    data.Add(value.Substring(0, value.Length - 1), string.Empty);
                }
                else
                {
                    data.Add(value.Substring(0, sepIdx), value.Substring(sepIdx + 1));
                }
            }

            return data;
        }
    }
}
