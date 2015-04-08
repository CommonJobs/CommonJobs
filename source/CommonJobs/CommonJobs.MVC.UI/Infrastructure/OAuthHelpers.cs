using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace CommonJobs.Mvc.UI.Infrastructure
{
    public static class OAuthHelpers
    {
        public static string BuildUri(string url, string path, NameValueCollection query)
        {
            var uriBuilder = new UriBuilder(url)
            {
                Path = path,
                Query = ConstructQueryString(query)
            };
            return uriBuilder.ToString();
        }

        public static String ConstructQueryString(NameValueCollection parameters, string separator = "&")
        {
            return String.Join(separator,
                parameters.Cast<string>().Select(parameter => parameter + "=" + parameters[parameter])
                );
        }

        public static string Load(string address)
        {
            using (var webClient = new WebClient())
            {
                return Encoding.UTF8.GetString(webClient.DownloadData(address));
            }
        }

        public static string PostRequest(string url, NameValueCollection param)
        {
            using (var wb = new WebClient())
            {
                return Encoding.UTF8.GetString(wb.UploadValues(url, "POST", param));
            }
        }

        public static dynamic GetObjectFromAddress(string address)
        {
            return JObject.Parse(Load(address));
        }

        public static dynamic GetObjectWithPost(string url, NameValueCollection param)
        {
            return JObject.Parse(PostRequest(url, param));
        }

    }
}
