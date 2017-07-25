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
    [Verb("export-to-zoho", HelpText = "Export applicants to Zoho")]
    public class ExportToZohoOptions : BaseCliOptions
    {
    }
}
