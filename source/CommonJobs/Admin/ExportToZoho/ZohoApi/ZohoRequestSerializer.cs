using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Admin.ExportToZoho.ZohoApi
{
    public class ZohoRequestSerializer : IZohoRequestSerializer
    {
        public string Serialize(params Candidate[] candidates) =>
            CreateCandidatesDocument(candidates).InnerXml;

        private static XmlDocument CreateCandidatesDocument(IEnumerable<Candidate> candidates)
        {
            var doc = new XmlDocument();
            doc.AppendChild(CreateCandidatesElement(doc, candidates));
            return doc;
        }

        private static XmlElement CreateCandidatesElement(XmlDocument doc, IEnumerable<Candidate> candidates)
        {
            var el = doc.CreateElement("Candidates");
            var rowNo = 0;
            foreach (var candidate in candidates)
            {
                el.AppendChild(CreateRowElement(doc, ++rowNo, candidate));
            }
            return el;
        }

        private static XmlElement CreateRowElement(XmlDocument doc, int rowNo, Candidate candidate)
        {
            var el = doc.CreateElement("row");
            el.SetAttribute("no", rowNo.ToString());
            el.AppendChild(CreateFieldElement(doc, "Last Name", candidate.LastName));
            el.AppendChild(CreateFieldElement(doc, "Email", candidate.Email));
            el.AppendChild(CreateFieldElement(doc, "Perfiles", candidate.Perfiles));
            return el;
        }

        private static XmlElement CreateFieldElement(XmlDocument doc, string fieldName, string filedValue)
        {
            var el = doc.CreateElement("FL");
            el.SetAttribute("val", fieldName);
            el.InnerText = filedValue;
            return el;
        }
    }
}
