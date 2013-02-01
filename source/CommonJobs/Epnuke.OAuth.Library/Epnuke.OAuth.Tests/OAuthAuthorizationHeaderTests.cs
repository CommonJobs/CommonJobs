using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epnuke.OAuth.Tests
{
    [TestClass]
    public class OAuthAuthorizationHeaderTests
    {
        [TestMethod]
        public void CtorWithDictMakesContentStartWithOAuth()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("oauth_consumerkey", "fookey");
            dict.Add("oauth_version", "1.0");
            var source = new OAuthAuthorizationHeader(dict);
            var contentString = source.Content;
            Assert.IsTrue(contentString.TrimStart().StartsWith("OAuth"));
        }

        [TestMethod]
        public void ParseIsInverseOfCtorWithDictionary()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("oauth_consumerkey", "fookey");
            dict.Add("oauth_version", "1.0");
            var source = new OAuthAuthorizationHeader(dict);
            var dictParsed = source.Parse();
            Assert.AreEqual(2, dictParsed.Keys.Count);
            Assert.IsTrue(dictParsed.ContainsKey("oauth_consumerkey"));
            Assert.IsTrue(dictParsed.ContainsKey("oauth_version"));
            Assert.AreEqual("fookey", dictParsed["oauth_consumerkey"]);
            Assert.AreEqual("1.0", dictParsed["oauth_version"]);
        }

        [TestMethod]
        public void AppendValueAppendsItAtEndOfContent()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("oauth_consumerkey", "fookey");
            dict.Add("oauth_version", "1.0");
            var source = new OAuthAuthorizationHeader(dict);
            source.AppendOAuthValue("oauth_signature","xxx==A");
            Assert.IsTrue(source.Content.TrimEnd().EndsWith(@"oauth_signature=""xxx==A"""));
        }

        [TestMethod]
        public void ParseWorksSuccesfullyAfterAppendValue()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("oauth_consumerkey", "fookey");
            dict.Add("oauth_version", "1.0");
            var source = new OAuthAuthorizationHeader(dict);
            source.AppendOAuthValue("oauth_signature", "xxx==A");
            var dictParsed = source.Parse();
            Assert.IsTrue(dictParsed.ContainsKey("oauth_signature"));
            Assert.AreEqual(3, dictParsed.Keys.Count);
            Assert.AreEqual("xxx==A", dictParsed["oauth_signature"]);
        }


    }
}
