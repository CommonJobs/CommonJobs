using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Epnuke.OAuth.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Epnuke.OAuth.Tests
{
    [TestClass]
    public class OAuthGenericResultTests
    {

        private StringBuilder _sbOutput;

        [TestInitialize]
        public void Init()
        {
            _sbOutput = new StringBuilder();
        }

        class HttpResponseBaseMock : HttpResponseBase
        {
            public override int StatusCode { get; set; }
            public override string ContentType { get; set; }
            public override TextWriter Output { get; set; }
        }

        private Mock<ControllerContext> GetConfiguredMock()
        {
            var response = new HttpResponseBaseMock();
            response.Output = new StringWriter(_sbOutput);
            var contextMock = new Mock<HttpContextBase>();
            contextMock.Setup(x => x.Response).Returns(response);
            var mock = new Mock<ControllerContext>();
            mock.Setup(x => x.HttpContext).Returns(contextMock.Object);
            return mock;
        }

        [TestMethod]
        public void NullDataIsAllowed()
        {
            var mock = GetConfiguredMock();
            var result = new OAuthGenericResult(null, 200);
            result.ExecuteResult(mock.Object);
            Assert.AreEqual(string.Empty, _sbOutput.ToString());
        }

        [TestMethod]
        public void DataIsSerializedInFormFormat()
        {
            var mock = GetConfiguredMock();
            var result = new OAuthGenericResult(new {p1="value1", p2="value2"}, 200);
            result.ExecuteResult(mock.Object);
            Assert.AreEqual("p1=value1&p2=value2",_sbOutput.ToString());
        }

        [TestMethod]
        public void HttpCodeIsPreserved()
        {
            var mock = GetConfiguredMock();
            var result = new OAuthGenericResult(new { p1 = "value1", p2 = "value2" }, 404);
            result.ExecuteResult(mock.Object);
            Assert.AreEqual(404, mock.Object.HttpContext.Response.StatusCode);
        }

        [TestMethod]
        public void ResultUsesCorrectContentType()
        {
            var mock = GetConfiguredMock();
            var result = new OAuthGenericResult(new { p1 = "value1", p2 = "value2" }, 200);
            result.ExecuteResult(mock.Object);
            Assert.AreEqual("application/x-www-form-urlencoded", mock.Object.HttpContext.Response.ContentType);
        }
    }
}
