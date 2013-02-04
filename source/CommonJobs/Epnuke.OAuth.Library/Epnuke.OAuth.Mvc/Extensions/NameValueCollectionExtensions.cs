using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Epnuke.OAuth.Mvc.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static IDictionary<string, string> AsDictionary(this NameValueCollection self)
        {
            return self.Keys.Cast<string>().ToDictionary(key => key, key => self[key]);
        }
    }
}
