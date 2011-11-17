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

namespace Vici.Mvc
{
    internal class ActionMethod
    {
        private readonly ActionFilterAttribute[] _filterAttributes;
        private readonly MethodInfo _methodInfo;
        private readonly string _defaultViewName;
        private readonly string _defaultLayoutName;
        private readonly bool _allowCaching;

        public ActionMethod(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;

            _filterAttributes = (ActionFilterAttribute[]) methodInfo.GetCustomAttributes(typeof(ActionFilterAttribute), false);

            OrderedAttribute.Sort(_filterAttributes);

            if (_methodInfo.IsDefined(typeof(LayoutAttribute), false))
                _defaultLayoutName = ((LayoutAttribute)_methodInfo.GetCustomAttributes(typeof(LayoutAttribute), false)[0]).LayoutName ?? "";

            if (_methodInfo.IsDefined(typeof(ViewAttribute), false))
                _defaultViewName = ((ViewAttribute)_methodInfo.GetCustomAttributes(typeof(ViewAttribute), false)[0]).ViewName ?? "";

            if (_methodInfo.IsDefined(typeof(AllowCachingAttribute), true))
                _allowCaching = true;
        }

        public bool AllowCaching
        {
            get { return _allowCaching; }
        }

        public string DefaultLayoutName
        {
            get { return _defaultLayoutName; }
        }

        public string DefaultViewName
        {
            get { return _defaultViewName; }
        }

        public MethodInfo MethodInfo
        {
            get { return _methodInfo; }
        }

        public ActionFilterAttribute[] FilterAttributes
        {
            get { return _filterAttributes; }
        }

        public object Invoke(object target)
        {
            return Invoke(target, WebAppHelper.CreateParameters(_methodInfo));
        }

        public object Invoke(object target, TemplateParserContext parserContext)
        {
            return Invoke(target, WebAppHelper.CreateParameters(_methodInfo,parserContext));
        }

        private object Invoke(object target, object[] parameters)
        {
            try
            {
                return _methodInfo.Invoke(target, parameters);
            }
            catch (TargetInvocationException ex)
            {
                throw ExceptionHelper.ResolveTargetInvocationException(ex);
            }
        }
    }
}