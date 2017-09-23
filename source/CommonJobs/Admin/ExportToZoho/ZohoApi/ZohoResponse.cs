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
        public ZohoResult Result { get; }

        public ZohoResponse(ZohoResult result)
        {
            Result = result;
        }

        public static ZohoResponse Parse(string xmlSring) =>
            Parse(ReadXml(xmlSring));

        public static ZohoResponse Parse(XmlDocument xml) =>
            Parse(xml["response"]);

        public static ZohoResponse Parse(XmlElement xml) =>
            new ZohoResponse(ZohoResult.Parse(xml["result"]));

        private static XmlDocument ReadXml(string xmlSring)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlSring);
            return xml;
        }
    }

    public class ZohoResult
    {
        public string Message { get; private set; }
        public RecordDetail[] Details { get; private set; } = new RecordDetail[0];

        public static ZohoResult CreateActionResponse(string message, IEnumerable<RecordDetail> details) =>
            new ZohoResult()
            {
                Message = message,
                Details = details.ToArray()
            };

        public static ZohoResult Parse(XmlElement xml) =>
            new ZohoResult()
            {
                Message = xml["message"]?.InnerText,
                Details = xml.GetElementsByTagName("recorddetail").Cast<XmlElement>().Select(x => new RecordDetail(x)).ToArray(),
            };
    }

    public class RowsCollection
    {
        public Row[] Rows { get; }

        public RowsCollection(IEnumerable<Row> rows)
        {
            Rows = rows.ToArray();
        }

        public RowsCollection(XmlElement xml)
            : this(xml.GetElementsByTagName("row").Cast<XmlElement>().Select(x => new Row(x)))
        {
        }

        public XmlElement ToXml(XmlDocument doc, string elementName)
        {
            var el = doc.CreateElement(elementName);
            foreach (var row in Rows)
            {
                el.AppendChild(row.ToXml(doc, "row"));
            }
            return el;
        }
    }

    public abstract class FieldsElement
    {
        public ZohoField[] Fields { get; }

        public int CountFields() => Fields.Length;

        public bool ContainsField(string fieldName) =>
            Fields.Any(x => x.Name == fieldName);

        public string GetString(string fieldName) =>
            Fields.Where(x => x.Name == fieldName).Select(x => x.GetString()).FirstOrDefault();


        public virtual XmlElement ToXml(XmlDocument doc, string elementName)
        {
            var el = doc.CreateElement(elementName);
            foreach (var field in Fields)
            {
                el.AppendChild(field.ToXml(doc, "FL"));
            }
            return el;
        }

        protected FieldsElement(IEnumerable<ZohoField> fields)
        {
            Fields = fields.ToArray();
        }

        protected FieldsElement(XmlElement xml)
            : this(xml.GetElementsByTagName("FL").Cast<XmlElement>()
                .Select(x => new ZohoField(x)))
        {
        }
    }

    public class RecordDetail : FieldsElement
    {
        public RecordDetail(IEnumerable<ZohoField> fields) : base(fields)
        {
        }

        public RecordDetail(XmlElement xml) : base(xml)
        {
        }
    }

    public class Row : FieldsElement
    {
        public int Number { get; }

        public XmlElement ToXml(XmlDocument doc, string elementName)
        {
            var el = base.ToXml(doc, elementName);
            el.SetAttribute("no", Number.ToString());
            return el;
        }

        public Row(int number, IEnumerable<ZohoField> fields) : base(fields)
        {
            Number = number;
        }

        public Row(XmlElement xml) : base(xml)
        {
            Number = int.Parse(xml.GetAttribute("no"));
        }
    }

    public class ZohoField
    {
        public string Name { get; }
        public string Value { get; }

        public ZohoField(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public ZohoField(XmlElement xml)
            : this(xml.GetAttribute("val"), xml.InnerText)
        {
        }

        public string GetString() => Value;

        public XmlElement ToXml(XmlDocument doc, string elementName)
        {
            var el = doc.CreateElement(elementName);
            el.SetAttribute("val", Name);
            el.InnerText = Value;
            return el;
        }
    }
}
