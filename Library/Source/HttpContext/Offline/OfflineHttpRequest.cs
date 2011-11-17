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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;

namespace Vici.Mvc
{
    public class OfflineHttpRequest : IHttpRequest
    {
        private readonly HttpCookieCollection _cookies;
        private readonly NameValueCollection _form;
        private readonly NameValueCollection _queryString;
        private readonly NameValueCollection _serverVariables;
        private readonly Uri _url;
        
        private HttpBrowserCapabilities _browserCaps = new HttpBrowserCapabilities();
        private Encoding _encoding = Encoding.UTF8;

        private string _filePath;
        private string _pathInfo;
        private string _rawUrl;

        public OfflineHttpRequest(string method, string url, NameValueCollection data)
        {
            _cookies = new HttpCookieCollection();
            _queryString = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            _serverVariables = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);

            if (data != null)
                _form = data;
            else 
                _form = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
        
            ProcessUrl(url);

            _url = new Uri("http://localhost:0" + _rawUrl,UriKind.Absolute);

            _serverVariables["REQUEST_METHOD"] = method.ToUpper();
            _serverVariables["REMOTE_ADDR"] = "127.0.0.1";
            _serverVariables["HTTP_USER_AGENT"] = "ViciMvc/Offline";
            _serverVariables["HTTP_REFERER"] = "";
        }

        private static OfflineHttpContext Context
        {
            get { return (OfflineHttpContext) WebAppContext.HttpContext; }
        }

        public NameValueCollection ServerVariables
        {
            get { return _serverVariables; }
        }

        public int TotalBytes
        {
            get { throw new NotImplementedException(); }
        }

        public NameValueCollection Form
        {
            get { return _form; }
        }

        public NameValueCollection Headers
        {
            get { throw new NotImplementedException(); }
        }

        public string HttpMethod
        {
            get { return _serverVariables["REQUEST_METHOD"]; }
        }

        public Stream InputStream
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAuthenticated
        {
            get { throw new NotImplementedException(); }
        }

        public HttpCookieCollection Cookies
        {
            get { return _cookies; }
        }

        public string CurrentExecutionFilePath
        {
            get { throw new NotImplementedException(); }
        }

        public byte[] BinaryRead(int count)
        {
            throw new NotImplementedException();
        }

        public int[] MapImageCoordinates(string imageFieldName)
        {
            throw new NotImplementedException();
        }

        public string MapPath(string virtualPath)
        {
            throw new NotImplementedException();
        }

        public string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping)
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string filename, bool includeHeaders)
        {
            throw new NotImplementedException();
        }

        public void ValidateInput()
        {
            throw new NotImplementedException();
        }

        public string[] AcceptTypes
        {
            get { throw new NotImplementedException(); }
        }

        public string AnonymousID
        {
            get { throw new NotImplementedException(); }
        }

        public string ApplicationPath
        {
            get { return "/"; }
        }

        public string AppRelativeCurrentExecutionFilePath
        {
            get { return "~" + Path; }
        }

        public HttpBrowserCapabilities Browser
        {
            get { return _browserCaps; }
            set { _browserCaps = value; }
        }

        public HttpClientCertificate ClientCertificate
        {
            get { return null; }
        }

        public NameValueCollection QueryString
        {
            get { return _queryString; }
        }

        public string PhysicalApplicationPath
        {
            get { return Context.OfflineSession.RootPath; }
        }

        public string PhysicalPath
        {
            get { throw new NotImplementedException(); }
        }

        public string RawUrl
        {
            get { return _rawUrl; }
        }

        public string RequestType
        {
            get { return HttpMethod; }
            set { throw new NotSupportedException(); }
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public HttpFileCollection Files
        {
            get { return null; }
        }

        public Stream Filter
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string PathInfo
        {
            get { return _pathInfo; }
        }

        public bool IsLocal
        {
            get { return true; }
        }

        public bool IsSecureConnection
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string key]
        {
            get { throw new NotImplementedException(); }
        }

        public NameValueCollection Params
        {
            get
            {
                NameValueCollection items = new NameValueCollection();

                items.Add(_queryString);
                items.Add(_form);
                items.Add(_serverVariables);

                foreach (string cookie in _cookies.Keys)
                    items.Add(cookie,_cookies[cookie].Value);

                return items;
            }
        }

        public Uri Url
        {
            get { return _url; }
        }

        public Uri UrlReferrer
        {
            get { return null; }
        }

        public string Path
        {
            get { return FilePath + PathInfo; }
        }

        public string UserHostAddress
        {
            get { return "127.0.0.1"; }
        }

        public string UserHostName
        {
            get { throw new NotImplementedException(); }
        }

        public string[] UserLanguages
        {
            get { throw new NotImplementedException(); }
        }

        public string UserAgent
        {
            get { return "Local"; }
        }

        public Encoding ContentEncoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        public int ContentLength
        {
            get { throw new NotImplementedException(); }
        }

        public string ContentType
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        private void ProcessUrl(string url)
        {
            if (url.StartsWith("~/"))
                url = url.Substring(1);

            if (!url.StartsWith("/"))
                throw new UriFormatException("URL cannot be a relative path");

            _rawUrl = url;

            int q = url.IndexOf('?');
            string queryString = "";

            if (q > 0)
            {
                queryString = url.Substring(q + 1);
                url = url.Substring(0, q);
            }

            q = url.LastIndexOf('.');

            if (q >= 0)
                q = url.IndexOf('/',q);

            if (q > 0)
            {
                _pathInfo = url.Substring(q);
                _filePath = url.Substring(0, q);
            }
            else
            {
                _pathInfo = "";
                _filePath = url;
            }

            string[] paramPairs = queryString.Split('&');

            foreach (string paramPair in paramPairs)
            {
                int eq = paramPair.IndexOf('=');

                if (eq > 0)
                    _queryString[paramPair.Substring(0, eq)] = paramPair.Substring(eq + 1);
                else
                    _queryString[paramPair] = "";
            }
        }

        public bool IsETagEqual(string eTag)
        {
            return false;
        }

        public bool IsChangedBasedOnTimeStamp(DateTime timeStamp)
        {
            return false;
        }
    }
}