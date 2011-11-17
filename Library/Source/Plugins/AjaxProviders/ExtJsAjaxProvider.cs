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
using System.Text;

namespace Vici.Mvc
{
    public class ExtJsAjaxProvider : IAjaxProvider
    {
        public string SetupFramework()
        {
            return "";
        }

        public string GenerateJavascriptClassName(string className)
        {
            return "Ext.namespace('" + className + "');";
        }

        public string GenerateJavascriptMethod(string url, string className, string methodName, string[] parameters, bool includeFormData, Dictionary<string, string> contextData, bool useXml)
        {
            StringBuilder output = new StringBuilder();

            if (!string.IsNullOrEmpty(className))
                output.Append(className + '.');

            output.Append(methodName);
            output.Append(" = function(");
            output.Append(String.Join(", ", parameters));

            if (parameters.Length > 0)
                output.Append(',');

            output.Append("__callback, __errorcallback");

            output.Append(")\r\n");
            output.Append("{ Ext.Ajax.request(\r\n");
            output.Append("       {\r\n");
            output.Append("         method: \"POST\",\r\n");
            output.Append("         url: \"" + url + "\",\r\n");
            output.Append("         params: { ");

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                    output.Append(',');

                output.Append("\"" + parameters[i] + "\" : " + parameters[i]);
            }

            output.Append("},\r\n");
            output.Append("         success: function (xhr) { var result = eval('result=' + xhr.responseText); if (__callback && !result.error) __callback(result.value); else if (result.error && __errorcallback) __errorcallback(result.error.Message); },\r\n");
            output.Append("         failure: function(xhr) { if (__errorcallback) __errorcallback(xhr.statusText); }\r\n");
            output.Append("       });\r\n");
            output.Append("};\r\n");

            return output.ToString();
        }
    }
}