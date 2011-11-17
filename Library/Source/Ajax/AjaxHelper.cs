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
using Vici.Core.Json;

namespace Vici.Mvc
{
    internal static class AjaxHelper
    {
        private class JSONError
        {
            public JSONError(string error)
            {
                this.error = error;
            }

            public string error;
        }

        private class JSONReturnValue
        {
            public JSONReturnValue(object value)
            {
                this.value = value;
            }

            public object value;
           
        }

        public static string GenerateJSONError(string message)
        {
            JSONError error = new JSONError(message);

            return Json.Stringify(error, WebAppConfig.JsonDateFormat);
        }

        public static string GenerateJSONReturnValue(object value)
        {
            JSONReturnValue returnValue = new JSONReturnValue(value);

            return Json.Stringify(returnValue, WebAppConfig.JsonDateFormat);
        }

        public static string GenerateXmlError(string message)
        {
            return "<?xml version=\"1.0\"?><error>" + message + "</error>";
        }
    }
}
