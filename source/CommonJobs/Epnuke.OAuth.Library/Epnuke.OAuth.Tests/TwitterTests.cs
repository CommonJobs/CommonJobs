using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epnuke.OAuth.Tests
{
    /// <summary>
    /// En esta clase NO se está probando ningún código específico para Twitter. El nombre viene porque
    /// los datos de estos tests están sacados de la página de desarrolladores de twitter
    /// https://dev.twitter.com/docs/auth/creating-signature
    /// </summary>
    [TestClass]
    public class TwitterTests
    {
        private OAuthRequest _twitterRequest;

        [TestInitialize]
        public void Init()
        {
            _twitterRequest = new OAuthRequest("https://api.twitter.com/1/statuses/update.json?include_entities=true","post");
            _twitterRequest.AddParameter("status", "Hello Ladies + Gentlemen, a signed OAuth request!", OAuthParamaterSource.Form);
            _twitterRequest.AddParameter("oauth_consumer_key", "xvz1evFS4wEEPTGEFPHBog", OAuthParamaterSource.AuthorizationHeader);
            _twitterRequest.AddParameter("oauth_nonce", "kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg", OAuthParamaterSource.AuthorizationHeader);
            _twitterRequest.AddParameter("oauth_signature_method", "HMAC-SHA1", OAuthParamaterSource.AuthorizationHeader);
            _twitterRequest.AddParameter("oauth_timestamp", "1318622958", OAuthParamaterSource.AuthorizationHeader);
            _twitterRequest.AddParameter("oauth_token", "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb", OAuthParamaterSource.AuthorizationHeader);
            _twitterRequest.AddParameter("oauth_version", "1.0", OAuthParamaterSource.AuthorizationHeader);
        }

        [TestMethod]
        public void ParameterStringIsAsExpected()
        {
            var pbs = _twitterRequest.GetParameterString();
            Assert.AreEqual("include_entities=true&oauth_consumer_key=xvz1evFS4wEEPTGEFPHBog&oauth_nonce=kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1318622958&oauth_token=370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb&oauth_version=1.0&status=Hello%20Ladies%20%2B%20Gentlemen%2C%20a%20signed%20OAuth%20request%21",pbs);
        }

        [TestMethod]
        public void SignatureBaseStringIsAsExpected()
        {
            var sbs = _twitterRequest.GetSignatureBaseString();
            Assert.AreEqual("POST&https%3A%2F%2Fapi.twitter.com%2F1%2Fstatuses%2Fupdate.json&include_entities%3Dtrue%26oauth_consumer_key%3Dxvz1evFS4wEEPTGEFPHBog%26oauth_nonce%3DkYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1318622958%26oauth_token%3D370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb%26oauth_version%3D1.0%26status%3DHello%2520Ladies%2520%252B%2520Gentlemen%252C%2520a%2520signed%2520OAuth%2520request%2521", sbs);
        }

        [TestMethod]
        public void SignatureIsCorrect()
        {
            var signature = _twitterRequest.ComputeSignature("kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw",
                                                             "LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE");
            Assert.AreEqual("tnnArxj06cWHq44gCs1OSKk/jLY=", signature);
        }
    }
}
