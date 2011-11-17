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
    public class ValidateRangeAttribute : ValidationAttribute
    {
        private readonly object _min;
        private readonly object _max;

        public ValidateRangeAttribute(long min, long max)
        {
            _min = min;
            _max = max;
        }

        public ValidateRangeAttribute(double min, double max)
        {
            _min = min;
            _max = max;
        }

        public object Min
        {
            get { return _min; }
        }

        public object Max
        {
            get { return _max; }
        }

        override protected bool Validate(object value, Type targetType)
        {
            if (value == null)
                return true;

            IComparable comparable = null;

            if (_min is long)
            {
                comparable = Convert.ToInt64(value);
            }
            else if (_min is double)
            {
                comparable = Convert.ToDouble(value);
            }

            return (comparable != null && comparable.CompareTo(_min) >= 0 && comparable.CompareTo(_max) <= 0);
        }
    }
}
