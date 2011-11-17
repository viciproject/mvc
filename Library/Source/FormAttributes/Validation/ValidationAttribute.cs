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
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ValidationAttribute : Attribute
    {
        private int _priority = Int32.MaxValue;
        private string _message;
        private Dictionary<string, object> _context;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        protected Dictionary<string, object> Context
        {
            get { return _context; }
        }

        internal bool Validate(Dictionary<string, object> context, object value, Type targetType)
        {
            _context = context;

            return Validate(value, targetType);
        }

        protected abstract bool Validate(object value, Type targetType);
    }
}