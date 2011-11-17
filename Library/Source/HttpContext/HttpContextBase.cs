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
using System.Collections;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;

namespace Vici.Mvc
{
    public abstract class HttpContextBase
    {
        [ThreadStatic]
        private static HttpContextBase _current;

        private MvcPageHandler _handler;
        private SessionBase _session;

        public abstract Exception[] AllErors { get; }
        public abstract Cache Cache { get; }
        public abstract IHttpRequest Request { get; }
        public abstract IHttpResponse Response { get; }
        public abstract IHttpServerUtility Server { get; }
        public abstract IHttpSessionState Session { get; }
        public abstract IDictionary Items { get; }
        public abstract IPrincipal User { get; set; }

        public static HttpContextBase Current
        {
            get
            {
                if (HttpContext.Current != null)
                    return (HttpContextBase) HttpContext.Current.Items["_CurrentContext"];
                else
                    return _current;
            }
            private set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items["_CurrentContext"] = value;
                else
                    _current = value;
            }
        }

        internal MvcPageHandler Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        internal SessionBase SessionObject
        {
            get { return _session; }
            set { _session = value; }
        }

        public static void CreateContext(HttpContext httpContext)
        {
            WebAppConfig.Init();

            HttpContextBase context = new OnlineHttpContext(httpContext);

            Current = context;

            if (httpContext.Session == null) 
                return;

            context.SessionObject = WebAppHelper.CreateSessionObject();

            WebAppConfig.Fire_SessionCreated(context.SessionObject);
        }

        public static void CreateContext(OfflineWebSession webSession, string method, string url, NameValueCollection postData )
        {
            WebAppConfig.Init(webSession.MapPath);

            HttpContextBase context = new OfflineHttpContext(webSession, method, url, postData);

            Current = context;

            context.SessionObject = WebAppHelper.CreateSessionObject();

            WebAppConfig.Fire_SessionCreated(context.SessionObject);
        }
    }
}