using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Vici.Mvc.Test.OfflineBrowserTests
{
    [TestFixture]
    public class UrlTests
    {
        private OfflineWebSession _browser;

        [TestFixtureSetUp]
        public void SetupSession()
        {
            _browser = BrowserFactory.SetupBrowser();
        }

        [Test]
        public void MapPath()
        {
            Assert.AreEqual(_browser.RootPath + "\\index", WebAppContext.Server.MapPath("~/index"));
        }

        [Test]
        public void TranslateAbsolutePath()
        {
            Assert.AreEqual("/index",PathHelper.TranslateAbsolutePath("~/index"));
        }

        [Test]
        public void UrlEncoding()
        {
            string toEncode = "lejkrzhfkljehrzglkjehrzgjklhejklrzg";

            Assert.AreEqual(toEncode, UrlHelper.DecodeFromUrl(UrlHelper.EncodeToUrl(toEncode)));
        }

    }
}
