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
using System.Collections.Generic;
using System.Web;

namespace Vici.Mvc
{
    public class HttpModule : IHttpModule
    {
        private static readonly object _contextItemKey = new object();

        private class RequestData
        {
            public RequestData(string url, IHttpHandler handler)
            {
                Url = url;
                Handler = handler;
            }

            public readonly string Url;
            public readonly IHttpHandler Handler;
        }

        public void Init(HttpApplication httpApplication)
        {
            httpApplication.PostResolveRequestCache += PostResolveRequestCache;
            httpApplication.PostMapRequestHandler += PostMapRequestHandler;
            httpApplication.PreRequestHandlerExecute += PreRequestHandlerExecute;
        }

        private static void PostResolveRequestCache(object sender, EventArgs e)
        {
            WebAppConfig.Init();

            HttpContext httpContext = ((HttpApplication)sender).Context;
            HttpRequest httpRequest = httpContext.Request;

            IHttpHandler httpHandler = null;

            string path = UrlHelper.GetUrlPath(httpRequest.AppRelativeCurrentExecutionFilePath, httpRequest.PathInfo);

            if (path.StartsWith("_$ajax$_.axd/"))
            {
                httpHandler = new AjaxPageHandler(path);
            }
            else if (path.StartsWith("_$res$_.axd"))
            {
                httpHandler = new ResourceHttpHandler(path);
            }
            else
            {
                ControllerAction controllerAction = WebAppHelper.GetControllerAction(path);

                if (controllerAction != null)
                    httpHandler = new PageHandler(new MvcPageHandler(controllerAction));
            }

            if (httpHandler != null)
            {
                httpContext.Items[_contextItemKey] = new RequestData(httpContext.Request.RawUrl, httpHandler);

                httpContext.RewritePath("~/ProMesh.axd");
            }
        }

        private static void PostMapRequestHandler(object sender, EventArgs e)
        {
            WebAppConfig.Init();
            
            HttpContext httpContext = ((HttpApplication) sender).Context;

            RequestData requestData = (httpContext.Items[_contextItemKey] as RequestData);

            if (requestData != null)
            {
                httpContext.RewritePath(requestData.Url);

                if (requestData.Handler != null)
                    httpContext.Handler = requestData.Handler;
            }
        }

        private static void PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpContextBase.CreateContext(HttpContext.Current);
        }

//        private static void HandleCompression()
//        {
//            HttpContext httpContext = HttpContext.Current;
//            HttpResponse httpResponse = httpContext.Response;
//
//            string acceptedEncoding = (httpContext.Request.Headers["Accept-Encoding"] ?? "").ToLower();
//
//            if (acceptedEncoding.Contains("gzip"))
//            {
//                httpResponse.Filter = new GZipStream(httpResponse.Filter, CompressionMode.Compress);
//
//                httpResponse.AppendHeader("Content-Encoding", "gzip");
//            }
//            else
//            {
//                httpResponse.Filter = new DeflateStream(httpResponse.Filter, CompressionMode.Compress);
//
//                httpResponse.AppendHeader("Content-Encoding", "deflate");
//            }
//        }

        public void Dispose()
        {
        }
    }
}
