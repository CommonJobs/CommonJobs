using Microsoft.VisualStudio.TestTools.UnitTesting;
using Admin.ExportToZoho.ZohoApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ExportToZoho.ZohoApi
{
    [TestClass]
    public class ZohoResponseTests
    {
        [TestMethod]
        public void Parse_Test()
        {
            // Arrange
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<response uri=""/recruit/private/xml/Candidates/addRecords"">
    <result>
        <message>Record(s) added successfully</message>
        <recorddetail>
            <FL val=""Id"">384840000000262001</FL>
            <FL val=""Created Time"">2017-07-26 16:09:50</FL>
            <FL val=""Modified Time"">2017-07-26 16:09:50</FL>
            <FL val=""Created By""><![CDATA[Gabriel Hernan]]></FL>
            <FL val=""Modified By""><![CDATA[Gabriel Hernan]]></FL>
            </recorddetail>
        <recorddetail>
            <FL val=""Id"">384840000000262002</FL>
            <FL val=""Created Time"">2017-07-26 16:09:50</FL>
            <FL val=""Modified Time"">2017-07-26 16:09:50</FL>
            <FL val=""Created By""><![CDATA[Gabriel Hernan]]></FL>
            <FL val=""Modified By""><![CDATA[Gabriel Hernan]]></FL>
        </recorddetail>
    </result>
</response>";

            // Act
            var response = ZohoResponse.Parse(xml);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual("Record(s) added successfully", response.Result.Message);
            Assert.AreEqual(2, response.Result.Details.Count());
            var item0 = response.Result.Details[0];
            Assert.IsNotNull(item0);
            Assert.AreEqual(5, item0.CountFields());
            Assert.IsTrue(item0.ContainsField("Id"));
            Assert.IsFalse(item0.ContainsField("NOEXISTE"));
            Assert.AreEqual("384840000000262001", item0.GetString("Id"));
            var item1 = response.Result.Details[1];
            Assert.IsNotNull(item1);
            Assert.AreEqual(5, item1.CountFields());
            Assert.IsTrue(item1.ContainsField("Id"));
            Assert.AreEqual("384840000000262002", item1.GetString("Id"));
        }
    }
}
