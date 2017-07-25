using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tavis.UriTemplates;

namespace Admin.ExportToZoho
{
    public class ZohoClient : IZohoClient
    {
        private readonly FlurlClient _http = new FlurlClient();
        private readonly ZohoConfiguration _configuration;
        private string token = null;

        public ZohoClient(ZohoConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            token = null;
            _http.Dispose();
        }

        public async Task LoginAsync()
        {
            var url = new UriTemplate(_configuration.LoginUriTemplate)
                .AddParameter("username", _configuration.Username)
                .AddParameter("password", _configuration.Password)
                .Resolve();

            var response = await _http.WithUrl(url).PostStringAsync(string.Empty).ReceiveString();

            var match = Regex.Match(response, @"AUTHTOKEN=(\w+)");
            if (!match.Success)
            {
                throw new ApplicationException($"Authentication error. {response}");
            }
            token = match.Groups[1].Value;
        }
    }
}
