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
    public class OnlineHttpServerUtility : IHttpServerUtility
    {
        private readonly HttpServerUtility _server;

        public OnlineHttpServerUtility(HttpServerUtility server)
        {
            _server = server;
        }

        public string MapPath(string path)
        {
            return _server.MapPath(path);
        }

        public string HtmlDecode(string s)
        {
            return _server.HtmlDecode(s);
        }

        public string HtmlEncode(string s)
        {
            return _server.HtmlEncode(s);
        }

        public string UrlDecode(string s)
        {
            return _server.UrlDecode(s);
        }

        public string UrlEncode(string s)
        {
            return _server.UrlEncode(s);
        }

        public string UrlPathEncode(string s)
        {
            return _server.UrlPathEncode(s);
        }

        public int ScriptTimeout
        {
            get { return _server.ScriptTimeout; }
            set { _server.ScriptTimeout = value; }
        }
    }
}