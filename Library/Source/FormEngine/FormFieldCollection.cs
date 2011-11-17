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
    public class FormFieldCollection<T> : IEnumerable<FormField<T>> where T:class
    {
        private readonly Dictionary<string, FormField<T>> _formFieldList = new Dictionary<string, FormField<T>>();

        public FormField<T> this[string fieldName]
        {
            get
            {
                return _formFieldList.ContainsKey(fieldName) ? _formFieldList[fieldName] : null;
            }
        }

        internal void Add(string name,FormField<T> field)
        {
            _formFieldList.Add(name,field);
        }

        IEnumerator<FormField<T>> IEnumerable<FormField<T>>.GetEnumerator()
        {
            return _formFieldList.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _formFieldList.Values.GetEnumerator();
        }

        public int Count
        {
            get { return _formFieldList.Count; }
        }
    }
}