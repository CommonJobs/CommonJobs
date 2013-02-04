using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Epnuke.OAuth.Extensions
{
    public static class OAuthRequestExtensions
    {
        public static WebResponse GetWebResponse(this OAuthRequest @this)
        {
            var wrequest = @this.GetWebRequest();
            var response = wrequest.GetResponse();
            return response;
        }

        /// <summary>
        /// (Extension Method): Obtiene una WebRequest preparada con los datos de la petición OAuth especificada
        /// </summary>
        /// <param name="this">OAuthRequest extendida, datos de la cual son copiados a la WebRequest</param>
        /// <returns>WebRequest lista para enviar</returns>
        public static WebRequest GetWebRequest(this OAuthRequest @this)
        {
            var uri = @this.FullUri;
            var request = WebRequest.Create(uri);
            request.Method = @this.HttpMethod;
            var authHeader = CreateAuthorizationHeader(@this);
            if (authHeader != null)
            {
                request.Headers.Add("Authorization", authHeader);
            }
            AddPostData(@this, request);
            return request;
        }

        private static void AddPostData(OAuthRequest oauthReq, WebRequest request)
        {
            var postParams = FlattenValues(oauthReq, OAuthParamaterSource.Form);

            if (postParams.Any())
            {
                request.ContentType = "application/x-www-form-urlencoded";
                var stream = request.GetRequestStream();
                using (var sw = new StreamWriter(stream))
                {
                    foreach (var postParamKey in postParams.Keys)
                    {
                        var postParam = postParams[postParamKey];
                        sw.WriteLine("{0}={1}", postParamKey, postParam);
                    }                    
                }
            }
        }

        private static string CreateAuthorizationHeader (OAuthRequest oauthReq)
        {
            var authParams = FlattenValues(oauthReq, OAuthParamaterSource.AuthorizationHeader);
            return authParams.Keys.Any() ? new OAuthAuthorizationHeader(authParams).Content : null;
        }


        private static IDictionary<string, string> FlattenValues(OAuthRequest oauthReq, OAuthParamaterSource source)
        {
            var authParams = new Dictionary<string, string>();
            foreach (var name in oauthReq.GetParameterNames(source))
            {
                var values = oauthReq.GetParameterValues(source, name).ToList();
                var sb = new StringBuilder();
                for (var idx = 0; idx < values.Count; idx++)
                {
                    sb.Append(values[idx]);
                    if (idx < values.Count - 1) sb.Append(',');

                }
                authParams.Add(name, sb.ToString());
            }
            return authParams;
        }

    }
}
