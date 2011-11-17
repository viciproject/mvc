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
using System.Security.Principal;
using System.Web;
using System.Web.Caching;

namespace Vici.Mvc
{
    public class OnlineHttpContext : HttpContextBase
    {
        private readonly HttpContext _httpContext;
        private readonly OnlineHttpRequest _httpRequest;
        private readonly OnlineHttpResponse _httpResponse;
        private readonly OnlineHttpServerUtility _httpServerUtility;
        private readonly OnlineHttpSessionState _httpSessionState;

        public OnlineHttpContext(HttpContext httpContext)
        {
            _httpContext = httpContext;

            _httpRequest = new OnlineHttpRequest(httpContext.Request);
            _httpResponse = new OnlineHttpResponse(httpContext.Response);
            _httpServerUtility = new OnlineHttpServerUtility(httpContext.Server);

            if (httpContext.Session != null)
                _httpSessionState = new OnlineHttpSessionState(httpContext.Session);
        }

        public override Exception[] AllErors
        {
            get { return _httpContext.AllErrors; }
        }

        public override Cache Cache
        {
            get { return _httpContext.Cache; }
        }

        public override IHttpRequest Request
        {
            get { return _httpRequest;  }
        }

        public override IHttpResponse Response
        {
            get { return _httpResponse; }
        }

        public override IHttpServerUtility Server
        {
            get { return _httpServerUtility; }
        }

        public override IHttpSessionState Session
        {
            get { return _httpSessionState; }
        }

        public override IDictionary Items
        {
            get { return _httpContext.Items; }
        }

        public override IPrincipal User
        {
            get { return _httpContext.User; }
            set { _httpContext.User = value; }
        }
    }
}