using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Vici.Mvc.Test.OfflineBrowserTests
{
    [TestFixture]
    public class LanguageUrlTest
    {
        private OfflineWebSession _browser;

        [TestFixtureSetUp]
        public void SetupSession()
        {
            _browser = BrowserFactory.SetupBrowser();
        }

        [Test]
        public void TestDefaultLanguage()
        {
            _browser.PageGet("/index");

            Assert.AreEqual(WebAppConfig.DefaultLanguage, WebAppContext.Session.LanguageCode);
        }

        [Test]
        public void TestSpecificLanguageInUrl()
        {
            WebAppConfig.UseLanguagePath = true;

            _browser.PageGet("/nl/index");

            Assert.AreEqual("nl", WebAppContext.Session.LanguageCode);

            _browser.PageGet("/fr/index");

            Assert.AreEqual("fr", WebAppContext.Session.LanguageCode);
        }



    }
}
