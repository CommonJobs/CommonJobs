using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonJobs.ContentExtraction.IFilterExtraction;

namespace CommonJobs.ContentExtraction.Extractors
{
    public class FilterContentExtractor : IContentExtractor
    {
        public bool TryExtract(string fullPath, Stream stream, string fileName, out ExtractionResult result)
        {
            result = null;
            var reader = new FilterReader(fullPath, fileName);
            if (reader.Filtered)
            {
                result = new ExtractionResult()
                {
                    ContentType = null,
                    PlainContent = reader.ReadToEnd()
                };
                return true;
            }
            return false;
        }
    }
}
