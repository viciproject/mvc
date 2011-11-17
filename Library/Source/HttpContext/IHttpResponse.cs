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
    public interface IHttpResponse
    {
        HttpCookieCollection Cookies { get; }
        void Write(string s);
        string Output { get; }
        void End();
        void Redirect(string url);
        void RedirectPermanent(string url);
        void RedirectPermanent(string url, bool endResponse);
        int StatusCode { get; set; }
        string ContentType { get; set; }
        void BinaryWrite(byte[] bytes);
        void AppendHeader(string header, string value);
        void ClearHeaders();
        Stream OutputStream { get; }
        void AddCacheItemDependencies(ArrayList cacheKeys);
        void AddCacheItemDependencies(string[] cacheKeys);
        void AddCacheItemDependency(string cacheKey);
        void AddFileDependencies(ArrayList filenames);
        void AddFileDependencies(string[] filenames);
        void AddFileDependency(string filename);
        void AddHeader(string name, string value);
        void AppendCookie(HttpCookie cookie);
        void AppendToLog(string param);
        string ApplyAppPathModifier(string virtualPath);
        void Clear();
        void ClearContent();
        void Close();
        void DisableKernelCache();
        void Flush();
        void Pics(string value);
        void Redirect(string url, bool endResponse);
        void SetCookie(HttpCookie cookie);
        void TransmitFile(string filename);
        void TransmitFile(string filename, long offset, long length);
        void Write(char ch);
        void Write(object obj);
        void Write(char[] buffer, int index, int count);
        void WriteFile(string filename);
        void WriteFile(string filename, bool readIntoMemory);
        void WriteFile(IntPtr fileHandle, long offset, long size);
        void WriteFile(string filename, long offset, long size);
        bool Buffer { get; set; }
        bool BufferOutput { get; set; }
        string CacheControl { get; set; }
        string Charset { get; set; }
        Encoding ContentEncoding { get; set; }
        int Expires { get; set; }
        DateTime ExpiresAbsolute { get; set; }
        Stream Filter { get; set; }
        Encoding HeaderEncoding { get; set; }
        NameValueCollection Headers { get; }
        bool IsClientConnected { get; }
        bool IsRequestBeingRedirected { get; }
        string RedirectLocation { get; set; }
        string Status { get; set; }
        string StatusDescription { get; set; }
        int SubStatusCode { get; set; }
        bool SuppressContent { get; set; }
        bool TrySkipIisCustomErrors { get; set; }
        void DisableCaching();
        void SetETag(string eTag);
        void SetLastModified(DateTime timeStamp);
        View RenderedView { get; set; }
        HttpCachePolicy Cache { get; }
    }
}