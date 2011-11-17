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
using System.Collections.Specialized;

namespace Vici.Mvc
{
    public class Url
    {
        private string _baseUrl;
        private readonly UrlParameterCollection _parameters;

        public Url(string baseUrl)
        {
            _baseUrl = baseUrl;
            _parameters = new UrlParameterCollection();
        }

        public Url(string baseUrl, NameValueCollection queryString)
        {
            _baseUrl = baseUrl;
            _parameters = new UrlParameterCollection(queryString);
        }

        private Url(Url source)
        {
            _baseUrl = source._baseUrl;
            _parameters = source._parameters.Clone();
        }

        public Url Clone()
        {
            return new Url(this);
        }

        public UrlParameterCollection Parameters
        {
            get { return _parameters; }
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        public static Url FromCurrent()
        {
            return new Url(WebAppContext.Request.Path, WebAppContext.Request.QueryString);
        }

        public override string ToString()
        {
            string queryString = _parameters.ToString();

            if (queryString.Length > 0)
                return BaseUrl + "?" + queryString;
            else
                return BaseUrl;
        }

    }
}
