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
    public class JQueryAjaxProvider : IAjaxProvider
    {
        public string SetupFramework()
        {
            StringBuilder output = new StringBuilder();

            output.Append("if (typeof $ProMesh == \"undefined\") $ProMesh = {};\r\n\r\n");
            output.Append("if (typeof $ViciMvc == \"undefined\") $ViciMvc = {};\r\n\r\n");

            output.Append("$ViciMvc.Ajax = function(url, paramObject, form, useXML, __callback, __errorcallback) {\r\n");

            output.Append("var postData = jQuery.param(paramObject);\r\n");

            output.Append("if (form) {\r\n");
            output.Append("   if (postData.length > 0) postData += '&';\r\n");
            output.Append("   postData += jQuery(form).serialize();\r\n");
            output.Append("}\r\n");

            output.Append("jQuery.ajax(\r\n");
            output.Append("  {\r\n");
            output.Append("   type: \"POST\",\r\n");
            output.Append("   url: url,\r\n");
            output.Append("   data: postData,\r\n");
            output.Append("   dataType: useXML ? \"xml\" : \"json\",\r\n");
            output.Append("   success: function (result) { if (__callback && !result.error) __callback(result.value); else if (result.error && __errorcallback) __errorcallback(result.error); },\r\n");
            output.Append("   error: function(xhr,errMsg,ex) { if (__errorcallback) __errorcallback(errMsg); }\r\n");
            output.Append("  });\r\n");
            output.Append("};\r\n");

            return output.ToString();
        }

        public string GenerateJavascriptClassName(string className)
        {
            StringBuilder output = new StringBuilder();

            string[] parts = className.Split('.');

            string runningPart = "";

            foreach (string part in parts)
            {
                runningPart += part;

                output.Append("if (typeof " + runningPart + " == \"undefined\") " + runningPart + " = {};\r\n");

                runningPart += '.';
            }

            return output.ToString();
        }

        public string GenerateJavascriptMethod(string url, string className, string methodName, string[] parameters, bool includeFormData, Dictionary<string,string> contextData, bool useXml)
        {
            StringBuilder output = new StringBuilder();

            if (!string.IsNullOrEmpty(className))
                output.Append(className + '.');

            output.Append(methodName);
            output.Append(" = function(");

            if (includeFormData)
            {
                output.Append("form,");
            }

            output.Append(String.Join(", ", parameters));

            if (parameters.Length > 0)
                output.Append(',');

            output.Append("__callback, __errorcallback) {\r\n");
            output.Append("  $ViciMvc.Ajax(\"" + url + "\"");


            List<string> objectFields = new List<string>();

            foreach (KeyValuePair<string, string> contextItem in contextData)
                objectFields.Add("\"" + contextItem.Key + "\":\"" + contextItem.Value + "\"");

            for (int i = 0; i < parameters.Length; i++)
                objectFields.Add("\"" + parameters[i] + "\" : " + parameters[i]);

            output.Append(",{");
            output.Append(string.Join(",", objectFields.ToArray()));
            output.Append("}");

            if (includeFormData)
            {
                output.Append(",form");
            }
            else
            {
                output.Append(",null");
            }

            output.Append("," + (useXml ? "true":"false") + ",__callback,__errorcallback);\r\n}");

            return output.ToString();
        }
    }
}
