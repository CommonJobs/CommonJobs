using CommandLine;
using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ExportToZoho
{
    [Verb("list-zoho-candidates", HelpText = "List Zoho candidates")]
    public class ListZohoCandidatesOptions : BaseCliOptions, IValidatableObject
    {
        [Option('z', "zoho-config", Required = false, HelpText = "File Zoho configuration")]
        public string ZohoConfigurationFile { get; set; } = "ExportToZoho/ZohoConfiguration.json";

        private readonly Lazy<ZohoConfiguration> _lazyZohoConfiguration;
        public ZohoConfiguration ZohoConfiguration => _lazyZohoConfiguration.Value;

        public ListZohoCandidatesOptions()
        {
            _lazyZohoConfiguration = new Lazy<ZohoConfiguration>(() => LoadConfiguration<ZohoConfiguration>(ZohoConfigurationFile));
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!TryValidateConfigurationFile(_lazyZohoConfiguration, nameof(ZohoConfigurationFile), out var error))
            {
                yield return error;
            }
        }

        private bool TryValidateConfigurationFile<T>(Lazy<T> lazyProperty, string memberName, out ValidationResult validationResult)
        {
            try
            {
                // Force load configuration
                var configuration = lazyProperty.Value;
                // TODO: Support ValidationAttributes and IValidatableObject in configuration values.
                // TODO: Move all this loading and validation logic into a reusable place.
                validationResult = null;
                return true;
            }
            catch (Exception e)
            {
                validationResult = new ValidationResult($"Error reading configuration file: {e.Message}", new[] { memberName });
                return false;
            }
        }

        private T LoadConfiguration<T>(string file)
        {
            var serializer = new JsonSerializer() { TypeNameHandling = TypeNameHandling.Auto };
            using (var s = GetStream(file))
            using (var sr = new StreamReader(s))
            using (var reader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(reader);
            }
        }

        private Stream GetStream(string jsonURI)
        {
            var uri = new Uri(jsonURI, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri || uri.IsFile)
            {
                return new FileStream(jsonURI, FileMode.Open);
            }
            else
            {
                return jsonURI.GetStreamAsync().GetAwaiter().GetResult();
            }
        }

    }
}
