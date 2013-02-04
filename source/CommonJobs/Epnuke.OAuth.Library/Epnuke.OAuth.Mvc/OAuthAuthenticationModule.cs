using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using Epnuke.OAuth.Mvc.Extensions;

namespace Epnuke.OAuth.Mvc
{
    public sealed class OAuthAuthenticationModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnAuthenticateRequest;
        }

        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var request = new HttpRequestWrapper(application.Request);
            var oauthRequest = request.GetOAuthData();
            if (oauthRequest.IsValid)
            {

                var tokenProvider = OAuthProviders.TokenProvider;
                var customerSecret = tokenProvider.GetConsumerSecret(oauthRequest.ConsumerKey);
                var tokenSecret = oauthRequest.HasToken ? tokenProvider.GetAccessSecret(oauthRequest.Token, oauthRequest.ConsumerKey) : string.Empty;
                if (!string.Empty.Equals(tokenSecret))
                {
                    var signature = oauthRequest.ComputeSignature(customerSecret, tokenSecret);
                    if (signature == oauthRequest.Signature)
                    {
                        application.Context.User = new GenericPrincipal(new GenericIdentity(oauthRequest.Token), null);
                    }
                }
            }
        }


        public void Dispose()
        {
        }
    }
}
