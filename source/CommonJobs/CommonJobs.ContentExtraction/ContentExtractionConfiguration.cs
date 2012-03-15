using System;
using System.Collections.Generic;
using System.Linq;
using CommonJobs.Utilities;

namespace CommonJobs.ContentExtraction
{
    public class ContentExtractionConfiguration : List<IContentExtractor>
    {
        //TODO: hacer algo mejor que un singleton para esto
        public static readonly ContentExtractionConfiguration Current = new ContentExtractionConfiguration();

        public override int GetHashCode()
        {
            return string.Join("; ", this.Select(x => x.GetType().AssemblyQualifiedName)).GetHashCode();
        }

        public string HashCode
        {
            get { return Encoding.IntToBase64urlEncoding(GetHashCode()); }
        }

    }
}
