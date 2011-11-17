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
using System.Text.RegularExpressions;

namespace Vici.Mvc
{
	public class RegexRouteValidator : IRouteValidator
	{
		private readonly string _parameter;
		private readonly Regex _regex;
		private RouteValidationAction _passAction;
	    private RouteValidationAction _failAction;

		public RegexRouteValidator(string parameter, string regex) 
            : this(parameter, regex, RouteValidationAction.None, RouteValidationAction.Skip)
		{
		}

		public RegexRouteValidator(string parameter, string regex, RouteValidationAction passAction, RouteValidationAction failAction)
		{
			_parameter = parameter;

			_regex = new Regex("^" + regex + "$");

		    _passAction = passAction;
		    _failAction = failAction;
		}

        public bool ShouldValidate(string parameterName)
        {
            return _parameter == parameterName;
        }

		public string Parameter
		{
			get { return _parameter; }
		}

		public RouteValidationAction PassAction
		{
			get { return _passAction; }
            set { _passAction = value; }
		}

        public RouteValidationAction FailAction
        {
            get { return _failAction; }
            set { _failAction = value; }
        }

		public bool Validate(string parameterName, string param)
		{
			return _regex.IsMatch(param);
		}
	}
}