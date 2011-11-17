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

namespace Vici.Mvc
{
    internal class AjaxMethod
    {
        private readonly MethodInfo _methodInfo;
        private readonly Type _targetType;
        private readonly string _javaScriptAlias;
        private readonly string _url;
        private readonly bool _useFormData;
        private readonly bool _returnXml;

        private AjaxMethod(AjaxAttribute ajaxAttribute)
        {
            _useFormData = ajaxAttribute.UseFormData;
            _javaScriptAlias = ajaxAttribute.JavaScriptAlias;
            _returnXml = ajaxAttribute.ReturnXml;
        }

        public AjaxMethod(MethodInfo methodInfo, AjaxAttribute ajaxAttribute) : this(ajaxAttribute)
        {
            _methodInfo = methodInfo;
            _targetType = methodInfo.DeclaringType;

            _url = "~/_$ajax$_.axd/" + UrlHelper.EncodeToUrl(_targetType.Assembly.GetName().Name) + '/' + UrlHelper.EncodeToUrl(_targetType.FullName) + '/' + UrlHelper.EncodeToUrl(methodInfo.Name);
        }

        public AjaxMethod(MethodInfo methodInfo, Type targetType, AjaxAttribute ajaxAttribute) : this(ajaxAttribute)
        {
            _methodInfo = methodInfo;
            _targetType = targetType;

            _url = "~/_$ajax$_.axd/" + UrlHelper.EncodeToUrl(targetType.Assembly.GetName().Name) + '/' + UrlHelper.EncodeToUrl(targetType.FullName) + '/' + UrlHelper.EncodeToUrl(methodInfo.Name);
        }

        public MethodInfo MethodInfo
        {
            get { return _methodInfo; }
        }

        public bool ReturnXml
        {
            get { return _returnXml; }
        }

        public void Register(View view)
        {
            string[] stringparameters = Array.ConvertAll(_methodInfo.GetParameters(), pi => pi.Name);

            string className = _targetType.FullName;
            string methodName = _methodInfo.Name;

            if (!string.IsNullOrEmpty(_javaScriptAlias))
            {
                int lastDot = _javaScriptAlias.LastIndexOf('.');

                if (lastDot >= 0)
                {
                    className = _javaScriptAlias.Substring(0, lastDot);
                    methodName = _javaScriptAlias.Substring(lastDot + 1);
                }
                else
                {
                    className = "";
                    methodName = _javaScriptAlias;
                }
            }

            if (!view.AjaxGenerated)
            {
                view.RegisterJavascript("_JS_AJAX_SETUP_", true, WebAppConfig.AjaxProvider.SetupFramework());

                view.AjaxGenerated = true;
            }

            Dictionary<string,string> ajaxContext = new Dictionary<string, string>();

            ajaxContext.Add("_VIEWNAME_",view.ViewName);

            string jsMethod = WebAppConfig.AjaxProvider.GenerateJavascriptMethod(PathHelper.TranslateAbsolutePath(_url), className, methodName, stringparameters, _useFormData, ajaxContext, ReturnXml);

            if (className.Length > 0)
                view.RegisterJavascript("_JS_" + className, true, WebAppConfig.AjaxProvider.GenerateJavascriptClassName(className));

            view.RegisterJavascript("_JS_" + className + "." + methodName, true, jsMethod);
        }

    }
}
