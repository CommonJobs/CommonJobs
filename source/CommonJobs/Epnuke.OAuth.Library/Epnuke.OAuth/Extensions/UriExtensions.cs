using System;

namespace Epnuke.OAuth.Extensions
{
    public static class UriExtensions
    {
        public static string ToNormalizedString(this Uri @this)
        {
            return string.Format("{0}://{1}{2}{3}",
                     @this.Scheme,
                     @this.Host,
                     @this.IsDefaultPort ? string.Empty : string.Concat(":", @this.Port),
                     @this.AbsolutePath).ToLowerInvariant();
        }
    }
}
