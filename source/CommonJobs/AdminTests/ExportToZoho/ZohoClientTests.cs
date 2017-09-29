using Microsoft.VisualStudio.TestTools.UnitTesting;
using Admin.ExportToZoho.ZohoApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using System.Text.RegularExpressions;

namespace Admin.ExportToZoho
{
    [TestClass]
    public class ZohoClientTests
    {
        [TestMethod]
        public async Task CreateCandidateAsync_Test()
        {
            // Arrange
            var sut = CreateSut();
            var expectedId = "384840000000262001";
            var expectedLastName = "testing";
            var expectedEmail = "test7@test.com";
            var expectedPerfiles = "QA Manual";
            var expectedUrl = "https://recruit.zoho.com/recruit/private/xml/Candidates/addRecords?duplicateCheck=1&scope=recruitapi&version=2&authtoken=_TOKEN_";
            var candidate = new Candidate()
            {
                LastName = expectedLastName,
                Email = expectedEmail,
                Perfiles = expectedPerfiles
            };
            // Sadly it is wrong:
            // var expectedRequestBody = RemoveExtraSpaces(@"
            // <Candidates>
            //     <row no=""1"">
            //         <FL val=""Last Name"">testing</FL>
            //         <FL val=""Email"">test7@test.com</FL>
            //         <FL val=""Perfiles"">QA Manual</FL>
            //     </row>
            // </Candidates>");
            // In fact, it is very ugly :P
            var expectedRequestBody = "xmlData=%3CCandidates%3E%3Crow+no%3D%221%22%3E%3CFL+val%3D%22Last+Name%22%3Etesting%3C%2FFL%3E%3CFL+val%3D%22Email%22%3Etest7%40test.com%3C%2FFL%3E%3CFL+val%3D%22Source%22%3ECommonJobs%3C%2FFL%3E%3CFL+val%3D%22Perfiles%22%3EQA+Manual%3C%2FFL%3E%3C%2Frow%3E%3C%2FCandidates%3E";

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith($@"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<response uri=""/recruit/private/xml/Candidates/addRecords"">
    <result>
        <message>Record(s) added successfully</message>
        <recorddetail>
            <FL val=""Id"">{expectedId}</FL>
            <FL val=""Created Time"">2017-07-26 16:09:50</FL>
            <FL val=""Modified Time"">2017-07-26 16:09:50</FL>
            <FL val=""Created By""><![CDATA[Gabriel Hernan]]></FL>
            <FL val=""Modified By""><![CDATA[Gabriel Hernan]]></FL>
            </recorddetail>
    </result>
</response>");

                // Act
                var response = await sut.CreateCandidateAsync(candidate);

                // Assert
                httpTest.ShouldHaveCalled(expectedUrl)
                    .WithRequestBody(expectedRequestBody);

                Assert.AreEqual("Record(s) added successfully", response.Result.Message);
                Assert.AreEqual(1, response.Result.Details.Count());
                var id = response.Result.Details.First().GetString("Id");
                Assert.AreEqual(expectedId, id);
            }
        }

        [TestMethod]
        public async Task GetCandidatesAsync_Test()
        {
            // Arrange
            var sut = CreateSut();
            var expectedUrl = "https://recruit.zoho.com/recruit/private/xml/Candidates/getRecords?fromIndex=0&toIndex=20&scope=recruitapi&version=2&authtoken=_TOKEN_";

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(
                #region Large Response
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response uri=""/recruit/private/xml/Candidates/getRecords"">
    <result>
        <Candidates>
            <row no=""1"">
                <FL val=""CANDIDATEID"">384840000000374174</FL>
                <FL val=""Candidate ID""><![CDATA[ZR_370_CAND]]></FL>
                <FL val=""First Name""><![CDATA[Laura]]></FL>
                <FL val=""Last Name""><![CDATA[Perez]]></FL>
                <FL val=""Email""><![CDATA[lau.perez@gmail.com]]></FL>
                <FL val=""Mobile""><![CDATA[0351156123456]]></FL>
                <FL val=""City""><![CDATA[Cordoba]]></FL>
                <FL val=""Country""><![CDATA[Argentina]]></FL>
                <FL val=""SMCREATORID"">384840000000154171</FL>
                <FL val=""Created By""><![CDATA[Julia Rodrigues]]></FL>
                <FL val=""MODIFIEDBY"">384840000000154171</FL>
                <FL val=""Modified By""><![CDATA[Julia Rodrigues]]></FL>
                <FL val=""Created Time""><![CDATA[2017-09-21 09:59:21]]></FL>
                <FL val=""Updated On""><![CDATA[2017-09-21 10:00:13]]></FL>
                <FL val=""Last Activity Time""><![CDATA[2017-09-22 12:41:20]]></FL>
                <FL val=""SMOWNERID"">444840000000154444</FL>
                <FL val=""Candidate Owner""><![CDATA[Julia Rodrigues]]></FL>
                <FL val=""Source""><![CDATA[LinkedIn]]></FL>
                <FL val=""Email Opt Out""><![CDATA[false]]></FL>
                <FL val=""Is Locked""><![CDATA[false]]></FL>
                <FL val=""Is Unqualified""><![CDATA[false]]></FL>
                <FL val=""Is Attachment Present""><![CDATA[true]]></FL>
                <FL val=""Candidate Status""><![CDATA[Desclasificado]]></FL>
                <FL val=""Linkedin""><![CDATA[https://www.linkedin.com/in/laura-perez-4b0b1234/]]></FL>
                <FL val=""Perfiles""><![CDATA[Fullstack Developer]]></FL>
                <FL val=""Stack Predominante""><![CDATA[.Net]]></FL>
                <FL val=""Subcontracting""><![CDATA[false]]></FL>
                <FL val=""Is Hot Candidate""><![CDATA[false]]></FL>
            </row>
            <row no=""2"">
                <FL val=""CANDIDATEID"">123440000000371234</FL>
                <FL val=""Candidate ID""><![CDATA[ZR_369_CAND]]></FL>
                <FL val=""First Name""><![CDATA[Leonardo Alberto]]></FL>
                <FL val=""Last Name""><![CDATA[Smith]]></FL>
                <FL val=""Email""><![CDATA[leosmith@gmail.com]]></FL>
                <FL val=""Mobile""><![CDATA[+54 379 489-1234]]></FL>
                <FL val=""City""><![CDATA[Resistencia]]></FL>
                <FL val=""Country""><![CDATA[Argentina]]></FL>
                <FL val=""SMCREATORID"">384840000000154171</FL>
                <FL val=""Created By""><![CDATA[Julia Rodrigues]]></FL>
                <FL val=""MODIFIEDBY"">384840000000154171</FL>
                <FL val=""Modified By""><![CDATA[Julia Rodrigues]]></FL>
                <FL val=""Created Time""><![CDATA[2017-09-21 09:38:28]]></FL>
                <FL val=""Updated On""><![CDATA[2017-09-21 09:38:28]]></FL>
                <FL val=""Last Activity Time""><![CDATA[2017-09-21 09:38:47]]></FL>
                <FL val=""SMOWNERID"">384840000000154171</FL>
                <FL val=""Candidate Owner""><![CDATA[Julia Rodrigues]]></FL>
                <FL val=""Source""><![CDATA[LinkedIn]]></FL>
                <FL val=""Email Opt Out""><![CDATA[false]]></FL>
                <FL val=""Is Locked""><![CDATA[false]]></FL>
                <FL val=""Is Unqualified""><![CDATA[false]]></FL>
                <FL val=""Is Attachment Present""><![CDATA[false]]></FL>
                <FL val=""Candidate Status""><![CDATA[Esperando evaluación]]></FL>
                <FL val=""Linkedin""><![CDATA[https://www.linkedin.com/in/leonardo-alberto-smith-123a0b12]]></FL>
                <FL val=""Perfiles""><![CDATA[QA Automation]]></FL>
                <FL val=""Stack Predominante""><![CDATA[Java;NodeJs]]></FL>
                <FL val=""Subcontracting""><![CDATA[false]]></FL>
                <FL val=""Is Hot Candidate""><![CDATA[false]]></FL>
            </row>
        </Candidates>
    </result>
</response>");
                #endregion

                // Act
                var result = await sut.GetCandidatesAsync();

                // Assert
                httpTest.ShouldHaveCalled(expectedUrl);

                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count());
            }
        }

        private ZohoClient CreateSut() =>
            new ZohoClient(
                new ZohoConfiguration() { Password = "_PASSWORD_", Token = "_TOKEN_", Username = "_USERNAME_" },
                new ZohoRequestSerializer());
    }
}
