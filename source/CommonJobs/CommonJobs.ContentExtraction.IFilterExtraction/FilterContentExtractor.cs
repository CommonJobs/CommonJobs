using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonJobs.ContentExtraction.IFilterExtraction;
using System.Text.RegularExpressions;
using System.Globalization;
using CommonJobs.Utilities;

namespace CommonJobs.ContentExtraction.Extractors
{
    public class FilterContentExtractor : IContentExtractor
    {
        public bool TryExtract(string fullPath, Stream stream, string fileName, out ExtractionResult result)
        {
            var builder = new CleanTextBuilder();
            var reader = new FilterReader(fullPath, fileName);
            if (reader.Filtered)
            {
                builder.Add(reader);

                result = new ExtractionResult()
                {
                    ContentType = null,
                    PlainContent = builder.ToString()
                };

                return true;
            }
            result = null;
            return false;
        }
    }
}
