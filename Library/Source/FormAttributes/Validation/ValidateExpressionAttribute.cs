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
using Vici.Core.Parser;

namespace Vici.Mvc
{
    public class ValidateExpressionAttribute : ValidationAttribute
    {
        private readonly string _expression;

        private class ValidationContext : ParserContext
        {
            public ValidationContext(object currentValue, Dictionary<string, object> validationContext) : base(ParserContextBehavior.Easy)
            {
                foreach (var pair in validationContext)
                    Set(pair.Key, pair.Value);

                Set("this", currentValue);

                AddType("DateTime", typeof(DateTime));
            }

            public void Set(string name, object value)
            {
                Set(name, value, value == null ? typeof(object) : value.GetType());
            }
        }

        public ValidateExpressionAttribute(string expression)
        {
            _expression = expression;
        }

        protected override bool Validate(object value, Type targetType)
        {
            ExpressionParser parser = new CSharpParser();

            return parser.Evaluate<bool>("!!(" + _expression + ")", new ValidationContext(value, Context));
        }
    }
}
