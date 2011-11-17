using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Vici.Mvc.Test.OfflineBrowserTests
{
    [TestFixture]
    public class RoutingTests
    {
        private OfflineWebSession _browser;

        [TestFixtureSetUp]
        public void SetupSession()
        {
            _browser = BrowserFactory.SetupBrowser();

        }

        [SetUp]
        public void SetRouting()
        {
            WebAppConfig.Router.ClearRoutingTable();

            WebAppConfig.Router.AddRoute("{controller}/{action}/anyid/{id}", "{controller}", "{action}");
            WebAppConfig.Router.AddRoute("{controller}/{action}/mappedid/{param}", "{controller}", "{action}", new RouteParameter("id", "{param}"));
            WebAppConfig.Router.AddRoute("{controller}/{action}/numericid/{id}", "{controller}", "{action}", new RegexRouteValidator("id","\\d+"));
            WebAppConfig.Router.AddRoute("endless/{therest*}","Endless");

            WebAppConfig.Router.AddDefaultRoutes(null);
        }

        [Test]
        public void DefaultRoute1()
        {
            RouteResult routeResult = WebAppConfig.Router.Resolve("con");

            Assert.AreEqual(routeResult.Controller, "con");
            Assert.AreEqual(routeResult.Action, "Run");
            Assert.AreEqual(0, routeResult.Parameters.Count);
        }

        [Test]
        public void DefaultRoute2()
        {
            RouteResult routeResult = WebAppConfig.Router.Resolve("con/act");

            Assert.AreEqual(routeResult.Controller, "con");
            Assert.AreEqual(routeResult.Action, "act");
            Assert.AreEqual(0, routeResult.Parameters.Count);
        }

        [Test]
        public void DefaultRoute3()
        {
            RouteResult routeResult = WebAppConfig.Router.Resolve("con/act/id");

            Assert.AreEqual(routeResult.Controller, "con");
            Assert.AreEqual(routeResult.Action, "act");
            Assert.AreEqual(1, routeResult.Parameters.Count);
            Assert.AreEqual(routeResult.Parameters["id"], "id");
        }

        [Test]
        public void RouteAnyId()
        {
            RouteResult routeResult = WebAppConfig.Router.Resolve("con/act/anyid/500");

            Assert.AreEqual(routeResult.Controller, "con");
            Assert.AreEqual(routeResult.Action, "act");
            Assert.AreEqual(routeResult.Parameters["id"],"500");
        }

        [Test]
        public void RouteMappedId()
        {
            RouteResult routeResult = WebAppConfig.Router.Resolve("con/act/mappedid/99");

            Assert.AreEqual(routeResult.Controller, "con");
            Assert.AreEqual(routeResult.Action, "act");
            Assert.AreEqual(routeResult.Parameters["id"], "99");
        }

        [Test]
        public void NumericId()
        {
            RouteResult routeResult = WebAppConfig.Router.Resolve("con/act/numericid/75");

            Assert.AreEqual(routeResult.Controller, "con");
            Assert.AreEqual(routeResult.Action, "act");
            Assert.AreEqual(routeResult.Parameters["id"], "75");

            routeResult = WebAppConfig.Router.Resolve("con/act/numericid/75a");

            Assert.IsNull(routeResult);
        }

        [Test]
        public void Endless()
        {
            RouteResult routeResult = WebAppConfig.Router.Resolve("endless/x1/x2/x3");

            Assert.AreEqual(routeResult.Controller,"Endless");
            Assert.AreEqual(routeResult.Parameters["therest"], "x1/x2/x3");
        }

        [Test]
        public void ValidationDecidedRouting1()
        {
            WebAppConfig.Router.ClearRoutingTable();

            WebAppConfig.Router.AddRoute("{controller}/{action}/{id}", "BadController", "{action}", new RegexRouteValidator("id", "xxx", RouteValidationAction.Error, RouteValidationAction.Skip));
            WebAppConfig.Router.AddRoute("{controller}/{action}/{id}", "NumericController", "{action}", new RegexRouteValidator("id","\\d+"));
            WebAppConfig.Router.AddRoute("{controller}/{action}/{id}", "AlphaController", "{action}", new RegexRouteValidator("id", "[a-z]+"));
            WebAppConfig.Router.AddRoute("{controller}/{action}/{id}", "MixedController", "{action}", new RegexRouteValidator("id","[a-z].*"));

            RouteResult routeResult = WebAppConfig.Router.Resolve("con/act/75");

            Assert.AreEqual("NumericController", routeResult.Controller);
            Assert.AreEqual(routeResult.Action, "act");
            Assert.AreEqual(routeResult.Parameters["id"], "75");

            routeResult = WebAppConfig.Router.Resolve("con/act/x75");

            Assert.AreEqual("MixedController", routeResult.Controller);
            Assert.AreEqual(routeResult.Action, "act");
            Assert.AreEqual(routeResult.Parameters["id"], "x75");

            routeResult = WebAppConfig.Router.Resolve("con/act/xyz");

            Assert.AreEqual("AlphaController", routeResult.Controller);
            Assert.AreEqual(routeResult.Action, "act");
            Assert.AreEqual(routeResult.Parameters["id"], "xyz");

            routeResult = WebAppConfig.Router.Resolve("con/act/75x");

            Assert.IsNull(routeResult);

            routeResult = WebAppConfig.Router.Resolve("con/act/xxx");

            Assert.IsNull(routeResult);
        }
    }
}
