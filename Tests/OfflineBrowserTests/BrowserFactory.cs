using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Vici.Mvc.Test.OfflineBrowserTests
{
    public static class BrowserFactory
    {

        public static OfflineWebSession SetupBrowser()
        {
            OfflineWebSession _browser;
            
            string assemblyPath = Assembly.GetExecutingAssembly().CodeBase;

            if (assemblyPath.StartsWith("file://"))
                assemblyPath = assemblyPath.Substring(8);

            string rootPath = Path.GetFullPath(Path.GetDirectoryName(assemblyPath).TrimEnd('\\') + "\\..\\..\\WebServer");

            _browser = new OfflineWebSession(rootPath);

            _browser.PageGet("~/index"); // Initialize the offline web server by requesting a page

            return _browser;
        }


    }
}
