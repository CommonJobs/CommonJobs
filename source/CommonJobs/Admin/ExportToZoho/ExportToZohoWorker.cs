using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ExportToZoho
{
    public class ExportToZohoWorker : ICliWorker<ExportToZohoOptions>
    {
        public ExportToZohoWorker(
            IOutputHelper outputHelper)
        {

        }

        public Task Run(ExportToZohoOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
