using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epnuke.OAuth
{
    public static class OAuthRequestBuilder
    {
        public static OAuthAuthorizationHeader BuildAuthHeader(string consumerkey, INonceGenerator nonceGenerator)
        {
            var authHeaderValues = new Dictionary<string, string>();

            authHeaderValues.Add(OAuthParameterNames.OAuthConsumerKey, consumerkey);
            authHeaderValues.Add(OAuthParameterNames.OAuthSignatureMethod, OAuthSigner.SIGNATURE_METHOD);
            authHeaderValues.Add(OAuthParameterNames.OAuthVersion, "1.0");
            authHeaderValues.Add(OAuthParameterNames.OAuthTimestamp, OAuthRequest.CurrentTimespan.ToString());
            authHeaderValues.Add(OAuthParameterNames.OAuthNonce, nonceGenerator.CreateNonce());
            return new OAuthAuthorizationHeader(authHeaderValues);
        }

    }
}
