using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epnuke.OAuth
{
    public interface ITokenProvider
    {
        /// <summary>
        /// Retorna el consumer secret (privat) a partir del consumer key (public)
        /// </summary>
        string GetConsumerSecret(string consumerKey);

        /// <summary>
        /// Retorna el request secret a partir del request token.
        /// </summary>
        string GetRequestSecret(string requestToken);

        /// <summary>
        /// Retorna el access secret a partir del access token.
        /// </summary>
        string GetAccessSecret(string oauthToken, string consumerKey);

        /// <summary>
        /// Crea un parell de credencials temporals (oauth_token i oauth_token_secret) vinculats al consumer_key que s'indica
        /// </summary>
        OAuthTokenPair CreateTemporaryCredentials(string consumerKey);

        OAuthTokenPair CreateTemporaryCredentials(string consumerKey, string callback);

        bool ValidateOAuthToken(string oauthToken);
        string GetCallbackUrl(string oauthToken);
        string CreateVerificationCodeForUser(string oauthToken, VerificationCodeFormats verificationCodeFormat, string userid);
        OAuthTokenPair VerifyCode(string oauthToken, string consumerKey, string oauthVerifier, out string userid);
        void SaveFinalTokens(OAuthTokenPair finalTokens, OAuthTokenPair consumerTokens, string userId, dynamic additionalValues);
    }
}
