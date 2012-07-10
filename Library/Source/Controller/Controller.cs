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
using System.Reflection;
using Vici.Core;

namespace Vici.Mvc
{
    public abstract class Controller<T> : Controller where T:ViewDataContainer,new()
    {
        protected Controller() : base(new View<T>())
        {
        }

        protected Controller(View<T> view) : base(view)
        {
        }

        public new T ViewData
        {
            get { return (T) base.ViewData; }
        }
    }

    public abstract class Controller : IDisposable
    {
        protected static IHttpResponse         Response   { get { return WebAppContext.Response; } }
        protected static IHttpRequest          Request    { get { return WebAppContext.Request;  } }
        protected static IHttpServerUtility    Server     { get { return WebAppContext.Server;   } }
        protected static SessionBase           Session    { get { return WebAppContext.Session;  } }
        protected static ClientDataCollection  Parameters { get { return WebAppContext.Parameters; } }
        protected static ClientDataCollection  FormData   { get { return WebAppContext.FormData; } }

        [Obsolete("Use Controller.Parameters")]
        protected static ClientDataCollection GetData { get { return WebAppContext.Parameters; } }
        [Obsolete("Use Controller.FormData")]
        protected static ClientDataCollection PostData { get { return WebAppContext.FormData; } }

        private ControllerClass _controllerClass;
        private TemplateParserContext _templateParserContext;

        private View _view;

        private bool _skipTearDown;

        protected Controller()
        {
            _view = new View();
        }

        protected Controller(View view)
        {
            _view = view;
        }

        public ViewDataContainer ViewData
        {
            get { return _view.ViewData; }
        }

        internal void Initialize(ControllerClass controllerClass, TemplateParserContext parserContext) // parse context used for view components
        {
            _controllerClass = controllerClass;
            _templateParserContext = parserContext;

            if (_controllerClass.DefaultLayoutName != null)
                _view.LayoutName = _controllerClass.DefaultLayoutName;

            if (_controllerClass.DefaultViewName != null)
                _view.ViewName = _controllerClass.DefaultViewName;

            MapClientData(parserContext);
        }

        public View View
        {
            get { return _view; }
            internal set { _view = value; }
        }

        public string RequestMethod
        {
            get { return Request.RequestType.ToUpper(); }
        }

        public bool IsPost()
        {
            return RequestMethod == "POST";
        }

        public bool IsPost(string buttonName)
        {
            return IsPost() && (!string.IsNullOrEmpty(FormData[buttonName]));
        }

        private void MapClientData(TemplateParserContext context)
        {
            //TODO: Optimize (preload in ControllerClass)
            Type type = GetType();

            MemberInfo[] members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            foreach (MemberInfo member in members)
            {
                if (member is MethodInfo)
                    continue;

                if (member.IsDefined(typeof(ClientDataAttribute), true))
                {
                    ClientDataAttribute attribute = (ClientDataAttribute) member.GetCustomAttributes(typeof(ClientDataAttribute), true)[0];

                    object value;

                    if (member is FieldInfo)
                    {
                        if (context != null && context.Get(attribute.Name, out value, out type))
                            value = ObjectConverter.Convert(value, ((FieldInfo)member).FieldType);
                        else
                            value = WebAppHelper.GetClientValue(attribute, ((FieldInfo)member).FieldType);

                        ((FieldInfo) member).SetValue(this, value);
                    }
                    else if (member is PropertyInfo)
                    {
                        if (context != null && context.Get(attribute.Name, out value, out type))
                            value = ObjectConverter.Convert(value, ((PropertyInfo)member).PropertyType);
                        else
                            value = WebAppHelper.GetClientValue(attribute, ((PropertyInfo)member).PropertyType);

                        ((PropertyInfo)member).SetValue(this, value, null);
                    }
                }
            }
        }

        public string Url
        {
            get { return Request.Path; }
        }

        public string UrlWithParameters
        {
            get { return Request.RawUrl; }
        }

        public bool SkipTearDown
        {
            get { return _skipTearDown; }
            set { _skipTearDown = value; }
        }

        public void RenderView(string viewName)
        {
            _view.ViewName = viewName;
        }

        public void RenderView(string viewName, string layoutName)
        {
            _view.LayoutName = layoutName;
            _view.ViewName = viewName;
        }

        public void ChangeLayout(string layoutName)
        {
            _view.LayoutName = layoutName;
        }

        public void Redirect(string newUrl)
        {
            Response.Redirect(newUrl);
        }

        public void RedirectWithoutParameters()
        {
            Response.Redirect(WebAppContext.Request.Path);
        }

        public void RedirectWithParameters()
        {
            Response.Redirect(WebAppContext.Request.RawUrl);
        }

        private static Controller CreateController(ControllerClass controllerClass, View view)
        {
            Controller controller = controllerClass.CreateController();

            controller._view = view;

            controllerClass.SetupController(controller, null);

            return controller;
        }

        public static Controller CreateController(string controllerName, View view)
        {
            ControllerClass controllerClass = WebAppConfig.GetControllerClass(controllerName);

            return CreateController(controllerClass, view);
        }

        public static Controller CreateController(Type controllerType, View view)
        {
            ControllerClass controllerClass = WebAppConfig.GetControllerClass(controllerType);

            return CreateController(controllerClass, view);
        }

        public static T CreateController<T>(View view) where T:Controller
        {
            return (T) CreateController(typeof (T), view);
        }

        public void Dispose()
        {
            if (_controllerClass != null && !_skipTearDown)
            {
                _controllerClass.TearDownController(this, _templateParserContext);
            }
        }
    }
}