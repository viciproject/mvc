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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Vici.Mvc
{
    public class XmlActionResult : ActionResult
    {
        private readonly string _xml;
        private readonly string _contentType = "application/xml";
        private readonly XDocument _xDocument;
        private readonly XmlDocument _xmlDocument;

        public XmlActionResult(string xml, string contentType) : base(true)
        {
            if (xml == null)
                throw new ArgumentNullException(xml);

            if (contentType != null)
                _contentType = contentType;

            _xml = xml;
        }

        public XmlActionResult(string xml) : this(xml, null)
        {
        }

        public XmlActionResult(XDocument xDocument, string contentType) : base(true)
        {
            if (xDocument == null)
                throw new ArgumentNullException("xDocument");

            if (contentType != null)
                _contentType = contentType;

            _xDocument = xDocument;
        }

        public XmlActionResult(XDocument xDocument) : this(xDocument, null)
        {
        }

        public XmlActionResult(XmlDocument xmlDocument, string contentType) : base(true)
        {
            if (xmlDocument == null)
                throw new ArgumentNullException("xmlDocument");

            if (contentType != null)
                _contentType = contentType;

            _xmlDocument = xmlDocument;
        }

        public XmlActionResult(XmlDocument xmlDocument) : this(xmlDocument, null)
        {
        }

        protected internal override void Execute(HttpContextBase httpContext)
        {
            httpContext.Response.ContentType = _contentType;
            httpContext.Response.ContentEncoding = Encoding.UTF8;
            httpContext.Response.Charset = "utf-8";

            if (_xml != null)
            {
                httpContext.Response.Write(_xml);
            }
            else if (_xDocument != null)
            {
                using (StreamWriter responseStream = new StreamWriter(httpContext.Response.OutputStream))
                {
                    _xDocument.Save(responseStream);
                }
            }
            else if (_xmlDocument != null)
            {
                _xmlDocument.Save(httpContext.Response.OutputStream);
            }
        }
    }
}