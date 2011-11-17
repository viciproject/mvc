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
using System.Collections.Specialized;
using Vici.Core;

namespace Vici.Mvc
{
    public class ClientDataCollection
    {
        private readonly bool _post;

        public ClientDataCollection(bool post)
        {
            _post = post;

        }

        private NameValueCollection Data
        {
            get
            {
                if (_post)
                    return WebAppContext.Request.Form;
                else
                    return WebAppContext.QueryParameters;
            }
        }

        public string[] Variables
        {
            get { return Data.AllKeys; }
        }

        public bool Has(string name)
        {
            return Data[name] != null;
        }

        public string Get(string name)
        {
            return Data[name];
        }

        public string[] GetValues(string name)
        {
            return Data.GetValues(name);
        }

        public string Get(string name, string defaultValue)
        {
            return Data[name] ?? defaultValue;
        }

        public object Get(string name, Type t)
        {
            if (typeof(Array).IsAssignableFrom(t))
            {
                string[] values = Data.GetValues(name);

                Type elementType = t.GetElementType();

                if (values == null || values.Length == 0)
                    return Array.CreateInstance(elementType, 0);

                Array array = Array.CreateInstance(elementType, values.Length);

                for (int i = 0; i < values.Length; i++)
                    array.SetValue(values[i].Convert(elementType), i);

                return array;
            }

            return Get(name).Convert(t);
        }

        public string this[string name]
        {
            get { return Data[name]; }
        }

        public T Get<T>(string name)
        {
            return Get(name, default(T));
        }

        public T Get<T>(string name, T defaultValue)
        {
            if (!Has(name))
                return defaultValue;

            return (T)Get(name, typeof(T));
        }

    }

 
}
