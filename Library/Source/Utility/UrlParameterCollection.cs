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
using System.Web;

namespace Vici.Mvc
{
    public class UrlParameterCollection
    {
        private readonly Dictionary<string,List<string>> _parameters = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

        public UrlParameterCollection()
        {
        }

        public void Add(string name, string value)
        {
            List<string> values;

            if (!_parameters.TryGetValue(name, out values))
            {
                values = new List<string>();

                _parameters[name] = values;
            }

            if (value != null)
                values.Add(value);
        }

        public void Remove(string name)
        {
            _parameters.Remove(name);
        }

        public void Replace(string name, string value)
        {
            Remove(name);
            Add(name, value);
        }

        public void RemoveAll()
        {
            _parameters.Clear();
        }

        public UrlParameterCollection Clone()
        {
            UrlParameterCollection collection = new UrlParameterCollection();

            foreach (KeyValuePair<string,List<string>> entry in _parameters)
            {
                collection._parameters[entry.Key] = new List<string>(entry.Value);
            }

            return collection;
        }

        public UrlParameterCollection(NameValueCollection queryString)
        {
            //MethodInfo baseGet = typeof(NameObjectCollectionBase).GetMethod("BaseGet", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, new Type[] { typeof(Int32) }, null);

            for (int i = 0; i < queryString.Count; i++)
            {
                string key = queryString.GetKey(i);

                string[] values = queryString.GetValues(i);

                //ArrayList valueList = (ArrayList)baseGet.Invoke(queryString, new object[] { i });

                if (values == null || values.Length == 0)
                {
                    Add(key,null);
                }
                else
                {
                    foreach (string value in values)
                    {
                        Add(key,value);
                    }
                }
            }
        }

        public override string ToString()
        {
            string q = "";

            foreach (KeyValuePair<string,List<string>> entry in _parameters)
            {
                string key = entry.Key;

                if (key.Length > 0)
                    key = HttpUtility.UrlEncode(key) + '=';

                if (entry.Value.Count == 0)
                {
                    if (q.Length > 0)
                        q += '&';

                    q += key;
                }
                else
                {
                    foreach (string value in entry.Value)
                    {
                        if (q.Length > 0)
                            q += '&';

                        q += key + HttpUtility.UrlEncode(value);
                    }
                }
            }

            return q;
        }


    }
}