using CommonJobs.Raven.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;

namespace CommonJobs.Raven.Mvc.Test
{
    
    
    /// <summary>
    ///This is a test class for ScriptManagerTest and is intended
    ///to contain all ScriptManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ScriptManagerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        public class ViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }
            public ViewDataContainer()
            {
                ViewData = new ViewDataDictionary();
            }
        }

        [TestMethod()]
        public void RegisterCss_Basic()
        {
            var viewContext = new ViewContext();
            var viewDataContainer = new ViewDataContainer();

            var path = "path";
            var priority = 123;
            var omitAppVersion = true;
            var expected = "<link href=\"path\" rel=\"stylesheet\" type=\"text/css\" />";
            object htmlAttributes = null;

            ScriptManager target = ScriptManager.GetFromViewData(viewDataContainer.ViewData);
            target.RegisterCss(path, priority, htmlAttributes, omitAppVersion);
            var htmlHelper = new HtmlHelper<dynamic>(viewContext, viewDataContainer);
            var result = htmlHelper.RenderScriptManagerEntries().ToHtmlString().Trim();

            Assert.AreEqual(result, expected);
        }

        [TestMethod()]
        public void RegisterCss_Patch()
        {
            var path = "path";
            var priority = 123;
            var omitAppVersion = true;
            object htmlAttributes = null;

            var viewContext = new ViewContext();
            var viewDataContainer = new ViewDataContainer();
            var scriptManager = ScriptManager.GetFromViewData(viewDataContainer.ViewData);
            scriptManager.RegisterCss(path, priority, htmlAttributes, omitAppVersion);
            var htmlHelper = new HtmlHelper<dynamic>(viewContext, viewDataContainer);
            var expected = "<!--[if lte IE 9]>" + htmlHelper.RenderScriptManagerEntries().ToHtmlString().Trim() + "<![endif]-->";

            var viewContext2 = new ViewContext();
            var viewDataContainer2 = new ViewDataContainer();
            var scriptManager2 = ScriptManager.GetFromViewData(viewDataContainer2.ViewData);
            scriptManager2.RegisterCss(path, priority, htmlAttributes, omitAppVersion, patchCondition: "lte IE 9");
            var htmlHelper2 = new HtmlHelper<dynamic>(viewContext2, viewDataContainer2);
            var result = htmlHelper2.RenderScriptManagerEntries().ToHtmlString().Trim();

            Assert.AreEqual(result, expected);
        }

        [TestMethod()]
        public void RenderGlobalJavascript_NotEncodeQuotes()
        {
            var viewContext = new ViewContext();
            var viewDataContainer = new ViewDataContainer();

            var expected = "<script type=\"text/javascript\">window.variable = \"value\";</script>";

            var scriptManager = ScriptManager.GetFromViewData(viewDataContainer.ViewData);
            scriptManager.RegisterGlobalJavascript("variable", "value" );
            var htmlHelper = new HtmlHelper<dynamic>(viewContext, viewDataContainer);
            var result = htmlHelper.RenderScriptManagerEntries().ToHtmlString().Trim();

            Assert.AreEqual(result, expected);
        }

    }
}
