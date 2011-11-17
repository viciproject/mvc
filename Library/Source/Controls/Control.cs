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
using System.Threading;
using System.Web;

namespace Vici.Mvc
{
    public abstract class Control
    {
        private string _id;
        private string _name;
        private bool _error;
        private string _cssClass;
        private string _cssClassError;
        private bool _autoPost;
        private string _onChange;
        private bool _enabled = true;

        private static readonly Dictionary<Type, string> _defaultCssClasses = new Dictionary<Type, string>();
        private static readonly Dictionary<Type, string> _defaultCssClassesError = new Dictionary<Type, string>();
        private static readonly Dictionary<Type, AjaxMethod[]> _ajaxMethods = new Dictionary<Type, AjaxMethod[]>();
        private static readonly ReaderWriterLock _ajaxMethodsLock = new ReaderWriterLock();

        private static IControlIdProvider _controlIdProvider = new DefaultControlIdGenerator();

        protected Control(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _name = name;
        }

        private AjaxMethod[] AjaxMethods
        {
            get
            {
                Type type = GetType();
                AjaxMethod[] returnValue;

                _ajaxMethodsLock.AcquireReaderLock(-1);

                try
                {
                    if (_ajaxMethods.TryGetValue(type, out returnValue))
                        return returnValue;
                }
                finally
                {
                    _ajaxMethodsLock.ReleaseReaderLock();
                }

                _ajaxMethodsLock.AcquireWriterLock(-1);

                try
                {
                    // double check because another thread could have changed the 
                    // cache between releasing and acquiring the lock

                    if (_ajaxMethods.TryGetValue(type, out returnValue))
                        return returnValue;

                    List<AjaxMethod> ajaxMethods = new List<AjaxMethod>();

                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
                    {
                        AjaxAttribute[] ajaxAttributes = (AjaxAttribute[])method.GetCustomAttributes(typeof(AjaxAttribute), true);

                        if (ajaxAttributes.Length > 0)
                        {
                            if (!method.IsStatic)
                                throw new ViciMvcException("Ajax methods on Control objects should be static");

                            ajaxMethods.Add(new AjaxMethod(method, ajaxAttributes[0]));
                        }
                    }

                    returnValue = ajaxMethods.ToArray();

                    _ajaxMethods[type] = returnValue;

                    return returnValue;
                }
                finally
                {
                    _ajaxMethodsLock.ReleaseWriterLock();
                }
            }
        }

        public virtual string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual bool Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }

        public string CssClassError
        {
            get { return _cssClassError; }
            set { _cssClassError = value; }
        }

        public bool AutoPost
        {
            get { return _autoPost; }
            set { _autoPost = value; }
        }

        public string OnChange
        {
            get { return _onChange; }
            set { _onChange = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public abstract object DataSource { set; get; }

        protected abstract string GenerateHtml(View view, string className, string classNameError);
        protected abstract void HandlePostback(ClientDataCollection postData);

        public string Render(View view)
        {
            return Render(view, null, null);
        }

        public string Render(View view, string cssClass, string cssClassError)
        {
            view = view ?? new View();

            foreach (AjaxMethod method in AjaxMethods)
                method.Register(view);

            Id = ControlIdProvider.GenerateControlId(this, view.NextControlIndex);

            return GenerateHtml(view, cssClass, cssClassError);
        }

        protected string AddIdAttribute(string html)
        {
            if (!string.IsNullOrEmpty(Id))
                html += " id=\"" + HttpUtility.HtmlAttributeEncode(Id) + "\"";

            return html;
        }

        protected string AddNameAttribute(string html)
        {
            if (!string.IsNullOrEmpty(Name))
                html += " name=\"" + HttpUtility.HtmlAttributeEncode(Name) + "\"";

            return html;
        }

        protected string AddOnChangeAttribute(string html)
        {
            string onChange = "";

            if (!string.IsNullOrEmpty(OnChange))
                onChange += OnChange;

            if (AutoPost)
            {
                if (onChange.Length > 0)
                    onChange += ";";

                onChange += "this.form.submit()";
            }

            if (onChange.Length > 0)
                html += " onchange=\"" + HttpUtility.HtmlAttributeEncode(onChange) + "\"";

            return html;
        }

        private static string ApplyClassName(string existingClass, string newClass)
        {
            if (string.IsNullOrEmpty(newClass))
                return existingClass;

            if (newClass[0] == '+')
                return (existingClass ?? "") + ' ' + newClass.Substring(1);

            return newClass;
        }

        protected string AddClassAttribute(string html, string className, string classNameError)
        {
            string cl = CssClass;
            string cle = CssClassError;

            cl = ApplyClassName(cl, className);

            if (cle == null)
                cle = cl;

            cle = ApplyClassName(cle, classNameError);

            if (cl == null)
                cl = DefaultCssClass;

            if (cle == null)
                cle = DefaultCssClassError;

            if (Error && cle != null)
            {
                if (cle.Length > 0)
                    html += " class=\"" + cle + "\"";
            }
            else
            {
                if (!string.IsNullOrEmpty(cl))
                    html += " class=\"" + cl + "\"";
            }

            return html;
        }

        protected string AddEnabledAttribute(string html)
        {
            if (!Enabled)
                html += " disabled=\"disabled\"";

            return html;
        }


        public void HandlePostBack()
        {
            if (WebAppContext.IsPost())
                HandlePostback(WebAppContext.FormData);
        }


        protected static void SetDefaultCssClass<T>(string cssClass, string cssClassError) where T : Control
        {
            if (!string.IsNullOrEmpty(cssClass))
                _defaultCssClasses[typeof(T)] = cssClass;

            if (!string.IsNullOrEmpty(cssClassError))
                _defaultCssClassesError[typeof(T)] = cssClassError;
        }

        private string DefaultCssClass
        {
            get
            {
                Type type = GetType();

                while (type != typeof(Control))
                {
                    if (_defaultCssClasses.ContainsKey(type))
                        return _defaultCssClasses[type];

                    type = type.BaseType;
                }

                return null;
            }
        }

        private string DefaultCssClassError
        {
            get
            {
                Type type = GetType();

                while (type != typeof(Control))
                {
                    if (_defaultCssClassesError.ContainsKey(type))
                        return _defaultCssClassesError[type];

                    type = type.BaseType;
                }

                return null;
            }
        }

        public static IControlIdProvider ControlIdProvider
        {
            get { return _controlIdProvider; }
            set { _controlIdProvider = value; }
        }
    }
}
