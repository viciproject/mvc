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
    public class Route<T> : Route where T:Controller
    {
        public Route(string url) : base(url, typeof(T))
        {
        }

        public Route(string url, string action) : base(url, typeof(T), action)
        {
        }

        public Route(string url, string action, IEnumerable<IRouteValidator> validators) : base(url, typeof(T), action, validators)
        {
        }

        public Route(string url, string action, params IRouteValidator[] validators) : base(url, typeof(T), action, validators)
        {
        }

        public Route(string url, IEnumerable<IRouteValidator> validators) : base(url, typeof(T), validators)
        {
        }

        public Route(string url, params IRouteValidator[] validators) : base(url, typeof(T), validators)
        {
        }

        public Route(string url, IEnumerable<RouteParameter> routeParameters) : base(url, typeof(T), routeParameters)
        {
        }

        public Route(string url, params RouteParameter[] routeParameters) : base(url, typeof(T), routeParameters)
        {
        }

        public Route(string url, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters) : base(url, typeof(T), validators, routeParameters)
        {
        }

        public Route(string url, string action, IEnumerable<RouteParameter> routeParameters) : base(url, typeof(T), action, routeParameters)
        {
        }

        public Route(string url, string action, params RouteParameter[] routeParameters) : base(url, typeof(T), action, routeParameters)
        {
        }

        public Route(string url, string action, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters) : base(url, typeof(T), action, validators, routeParameters)
        {
        }
    }

	public class Route
	{
		private string _url;
		private string _controllerName;
	    private Type _controllerType;
		private string _action = "Run";
	    private Dictionary<string, string> _parameters;
		private string[] _parameterNames;
    	private Regex _regex;
		private string[] _regexGroups;
	    private List<IRouteValidator> _validators;

		public Route(string url, string controllerName, string action, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters)
		{
            Url = url;
            ControllerName = controllerName;
            Action = action;
		    Validators = validators;
		    RouteParameters = routeParameters;
		}

        public Route(string url, Type controllerType, string action, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters)
        {
            Url = url;
            ControllerType = controllerType;
            Action = action;
            Validators = validators;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controllerName, string action, params RouteParameter[] routeParameters)
        {
            Url = url;
            ControllerName = controllerName;
            Action = action;
            RouteParameters = routeParameters;
        }

        public Route(string url, Type controllerType, string action, params RouteParameter[] routeParameters)
        {
            Url = url;
            ControllerType = controllerType;
            Action = action;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controllerName, string action, IEnumerable<RouteParameter> routeParameters)
		{
            Url = url;
            ControllerName = controllerName;
            Action = action;
            RouteParameters = routeParameters;
        }

        public Route(string url, Type controllerType, string action, IEnumerable<RouteParameter> routeParameters)
        {
            Url = url;
            ControllerType = controllerType;
            Action = action;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controllerName, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters)
        {
            Url = url;
            ControllerName = controllerName;
            Validators = validators;
            RouteParameters = routeParameters;
        }

        public Route(string url, Type controllerType, IEnumerable<IRouteValidator> validators, IEnumerable<RouteParameter> routeParameters)
        {
            Url = url;
            ControllerType = controllerType;
            Validators = validators;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controllerName, params RouteParameter[] routeParameters)
        {
            Url = url;
            ControllerName = controllerName;
            RouteParameters = routeParameters;
        }

        public Route(string url, Type controllerType, params RouteParameter[] routeParameters)
        {
            Url = url;
            ControllerType = controllerType;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controllerName, IEnumerable<RouteParameter> routeParameters)
        {
            Url = url;
            ControllerName = controllerName;
            RouteParameters = routeParameters;
        }

        public Route(string url, Type controllerType, IEnumerable<RouteParameter> routeParameters)
        {
            Url = url;
            ControllerType = controllerType;
            RouteParameters = routeParameters;
        }

        public Route(string url, string controllerName, params IRouteValidator[] validators)
        {
            Url = url;
            ControllerName = controllerName;
            Validators = validators;
        }

        public Route(string url, Type controllerType, params IRouteValidator[] validators)
        {
            Url = url;
            ControllerType = controllerType;
            Validators = validators;
        }

        public Route(string url, string controllerName, IEnumerable<IRouteValidator> validators)
        {
            Url = url;
            ControllerName = controllerName;
            Validators = validators;
        }

        public Route(string url, Type controllerType, IEnumerable<IRouteValidator> validators)
        {
            Url = url;
            ControllerType = controllerType;
            Validators = validators;
        }

        public Route(string url, string controllerName, string action, params IRouteValidator[] validators)
        {
            Url = url;
            ControllerName = controllerName;
            Action = action;
            Validators = validators;
        }

        public Route(string url, Type controllerType, string action, params IRouteValidator[] validators)
        {
            Url = url;
            ControllerType = controllerType;
            Action = action;
            Validators = validators;
        }

        public Route(string url, string controllerName, string action, IEnumerable<IRouteValidator> validators)
        {
            Url = url;
            ControllerName = controllerName;
            Action = action;
            Validators = validators;
        }

        public Route(string url, Type controllerType, string action, IEnumerable<IRouteValidator> validators)
        {
            Url = url;
            ControllerType = controllerType;
            Action = action;
            Validators = validators;
        }

        public Route(string url, string controllerName, string action) 
        {
            Url = url;
            ControllerName = controllerName;
            Action = action;
        }

        public Route(string url, Type controllerType, string action)
        {
            Url = url;
            ControllerType = controllerType;
            Action = action;
        }

        public Route(string url, string controllerName) 
        {
            Url = url;
            ControllerName = controllerName;
        }

        public Route(string url, Type controllerType)
        {
            Url = url;
            ControllerType = controllerType;
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

		public string ControllerName
		{
			get { return _controllerName; }
			set { _controllerName = value; }
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

	    public Type ControllerType
	    {
	        get { return _controllerType; }
	        set { _controllerType = value; }
	    }

	    internal RouteResult Resolve(string url)
		{
			Match m = _regex.Match(url);

			if (m.Success)
			{
				RouteResult result;
                
                if (ControllerType != null)
                    result = new RouteResult(ControllerType, Action, _parameters);
                else
                    result = new RouteResult(ControllerName, Action, _parameters);

				foreach (string groupName in _regexGroups)
				{
					string value = m.Groups[groupName].Value;
					string searchString = "{" + groupName + "}";
					bool found = false;

                    if (result.Controller != null && result.Controller.Contains(searchString))
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