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
            var expectedRequestBody = "xmlData=%3CCandidates%3E%3Crow+no%3D%221%22%3E%3CFL+val%3D%22Last+Name%22%3Etesting%3C%2FFL%3E%3CFL+val%3D%22Email%22%3Etest7%40test.com%3C%2FFL%3E%3CFL+val%3D%22Perfiles%22%3EQA+Manual%3C%2FFL%3E%3C%2Frow%3E%3C%2FCandidates%3E";

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

        private ZohoClient CreateSut() =>
            new ZohoClient(
                new ZohoConfiguration() { Password = "_PASSWORD_", Token = "_TOKEN_", Username = "_USERNAME_" },
                new ZohoRequestSerializer());
    }
}
