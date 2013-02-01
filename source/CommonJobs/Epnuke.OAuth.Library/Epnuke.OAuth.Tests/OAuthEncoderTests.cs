using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epnuke.OAuth.Tests
{
    /// <summary>
    /// Summary description for OAuthEncoderTests
    /// </summary>
    [TestClass]
    public class OAuthEncoderTests
    {

        [TestMethod]
        public void SpecialCharsAreEncoded()
        {
            var param = "=%3D";
            var encoded = new OAuthEncoder().Encode(param);
            Assert.AreEqual("%3D%253D", encoded);
        }

        [TestMethod]
        public void SpecialCharsAreDecoded()
        {
            var encoded = "%3D%253D";
            var decoded = new OAuthEncoder().Decode(encoded);
            Assert.AreEqual("=%3D", decoded);
        }

        [TestMethod]
        public void NormalCharsAreNotEncoded()
        {
            var param = "abcdE1";
            var encoded = new OAuthEncoder().Encode(param);
            Assert.AreEqual(param, encoded);
        }

        [TestMethod]
        public void NormalCharsAreNotDecoded()
        {
            var param = "abcdE1";
            var decoded = new OAuthEncoder().Decode(param);
            Assert.AreEqual(param, decoded);
        }


        [TestMethod]
        public void SpaceIsEncodedAsHex20()
        {
            var param = "r b";
            var encoded = new OAuthEncoder().Encode(param);
            Assert.AreEqual("r%20b", encoded);            
        }

        [TestMethod]
        public void Hex20IsDecodedAsSpace()
        {
            var encoded = "r%20b";
            var decoded = new OAuthEncoder().Decode(encoded);
            Assert.AreEqual("r b", decoded);                        
        }

        [TestMethod]
        public void RareCharactersAreEncoded()
        {
            var rare ="☃";
            var encoded = new OAuthEncoder().Encode(rare);
            Assert.AreEqual("%E2%98%83", encoded);
        }

        [TestMethod]
        public void RareCharactersAreDecoded()
        {
            var rare = "☃";
            var encoded = "%E2%98%83";
            var decoded = new OAuthEncoder().Decode(encoded);
            Assert.AreEqual(rare, decoded);
        }

        [TestMethod]
        public void SpecialCharsAreEncoded2()
        {
            var param = "Dogs, Cats & Mice";
            var encoded = new OAuthEncoder().Encode(param);
            Assert.AreEqual("Dogs%2C%20Cats%20%26%20Mice", encoded);
        }

        [TestMethod]
        public void SpecialCharsAreDecoded2()
        {
            var encoded = "Dogs%2C%20Cats%20%26%20Mice";
            var decoded = new OAuthEncoder().Decode(encoded);
            Assert.AreEqual("Dogs, Cats & Mice", decoded);
        }


        [TestMethod]
        public void PlusIsEncoded()
        {
            var param = "Ladies + Gentlemen";
            var encoded = new OAuthEncoder().Encode(param);
            Assert.AreEqual("Ladies%20%2B%20Gentlemen", encoded);
        }

        [TestMethod]
        public void PlusIsDecoded()
        {
            var encoded = "Ladies%20%2B%20Gentlemen";
            var decoded= new OAuthEncoder().Decode(encoded);
            Assert.AreEqual("Ladies + Gentlemen", decoded);
        }
    }
}
