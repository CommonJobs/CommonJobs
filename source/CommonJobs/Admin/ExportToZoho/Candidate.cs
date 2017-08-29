using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Admin.ExportToZoho
{
    public class Candidate
    {
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Perfiles { get; set; }
    }
}
