using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace CommonJobs.Raven.Mvc
{
    public class RequestAttachmentReader
    {
        public string FileNameRequestParameter { get; set; }

        public readonly string FileName;
        public readonly Stream Stream;


        public RequestAttachmentReader(HttpRequestBase request, string fileNameRequestParameter = "fileName")
        {
            FileName = request.Params[fileNameRequestParameter] as string
                ?? request.Params["HTTP_X_FILE_NAME"] as string
                ?? Path.GetRandomFileName();

            // To handle differences in FireFox/Chrome/Safari/Opera
            Stream = request.Files.Count > 0
                ? request.Files[0].InputStream
                : request.InputStream;
        }
    }
}
