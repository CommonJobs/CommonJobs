using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epnuke.OAuth
{
    public class NonceGenerator32Bytes : INonceGenerator
    {
        public string CreateNonce()
        {
            var rnd = new Random();
            var buffer = new byte[32];
            rnd.NextBytes(buffer);
            var base64 = Convert.ToBase64String(buffer);
            return base64.Select(x => char.IsLetterOrDigit(x) ? x.ToString() : string.Empty).Aggregate((x, y) => x + y);
        }
    }
}
