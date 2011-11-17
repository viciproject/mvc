#region License
//=============================================================================
// Vici MVC - .NET Web Application Framework 
//
// Copyright (c) 2003-2010 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace Vici.Mvc
{
    internal class MvcPageHandler
    {
        private readonly ControllerAction _controllerAction;

        internal MvcPageHandler(ControllerAction controllerAction)
        {
            _controllerAction = controllerAction;
        }

        public void ProcessRequest(HttpContextBase httpContext)
        {
            httpContext.Handler = this;

            string baseUrl = UrlHelper.GetUrlPath(httpContext.Request.AppRelativeCurrentExecutionFilePath, httpContext.Request.PathInfo);

            Stopwatch stopWatchRun = new Stopwatch();
            Stopwatch stopWatchRender = new Stopwatch();
            Stopwatch stopWatchTotal = new Stopwatch();

            ActionResult actionResult = null;

            try
            {
                try
                {
                    stopWatchTotal.Start();

                    try
                    {
                        stopWatchRun.Start();

                        actionResult = WebAppHelper.RunControllerAction(_controllerAction);
                    }
                    finally
                    {
                        stopWatchRun.Stop();
                    }

                    if (actionResult != null)
                    {
                        try
                        {
                            stopWatchRender.Start();

                            actionResult.Execute(httpContext);
                        }
                        finally
                        {
                            stopWatchRender.Stop();
                        }
                    }
                }
                finally
                {
                    stopWatchTotal.Stop();

                    if (WebAppConfig.LoggingProvider != null)
                    {
                        WebAppConfig.LoggingProvider.LogPage(WebAppContext.Session.SessionID,
                                                             baseUrl,
                                                             GetQueryString(),
                                                             WebAppContext.Session.LanguageCode,
                                                             actionResult != null ? actionResult.LayoutName : null,
                                                             actionResult != null ? actionResult.ViewName : null,
                                                             (int) stopWatchRun.ElapsedMilliseconds,
                                                             (int) stopWatchRender.ElapsedMilliseconds,
                                                             (int) stopWatchTotal.ElapsedMilliseconds);
                    }
                }
            }

            catch (EndResponseException)
            {
                // occurs when Response.End() is called from an offline session (unit test)
            }

            catch (ThreadAbortException)
            {
                throw; // Occurs when Response.End() is called. Rethrow it, because it is handled by the ASP.NET runtime
            }

            catch (Exception ex)
            {
                if (ex.InnerException is ThreadAbortException)
                    throw ex.InnerException;

                WebAppConfig.Fire_ExceptionHandler(ex);
            }
        }

        private static string GetQueryString()
        {
            string rawUrl = WebAppContext.Request.RawUrl;

            if (rawUrl.IndexOf("?") >= 0)
                return rawUrl.Substring(rawUrl.IndexOf("?") + 1);
            else
                return "";
        }
    }

}
