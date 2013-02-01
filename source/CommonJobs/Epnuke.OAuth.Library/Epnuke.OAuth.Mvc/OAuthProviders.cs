using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epnuke.OAuth.Mvc
{
    public static class OAuthProviders
    {
        public static ITokenProvider TokenProvider { get; set; }
    }
}
