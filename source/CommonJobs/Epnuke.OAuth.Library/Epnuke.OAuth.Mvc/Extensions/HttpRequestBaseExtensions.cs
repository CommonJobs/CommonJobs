using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Epnuke.OAuth.Extensions;

namespace Epnuke.OAuth.Mvc.Extensions
{
    public static class HttpRequestBaseExtensions
    {
        public static OAuthRequest GetOAuthData(this HttpRequestBase @this)
        {
            var authHeader = new OAuthAuthorizationHeader(@this.Headers["Authorization"]);
            var pathQuery = !string.IsNullOrEmpty(@this.ServerVariables["QUERY_STRING"])
                                ? string.Format("{0}?{1}", @this.ServerVariables["PATH_INFO"],
                                                @this.ServerVariables["QUERY_STRING"])
                                : @this.ServerVariables["PATH_INFO"];
            var oAuthRequest = new OAuthRequest(@this.ServerVariables["HTTP_HOST"], pathQuery, @this.HttpMethod);
            if (!authHeader.IsEmpty)
            {
                oAuthRequest.AddRange(authHeader.ParseEncoded(), OAuthParamaterSource.AuthorizationHeader);
            }

            oAuthRequest.AddRange(@this.Form.AsDictionary(), OAuthParamaterSource.Form);
            oAuthRequest.ValidateOAuthData();
            return oAuthRequest;
        }

    }
}
