using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Utilities
{
    public static class Encoding
    {
        public static string Base64ToBase64urlEncoding(string base64)
        {
            return base64.TrimEnd('=').Replace("+", "-").Replace("/", "_");
        }

        public static string IntToBase64urlEncoding(int number)
        {
            return Base64ToBase64urlEncoding(Convert.ToBase64String(BitConverter.GetBytes(number)));
        }
    }
}
