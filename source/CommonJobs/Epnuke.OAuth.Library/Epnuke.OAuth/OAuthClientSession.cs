using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Epnuke.OAuth.Extensions;

namespace Epnuke.OAuth
{
    public class OAuthClientSession
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly INonceGenerator _nonceGenerator;
        private IDictionary<string, string> _additionalData;
        private OAuthTokenPair _temporaryCredentials;
        private OAuthTokenPair _finalCredentials;

        public OAuthTokenPair TemporaryCredentials { get { return _temporaryCredentials; } }
        public OAuthTokenPair AccessCredentials { get { return _finalCredentials; } }

        public bool IsAuthorized { get { return _finalCredentials != null; } }
        public bool HasTemporaryCredentials { get { return _temporaryCredentials != null; } }

        public IEnumerable<string> AdditionalDataKeys
        {
            get
            {
                return _additionalData != null
                           ? _additionalData.Keys
                           : Enumerable.Empty<string>();
            }
        }

        public void SetFinalCredentials (OAuthTokenPair tokens)
        {
            _temporaryCredentials = null;
            _finalCredentials = new OAuthTokenPair(tokens.Token, tokens.Secret);
        }

        public string GetAdditionalData(string key)
        {
            return _additionalData != null && _additionalData.ContainsKey(key)
                       ? _additionalData[key]
                       : null;
        }

        public OAuthClientSession(string ckey, string csecret, INonceGenerator nonceGenerator)
        {
            _consumerKey = ckey;
            _consumerSecret = csecret;
            _nonceGenerator = nonceGenerator;
        }


        public void RequestTemporaryCredentials(string requestTokenUri, string httpMethod, string callback)
        {
            var authHeader = OAuthRequestBuilder.BuildAuthHeader(_consumerKey, _nonceGenerator);
            authHeader.AppendOAuthValue(OAuthParameterNames.OAuthCallback, callback);
            var request = new OAuthRequest(requestTokenUri, httpMethod);
            request.AddRange(authHeader.Parse(), OAuthParamaterSource.AuthorizationHeader);
            var signature = request.ComputeSignature(_consumerSecret, null);
            request.AddParameter(OAuthParameterNames.OAuthSignature, signature, OAuthParamaterSource.AuthorizationHeader);
            var oauthResponse = new OAuthResponse(request, true);
            var requestToken = oauthResponse["oauth_token"];
            var requestSecret = oauthResponse["oauth_token_secret"];
            _temporaryCredentials = new OAuthTokenPair(requestToken, requestSecret);
        }

        public void Authorize(string verifier, string authorizeUri)
        {
            if (!HasTemporaryCredentials) throw new InvalidOperationException("Session has not temporary credentials.");
            var authHeader = OAuthRequestBuilder.BuildAuthHeader(_consumerKey, _nonceGenerator);
            authHeader.AppendOAuthValue(OAuthParameterNames.OAuthToken, _temporaryCredentials.Token);
            var oauthRequest = new OAuthRequest(string.Format("{0}?oauth_verifier={1}", authorizeUri, verifier), "post");
            oauthRequest.AddRange(authHeader.Parse(), OAuthParamaterSource.AuthorizationHeader);
            var signature = oauthRequest.ComputeSignature(_consumerSecret, _temporaryCredentials.Secret);
            oauthRequest.AddParameter("oauth_signature", signature, OAuthParamaterSource.AuthorizationHeader);
            var response = new OAuthResponse(oauthRequest, true);
            var oauthToken = response["oauth_token"];
            var oauthSecret = response["oauth_token_secret"];
            _additionalData = new Dictionary<string, string>();
            foreach (var key in response.Keys.Where(key => key != "oauth_token" && key != "oauth_token_secret"))
            {
                _additionalData.Add(key, response[key]);
            }
            _finalCredentials = new OAuthTokenPair(oauthToken, oauthSecret);
            _temporaryCredentials = null;
        }

        public string GetAuthorizationUri(string baseUri)
        {
            if (!HasTemporaryCredentials) throw new InvalidOperationException("Session has not temporary credentials.");
            return string.Format("{0}?oauth_token={1}", baseUri, _temporaryCredentials.Token);
        }

        /// <summary>
        /// Crea una petición autorizada (con el header Authorization válido) vacía. La petición no está firmadas
        /// <remarks>
        /// Si la uri pasada tiene querystring esa será añadida.
        /// </remarks>
        /// </summary>
        /// <param name="httpVerb">Verbo http a usar</param>
        /// <param name="uri">Uri de la petición</param>
        /// <returns>Petición oAuth con </returns>
        public OAuthRequest CreateEmptyAuthorizedRequest(string httpVerb, string uri)
        {
            if (!IsAuthorized) throw new InvalidOperationException("Session is not authorized.");
            var authHeader = OAuthRequestBuilder.BuildAuthHeader(_consumerKey, _nonceGenerator);
            authHeader.AppendOAuthValue(OAuthParameterNames.OAuthToken, _finalCredentials.Token);
            var oauthRequest = new OAuthRequest(uri, httpVerb);
            oauthRequest.AddRange(authHeader.Parse(), OAuthParamaterSource.AuthorizationHeader);
            return oauthRequest;
        }

        /// <summary>
        /// Firma digitalmente una petición.
        /// En este momento la petición ya no puede modificarse
        /// </summary>
        /// <param name="request">Petición a firmar</param>
        public void SignOAuthRequest(OAuthRequest request)
        {
            if (!IsAuthorized) throw new InvalidOperationException("Session is not authorized.");
            var signature = request.ComputeSignature(_consumerSecret, _finalCredentials.Secret);
            request.AddParameter("oauth_signature", signature, OAuthParamaterSource.AuthorizationHeader);
        }
    }
}
