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
                var pageSize = 100;
                var allResults = new List<ZohoApi.FieldsElement>();
                var from = 0;
                while (true)
                {
                    var fromIndex = from;
                    var to = from + pageSize;
                    _outputHelper.WriteLine($"Asking for candidates ({from} - {to})...");
                    var response = await client.GetCandidatesAsync(from, to);
                    if (response.NoData != null)
                    {
                        break;
                    }
                    var resultCount = response.Result.Candidates.Rows.Length;
                    _outputHelper.WriteLine($"Done ({resultCount} results)");
                    allResults.AddRange(response.Result.Candidates.Rows);
                    from = from + resultCount;
                }
                _outputHelper.WriteLine($"All results:");
                _outputHelper.DumpObject(allResults);
            }
        }
    }
}
