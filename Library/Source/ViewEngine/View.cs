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
    public class View<T> : View where T:ViewDataContainer, new()
    {
        public View() : base(new T())
        {
        }

        public View(T viewData) : base(viewData)
        {
        }

        public new T ViewData
        {
            get { return (T) base.ViewData;  }
        }
    }

    public class View
    {
        private readonly ViewDataContainer _viewData;

        private string _viewName;
        private string _layoutName;

        private Template _layoutTemplate;
        private Template _viewTemplate;

        private readonly Dictionary<string,string> _registeredJavascriptKeys = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _registeredCSSKeys = new Dictionary<string, string>();
        
        private readonly List<string> _javaScriptLibFragments = new List<string>();
        private readonly List<string> _javaScriptFragments = new List<string>();
        private readonly List<string> _javaScriptStartupFragments = new List<string>();
        private readonly List<string> _javaScriptLibIncludes = new List<string>();
        private readonly List<string> _javaScriptIncludes = new List<string>();

        private readonly List<string> _cssFiles = new List<string>();

        private int _nextControlIndex = 1;

        internal bool AjaxGenerated;

        public View() : this(new ViewDataContainer())
        {
        }
 
        internal View(ViewDataContainer viewData)
        {
            LayoutName = WebAppConfig.DefaultLayout;

            _viewData = viewData;
        }
 
        public string ViewName
        {
            get { return _viewName; }
            internal set { _viewName = value; _viewTemplate = null; }
        }

        public string LayoutName
        {
            get { return _layoutName; }
            internal set { _layoutName = value; _layoutTemplate = null; }
        }

        public ViewDataContainer ViewData
        {
            get { return _viewData; }
        }

        internal Template ViewTemplate
        {
            get
            {
                return _viewTemplate ?? (_viewTemplate = WebAppHelper.GetTemplate(_viewName, false));
            }
        }

        internal Template LayoutTemplate
        {
            get
            {
                return _layoutTemplate ?? (_layoutTemplate = WebAppHelper.GetTemplate(_layoutName, true));
            }
        }

        public int NextControlIndex
        {
            get { return _nextControlIndex++; }
        }

        public void RegisterJavascriptFromResource(bool library, Assembly assembly, string resourceName)
        {
            RegisterJavascriptFromResource(assembly.FullName + resourceName, library, assembly, resourceName);
        }

        public void RegisterJavascriptFromResource(string key, bool library, Assembly assembly, string resourceName)
        {
            string javaScript = ResourceHelper.GetResourceText(assembly, resourceName);

            if (!_registeredJavascriptKeys.ContainsKey(key))
            {
                if (library)
                    _javaScriptLibFragments.Add(javaScript + "\r\n");
                else
                    _javaScriptFragments.Add(javaScript + "\r\n");

                _registeredJavascriptKeys.Add(key, javaScript);
            }
        }

        public void RegisterJavascript(string key, bool library, string javaScript)
        {
            if (!_registeredJavascriptKeys.ContainsKey(key))
            {
                if (library)
                    _javaScriptLibFragments.Add(javaScript + "\r\n");
                else
                    _javaScriptFragments.Add(javaScript + "\r\n");
                
                _registeredJavascriptKeys.Add(key, javaScript);
            }
        }

        public void RegisterJavascriptIncludeFromResource(bool library, Assembly assembly, string resourceName)
        {
            RegisterJavascriptIncludeFromResource(assembly.FullName + resourceName,library,assembly,resourceName);
        }

        public void RegisterJavascriptIncludeFromResource(string key, bool library, Assembly assembly, string resourceName)
        {
            if (!_registeredJavascriptKeys.ContainsKey(key))
            {
                string javaScriptUrl = ResourceHelper.CreateResourceUrl(assembly, resourceName, "text/javascript");

                if (library)
                    _javaScriptLibIncludes.Add(javaScriptUrl);
                else
                    _javaScriptIncludes.Add(javaScriptUrl);

                _registeredJavascriptKeys.Add(key, javaScriptUrl);
            }
        }

        public void RegisterJavascriptInclude(string key, bool library, string javaScriptUrl)
        {
            if (!_registeredJavascriptKeys.ContainsKey(key))
            {
                if (library)
                    _javaScriptLibIncludes.Add(javaScriptUrl);
                else
                    _javaScriptIncludes.Add(javaScriptUrl);

                _registeredJavascriptKeys.Add(key, javaScriptUrl);
            }
        }

        public void RegisterJavascriptInclude(bool library, string javaScriptUrl)
        {
            RegisterJavascriptInclude(javaScriptUrl, library, javaScriptUrl);
        }

        public void RegisterStartupJavascript(string key, string javaScript)
        {
            if (!_registeredJavascriptKeys.ContainsKey(key))
            {
                _javaScriptStartupFragments.Add(javaScript + "\r\n");

                _registeredJavascriptKeys.Add(key, javaScript);
            }
        }

        public void RegisterCSSIncludeFromResource(string key, Assembly assembly, string resourceName)
        {
            if (!_registeredCSSKeys.ContainsKey(key))
            {
                string cssUrl = ResourceHelper.CreateResourceUrl(assembly, resourceName, "text/css");

                _cssFiles.Add(cssUrl);

                _registeredCSSKeys.Add(key, cssUrl);
            }
        }

        public void RegisterCSSInclude(string cssFileUrl)
        {
            RegisterCSSInclude(cssFileUrl,cssFileUrl);
        }

        public void RegisterCSSInclude(string key, string cssFileUrl)
        {
            if (!_registeredCSSKeys.ContainsKey(key))
            {
                _cssFiles.Add(cssFileUrl);

                _registeredCSSKeys.Add(key, cssFileUrl);
            }
        }

        public void RegisterAjaxMethods(Type type)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                AjaxAttribute[] ajaxAttributes = (AjaxAttribute[])method.GetCustomAttributes(typeof(AjaxAttribute), true);

                if (ajaxAttributes.Length > 0)
                {
                    new AjaxMethod(method, ajaxAttributes[0]).Register(this);
                }
            }
        }

        public void RegisterAjaxMethods<T>()
        {
            RegisterAjaxMethods(typeof(T));
        }

        internal string Render()
        {
            UrlHolder urlHolder = new UrlHolder();

            // Obsolete =====================================
            ViewData["Session"] = WebAppContext.Session;
            ViewData["_SELF_"] = urlHolder;
            ViewData["CurrentUrl"] = urlHolder;
            // ==============================================

            ViewData["@Session"] = WebAppContext.Session;
            ViewData["@Url"] = urlHolder;

            Template layoutTemplate = LayoutTemplate;
            Template viewTemplate = ViewTemplate;

            if (viewTemplate == null && layoutTemplate == null)
                return "<html><body></body></html>";

            string html = null;

            if (viewTemplate != null)
            {
                string viewHtml = viewTemplate.Render(this);

                if (layoutTemplate == null)
                {
                    html = viewHtml;
                }
                else
                {
                    viewHtml = TemplateUtil.ExtractBody(viewHtml);

                    ViewData["_VIEW_"] = viewHtml; // Obsolete
                    ViewData["@View"] = viewHtml;

                    string pageTitle = viewTemplate.RenderTitle(this);

                    if (!string.IsNullOrEmpty(pageTitle))
                    {
                        ViewData["_TITLE_"] = pageTitle; // Obsolete
                        ViewData["@Title"] = pageTitle;
                    }
                }

            }

            if (layoutTemplate != null)
            {
                html = layoutTemplate.Render(this);
            }

            int idx = html.IndexOf("</head>",StringComparison.OrdinalIgnoreCase);

            if (idx > 0)
            {
                string head = "";

                foreach (string cssFile in _cssFiles)
                    head += "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + PathHelper.TranslateAbsolutePath(cssFile) + "\" />\r\n";

                foreach (string js in _javaScriptLibIncludes)
                    head += "<script language=\"javascript\" type=\"text/javascript\" src=\"" + PathHelper.TranslateAbsolutePath(js) + "\"></script>\r\n";

                if (_javaScriptLibFragments.Count > 0)
                    head += "<script language=\"javascript\" type=\"text/javascript\">\r\n";

                foreach (string js in _javaScriptLibFragments)
                    head += js + "\r\n";

                if (_javaScriptLibFragments.Count > 0)
                    head += "</script>\r\n";

                foreach (string js in _javaScriptIncludes)
                    head += "<script language=\"javascript\" type=\"text/javascript\" src=\"" + PathHelper.TranslateAbsolutePath(js) + "\"></script>\r\n";

                if (_javaScriptFragments.Count > 0)
                    head += "<script language=\"javascript\" type=\"text/javascript\">\r\n";

                foreach (string js in _javaScriptFragments)
                    head += js + "\r\n";

                if (_javaScriptFragments.Count > 0)
                    head += "</script>\r\n";

                if (viewTemplate != null)
                    head += viewTemplate.RenderHeadSection(this);

                if (head.Length > 0)
                    html = html.Insert(idx, head);
            }

            idx = html.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);

            if (idx > 0)
            {
                string script = "";

                if (_javaScriptStartupFragments.Count > 0)
                    script += "<script language=\"javascript\" type=\"text/javascript\">\r\n";

                foreach (string js in _javaScriptStartupFragments)
                    script += js + "\r\n";

                if (_javaScriptStartupFragments.Count > 0)
                    script += "</script>\r\n";

                if (script.Length > 0)
                    html = html.Insert(idx, script);
            }

            return html;
        }

        public string ParseTranslations(string inputString)
        {
            return TranslationHelper.ParseTranslations(inputString, ViewName);
        }
    }
}
