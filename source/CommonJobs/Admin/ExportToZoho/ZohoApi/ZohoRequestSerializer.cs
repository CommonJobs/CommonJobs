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
                yield return new Row(rowNo, MapCandidateToFields(candidate));
            }
        }

        private static IEnumerable<ZohoField> MapCandidateToFields(Candidate candidate)
        {
            ZohoField field;
            if (TryExtractField("First Name", candidate.FirstName, out field)) { yield return field; }
            if (TryExtractField("Last Name", candidate.LastName, out field)) { yield return field; }
            if (TryExtractField("Email", candidate.Email, out field)) { yield return field; }
            if (TryExtractField("Mobile", candidate.Mobile, out field)) { yield return field; }
            if (TryExtractField("Skype ID", candidate.SkypeId, out field)) { yield return field; }
            if (TryExtractField("City", candidate.City, out field)) { yield return field; }
            if (TryExtractField("Country", candidate.Country, out field)) { yield return field; }
            if (TryExtractField("Experience In Years", candidate.ExperienceInYears, out field)) { yield return field; }
            if (TryExtractField("Current Salary", candidate.CurrentSalary, out field)) { yield return field; }
            if (TryExtractField("Expected Salary", candidate.ExpectedSalary, out field)) { yield return field; }
            if (TryExtractField("Current Employer", candidate.CurrentEmployer, out field)) { yield return field; }
            if (TryExtractField("Additional Info", candidate.AdditionalInfo, out field)) { yield return field; }
            if (TryExtractField("Source", candidate.Source, out field)) { yield return field; }
            if (TryExtractField("Linkedin", candidate.LinkedIn, out field)) { yield return field; }
            if (TryExtractField("Candidate Status", candidate.CandidateStatus, out field)) { yield return field; }
            if (TryExtractField("Perfiles", candidate.Perfiles, out field)) { yield return field; }
            if (TryExtractField("Stack Predominante", candidate.StackPredominante, out field)) { yield return field; }
            if (TryExtractField("Sitio Web Personal", candidate.PersonalWebsite, out field)) { yield return field; }
            if (TryExtractField("Is Hot Candidate", candidate.IsHotCandidate, out field)) { yield return field; }
        }

        private static bool TryExtractField(string fieldName, string fieldValue, out ZohoField field)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                field = null;
                return false;
            }

            field = new ZohoField(fieldName, fieldValue.ToString());
            return true;
        }

        private static bool TryExtractField<T>(string fieldName, T fieldValue, out ZohoField field)
        {
            if (fieldValue == null)
            {
                field = null;
                return false;
            }

            field = new ZohoField(fieldName, fieldValue.ToString());
            return true;
        }
    }
}
