using Admin.ExportToZoho.ZohoApi;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private string _token = null;
        private readonly IZohoRequestSerializer _requestSerializer;

        public ZohoClient(ZohoConfiguration configuration, IZohoRequestSerializer requestSerializer)
        {
            _configuration = configuration;
            if (!string.IsNullOrEmpty(_configuration.Token))
            {
                _token = _configuration.Token;
            }
            _requestSerializer = requestSerializer;
        }

        public void Dispose()
        {
            _token = null;
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
            _token = match.Groups[1].Value;
        }

        public async Task LoginIfNeedAsync()
        {
            if (_token == null)
            {
                await LoginAsync();
            }
        }

        private string GetToken([CallerMemberName] string caller = "")
        {
            return _token ?? throw new ApplicationException($"Login is required for calling {caller}");
        }

        public async Task<ZohoResponse> CreateCandidateAsync(Candidate candidate)
        {
            var url = new UriTemplate(_configuration.GeneralUriTemplate)
                .AddParameter("module", "Candidates")
                .AddParameter("method", "addRecords")
                .AddParameter("duplicateCheck", 1)
                .AddParameter("token", GetToken())
                .Resolve();

            var response = await _http.WithUrl(url)
                .PostUrlEncodedAsync(new
                {
                    xmlData = _requestSerializer.Serialize(candidate)
                })
                .ReceiveString();

            var result = ZohoResponse.Parse(response);

            return result;
        }

        public async Task<IEnumerable<Candidate>> GetCandidatesAsync(int fromIndex = 0, int toIndex = 20)
        {
            var url = new UriTemplate(_configuration.GeneralUriTemplate)
                .AddParameter("module", "Candidates")
                .AddParameter("method", "getRecords")
                .AddParameter("fromIndex", fromIndex)
                .AddParameter("toIndex", toIndex)
                .AddParameter("token", GetToken())
                .Resolve();

            var responseBody = await _http.WithUrl(url)
                .GetAsync()
                .ReceiveString();

            var response = ZohoResponse.Parse(responseBody);

            if (response.NoData != null)
            {
                return Enumerable.Empty<Candidate>();
            }

            return response.Result.Candidates.Rows.Select(MapFieldsToCandidate);
        }

        private static Candidate MapFieldsToCandidate(FieldsElement fieldsElement) =>
            new Candidate()
            {
                FirstName = fieldsElement.GetString("First Name"),
                LastName = fieldsElement.GetString("Last Name"),
                Email = fieldsElement.GetString("Email"),
                Mobile = fieldsElement.GetString("Mobile"),
                SkypeId = fieldsElement.GetString("Skype ID"),
                City = fieldsElement.GetString("City"),
                Country = fieldsElement.GetString("Country"),
                ExperienceInYears = fieldsElement.GetInt("Experience In Years"),
                CurrentSalary = fieldsElement.GetDecimal("Current Salary"),
                ExpectedSalary = fieldsElement.GetDecimal("Expected Salary"),
                CurrentEmployer = fieldsElement.GetString("Current Employer"),
                AdditionalInfo = fieldsElement.GetString("Additional Info"),
                Source = fieldsElement.GetString("Source"),
                LinkedIn = fieldsElement.GetString("Linkedin"),
                CandidateStatus = fieldsElement.GetString("Candidate Status"),
                Perfiles = fieldsElement.GetString("Perfiles"),
                StackPredominante = fieldsElement.GetString("Stack Predominante"),
                PersonalWebsite = fieldsElement.GetString("Sitio Web Personal"),
                IsHotCandidate = fieldsElement.GetBoolean("Is Hot Candidate")
            };
    }
}
