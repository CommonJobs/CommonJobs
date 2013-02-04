using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Epnuke.OAuth
{
    public class OAuthSigner
    {
        private readonly string _key;
        private readonly string _stringToSign;
        public static string SIGNATURE_METHOD = "HMAC-SHA1";

        public OAuthSigner(string key, string stringToSign)
        {
            _key = key;
            _stringToSign = stringToSign;
        }

        public string GetSignature()
        {
            var keyBytes = Encoding.UTF8.GetBytes(_key);
            var stringToSignBytes = Encoding.UTF8.GetBytes(_stringToSign);
            var signer = new HMACSHA1(keyBytes);
            var hash = signer.ComputeHash(stringToSignBytes);
            var signature = Convert.ToBase64String(hash);
            return signature;
        }

    }
}