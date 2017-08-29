using System.Collections.Generic;
using System.Xml;

namespace Admin.ExportToZoho.ZohoApi
{
    public interface IZohoRequestSerializer
    {
        string Serialize(params Candidate[] candidates);
    }
}
