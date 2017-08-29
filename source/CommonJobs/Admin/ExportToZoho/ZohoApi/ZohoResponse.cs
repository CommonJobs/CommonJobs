using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Admin.ExportToZoho.ZohoApi
{
    public class ZohoResponse
    {
        public ZohoResult Result { get; set; }

        public static ZohoResponse Parse(string xmlSring) =>
            Parse(ReadXml(xmlSring));

        public static ZohoResponse Parse(XmlDocument xml) =>
            Parse(xml["response"]);

        public static ZohoResponse Parse(XmlElement xml) =>
            new ZohoResponse()
            {
                Result = ZohoResult.Parse(xml["result"])
            };

        private static XmlDocument ReadXml(string xmlSring)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlSring);
            return xml;
        }
    }

    public class ZohoResult
    {
        public string Message { get; set; }
        public RecordDetail[] Details { get; set; } = new RecordDetail[0];

        public static ZohoResult Parse(XmlElement xml) =>
            new ZohoResult()
            {
                Message = xml["message"].InnerText,
                Details = xml.GetElementsByTagName("recorddetail").Cast<XmlElement>().Select(x => RecordDetail.Parse(x)).ToArray()
            };
    }

    public class RecordDetail
    {
        public ZohoField[] Fields { get; set; } = new ZohoField[0];

        public bool ContainsField(string fieldName) =>
            Fields.Any(x => x.Name == fieldName);

        public string GetFieldValue(string fieldName) =>
            Fields.FirstOrDefault(x => x.Name == fieldName)?.Value;

        public static RecordDetail Parse(XmlElement xml) =>
            new RecordDetail()
            {
                Fields = xml.GetElementsByTagName("FL").Cast<XmlElement>().Select(x => ZohoField.Parse(x)).ToArray()
            };
    }

    public class ZohoField
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public static ZohoField Parse(XmlElement xml) =>
            new ZohoField()
            {
                Name = xml.GetAttribute("val"),
                Value = xml.InnerText
            };
    }
}
