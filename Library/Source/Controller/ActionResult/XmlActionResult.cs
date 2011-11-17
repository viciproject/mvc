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
using System.Xml;
using System.Xml.Linq;

namespace Vici.Mvc
{
    public class XmlActionResult : ActionResult
    {
        private readonly string _xml;
        private readonly XDocument _xDocument;
        private readonly XmlDocument _xmlDocument;

        public XmlActionResult(string xml) : base(true)
        {
            if (xml == null)
                throw new ArgumentNullException(xml);

            _xml = xml;
        }

        public XmlActionResult(XDocument xDocument) : base(true)
        {
            if (xDocument == null)
                throw new ArgumentNullException("xDocument");

            _xDocument = xDocument;
        }

        public XmlActionResult(XmlDocument xmlDocument)
            : base(true)
        {
            if (xmlDocument == null)
                throw new ArgumentNullException("xmlDocument");

            _xmlDocument = xmlDocument;
        }

        public override void Execute(HttpContextBase httpContext)
        {
            httpContext.Response.ContentType = "text/xml";
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