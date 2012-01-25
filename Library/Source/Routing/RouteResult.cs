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

namespace Vici.Mvc
{
	internal class RouteResult
	{
	    private RouteValidationResult _validationResult = RouteValidationResult.Success;
		private string _controller;
	    private Type _controllerType;
		private string _action;
		private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();

        private RouteResult(string action, Dictionary<string,string> parameters)
        {
            Action = action;

            if (parameters != null)
                foreach (KeyValuePair<string, string> param in parameters)
                    _parameters[param.Key] = param.Value;
        }

		public RouteResult(string controller, string action, Dictionary<string,string> parameters) : this(action, parameters)
		{
			Controller = controller;
		}

        public RouteResult(Type controllerType, string action, Dictionary<string, string> parameters) : this(action, parameters)
        {
            ControllerType = controllerType;
        }

	    public ControllerClass CreateControllerClass()
	    {
            if (_controllerType != null)
                return WebAppConfig.GetControllerClass(_controllerType);

            if (_controller != null)
                return WebAppConfig.GetControllerClass(_controller);

	        return null;
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

		public Dictionary<string, string> Parameters
		{
			get { return _parameters; }
		}

	    public RouteValidationResult ValidationResult
	    {
	        get { return _validationResult; }
	        set { _validationResult = value; }
	    }

	    public Type ControllerType
	    {
	        set { _controllerType = value; }
	    }
	}
}