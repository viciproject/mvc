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
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace Vici.Mvc
{
    internal class ResourceHttpHandler : IHttpHandler
    {
        private readonly string _path;

        public ResourceHttpHandler(string path)
        {
            _path = path;
        }

        public void ProcessRequest(HttpContext context)
        {
            string[] parts = _path.Substring(12).Split('/');

            SendResource(parts[1], parts[2], parts[0]);
        }

        public bool IsReusable { get { return true; } }

        private static void SendResource(string assemblyName, string resourceKey, string contentType)
        {
            assemblyName = UrlHelper.DecodeFromUrl(assemblyName);
            resourceKey = UrlHelper.DecodeFromUrl(resourceKey);
            contentType = UrlHelper.DecodeFromUrl(contentType);

            Assembly assembly = Assembly.Load(assemblyName);

            string eTag = BuildETag(assembly, assemblyName, resourceKey);

            DateTime lastModified = File.GetLastWriteTime(new Uri(assembly.CodeBase).LocalPath);

            WebAppContext.Response.Cache.SetCacheability(HttpCacheability.Public);
            WebAppContext.Response.Cache.SetLastModified(lastModified);
            WebAppContext.Response.Cache.SetMaxAge(TimeSpan.Zero);
            WebAppContext.Response.SetETag(eTag);

            if (WebAppContext.Request.IsETagEqual(eTag) && !WebAppContext.Request.IsChangedBasedOnTimeStamp(lastModified))
            {
                WebAppContext.Response.StatusCode = 304;
            }
            else
            {
                HandleCompression();

                WebAppContext.Response.ContentType = contentType;

                byte[] resourceData = ResourceHelper.GetResourceBytes(assembly, resourceKey);

                if (contentType.StartsWith("text/"))
                {
                    Encoding encoding;

                    resourceData = ResourceHelper.StripEncodingBOM(resourceData, out encoding);

                    WebAppContext.Response.ContentEncoding = encoding;
                }

                WebAppContext.Response.BinaryWrite(resourceData);    
            }
        }

        private static string BuildETag(Assembly assembly, string assemblyName, string resource)
        {
            return assembly.GetName().Version.ToString().Replace(".","") + ":" + assemblyName.GetHashCode().ToString("X8") + resource.GetHashCode().ToString("X8");
        }

//        private static readonly string _etagSignature = DateTime.Now.Ticks.ToString("X16");

        private static void HandleCompression()
        {
//            HttpContext httpContext = HttpContext.Current;
//            HttpResponse httpResponse = httpContext.Response;
//
//            string acceptedEncoding = (httpContext.Request.Headers["Accept-Encoding"] ?? "").ToLower();
//
//            if (acceptedEncoding.Contains("gzip"))
//            {
//                httpResponse.Filter = new GZipStream(httpResponse.Filter, CompressionMode.Compress);
//
//                httpResponse.AppendHeader("Content-Encoding", "gzip");
//            }
//            else
//            {
//                httpResponse.Filter = new DeflateStream(httpResponse.Filter,CompressionMode.Compress);
//
//                httpResponse.AppendHeader("Content-Encoding", "deflate");
//            }
        }
    }
}
