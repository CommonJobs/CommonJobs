using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.ContentExtraction
{
    public class ExtractionResult
    {
        public string ContentType { get; set; }
        public string PlainContent { get; set; }
        //TODO: permitir una coleccion de key/values para metadatos (Por ejemplo los de los archivos de word)
    }
}
