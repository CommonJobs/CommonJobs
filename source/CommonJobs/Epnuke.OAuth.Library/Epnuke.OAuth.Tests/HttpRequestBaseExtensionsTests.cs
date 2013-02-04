using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Epnuke.OAuth.Mvc.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Epnuke.OAuth.Tests
{
    [TestClass]
    public class HttpRequestBaseExtensionsTests
    {

        private NameValueCollection GetServerVariables(string queryString)
        {
            var nvc = new NameValueCollection();
            nvc.Add("QUERY_STRING", queryString);
            nvc.Add("PATH_INFO", "/path1/path2");
            nvc.Add("HTTP_HOST", "www.wof.com");
            return nvc;
        }

        [TestMethod]
        public void QueryStringIsProcessed()
        {
            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.Form).Returns(new NameValueCollection());
            request.Setup(x => x.Headers).Returns(new NameValueCollection());
            request.Setup(x => x.ServerVariables).Returns(GetServerVariables("p1=value1"));
            var oauthRequest = request.Object.GetOAuthData();

            Assert.AreEqual(1, oauthRequest.ParametersCount);
            CollectionAssert.AreEqual(new[] { "p1" }, oauthRequest.GetParameterNames(OAuthParamaterSource.QueryString).ToArray());
            CollectionAssert.AreEqual(new[] { "value1" }, oauthRequest.GetParameterValues(OAuthParamaterSource.QueryString, "p1").ToArray());
        }

        [TestMethod]
        public void FormIsProcessed()
        {
            var form = new NameValueCollection();
            form.Add("p1", "value1");
            form.Add("p2", "value2");
            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.QueryString).Returns(new NameValueCollection());
            request.Setup(x => x.Form).Returns(form);
            request.Setup(x => x.Headers).Returns(new NameValueCollection());
            request.Setup(x => x.ServerVariables).Returns(GetServerVariables(string.Empty)); 
            var oauthRequest = request.Object.GetOAuthData();
            Assert.AreEqual(2, oauthRequest.ParametersCount);
            CollectionAssert.AreEqual(new[] { "p1", "p2" }, oauthRequest.GetParameterNames(OAuthParamaterSource.Form).ToArray());
            CollectionAssert.AreEqual(new[] { "value1" }, oauthRequest.GetParameterValues(OAuthParamaterSource.Form, "p1").ToArray());
            CollectionAssert.AreEqual(new[] { "value2" }, oauthRequest.GetParameterValues(OAuthParamaterSource.Form, "p2").ToArray());
        }

        [TestMethod]
        public void AuthHeaderIsProcessed()
        {
            var headers = new NameValueCollection();
            var headerData =
                @"OAuth oauth_consumer_key=""ckey"",
                oauth_token=""token"",
                oauth_signature_method=""signature_method"",
                oauth_signature=""signature"",
                oauth_timestamp=""timestamp"",
                oauth_nonce=""nonce"",
                oauth_version=""version""";

            headers.Add("Authorization", headerData);
            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.Form).Returns(new NameValueCollection());
            request.Setup(x => x.Headers).Returns(headers);
            request.Setup(x => x.ServerVariables).Returns(GetServerVariables(string.Empty));
            var oauthRequest = request.Object.GetOAuthData();
            Assert.AreEqual(7, oauthRequest.ParametersCount);
            Assert.AreEqual("ckey", oauthRequest.ConsumerKey);
            Assert.AreEqual("token", oauthRequest.Token);
            Assert.AreEqual("signature_method", oauthRequest.SignatureMethod);
            Assert.AreEqual("signature", oauthRequest.Signature);
            Assert.AreEqual("timestamp", oauthRequest.Timestamp);
            Assert.AreEqual("nonce", oauthRequest.Nonce);
            Assert.AreEqual("version", oauthRequest.Version);
        }

        [TestMethod]
        public void OAuthRequestIsValidated()
        {
            var headers = new NameValueCollection();
            var headerData =
                @"OAuth oauth_consumer_key=""ckey"",
                oauth_version=""version""";
            headers.Add("Authorization", headerData);
            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.QueryString).Returns(new NameValueCollection());
            request.Setup(x => x.Form).Returns(new NameValueCollection());
            request.Setup(x => x.Headers).Returns(headers);
            request.Setup(x => x.ServerVariables).Returns(GetServerVariables(string.Empty));
            var oauthRequest = request.Object.GetOAuthData();
            Assert.IsTrue(oauthRequest.OAuthErrors.Contains(OAuthRequestError.InvalidVersion));

        }

        [TestMethod]
        public void UrlIsProcessed()
        {
            var url = "http://www.wof.com/path1/path2";
            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.QueryString).Returns(new NameValueCollection());
            request.Setup(x => x.Form).Returns(new NameValueCollection());
            request.Setup(x => x.Headers).Returns(new NameValueCollection());
            request.Setup(x => x.ServerVariables).Returns(GetServerVariables(string.Empty));
            var oauthRequest = request.Object.GetOAuthData();
            Assert.AreEqual(url,oauthRequest.FullUri);
        }

    }
}
