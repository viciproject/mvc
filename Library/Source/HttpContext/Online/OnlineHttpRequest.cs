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
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

namespace Vici.Mvc
{
    public class OnlineHttpRequest : IHttpRequest
    {
        private readonly HttpRequest _request;

        public OnlineHttpRequest(HttpRequest request)
        {
            _request = request;
        }

        public NameValueCollection ServerVariables
        {
            get { return _request.ServerVariables; }
        }

        public int TotalBytes
        {
            get { return _request.TotalBytes; }
        }

        public NameValueCollection Form
        {
            get { return _request.Form; }
        }

        public NameValueCollection Headers
        {
            get { return _request.Headers; }
        }

        public string HttpMethod
        {
            get { return _request.HttpMethod; }
        }

        public Stream InputStream
        {
            get { return _request.InputStream; }
        }

        public bool IsAuthenticated
        {
            get { return _request.IsAuthenticated; }
        }

        public HttpCookieCollection Cookies
        {
            get { return _request.Cookies; }
        }

        public string CurrentExecutionFilePath
        {
            get { return _request.CurrentExecutionFilePath; }
        }

        public byte[] BinaryRead(int count)
        {
            return _request.BinaryRead(count);
        }

        public int[] MapImageCoordinates(string imageFieldName)
        {
            return _request.MapImageCoordinates(imageFieldName);
        }

        public string MapPath(string virtualPath)
        {
            return _request.MapPath(virtualPath);
        }

        public string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping)
        {
            return _request.MapPath(virtualPath, baseVirtualDir, allowCrossAppMapping);
        }

        public void SaveAs(string filename, bool includeHeaders)
        {
            _request.SaveAs(filename, includeHeaders);
        }

        public void ValidateInput()
        {
            _request.ValidateInput();
        }

        public string[] AcceptTypes
        {
            get { return _request.AcceptTypes; }
        }

        public string AnonymousID
        {
            get { return _request.AnonymousID; }
        }

        public string ApplicationPath
        {
            get { return _request.ApplicationPath; }
        }

        public string AppRelativeCurrentExecutionFilePath
        {
            get { return _request.AppRelativeCurrentExecutionFilePath; }
        }

        public NameValueCollection QueryString
        {
            get { return _request.QueryString; }
        }

        public string PhysicalApplicationPath
        {
            get { return _request.PhysicalApplicationPath; }
        }

        public string PhysicalPath
        {
            get { return _request.PhysicalPath; }
        }

        public string RawUrl
        {
            get { return _request.RawUrl; }
        }

        public string RequestType
        {
            get { return _request.RequestType; }
            set { _request.RequestType = value; }
        }

        public string FilePath
        {
            get { return _request.FilePath; }
        }

        public HttpFileCollection Files
        {
            get { return _request.Files; }
        }

        public Stream Filter
        {
            get { return _request.Filter; }
            set { _request.Filter = value; }
        }

        public string PathInfo
        {
            get { return _request.PathInfo; }
        }

        public bool IsLocal
        {
            get { return _request.IsLocal; }
        }

        public bool IsSecureConnection
        {
            get { return _request.IsSecureConnection; }
        }

        public string this[string key]
        {
            get { return _request[key]; }
        }

        public NameValueCollection Params
        {
            get { return _request.Params; }
        }

        public HttpBrowserCapabilities Browser
        {
            get { return _request.Browser; }
            set { _request.Browser = value; }
        }

        public HttpClientCertificate ClientCertificate
        {
            get { return _request.ClientCertificate; }
        }

        public Encoding ContentEncoding
        {
            get { return _request.ContentEncoding; }
            set { _request.ContentEncoding = value; }
        }

        public int ContentLength
        {
            get { return _request.ContentLength; }
        }

        public string ContentType
        {
            get { return _request.ContentType; }
            set { _request.ContentType = value; }
        }

        public Uri Url
        {
            get { return _request.Url; }
        }

        public Uri UrlReferrer
        {
            get { return _request.UrlReferrer; }
        }

        public string Path
        {
            get { return _request.Path; }
        }

        public string UserHostAddress
        {
            get { return _request.UserHostAddress; }
        }

        public string UserHostName
        {
            get { return _request.UserHostName; }
        }

        public string[] UserLanguages
        {
            get { return _request.UserLanguages; }
        }

        public string UserAgent
        {
            get { return _request.UserAgent; }
        }

        public bool IsETagEqual(string eTag)
        {
            string match = _request.Headers["If-None-Match"];

            return match != null && match == ('"' + eTag+ '"');
        }

        public bool IsChangedBasedOnTimeStamp(DateTime timeStamp)
        {
            string ifModifiedSince = _request.Headers["If-Modified-Since"];

            DateTime parsedTimeStamp;

            if (string.IsNullOrEmpty(ifModifiedSince) || !DateTime.TryParseExact(ifModifiedSince,
                                new []
                                    {
                                        "ddd, dd MMM yyyy HH:mm:ss \"GMT\"",
                                        "dddd, dd-MMM-yy HH:mm:ss \"GMT\"",
                                        "ddd MM dd HH:mm:ss yyyy",
                                        "ddd MM  d HH:mm:ss yyyy",
                                    }, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out parsedTimeStamp))
                return true;

            timeStamp = new DateTime(timeStamp.Year,timeStamp.Month,timeStamp.Day,timeStamp.Hour,timeStamp.Minute,timeStamp.Second);

            return timeStamp > parsedTimeStamp;
        }
    }
}