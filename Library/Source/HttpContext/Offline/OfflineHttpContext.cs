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
using System.Web.Caching;

namespace Vici.Mvc
{
    public class OfflineHttpContext : HttpContextBase
    {
        private readonly IHttpRequest _request;
        private readonly OfflineHttpResponse _response;
        private readonly OfflineHttpServerUtility _server;
        private readonly IHttpSessionState _session;

        private readonly OfflineWebSession _offlineSession;

        private readonly Hashtable _items = new Hashtable();

        private IPrincipal _user = null;

        public OfflineHttpContext(OfflineWebSession offlineSession, string method, string url, NameValueCollection postData)
        {
            _offlineSession = offlineSession;

            _request = new OfflineHttpRequest(method, url, postData);
            _session = new OfflineHttpSessionState(offlineSession);
            _server = new OfflineHttpServerUtility(offlineSession);
            _response = new OfflineHttpResponse();
        }

        public override Exception[] AllErors
        {
            get { return new Exception[0]; }
        }

        public override Cache Cache
        {
            get { return null; }
        }

        public override IHttpRequest Request
        {
            get { return _request; }
        }

        public override IHttpResponse Response
        {
            get { return _response; }
        }

        public override IHttpServerUtility Server
        {
            get { return _server; }
        }

        public override IHttpSessionState Session
        {
            get { return _session; }
        }

        public OfflineWebSession OfflineSession
        {
            get { return _offlineSession;  }
        }

        public override IDictionary Items
        {
            get { return _items; }
        }

        public override IPrincipal User
        {
            get { return _user; }
            set { _user = value; }
        }
    }
}
