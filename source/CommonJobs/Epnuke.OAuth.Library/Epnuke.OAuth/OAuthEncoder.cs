using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Epnuke.OAuth
{
    public class OAuthEncoder
    {

        public string Encode(string decodedUri)
        {
            if (string.IsNullOrEmpty(decodedUri)) return string.Empty;

            var finalBytes = new List<byte>();
            var bytes = Encoding.UTF8.GetBytes(decodedUri);
            foreach (var ubyte in bytes)
            {

                if ((ubyte >= 0x30 && ubyte <= 0x39) ||
                     (ubyte >= 0x41 && ubyte <= 0x5A) ||
                     (ubyte >= 0x61 && ubyte <= 0x7A) ||
                    ubyte == 0x2D || ubyte == 0x2E || ubyte == 0x5F || ubyte == 0x7E)
                {
                    finalBytes.Add(ubyte);
                }
                else
                {
                    finalBytes.AddRange(Encoding.ASCII.GetBytes(string.Format("%{0:X}", ubyte)));
                }
            }
            var encodedUri = Encoding.ASCII.GetString(finalBytes.ToArray());
            return encodedUri;
        }

        public string Decode(string encodedUri)
        {
            if (string.IsNullOrEmpty(encodedUri)) return string.Empty;

            var finalBytes = new List<byte>();
            var bytes = Encoding.ASCII.GetBytes(encodedUri);
            int skip = 0;
            for (var idx = 0; idx < bytes.Length; idx++)
            {
                var ubyte = bytes[idx];
                if (skip > 0)
                {
                    skip--;
                    continue;
                }

                if (ubyte == 0x25)
                {
                    skip = 2;
                    var hexString = Encoding.ASCII.GetString(new byte[] { bytes[idx + 1], bytes[idx + 2] });
                    var hexVal = int.Parse(hexString, NumberStyles.HexNumber);
                    finalBytes.Add((byte)hexVal);
                }
                else
                {
                    finalBytes.Add(ubyte);
                }
            }

            return Encoding.UTF8.GetString(finalBytes.ToArray());
        }
    }
}
