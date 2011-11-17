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
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace Vici.Mvc
{
    internal class AjaxPageHandler : IHttpHandler, IRequiresSessionState
    {
        private readonly string _path;

        public AjaxPageHandler(string path)
        {
            _path = path;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            RunAjaxMethod(_path.Substring(13));
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private static void RunAjaxMethod(string pageUrl)
        {
            IHttpResponse response = WebAppContext.Response;

            response.ContentType = "application/json";

            string[] parts = pageUrl.Split('/');

            if (parts.Length != 3)
            {
                throw new ViciMvcException("Unrecognized Ajax URL");
            }

            string assemblyName = UrlHelper.DecodeFromUrl(parts[0]);
            string className = UrlHelper.DecodeFromUrl(parts[1]);
            string methodName = UrlHelper.DecodeFromUrl(parts[2]);

            Type type = Type.GetType(className + ", " + assemblyName);

            if (type == null)
            {
                response.Write(AjaxHelper.GenerateJSONError("Unknown class " + className + " in assembly " + assemblyName));
                return;
            }


            Type currentType = type;
            MethodInfo method = null;
           
            while (method == null && currentType != null && currentType != typeof(object))
            {
                method = currentType.GetMethod(methodName);

                currentType = currentType.BaseType;
            }

            if (method == null)
            {
                response.Write(AjaxHelper.GenerateJSONError("Unknown method " + methodName + " in class " + className));
                return;
            }

            AjaxAttribute[] ajaxAttributes = (AjaxAttribute[])method.GetCustomAttributes(typeof(AjaxAttribute), false);

            if (ajaxAttributes.Length == 0 && !method.IsDefined(typeof(AjaxOkAttribute),true))
            {
                response.Write(AjaxHelper.GenerateJSONError("Method " + methodName + " is not an Ajax method"));
                return;
            }

            bool returnXml = ajaxAttributes.Length > 0 && ajaxAttributes[0].ReturnXml;

            object obj = null;

            if (!method.IsStatic)
                obj = Activator.CreateInstance(type);

            if (returnXml)
                response.ContentType = "text/xml";

            response.Write(WebAppHelper.RunAjaxMethod(method, obj, returnXml));
        }
    }
}
