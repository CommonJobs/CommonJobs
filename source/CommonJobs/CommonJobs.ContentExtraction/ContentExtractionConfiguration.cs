using System;
using System.Collections.Generic;
using System.Linq;
using CommonJobs.Utilities;
using System.IO;

namespace CommonJobs.ContentExtraction
{
    public class ContentExtractionConfiguration : List<IContentExtractor>, IContentExtractor
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

        public bool TryExtract(string fullPath, Stream stream, string fileName, out ExtractionResult result)
        {
            result = null;
            foreach (var extractor in this)
            {
                try
                {
                    if (extractor.TryExtract(fullPath, stream, fileName, out result))
                        return true;
                }
                catch
                {
                    //Extractor fails, omit it.
                }
            }
            return false;
        }
    }
}
