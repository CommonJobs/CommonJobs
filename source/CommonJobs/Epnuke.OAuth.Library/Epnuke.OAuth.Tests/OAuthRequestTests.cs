using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epnuke.OAuth.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class OAuthRequestTests
    {
        private string _headerValue;
        private string _headerValueWithEmptyParams;

        private const string CONSUMER_KEY = "0685bd9184jfhq22";
        private const string TOKEN = "ad180jjd733klru7";
        private const string SIGNATURE_METHOD = "HMAC-SHA1";
        private const string SIGNATURE = "wOJIO9A2W5mFwDgiDvZbTSMK%2FPY%3D";
        private const string TIMESTAMP = "137131200";
        private const string NONCE = "4572616e48616d6d65724c61686176";
        private const string VERSION = "1.0";


        public OAuthRequestTests()
        {
            _headerValue =
                string.Format(
                @"OAuth oauth_consumer_key=""{0}"",
                oauth_token=""{1}"",
                oauth_signature_method=""{2}"",
                oauth_signature=""{3}"",
                oauth_timestamp=""{4}"",
                oauth_nonce=""{5}"",
                oauth_version=""{6}""", CONSUMER_KEY, TOKEN, SIGNATURE_METHOD, SIGNATURE, TIMESTAMP, NONCE, VERSION);

            _headerValueWithEmptyParams =
                string.Format(
                @"OAuth oauth_consumer_key=""{0}"",
                oauth_token=""{1}"",
                oauth_signature_method=""{2}"",
                oauth_signature=""{3}"",
                oauth_timestamp=""{4}"",
                oauth_nonce=""{5}"",
                oauth_version=""{6}""", CONSUMER_KEY, TOKEN, string.Empty, SIGNATURE, TIMESTAMP, NONCE, VERSION);

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CorrectNumOfParametersAreIncluded()
        {

            var dict = new OAuthAuthorizationHeader(_headerValue).Parse();
            var data = new OAuthRequest("http://foo", "post");
            data.AddRange(dict, OAuthParamaterSource.AuthorizationHeader);
            Assert.AreEqual(7, data.ParametersCount);
        }

        [TestMethod]
        public void AllParametersAreIncluded()
        {
            var dict = new OAuthAuthorizationHeader(_headerValue).Parse();
            var data = new OAuthRequest("http://foo", "post");
            data.AddRange(dict, OAuthParamaterSource.AuthorizationHeader);
            data.ValidateOAuthData();
            Assert.AreEqual(CONSUMER_KEY, data.ConsumerKey);
            Assert.AreEqual(TOKEN, data.Token);
            Assert.AreEqual(SIGNATURE, data.Signature);
            Assert.AreEqual(SIGNATURE_METHOD, data.SignatureMethod);
            Assert.AreEqual(NONCE, data.Nonce);
            Assert.AreEqual(TIMESTAMP, data.Timestamp);
            Assert.AreEqual(VERSION, data.Version);
        }


        [TestMethod]
        public void EmptyParametersAreIncluded()
        {
            var dict = new OAuthAuthorizationHeader(_headerValueWithEmptyParams).Parse();
            var data = new OAuthRequest("http://foo", "post");
            data.AddRange(dict, OAuthParamaterSource.AuthorizationHeader);
            data.ValidateOAuthData();
            Assert.IsTrue(data.HasSignatureMethod);
            Assert.AreEqual(string.Empty, data.SignatureMethod);
        }

        [TestMethod]
        public void NotEmptyParametersAreIncludedIfAnotherWasEmpty()
        {
            var dict = new OAuthAuthorizationHeader(_headerValue).Parse();
            var data = new OAuthRequest("http://foo", "post");
            data.AddRange(dict, OAuthParamaterSource.AuthorizationHeader);
            data.ValidateOAuthData();
            Assert.IsTrue(data.HasToken);
            Assert.AreEqual(TOKEN, data.Token);
        }

        /// <summary>
        /// Prueba que la cadena base de implementación sea correcta.
        /// Usa los valores de ejemplo del punto 3.4.1.1 la RFC 5849
        /// </summary>
        [TestMethod]
        public void SignBaseStringIsCorrect()
        {
            var req = new OAuthRequest("http://example.com/request", "post");
            req.AddParameter("oauth_consumer_key", "9djdj82h48djs9d2", OAuthParamaterSource.AuthorizationHeader);
            req.AddParameter("oauth_token", "kkk9d7dh3k39sjv7", OAuthParamaterSource.AuthorizationHeader);
            req.AddParameter("oauth_signature_method", "HMAC-SHA1", OAuthParamaterSource.AuthorizationHeader);
            req.AddParameter("oauth_timestamp", "137131201", OAuthParamaterSource.AuthorizationHeader);
            req.AddParameter("oauth_nonce", "7d8f3e4a", OAuthParamaterSource.AuthorizationHeader);
            req.AddParameter("b5", "=%3D", OAuthParamaterSource.QueryString);
            req.AddParameter("a3", "a", OAuthParamaterSource.QueryString);
            req.AddParameter("c@", string.Empty, OAuthParamaterSource.Form);
            req.AddParameter("a2", "r b", OAuthParamaterSource.Form);
            req.AddParameter("c2", string.Empty, OAuthParamaterSource.Form);
            req.AddParameter("a3", "2 q", OAuthParamaterSource.Form);
            req.ValidateOAuthData();
            var sbase = req.GetSignatureBaseString();

            var result =
                "POST&http%3A%2F%2Fexample.com%2Frequest&a2%3Dr%2520b%26a3%3D2%2520q%26a3%3Da%26b5%3D%253D%25253D%26c%2540%3D%26c2%3D%26oauth_consumer_key%3D9djdj82h48djs9d2%26oauth_nonce%3D7d8f3e4a%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D137131201%26oauth_token%3Dkkk9d7dh3k39sjv7";
            Assert.AreEqual(result, sbase);
        }

        [TestMethod]
        public void QueryStringParamsAreAdded()
        {
            var req = new OAuthRequest("http://example.com/request?p1=v1&p2=v2", "post");
            Assert.AreEqual(req.ParametersCount, 2);
        }

        [TestMethod]
        public void EmptyQueryStringDoesNotAddAnyParam()
        {
            var req = new OAuthRequest("http://example.com/request?", "post");
            Assert.AreEqual(0, req.ParametersCount);
        }

        [TestMethod]
        public void EmptyParamInQueryStringIsAddedWithNoValue()
        {
            var req = new OAuthRequest("http://example.com/request?a=", "post");
            Assert.AreEqual(1, req.ParametersCount);
        }

        [TestMethod]
        public void ParameterNamesInQueryStringArePreserved()
        {
            var req = new OAuthRequest("http://example.com/request?a=1", "post");
            Assert.AreEqual(req.GetParameterNames(OAuthParamaterSource.QueryString).Count(), 1);
            Assert.AreEqual("a", req.GetParameterNames(OAuthParamaterSource.QueryString).First());
        }

        [TestMethod]
        public void ParameterValuesInQueryStringArePreserved()
        {
            var req = new OAuthRequest("http://example.com/request?a=value", "post");
            Assert.AreEqual(req.GetParameterValues(OAuthParamaterSource.QueryString, "a").Count(), 1);
            Assert.AreEqual("value", req.GetParameterValues(OAuthParamaterSource.QueryString, "a").First());
        }

        [TestMethod]
        public void MultipleQueryStringParameterIsProcessed()
        {
            var req = new OAuthRequest("http://example.com/request?a=value&a=value2", "post");
            CollectionAssert.AreEqual(new string[] {"a"}, req.GetParameterNames(OAuthParamaterSource.QueryString).ToArray());
            CollectionAssert.AreEqual(new string[] { "value", "value2" }, req.GetParameterValues(OAuthParamaterSource.QueryString, "a").ToArray());
        }

        [TestMethod]
        public void NormalizedUriIsCorrect()
        {
            var req = new OAuthRequest("http://example.com/request?a=value&a2=value2&a3=value3", "post");
            Assert.AreEqual("http://example.com/request", req.NormalizedUri);
        }
    }
}
