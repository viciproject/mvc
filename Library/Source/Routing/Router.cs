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
using System.Threading;

namespace Vici.Mvc
{
	public class Router
	{
		private readonly List<Route> _routes = new List<Route>();
		private readonly Dictionary<string,RouteResult> _routeCache = new Dictionary<string, RouteResult>();
		private readonly ReaderWriterLock _routeCacheLock = new ReaderWriterLock();

        internal Router()
		{
		}

        public void ClearRoutingTable() // only for use in unit tests
        {
            _routes.Clear();
            _routeCache.Clear();
        }

	    public void AddDefaultRoutes(string extension)
        {
            if (extension == null)
                extension = "";
            else
            {
                extension = extension.Trim();

                if (extension.Length > 0 && extension[0] != '.')
                    extension = '.' + extension;
            }

            AddRoute(new Route("{controller}/{action}/{id}" + extension, "{controller}", "{action}"));
            AddRoute(new Route("{controller}/{action}" + extension, "{controller}", "{action}"));
            AddRoute(new Route("{controller}" + extension, "{controller}", "Run"));
        }

        public void AddRoute(Route route)
        {
            _routes.Add(route);

            _routeCache.Clear();
        }

		public void InsertRoute(Route route)
		{
			_routes.Insert(0, route);

			_routeCache.Clear();
		}

        public void AddRoute(string url, string controller, string action, params object[] options)
        {
            IEnumerable<IRouteValidator> validators = from option in options where option is IRouteValidator select (IRouteValidator) option;
            IEnumerable<RouteParameter> parameters = from option in options where option is RouteParameter select (RouteParameter)option;

            AddRoute(url,controller,action,validators,parameters);
        }

        public void InsertRoute(string url, string controller, string action, params object[] options)
        {
            IEnumerable<IRouteValidator> validators = from option in options where option is IRouteValidator select (IRouteValidator)option;
            IEnumerable<RouteParameter> parameters = from option in options where option is RouteParameter select (RouteParameter)option;

            InsertRoute(url, controller, action, validators, parameters);
        }

        public void AddRoute(string url, string controller, string action, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters)
		{
            AddRoute(new Route(url, controller, action, validators, routeParameters));
		}

        public void InsertRoute(string url, string controller, string action, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters)
		{
            InsertRoute(new Route(url, controller, action, validators, routeParameters));
		}

        public void AddRoute(string url, string controller, string action, params RouteParameter[] routeParameters)
        {
            AddRoute(new Route(url, controller, action, routeParameters));
        }

        public void InsertRoute(string url, string controller, string action, params RouteParameter[] routeParameters)
        {
            InsertRoute(new Route(url, controller, action, routeParameters));
        }

        public void AddRoute(string url, string controller, string action, IEnumerable<RouteParameter> routeParameters)
		{
            AddRoute(new Route(url, controller, action, routeParameters));
        }

        public void InsertRoute(string url, string controller, string action, IEnumerable<RouteParameter> routeParameters)
		{
            InsertRoute(new Route(url, controller, action, routeParameters));
        }

        public void AddRoute(string url, string controller, string action, params IRouteValidator[] validators)
        {
            AddRoute(new Route(url, controller, action, validators));
        }

        public void InsertRoute(string url, string controller, string action, params IRouteValidator[] validators)
        {
            InsertRoute(new Route(url, controller, action, validators));
        }

        public void AddRoute(string url, string controller, string action, IEnumerable<IRouteValidator> validators)
        {
            AddRoute(new Route(url, controller, action, validators));
        }

        public void InsertRoute(string url, string controller, string action, IEnumerable<IRouteValidator> validators)
        {
            InsertRoute(new Route(url, controller, action, validators));
        }

        public void AddRoute(string url, string controller, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters)
        {
            AddRoute(new Route(url, controller, validators, routeParameters));
        }

        public void InsertRoute(string url, string controller, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters)
        {
            InsertRoute(new Route(url, controller, validators, routeParameters));
        }

        public void AddRoute(string url, string controller, params RouteParameter[] routeParameters)
        {
            AddRoute(new Route(url, controller, routeParameters));
        }

        public void InsertRoute(string url, string controller, params RouteParameter[] routeParameters)
        {
            InsertRoute(new Route(url, controller, routeParameters));
        }

        public void AddRoute(string url, string controller, IEnumerable<RouteParameter> routeParameters)
        {
            AddRoute(new Route(url, controller, routeParameters));
        }

        public void InsertRoute(string url, string controller, IEnumerable<RouteParameter> routeParameters)
        {
            InsertRoute(new Route(url, controller, routeParameters));
        }

        public void AddRoute(string url, string controller, params IRouteValidator[] validators)
        {
            AddRoute(new Route(url, controller, validators));
        }

        public void InsertRoute(string url, string controller, params IRouteValidator[] validators)
        {
            InsertRoute(new Route(url, controller, validators));
        }

        public void AddRoute(string url, string controller, IEnumerable<IRouteValidator> validators)
        {
            AddRoute(new Route(url, controller, validators));
        }

        public void InsertRoute(string url, string controller, IEnumerable<IRouteValidator> validators)
        {
            InsertRoute(new Route(url, controller, validators));
        }

        public void AddRoute(string url, string controller, string action) 
        {
            AddRoute(new Route(url, controller, action));
        }

        public void InsertRoute(string url, string controller, string action)
        {
            InsertRoute(new Route(url, controller, action));
        }

        public void AddRoute(string url, string controller) 
        {
            AddRoute(new Route(url, controller));
        }

		internal RouteResult Resolve(string url)
		{
			RouteResult result;

            _routeCacheLock.AcquireReaderLock(-1);

            try
            {
                if (_routeCache.TryGetValue(url, out result))
                    return result;
            }
            finally
            {
                _routeCacheLock.ReleaseReaderLock();
            }

			foreach (Route route in _routes)
			{
				result = route.Resolve(url);

                if (result == null || result.ValidationResult == RouteValidationResult.Skip)
                {
                    result = null;
                    continue;
                }

			    if (result.ValidationResult == RouteValidationResult.Fail)
                    result = null;

			    break;
			}

            _routeCacheLock.AcquireWriterLock(-1);

            try
            {
                if (!_routeCache.ContainsKey(url))
                    _routeCache[url] = result;
            }
            finally
            {
                _routeCacheLock.ReleaseWriterLock();
            }

			return result;
		}

	}
}