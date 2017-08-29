using Microsoft.VisualStudio.TestTools.UnitTesting;
using Admin.ExportToZoho.ZohoApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Admin.ExportToZoho.ZohoApi
{
    [TestClass]
    public class RequestSerializerTests
    {
        [TestMethod]
        public void ToXmlString_Test()
        {
            // Arrange
            var expectedXml = RemoveExtraSpaces(@"
<Candidates>
    <row no=""1"">
        <FL val=""Last Name"">testing</FL>
        <FL val=""Email"">test7@test.com</FL>
        <FL val=""Perfiles"">QA Manual</FL>
    </row>
    <row no=""2"">
        <FL val=""Last Name"">testing</FL>
        <FL val=""Email"">test8@test.com</FL>
        <FL val=""Perfiles"">QA Manual</FL>
    </row>
</Candidates>"); ;

            var candidate1 = new Candidate()
            {
                LastName = "testing",
                Email = "test7@test.com",
                Perfiles = "QA Manual"
            };

            var candidate2 = new Candidate()
            {
                LastName = "testing",
                Email = "test8@test.com",
                Perfiles = "QA Manual"
            };

            var sut = new ZohoRequestSerializer();

            // Act
            var requestString = sut.Serialize(candidate1, candidate2);

            // Assert
            Assert.AreEqual(expectedXml, requestString);
        }

        private static string RemoveExtraSpaces(string expectedXml) =>
            Regex.Replace(expectedXml.Trim(), @">\s+<", "><");
    }
}
