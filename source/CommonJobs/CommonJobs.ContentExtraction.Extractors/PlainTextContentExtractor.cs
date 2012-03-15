using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonJobs.ContentExtraction.Extractors
{
    public class PlainTextContentExtractor : IContentExtractor
    {
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string> { ".txt", ".md", ".markdown" };

        private bool IsExtractable(string fileName)
        {
            //TODO: it should not be based on extensions. I should be based in content
            return AllowedExtensions.Contains(Path.GetExtension(fileName));
        }

        public bool TryExtract(string fileName, Stream stream, out ExtractionResult result)
        {
            result = null;
            if (IsExtractable(fileName))
            {
                //TODO: Mejorar el soporte para archivos ANSI

                stream.Position = 0; //TODO: buscar una forma mas elegante de hacer esto
                result = new ExtractionResult()
                {
                    ContentType = "text/plain",
                    PlainContent = new StreamReader(stream, true).ReadToEnd()
                };
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
