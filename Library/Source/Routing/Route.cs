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
using System.Text.RegularExpressions;

namespace Vici.Mvc
{
	public class Route
	{
		private string _url;
		private string _controller;
		private string _action = "Run";
	    private Dictionary<string, string> _parameters;
		private string[] _parameterNames;
    	private Regex _regex;
		private string[] _regexGroups;
	    private List<IRouteValidator> _validators;

		public Route(string url, string controller, string action, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters)
		{
            Url = url;
            Controller = controller;
            Action = action;
		    Validators = validators;
		    RouteParameters = routeParameters;
		}

        public Route(string url, string controller, string action, params RouteParameter[] routeParameters)
        {
            Url = url;
            Controller = controller;
            Action = action;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controller, string action, IEnumerable<RouteParameter> routeParameters)
		{
            Url = url;
            Controller = controller;
            Action = action;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controller, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters)
        {
            Url = url;
            Controller = controller;
            Validators = validators;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controller, params RouteParameter[] routeParameters)
        {
            Url = url;
            Controller = controller;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controller, IEnumerable<RouteParameter> routeParameters)
        {
            Url = url;
            Controller = controller;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controller, params IRouteValidator[] validators)
        {
            Url = url;
            Controller = controller;
            Validators = validators;
        }

        public Route(string url, string controller, IEnumerable<IRouteValidator> validators)
        {
            Url = url;
            Controller = controller;
            Validators = validators;
        }

        public Route(string url, string controller, string action, params IRouteValidator[] validators)
        {
            Url = url;
            Controller = controller;
            Action = action;
            Validators = validators;
        }

        public Route(string url, string controller, string action, IEnumerable<IRouteValidator> validators)
        {
            Url = url;
            Controller = controller;
            Action = action;
            Validators = validators;
        }


        public Route(string url, string controller, string action) 
        {
            Url = url;
            Controller = controller;
            Action = action;
        }

        public Route(string url, string controller) 
        {
            Url = url;
            Controller = controller;
        }

		public IEnumerable<IRouteValidator> Validators
		{
            set { _validators = new List<IRouteValidator>(value); }
		}

        public void AddValidator(IRouteValidator validator)
        {
            if (_validators == null)
                _validators = new List<IRouteValidator>();

            _validators.Add(validator);
        }

		public string Url
		{
			get { return _url; }
			set
			{
                _url = value.Trim().TrimEnd('/');
			    _url = _url.Replace("[", "\\[");
                _url = _url.Replace("]", "\\]");

               

			    _url = Regex.Replace(
			        _url, 
			        @"{(?<name>[a-zA-Z0-9_]+)}",
			        "(?<${name}>[^/]+?)");

                _url = Regex.Replace(
                    _url,
                    @"{(?<name>[a-zA-Z0-9_]+)\*}",
                    "(?<${name}>.+)");

			    _regex = new Regex("^" + _url + "/?$", 
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);

                List<string> groupNames = new List<string>(_regex.GetGroupNames());

                groupNames.RemoveAll(s => Regex.IsMatch(s, "^\\d+$"));

                _regexGroups = groupNames.ToArray();
			}
		}

		public string Controller
		{
			get { return _controller; }
			set { _controller = value; }
		}

		public string Action
		{
			get { return _action; }
			set { _action = value; }
		}

	    public IEnumerable<RouteParameter> RouteParameters
	    {
            set
            {
                _parameters = new Dictionary<string, string>();

                if (value != null)
                    foreach (RouteParameter routeParameter in value)
                        _parameters[routeParameter.Name] = routeParameter.Value;

                _parameterNames = new List<string>(_parameters.Keys).ToArray();
            }
	    }

		internal RouteResult Resolve(string url)
		{
			Match m = _regex.Match(url);

			if (m.Success)
			{
				RouteResult result = new RouteResult(Controller, Action, _parameters);

				foreach (string groupName in _regexGroups)
				{
					string value = m.Groups[groupName].Value;
					string searchString = "{" + groupName + "}";
					bool found = false;

                    if (result.Controller.Contains(searchString))
                    {
                        result.Controller = result.Controller.Replace(searchString, value);
                        found = true;
                    }

                    if (result.Action.Contains(searchString))
                    {
                        result.Action = result.Action.Replace(searchString, value);
                        found = true;
                    }

                    if (_parameterNames != null)
                        foreach (string paramName in _parameterNames)
                        {
                            string paramValue = result.Parameters[paramName];

                            if (paramValue.Contains(searchString))
                            {
                                result.Parameters[paramName] = paramValue.Replace(searchString, value);
                                found = true;
                            }
                        }

                    if (!found)
                        result.Parameters[groupName] = value;

                    if (_validators != null)
					    foreach (IRouteValidator validator in _validators)
						    if (validator.ShouldValidate(groupName))
						    {
						        bool ok = validator.Validate(groupName, value);

						        RouteValidationAction actionToExecute = ok ? validator.PassAction : validator.FailAction;

                                switch (actionToExecute)
                                {
                                    case RouteValidationAction.Skip:
                                        result.ValidationResult = RouteValidationResult.Skip;
                                        break;
                                    case RouteValidationAction.Error:
                                        result.ValidationResult = RouteValidationResult.Fail;
                                        break;
                                    case RouteValidationAction.Accept:
                                        result.ValidationResult = RouteValidationResult.Success;
                                        break;
                                    case RouteValidationAction.None:
                                        continue;
                                }

						        return result;
						    }

				}

				return result;
			}

			return null;
		}
	}
}