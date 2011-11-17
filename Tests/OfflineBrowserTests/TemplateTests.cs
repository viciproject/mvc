using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Vici.Mvc.Test.OfflineBrowserTests
{
    [TestFixture]
    public class TemplateTests
    {
        private OfflineWebSession _browser;

        [TestFixtureSetUp]
        public void SetupSession()
        {
            _browser = BrowserFactory.SetupBrowser();

            WebAppConfig.Router.ClearRoutingTable();
            WebAppConfig.LoadControllerClasses();    // re-reads Url attributes
        }

        [Test]
        public void MasterTemplate()
        {
            string html = _browser.PageGet("~/index");

            Assert.IsTrue(html.Contains("<!-- HEADMARKER -->"));
        }

        [Test]
        public void BlankView()
        {
            string html = _browser.PageGet("~/blank");

            Assert.IsTrue(html.Contains("<!-- HEADMARKER -->"));
            Assert.IsTrue(html.Contains("<body> </body>"));
        }
    }
}
