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

        public static Encoding GetFileEncoding(Stream stream)
        {
            //Stolen from http://www.west-wind.com/weblog/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader
            // *** Use Default of Encoding.Default (Ansi CodePage)
            Encoding enc = Encoding.Default;

            // *** Detect byte order mark if any - otherwise assume default
            byte[] buffer = new byte[5];
            stream.Position = 0; //TODO: buscar una forma mas elegante de hacer esto
            stream.Read(buffer, 0, 5);

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                enc = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                enc = Encoding.UTF7;

            return enc;
        }

        public bool TryExtract(string fullPath, Stream stream, string fileName, out ExtractionResult result)
        {
            result = null;
            if (IsExtractable(fileName))
            {
                var encoding = GetFileEncoding(stream);

                stream.Position = 0; //TODO: buscar una forma mas elegante de hacer esto

                var text = new StreamReader(stream, encoding).ReadToEnd();
                //TODO: mejorar este parche feo y caro para soportar a la vez ansi y utf sin bom
                if (text.Contains("Ã"))
                {
                    var bytes = encoding.GetBytes(text);
                    text = Encoding.UTF8.GetString(bytes);
                }

                result = new ExtractionResult()
                {
                    ContentType = "text/plain",
                    PlainContent = text
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
