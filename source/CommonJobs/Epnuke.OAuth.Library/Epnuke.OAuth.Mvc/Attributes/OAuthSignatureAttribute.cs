using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Epnuke.OAuth;
using Epnuke.OAuth.Mvc;
using Epnuke.OAuth.Mvc.Extensions;

namespace Epnuke.OAuth.Mvc.Attributes
{
    public class OAuthSignatureAttribute : FilterAttribute, IAuthorizationFilter
    {

        private readonly OAuthSignatureParameters _signatureParameters;

        public OAuthSignatureAttribute(OAuthSignatureParameters signParameters)
        {
            _signatureParameters = signParameters;
        }

        public OAuthSignatureAttribute()
        {
            _signatureParameters = OAuthSignatureParameters.ConsumerAndAccessSecrets;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var oAuthRequest = request.GetOAuthData();
            if (oAuthRequest.IsValid)
            {
                // Debemos validar la firma y compararla
                var tokenProvider = OAuthProviders.TokenProvider;
                var consumerSecret = tokenProvider.GetConsumerSecret(oAuthRequest.ConsumerKey);

                string ssp = null;
                if (NeedSecondSignatureKey)
                {
                    ssp = _signatureParameters == OAuthSignatureParameters.ConsumerAndRequestSecrets ? 
                        tokenProvider.GetRequestSecret(oAuthRequest.Token) : 
                        tokenProvider.GetAccessSecret(oAuthRequest.Token, oAuthRequest.ConsumerKey);
                }
                var signature = oAuthRequest.ComputeSignature(consumerSecret, ssp);
                if (signature != oAuthRequest.Signature)
                {
                    oAuthRequest.SetInvalidSignature();
                }

                filterContext.Controller.ControllerContext.HttpContext.Items.Add("_oauth_request", oAuthRequest);
            }

            foreach (var error in oAuthRequest.OAuthErrors)
            {
                filterContext.Controller.ViewData.ModelState.AddModelError("_oauth", error.ToString());
            }
        }

        private bool NeedSecondSignatureKey
        {
            get
            {
                return _signatureParameters ==
                       OAuthSignatureParameters.ConsumerAndRequestSecrets ||
                       _signatureParameters == OAuthSignatureParameters.ConsumerAndAccessSecrets;
            }
        }
    }
}

