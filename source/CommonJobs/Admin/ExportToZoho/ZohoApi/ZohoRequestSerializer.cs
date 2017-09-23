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
            var rows = new RowsCollection(MapCandidates(candidates));
            doc.AppendChild(rows.ToXml(doc, "Candidates"));
            return doc;
        }

        private static IEnumerable<Row> MapCandidates(IEnumerable<Candidate> candidates)
        {
            var rowNo = 0;
            foreach (var candidate in candidates)
            {
                rowNo++;
                yield return new Row(rowNo, MapCandidate(candidate));
            }
        }

        private static IEnumerable<ZohoField> MapCandidate(Candidate candidate)
        {
            return new[]
            {
                new ZohoField("Last Name", candidate.LastName ),
                new ZohoField("Email", candidate.Email ),
                new ZohoField("Perfiles", candidate.Perfiles )
            };
        }
    }
}
