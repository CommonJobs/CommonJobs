using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ExportToZoho
{
    public class ExportToZohoWorker : ICliWorker<ExportToZohoOptions>
    {
        private readonly IOutputHelper _outputHelper;
        private readonly Func<ZohoConfiguration, IZohoClient> _zohoClientFactory;

        public ExportToZohoWorker(
            IOutputHelper outputHelper,
            Func<ZohoConfiguration, IZohoClient> zohoClientFactory)
        {
            _outputHelper = outputHelper;
            _zohoClientFactory = zohoClientFactory;
        }

        public async Task Run(ExportToZohoOptions options)
        {
            _outputHelper.DumpObject(options);
            using (var client = _zohoClientFactory(options.ZohoConfiguration))
            {
                await client.LoginIfNeedAsync();
                var id = await client.CreateCandidateAsync(new Candidate()
                {
                    Email = "test8@test.com",
                    LastName = "Test"
                });
            }
        }
    }
}
