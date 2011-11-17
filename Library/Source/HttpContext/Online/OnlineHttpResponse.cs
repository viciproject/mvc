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
using System.IO;
using System.Text;
using System.Web;

namespace Vici.Mvc
{
    public class OnlineHttpResponse : IHttpResponse
    {
        private readonly HttpResponse _response;

        public OnlineHttpResponse(HttpResponse response)
        {
            _response = response;
        }

        public virtual HttpCookieCollection Cookies
        {
            get { return _response.Cookies; }
        }

        public virtual string Output
        {
            get { throw new NotImplementedException(); }
        }

        public virtual int StatusCode
        {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }

        public virtual string ContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        public virtual Stream OutputStream
        {
            get { return _response.OutputStream; }
        }

        public virtual bool Buffer
        {
            get { return _response.Buffer; }
            set { _response.Buffer = value; }
        }

        public virtual bool BufferOutput
        {
            get { return _response.BufferOutput; }
            set { _response.BufferOutput = value; }
        }

        public virtual string CacheControl
        {
            get { return _response.CacheControl; }
            set { _response.CacheControl = value; }
        }

        public virtual string Charset
        {
            get { return _response.Charset; }
            set { _response.Charset = value; }
        }

        public virtual Encoding ContentEncoding
        {
            get { return _response.ContentEncoding; }
            set { _response.ContentEncoding = value; }
        }

        public virtual int Expires
        {
            get { return _response.Expires; }
            set { _response.Expires = value; }
        }

        public virtual DateTime ExpiresAbsolute
        {
            get { return _response.ExpiresAbsolute; }
            set { _response.ExpiresAbsolute = value; }
        }

        public virtual Stream Filter
        {
            get { return _response.Filter; }
            set { _response.Filter = value; }
        }

        public virtual Encoding HeaderEncoding
        {
            get { return _response.HeaderEncoding; }
            set { _response.HeaderEncoding = value; }
        }

        public virtual NameValueCollection Headers
        {
            get { return _response.Headers; }
        }

        public virtual bool IsClientConnected
        {
            get { return _response.IsClientConnected; }
        }

        public virtual bool IsRequestBeingRedirected
        {
            get { return _response.IsRequestBeingRedirected; }
        }

        public virtual string RedirectLocation
        {
            get { return _response.RedirectLocation; }
            set { _response.RedirectLocation = value; }
        }

        public virtual string Status
        {
            get { return _response.Status; }
            set { _response.Status = value; }
        }

        public virtual string StatusDescription
        {
            get { return _response.StatusDescription; }
            set { _response.StatusDescription = value; }
        }

        public virtual int SubStatusCode
        {
            get { return _response.SubStatusCode; }
            set { _response.SubStatusCode = value; }
        }

        public virtual bool SuppressContent
        {
            get { return _response.SuppressContent; }
            set { _response.SuppressContent = value; }
        }

        public virtual bool TrySkipIisCustomErrors
        {
            get { return _response.TrySkipIisCustomErrors; }
            set { _response.TrySkipIisCustomErrors = value; }
        }

        public virtual View RenderedView
        {
            get { return null; }
            set { }
        }

        public HttpCachePolicy Cache
        {
            get { return _response.Cache; }
        }

        public virtual void Write(string s)
        {
            _response.Write(s);
        }

        public virtual void AddCacheItemDependencies(ArrayList cacheKeys)
        {
            _response.AddCacheItemDependencies(cacheKeys);
        }

        public virtual void AddCacheItemDependencies(string[] cacheKeys)
        {
            _response.AddCacheItemDependencies(cacheKeys);
        }

        public virtual void AddCacheItemDependency(string cacheKey)
        {
            _response.AddCacheItemDependency(cacheKey);
        }

        public virtual void AddFileDependencies(ArrayList filenames)
        {
            _response.AddFileDependencies(filenames);
        }

        public virtual void AddFileDependencies(string[] filenames)
        {
            _response.AddFileDependencies(filenames);
        }

        public virtual void AddFileDependency(string filename)
        {
            _response.AddFileDependency(filename);
        }

        public virtual void AddHeader(string name, string value)
        {
            _response.AddHeader(name,value);
        }

        public virtual void AppendCookie(HttpCookie cookie)
        {
            _response.AppendCookie(cookie);
        }

        public virtual void AppendToLog(string param)
        {
            _response.AppendToLog(param);
        }

        public virtual string ApplyAppPathModifier(string virtualPath)
        {
            return _response.ApplyAppPathModifier(virtualPath);
        }

        public virtual void Clear()
        {
            _response.Clear();
        }

        public virtual void ClearContent()
        {
            _response.ClearContent();
        }

        public virtual void Close()
        {
            _response.Close();
        }

        public virtual void DisableKernelCache()
        {
            _response.DisableKernelCache();
        }

        public virtual void Flush()
        {
            _response.Flush();
        }

        public virtual void Pics(string value)
        {
            _response.Pics(value);
        }

        public virtual void Redirect(string url, bool endResponse)
        {
            _response.Redirect(url,endResponse);
        }

        public virtual void RedirectPermanent(string url, bool endResponse)
        {
            _response.Redirect(url,false);
            _response.StatusCode = 301;

            //_response.RedirectLocation = url;
            
            if (endResponse)
                _response.End();
        }

        public virtual void SetCookie(HttpCookie cookie)
        {
            _response.SetCookie(cookie);
        }

        public virtual void TransmitFile(string filename)
        {
            _response.TransmitFile(filename);
        }

        public virtual void TransmitFile(string filename, long offset, long length)
        {
            _response.TransmitFile(filename,offset,length);
        }

        public virtual void Write(char ch)
        {
            _response.Write(ch);
        }

        public virtual void Write(object obj)
        {
            _response.Write(obj);
        }

        public virtual void Write(char[] buffer, int index, int count)
        {
            _response.Write(buffer,index,count);
        }

        public virtual void WriteFile(string filename)
        {
            _response.WriteFile(filename);
        }

        public virtual void WriteFile(string filename, bool readIntoMemory)
        {
            _response.WriteFile(filename, readIntoMemory);
        }

        public virtual void WriteFile(IntPtr fileHandle, long offset, long size)
        {
            _response.WriteFile(fileHandle,offset,size);
        }

        public virtual void WriteFile(string filename, long offset, long size)
        {
            _response.WriteFile(filename,offset,size);
        }

        public virtual void End()
        {
            _response.End();
        }

        public virtual void Redirect(string url)
        {
            _response.Redirect(url);
        }

        public virtual void RedirectPermanent(string url)
        {
            RedirectPermanent(url,true);
        }

        public virtual void BinaryWrite(byte[] bytes)
        {
            _response.BinaryWrite(bytes);
        }

        public virtual void AppendHeader(string header, string value)
        {
            _response.AppendHeader(header,value);
        }

        public virtual void DisableCaching()
        {
            _response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        public virtual void SetETag(string eTag)
        {
            _response.Cache.SetCacheability(HttpCacheability.Public);
            _response.Cache.SetETag('"' + eTag + '"');
        }

        public virtual void SetLastModified(DateTime timeStamp)
        {
            _response.Cache.SetLastModified(timeStamp);
        }


        public virtual void ClearHeaders()
        {
            _response.ClearHeaders();
        }
    }
}