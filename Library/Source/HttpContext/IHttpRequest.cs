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
    public interface IHttpRequest
    {
        byte[] BinaryRead(int count);
        int[] MapImageCoordinates(string imageFieldName);
        string MapPath(string virtualPath);
        string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping);
        void SaveAs(string filename, bool includeHeaders);
        void ValidateInput();

        string[] AcceptTypes { get; }
        string AnonymousID { get; }
        string ApplicationPath { get; }
        string AppRelativeCurrentExecutionFilePath { get; }
        HttpBrowserCapabilities Browser { get; set; }
        HttpClientCertificate ClientCertificate { get; }
        Encoding ContentEncoding { get; set; }
        int ContentLength { get; }
        string ContentType { get; set; }
        HttpCookieCollection Cookies { get; }
        string CurrentExecutionFilePath { get; }
        string FilePath { get; }
        HttpFileCollection Files { get; }
        Stream Filter { get; set; }
        NameValueCollection Form { get; }
        NameValueCollection Headers { get; }
        string HttpMethod { get; }
        Stream InputStream { get; }
        bool IsAuthenticated { get; }
        bool IsLocal { get; }
        bool IsSecureConnection { get; }
        string this[string key] { get; }
        NameValueCollection Params { get; }
        string Path { get; }
        string PathInfo { get; }
        string PhysicalApplicationPath { get; }
        string PhysicalPath { get; }
        NameValueCollection QueryString { get; }
        string RawUrl { get; }
        string RequestType { get; set; }
        NameValueCollection ServerVariables { get; }
        int TotalBytes { get; }
        Uri Url { get; }
        Uri UrlReferrer { get; }
        string UserAgent { get; }
        string UserHostAddress { get; }
        string UserHostName { get; }
        string[] UserLanguages { get; }

        // Vici MVC Specific

        bool IsETagEqual(string eTag);
        bool IsChangedBasedOnTimeStamp(DateTime timeStamp);



    }

}