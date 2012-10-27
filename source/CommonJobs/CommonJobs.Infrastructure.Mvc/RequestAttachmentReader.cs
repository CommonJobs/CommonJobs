using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace CommonJobs.Infrastructure.Mvc
{
    public class RequestAttachmentReader : IDisposable, IEnumerable<KeyValuePair<string, Stream>>
    {
        private Dictionary<string, Stream> data = new Dictionary<string, Stream>();

        public Stream this[string fileName]
        {
            get { return data[fileName]; }
        }

        public IEnumerable<string> AllFileNames
        {
            get { return data.Keys; }
        }

        public int Count
        {
            get { return data.Count; }
        }

        public RequestAttachmentReader(HttpRequestBase request, string fileNameRequestParameter = "fileName")
        {
            // To handle differences in FireFox/Chrome/Safari/Opera
            if (request.Files == null || request.Files.Count == 0)
            {
                var filename =
                    request.Params[fileNameRequestParameter] as string
                    ?? request.Params["HTTP_X_FILE_NAME"] as string
                    ?? Path.GetRandomFileName();

                filename = Path.GetFileName(filename); 

                var stream = 
                    request.InputStream;

                data.Add(filename, stream);
            }
            else if (request.Files.Count == 1)
            {
                var filename =
                    request.Params[fileNameRequestParameter] as string
                    ?? request.Params["HTTP_X_FILE_NAME"] as string
                    ?? request.Files[0].FileName
                    ?? Path.GetRandomFileName();

                //IE
                filename = Path.GetFileName(filename); 

                var stream = 
                    request.Files[0].InputStream
                    ?? request.InputStream;

                data.Add(filename, stream);
            }
            else
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    var file = request.Files[i];

                    var filename = 
                        data.ContainsKey(file.FileName) ? Path.GetRandomFileName()
                        : file.FileName;

                    filename = Path.GetFileName(filename); 

                    var stream = file.InputStream;

                    data.Add(filename, stream);
                }
            }
        }

        public IEnumerator<KeyValuePair<string, Stream>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (data as System.Collections.IEnumerable).GetEnumerator();
        }

        public void Dispose()
        {
            var keys = AllFileNames.ToArray();
            foreach (var key in keys)
            {
                data[key].Dispose();
                data.Remove(key);
            }
        }
    }
}
