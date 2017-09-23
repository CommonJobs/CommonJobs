using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ExportToZoho
{
    public class ListZohoCandidatesWorker : ICliWorker<ListZohoCandidatesOptions>
    {
        private readonly IOutputHelper _outputHelper;
        private readonly Func<ZohoConfiguration, IZohoClient> _zohoClientFactory;

        public ListZohoCandidatesWorker(
            IOutputHelper outputHelper,
            Func<ZohoConfiguration, IZohoClient> zohoClientFactory)
        {
            _outputHelper = outputHelper;
            _zohoClientFactory = zohoClientFactory;
        }

        public async Task Run(ListZohoCandidatesOptions options)
        {
            _outputHelper.DumpObject(options);
            using (var client = _zohoClientFactory(options.ZohoConfiguration))
            {
                await client.LoginIfNeedAsync();
                var result = await client.GetCandidatesAsync(20, 22);
                _outputHelper.DumpObject(result);
            }
        }
    }
}
