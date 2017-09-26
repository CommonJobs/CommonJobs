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
        public NoData NoData { get; }

        public ZohoResponse(ZohoResult result)
        {
            Result = result;
        }

        public ZohoResponse(NoData noData)
        {
            NoData = noData;
        }

        public static ZohoResponse Parse(string xmlSring) =>
            Parse(ReadXml(xmlSring));

        public static ZohoResponse Parse(XmlDocument xml) =>
            Parse(xml["response"]);

        public static ZohoResponse Parse(XmlElement xml)
        {
            var result = xml["result"];
            var noData = xml["nodata"];
            if (result != null && noData == null)
            {
                return new ZohoResponse(ZohoResult.Parse(result));
            }
            else if (result == null && noData != null)
            {
                return new ZohoResponse(NoData.Parse(noData));
            }
            else
            {
                throw new InvalidOperationException("Invalid XML");
            }
        }

        private static XmlDocument ReadXml(string xmlSring)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlSring);
            return xml;
        }
    }

    public class NoData
    {
        public string Message { get; private set; }
        public int Code { get; private set; }

        public static NoData Parse(XmlElement xml) =>
            new NoData()
            {
                Message = xml["message"]?.InnerText,
                Code = int.Parse(xml["code"]?.InnerText)
            };
    }

    public class ZohoResult
    {
        public string Message { get; private set; }
        public RecordDetail[] Details { get; private set; } = new RecordDetail[0];
        public RowsCollection Candidates { get; private set; }

        public static ZohoResult CreateActionResponse(string message, IEnumerable<RecordDetail> details) =>
            new ZohoResult()
            {
                Message = message,
                Details = details.ToArray()
            };

        public static ZohoResult CreateGetCandidatesResponse(RowsCollection candidatesData) =>
            new ZohoResult()
            {
                Candidates = candidatesData
            };

        public static ZohoResult Parse(XmlElement xml) =>
            new ZohoResult()
            {
                Message = xml["message"]?.InnerText,
                Details = xml.GetElementsByTagName("recorddetail").Cast<XmlElement>().Select(x => new RecordDetail(x)).ToArray(),
                Candidates = xml["Candidates"] == null ? null : new RowsCollection(xml["Candidates"])
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
