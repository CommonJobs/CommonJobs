using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonJobs.ContentExtraction
{
    public interface IContentExtractor
    {
        bool TryExtract(string fileName, Stream stream, out ExtractionResult result);
    }
}
