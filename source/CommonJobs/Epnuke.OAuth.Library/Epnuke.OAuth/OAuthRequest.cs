using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Epnuke.OAuth.Extensions;

namespace Epnuke.OAuth
{
    public class OAuthRequest
    {
        private const string OAUTH_VERSION = "1.0";
        private const string SIGNATURE_METHOD = "HMAC-SHA1";

        private readonly List<OAuthRequestError> _oAuthErrors;
        private readonly string _fullUri;
        private readonly string _httpMethod;


        private string _consumerKey;
        private string _nonce;
        private string _signature;
        private string _signatureMethod;
        private string _timestamp;
        private string _version;
        private string _token;
        private string _callback;

        private readonly IDictionary<string, List<OAuthParameter>> _allParams;
        private readonly string _normalizedUri;

        public string ConsumerKey { get { return _consumerKey; } }
        public string Nonce { get { return _nonce; } }
        public string Signature { get { return _signature; } }
        public string SignatureMethod { get { return _signatureMethod; } }
        public string Timestamp { get { return _timestamp; } }
        public string Version { get { return _version; } }
        public string Token { get { return _token; } }
        public string Callback { get { return _callback; } }
        public string HttpMethod { get { return _httpMethod; } }
        public string FullUri { get { return _fullUri; } }
        public string NormalizedUri { get { return _normalizedUri; } }
        
        public IEnumerable<OAuthRequestError> OAuthErrors { get { return _oAuthErrors; } }

        public OAuthRequest(string uri, string httpMethod)
        {
            _oAuthErrors = new List<OAuthRequestError>();

            _consumerKey = string.Empty;
            _nonce = string.Empty;
            _signature = string.Empty;
            _signatureMethod = string.Empty;
            _timestamp = string.Empty;
            _token = string.Empty;
            _version = string.Empty;
            _callback = string.Empty;
            _allParams = new Dictionary<string, List<OAuthParameter>>();
            _fullUri = uri;
            _httpMethod = httpMethod;
            ParseQueryString();
            _normalizedUri = new Uri(uri).ToNormalizedString();
        }

        public OAuthRequest(string host, string pathQuery, string httpMethod) : this (string.Format("http://{0}{1}", host, pathQuery), httpMethod)
        {
        }

        private void ParseQueryString()
        {
            var uri = new Uri(_fullUri);
            var qs = uri.Query;
            if (string.IsNullOrEmpty(qs) || qs.Trim() == "?") return;
            qs = qs.Trim();
            if (qs[0] == '?') qs = qs.Substring(1);

            foreach (var qtoken in qs.Split('&'))
            {
                var value = string.Empty;
                var name = string.Empty;
                var idxIgual = qtoken.IndexOf('=');
                if (idxIgual == -1)
                {
                    name = qtoken;
                }
                else
                {
                    name = qtoken.Substring(0, idxIgual);
                    value = qtoken.Substring(idxIgual + 1);
                }
                AddParameter(name, value, OAuthParamaterSource.QueryString);
            }
        }

        public void AddRange(IDictionary<string, string> values, OAuthParamaterSource source)
        {

            foreach (var key in values.Keys)
            {
                AddParameter(key, values[key], source);
            }
        }

        public void AddParameter(string name, string value, OAuthParamaterSource source)
        {

            if (!_allParams.ContainsKey(name))
            {
                _allParams.Add(name, new List<OAuthParameter>());
            }

            _allParams[name].Add(new OAuthParameter(value, source));
        }


        public int ParametersCount
        {
            get { return _allParams.Keys.Count; }
        }

        public bool HasConsumerKey
        {
            get { return _allParams.ContainsKey(OAuthParameterNames.OAuthConsumerKey); }
        }

        public bool HasSignatureMethod
        {
            get { return _allParams.ContainsKey(OAuthParameterNames.OAuthSignatureMethod); }
        }

        public bool HasSignature
        {
            get { return _allParams.ContainsKey(OAuthParameterNames.OAuthSignature); }
        }

        public bool HasNonce
        {
            get { return _allParams.ContainsKey(OAuthParameterNames.OAuthNonce); }
        }

        public bool HasTimestamp
        {
            get { return _allParams.ContainsKey(OAuthParameterNames.OAuthTimestamp); }
        }

        public bool HasToken
        {
            get { return _allParams.ContainsKey(OAuthParameterNames.OAuthToken); }
        }

        public bool HasVersion
        {
            get { return _allParams.ContainsKey(OAuthParameterNames.OAuthVersion); }
        }
        public bool HasCallback
        {
            get { return _allParams.ContainsKey(OAuthParameterNames.OAuthCallback); }
        }


        public bool IsValidSignatureMethod
        {
            get { return _signatureMethod == SIGNATURE_METHOD; }
        }

        public bool IsValidVersion
        {
            get { return _version == OAUTH_VERSION; }
        }

        public bool IsValidNonce
        {
            get { return true; }
        }

        public bool IsValidTimestamp
        {
            get
            {
                long ts;
                var isLong = long.TryParse(_timestamp, out ts);
                return isLong && (Math.Abs(ts - CurrentTimespan) < 1800);
            }
        }

        public static long CurrentTimespan
        {
            get
            {
                var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return (long)ts.TotalSeconds;
            }
        }

        public bool IsValid
        {
            get { return _oAuthErrors.Count == 0; }
        }


        public string GetSignatureBaseString()
        {

            var paramString = GetParameterString();
            var encoder = new OAuthEncoder();
            return string.Format("{0}&{1}&{2}", _httpMethod.ToUpper(), encoder.Encode(_normalizedUri), encoder.Encode(paramString));
        }

        public string GetParameterString()
        {
            var encoder = new OAuthEncoder();
            var sb = new StringBuilder();

            foreach (var key in _allParams.Keys.Where(x => x != OAuthParameterNames.OAuthSignature).
                Select(x => new { UnencodedKey = x, EncodedKey = encoder.Encode(x) }).OrderBy(y => y.EncodedKey))
            {
                var values = _allParams[key.UnencodedKey].Select(x => encoder.Encode(x.Value)).OrderBy(x => x);
                foreach (var value in values)
                {
                    sb.Append(string.Format("{0}={1}&", key.EncodedKey, value));
                }
            }

            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public IEnumerable<string> GetParameterNames(OAuthParamaterSource source)
        {
            return _allParams.Where(x => x.Value.Any(y => y.Source == source)).Select(x => x.Key);
        }

        public IEnumerable<string> GetParameterValues(OAuthParamaterSource source, string paramName)
        {
            if (_allParams.ContainsKey(paramName))
            {
                return _allParams[paramName].Where(x => x.Source == source).Select(x => x.Value);
            }
            return Enumerable.Empty<string>();
        }


        public void ValidateOAuthData()
        {
            CheckForDuplicateValue(OAuthParameterNames.OAuthConsumerKey, ref _consumerKey, OAuthRequestError.DuplicatedConsumerKey);
            CheckForDuplicateValue(OAuthParameterNames.OAuthSignature, ref _signature, OAuthRequestError.DuplicatedSignature);
            CheckForDuplicateValue(OAuthParameterNames.OAuthSignatureMethod, ref _signatureMethod, OAuthRequestError.DuplicatedSignatureMethod);
            CheckForDuplicateValue(OAuthParameterNames.OAuthNonce, ref _nonce, OAuthRequestError.DuplicatedNonce);
            CheckForDuplicateValue(OAuthParameterNames.OAuthTimestamp, ref _timestamp, OAuthRequestError.DuplicatedTimestamp);
            CheckForDuplicateValue(OAuthParameterNames.OAuthToken, ref _token, OAuthRequestError.DuplicatedToken);
            CheckForDuplicateValue(OAuthParameterNames.OAuthVersion, ref _version, OAuthRequestError.DuplicatedVersion, OAUTH_VERSION);
            CheckForDuplicateValue(OAuthParameterNames.OAuthCallback, ref _callback, OAuthRequestError.DuplicateCallback);

            if (!IsValidVersion) _oAuthErrors.Add(OAuthRequestError.InvalidVersion);
            if (string.IsNullOrEmpty(_timestamp)) _oAuthErrors.Add(OAuthRequestError.NoTimestamp);
            if (!IsValidTimestamp) _oAuthErrors.Add(OAuthRequestError.InvalidTimestamp);
            if (!IsValidNonce) _oAuthErrors.Add(OAuthRequestError.InvalidNonce);
            if (!IsValidSignatureMethod) _oAuthErrors.Add(OAuthRequestError.InvalidSignatureMethod);
        }

        private void CheckForDuplicateValue(string key, ref string result, OAuthRequestError errorToAddIfDuplicated, string defaultValue = null)
        {
            if (_allParams.ContainsKey(key))
            {
                var lst = _allParams[key];
                if (lst.Count == 1)
                {
                    result = lst[0].Value;
                }
                else
                {
                    result = null;
                    _oAuthErrors.Add(errorToAddIfDuplicated);
                }
            }
            else
            {
                result = defaultValue;
            }
        }

        public string ComputeSignature(string consumerSecret, string tokenSecret)
        {
            var baseString = GetSignatureBaseString();
            var encoder = new OAuthEncoder();

            var key = string.Format("{0}&{1}", encoder.Encode(consumerSecret), tokenSecret != null ? encoder.Encode(tokenSecret) : string.Empty);

            var result = new OAuthSigner(key, baseString).GetSignature();
            return result;
        }

        public void SetInvalidSignature()
        {
            _oAuthErrors.Add(OAuthRequestError.InvalidSignature);
        }

    }
}