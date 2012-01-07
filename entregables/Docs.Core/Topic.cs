using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Text.RegularExpressions;

namespace Docs.Core {
    public class Topic {
        private const string MetadataFile = "_metadata";
        private static readonly HashSet<string> DocumentExtensions = new HashSet<string> { ".markdown"/*, ".cshtml"*/ };
        private static readonly HashSet<string> IndexFiles = new HashSet<string> { "index.markdown", "index.cshtml" };
        public string Title { get; private set; }
        public string Url { get; private set; }
        public IEnumerable<Topic> SubTopics { get; private set; }

        public Topic() {
            SubTopics = new List<Topic>();
        }

        /// <summary>
        /// Gets a list of topics from the a directory. Only supports one level of nesting.
        /// </summary>
        public static IEnumerable<Topic> GetTopics(string virtualPath) {
            VirtualDirectory topicDir = HostingEnvironment.VirtualPathProvider.GetDirectory(virtualPath);
            return GetTopics(topicDir);
        }

        private static IEnumerable<Topic> GetTopics(VirtualDirectory topicDir)
        {
            var directoryMetadata = GetDirectoryMetadata(topicDir.VirtualPath);
            return from directory in topicDir.Directories.Cast<VirtualDirectory>()
                   let title = GetTitle(directory)
                   let metadata = GetMetadata(title, directoryMetadata)
                   orderby metadata.Order, title
                   select new Topic
                   {
                       Title = title,
                       SubTopics = GetSubTopics(directory)
                   };
        }

        /// <summary>
        /// Gets a list of topics from the a directory. Only supports one level of nesting.
        /// </summary>
        public static IEnumerable<Topic> GetSubTopics(string virtualPath)
        {
            VirtualDirectory subtopicDir = HostingEnvironment.VirtualPathProvider.GetDirectory(virtualPath);
            return GetSubTopics(subtopicDir);
        }

        private static IEnumerable<Topic> GetSubTopics(VirtualDirectory directory)
        {
            var directoryMetadata = GetDirectoryMetadata(directory.VirtualPath);
            var files = from file in directory.Files.Cast<VirtualFile>()
                   let subTitle = GetTitle(file)
                   let subMetadata = GetMetadata(subTitle, directoryMetadata)
                   where !subTitle.Equals(MetadataFile, StringComparison.OrdinalIgnoreCase)
                        && DocumentExtensions.Contains(Path.GetExtension(file.Name))
                   select new 
                   {
                       Title = subTitle,
                       Url = GetUrl(file),
                       Order = subMetadata.Order
                   };

            var subdirectories = from subdirectory in directory.Directories.Cast<VirtualDirectory>()
                   let subTitle = GetTitle(subdirectory)
                   let subMetadata = GetMetadata(subTitle, directoryMetadata)
                   where subdirectory.Files.Cast<VirtualFile>().Any(x => IndexFiles.Contains(x.Name))
                   select new 
                   {
                       Title = subTitle,
                       Url = GetUrl(subdirectory),
                       Order = subMetadata.Order
                   };

            return files.Union(subdirectories)
                .OrderBy(x => x.Order).ThenBy(x => x.Title)
                .Select(x => new Topic() { Title = x.Title, Url = x.Url });
                  
        }

        private static Dictionary<string, Metadata> GetDirectoryMetadata(string virtualPath)
        {
            var vpp = HostingEnvironment.VirtualPathProvider;
            string metadataFile = VirtualPathUtility.AppendTrailingSlash(virtualPath) + MetadataFile;

            var mapping = new Dictionary<string, Metadata>();
            int index = 0;
            if (vpp.FileExists(metadataFile))
            {
                VirtualFile file = vpp.GetFile(metadataFile);
                Stream stream = file.Open();
                using (var reader = new StreamReader(stream))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        mapping[Normalize(line)] = new Metadata
                        {
                            Order = index++
                        };
                    }
                }
            }
            return mapping;
        }

        private static Metadata GetMetadata(string title, Dictionary<string, Metadata> mapping)
        {
            Metadata metadata;
            if (mapping.TryGetValue(title, out metadata)) {
                return metadata;
            }
            return Metadata.Empty;
        }

        private static string GetTitle(VirtualDirectory dir) {
            return Normalize(dir.VirtualPath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Last());
        }

        private static string GetTitle(VirtualFile file) {
            return Normalize(Path.GetFileNameWithoutExtension(file.VirtualPath));
        }

        private static string Normalize(string path) {
            return path.Replace("-", " ").Trim();
        }

        private static string GetUrl(VirtualFile file) {
            string dir = VirtualPathUtility.GetDirectory(file.VirtualPath);
            string filePath = Path.GetFileNameWithoutExtension(file.VirtualPath);
            return VirtualPathUtility.Combine(dir, filePath).ToLowerInvariant();
        }

        private static string GetUrl(VirtualDirectory directory)
        {
                return directory.VirtualPath.ToLowerInvariant();
        }

        private class Metadata {
            public static readonly Metadata Empty = new Metadata() {
                Order = Int32.MaxValue
            };

            public int Order { get; set; }
        }
    }
}
