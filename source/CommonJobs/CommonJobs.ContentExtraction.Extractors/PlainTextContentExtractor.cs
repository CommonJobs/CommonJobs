using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonJobs.ContentExtraction.Extractors
{
    public class PlainTextContentExtractor : IContentExtractor
    {
        public bool TryExtract(string fileName, Stream stream, out ExtractionResult result)
        {
            stream.Position = 0; //TODO: buscar una forma mas elegante de hacer esto
            if (true /* TODO: verify encoding, etc */)
            {
                result = new ExtractionResult()
                {
                    ContentType = "text/plain",
                    PlainContent = new StreamReader(stream, true).ReadToEnd()
                };
                return true;
            }
            return false;
        }
    }
}
