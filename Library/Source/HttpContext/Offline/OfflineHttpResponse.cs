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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;

namespace Vici.Mvc
{
    public class OfflineHttpResponse : IHttpResponse
    {
        private readonly HttpCookieCollection _cookies;
        private MemoryStream _outputStream = new MemoryStream();
        //private StringBuilder _output = new StringBuilder();
        private string _redirectedUrl;
        private int _statusCode;
        private string _contentType = "text/html";
        private bool _buffer;
        private Encoding _contentEncoding = Encoding.UTF8;
        private string _charset;
        private View _renderedView;


        public OfflineHttpResponse()
        {
            _cookies = new HttpCookieCollection();
        }

        private void AddBytes(string s)
        {
            byte[] b = _contentEncoding.GetBytes(s);

            _outputStream.Write(b,0,b.Length);
        }

        public virtual HttpCookieCollection Cookies
        {
            get { return _cookies; }
        }

        public virtual void Write(string s)
        {
            byte[] b = _contentEncoding.GetBytes(s);

            _outputStream.Write(b, 0, b.Length);
        }

        public virtual string Output
        {
            get
            {
                return _contentEncoding.GetString(_outputStream.ToArray());
            }
        }

        internal string RedirectedUrl
        {
            get { return _redirectedUrl; }
        }

        public virtual int StatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        public virtual string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        public virtual void End()
        {
            throw new EndResponseException();
        }

        public virtual void Redirect(string url)
        {
            Redirect(url,true);
        }

        public virtual void BinaryWrite(byte[] bytes)
        {
            _outputStream.Write(bytes,0,bytes.Length);
        }

        public virtual void AppendHeader(string header, string value)
        {
        }

        public virtual bool TrySkipIisCustomErrors
        {
            get { return false; }
            set { }
        }

        public virtual void DisableCaching()
        {
        }

        public virtual void SetETag(string eTag)
        {
        }

        public void SetLastModified(DateTime timeStamp)
        {
            
        }

        public virtual void ClearHeaders()
        {
        }

        public virtual Stream OutputStream
        {
            get { return _outputStream; }
        }

        public virtual void AddCacheItemDependencies(ArrayList cacheKeys)
        {
        }

        public virtual void AddCacheItemDependencies(string[] cacheKeys)
        {
        }

        public virtual void AddCacheItemDependency(string cacheKey)
        {
        }

        public virtual void AddFileDependencies(ArrayList filenames)
        {
        }

        public virtual void AddFileDependencies(string[] filenames)
        {
        }

        public virtual void AddFileDependency(string filename)
        {
        }

        public virtual void AddHeader(string name, string value)
        {
        }

        public virtual void AppendCookie(HttpCookie cookie)
        {
            throw new System.NotImplementedException();
        }

        public virtual void AppendToLog(string param)
        {
            throw new System.NotImplementedException();
        }

        public virtual string ApplyAppPathModifier(string virtualPath)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Clear()
        {
            ClearHeaders();
            ClearContent();
        }

        public virtual void ClearContent()
        {
            _outputStream = new MemoryStream();
        }

        public virtual void Close()
        {
           
        }

        public virtual void DisableKernelCache()
        {
        }

        public virtual void Flush()
        {
            _outputStream.Flush();
        }

        public virtual void Pics(string value)
        {
        }

        public void RedirectPermanent(string url)
        {
            Redirect(url);
        }

        public void RedirectPermanent(string url, bool endResponse)
        {
            Redirect(url,endResponse);
        }

        public virtual void Redirect(string url, bool endResponse)
        {
            if (url.StartsWith("~"))
                url = url.Substring(1);

            if (url.StartsWith("/"))
                _redirectedUrl = url;
            else
            {
                _redirectedUrl = WebAppContext.Request.FilePath.Substring(0, WebAppContext.Request.FilePath.LastIndexOf('/') + 1) + url;
            }

            if (endResponse)
                End();
        }

        public virtual void SetCookie(HttpCookie cookie)
        {
            throw new System.NotImplementedException();
        }

        public virtual void TransmitFile(string filename)
        {
            BinaryWrite(File.ReadAllBytes(filename));
        }

        public virtual void TransmitFile(string filename, long offset, long length)
        {
            byte[] buffer = new byte[length];

            using (Stream file = File.OpenRead(filename))
            {
                file.Read(buffer, (int) offset, (int) length);
            }

            BinaryWrite(buffer);
        }

        public virtual void Write(char ch)
        {
            BinaryWrite(_contentEncoding.GetBytes(new char[] {ch}));
        }

        public virtual void Write(object obj)
        {
            // TODO: implement
        }

        public virtual void Write(char[] buffer, int index, int count)
        {
            BinaryWrite(_contentEncoding.GetBytes(buffer, index, count));
        }

        public virtual void WriteFile(string filename)
        {
            throw new System.NotImplementedException();
        }

        public virtual void WriteFile(string filename, bool readIntoMemory)
        {
            throw new System.NotImplementedException();
        }

        public virtual void WriteFile(IntPtr fileHandle, long offset, long size)
        {
            throw new System.NotImplementedException();
        }

        public virtual void WriteFile(string filename, long offset, long size)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        public virtual bool BufferOutput
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual string CacheControl
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual string Charset
        {
            get { return _charset; }
            set { _charset = value; }
        }

        public virtual Encoding ContentEncoding
        {
            get { return _contentEncoding; }
            set { _contentEncoding = value; }
        }

        public virtual int Expires
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual DateTime ExpiresAbsolute
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual Stream Filter
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual Encoding HeaderEncoding
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual NameValueCollection Headers
        {
            get { throw new System.NotImplementedException(); }
        }

        public virtual bool IsClientConnected
        {
            get { return true; }
        }

        public virtual bool IsRequestBeingRedirected
        {
            get { throw new System.NotImplementedException(); }
        }

        public virtual string RedirectLocation
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual string Status
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual string StatusDescription
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual int SubStatusCode
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual bool SuppressContent
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual View RenderedView
        {
            get { return _renderedView; }
            set { _renderedView = value; }
        }

        public HttpCachePolicy Cache
        {
            get { return null; }
        }
    }
}