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
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vici.Mvc
{
    internal class ControllerClass
    {
        private readonly Type _classType;
        private readonly List<MethodInfo> _setupMethods = new List<MethodInfo>();
        private readonly List<MethodInfo> _teardownMethods = new List<MethodInfo>();
        private readonly Dictionary<string, ActionMethod> _actionMethods = new Dictionary<string, ActionMethod>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, AjaxMethod> _ajaxMethods = new Dictionary<string, AjaxMethod>();
        private readonly Route[] _routes;
        private readonly bool _allowCaching;
        private readonly string _defaultViewName;
        private readonly string _defaultLayoutName;

        private readonly string _name;

        internal ControllerClass(Type classType)
        {
            _classType = classType;
            _name = classType.Name;

            _defaultViewName = _name;

            Type currentClassType = _classType;
            Stack<Type> pageTypeStack = new Stack<Type>();

            if (_classType.IsDefined(typeof(AllowCachingAttribute), true))
                _allowCaching = true;

            if (IsViewComponent)
            {
                ComponentNameAttribute[] nameAttributes = (ComponentNameAttribute[])classType.GetCustomAttributes(typeof(ComponentNameAttribute), false);

                if (nameAttributes.Length > 0)
                    _name = nameAttributes[0].Name;
            }

            UrlAttribute[] urlAttributes = (UrlAttribute[])classType.GetCustomAttributes(typeof(UrlAttribute), false);

            List<Route> routes = urlAttributes.Select(attribute => new Route(attribute.Path, _name, attribute.Action)).ToList();

            while (currentClassType != typeof(Controller) && currentClassType != null)
            {
                pageTypeStack.Push(currentClassType);

                currentClassType = currentClassType.BaseType;
            }

            while (pageTypeStack.Count > 0)
            {
                currentClassType = pageTypeStack.Pop();

                if (currentClassType.IsDefined(typeof(LayoutAttribute), false))
                    _defaultLayoutName = ((LayoutAttribute)currentClassType.GetCustomAttributes(typeof(LayoutAttribute), false)[0]).LayoutName ?? "";

                if (currentClassType.IsDefined(typeof(ViewAttribute), false))
                    _defaultViewName = ((ViewAttribute)currentClassType.GetCustomAttributes(typeof(ViewAttribute), false)[0]).ViewName ?? "";

                MethodInfo[] methods = currentClassType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

                foreach (MethodInfo methodInfo in methods)
                {
                    if (methodInfo.IsSpecialName)
                        continue;

                    if (methodInfo.IsDefined(typeof (BeforeActionAttribute), true))
                    {
                        _setupMethods.Add(methodInfo);
                    }
                    else if (methodInfo.IsDefined(typeof (AfterActionAttribute), true))
                    {
                        _teardownMethods.Add(methodInfo);
                    }
                    else if (methodInfo.IsDefined(typeof(AjaxAttribute), true))
                    {
                        AjaxAttribute ajaxAttribute = (AjaxAttribute) methodInfo.GetCustomAttributes(typeof (AjaxAttribute), true)[0];

                        _ajaxMethods[methodInfo.Name] = new AjaxMethod(methodInfo, _classType, ajaxAttribute);
                    }
                    else if (methodInfo.IsPublic)
                    {
                        _actionMethods[methodInfo.Name] = new ActionMethod(methodInfo);

                        urlAttributes = (UrlAttribute[])methodInfo.GetCustomAttributes(typeof(UrlAttribute), false);

                        routes.AddRange(from urlAttr in urlAttributes select new Route(urlAttr.Path, _classType, methodInfo.Name));
                    }
                }
            }

            _routes = routes.ToArray();
        }

        public string DefaultLayoutName
        {
            get { return _defaultLayoutName; }
        }

        public string DefaultViewName
        {
            get { return _defaultViewName; }
        }

        public bool AllowCaching
        {
            get { return _allowCaching; }
        }

        internal string Name
        {
            get { return _name; }
        }

        internal Route[] Routes
        {
            get { return _routes; }
        }

        internal Controller CreateController()
        {
            Controller controller = (Controller) Activator.CreateInstance(_classType);

            controller.Initialize(this, null);

            return controller;
        }

        internal ViewComponent CreateViewComponent(TemplateParserContext context)
        {
            ViewComponent controller = (ViewComponent) Activator.CreateInstance(_classType);

            controller.Initialize(this, context);

            return controller;
        }

        internal bool IsViewComponent
        {
            get { return typeof (ViewComponent).IsAssignableFrom(_classType); }
        }

        public object Run(Controller controller, string methodName, TemplateParserContext context)
        {
            ActionMethod actionMethod = _actionMethods[methodName];

            if (actionMethod.DefaultLayoutName != null)
                controller.View.LayoutName = actionMethod.DefaultLayoutName;

            if (actionMethod.DefaultViewName != null)
                controller.View.ViewName = actionMethod.DefaultViewName;

            if (!AllowCaching && !actionMethod.AllowCaching)
                WebAppContext.Response.DisableCaching();

            object returnValue = actionMethod.Invoke(controller, context);

            if (returnValue is StringBuilder)
                return returnValue.ToString();

            return returnValue;
        }

        internal bool IsActionMethod(string methodName)
        {
            return _actionMethods.ContainsKey(methodName);
        }

        private void RegisterAjaxMethods(Controller controller)
        {
            View view = (controller is ViewComponent) ? WebAppContext.RootView : controller.View;

            foreach (AjaxMethod ajaxMethod in _ajaxMethods.Values)
                ajaxMethod.Register(view);
        }

        internal void SetupController(Controller controller, TemplateParserContext context)
        {
            try
            {
                foreach (MethodInfo method in _setupMethods)
                    method.Invoke(controller, WebAppHelper.CreateParameters(method, context));
            }
            catch (TargetInvocationException ex)
            {
                throw ExceptionHelper.ResolveTargetInvocationException(ex);
            }

            RegisterAjaxMethods(controller);
        }

        internal void TearDownController(Controller controller, TemplateParserContext context)
        {
            try
            {
                foreach (MethodInfo method in _teardownMethods)
                    method.Invoke(controller, WebAppHelper.CreateParameters(method, context));
            }
            catch (TargetInvocationException ex)
            {
                throw ExceptionHelper.ResolveTargetInvocationException(ex);
            }
        }

    }
}
