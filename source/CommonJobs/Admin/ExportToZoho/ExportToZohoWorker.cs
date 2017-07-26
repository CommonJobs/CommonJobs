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

        public ExportToZohoWorker(
            IOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public Task Run(ExportToZohoOptions options)
        {
            _outputHelper.DumpObject(options);
            return Task.CompletedTask;
        }
    }
}
