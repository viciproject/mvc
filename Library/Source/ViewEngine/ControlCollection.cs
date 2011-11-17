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
using System.Collections;
using System.Collections.Generic;

namespace Vici.Mvc
{
    public class ControlCollection : IEnumerable<Control>
    {
        private readonly Dictionary<string, Control> _dictionary = new Dictionary<string, Control>(StringComparer.InvariantCultureIgnoreCase);

        public Control this[string name]
        {
            get 
            {
                Control control;

                if (_dictionary.TryGetValue(name, out control))
                    return control;
                else 
                    return null;
            }
            set 
            { 
                _dictionary[name] = value; 
            }
        }

        public void Add(Control control)
        {
            this[control.Name] = control;
        }

        IEnumerator<Control> IEnumerable<Control>.GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }
    }
}
