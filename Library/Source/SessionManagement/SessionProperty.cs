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
    public delegate T Creator<T>();

    public abstract class SessionProperty
    {
        private readonly string _sessionKey;

        internal SessionProperty(string sessionKey)
        {
            _sessionKey = sessionKey;
        }

        public string SessionKey
        {
            get { return _sessionKey; }
        }

        internal abstract bool CreateNew { set; }

        public void Clear()
        {
            WebAppContext.HttpContext.Session.Remove(_sessionKey);
        }

        public class KeyAttribute : Attribute
        {
            private readonly string _key;

            public KeyAttribute(string key)
            {
                _key = key;
            }

            public string Key
            {
                get { return _key; }
            }
        }

        public class DefaultValueAttribute : Attribute
        {
            private readonly object _defaultValue;

            public DefaultValueAttribute(object defaultValue)
            {
                _defaultValue = defaultValue;
            }

            public object Value
            {
                get { return _defaultValue; }
            }
        }

        public class AutoCreateNewAttribute : Attribute
        {
        }
    }

    public class SessionProperty<T> : SessionProperty
    {
        private readonly T _defaultValue;
        private Creator<T> _defaultValueCreator;

        public SessionProperty(string sessionKey) : base(sessionKey)
        {
            _defaultValue = default(T);
        }

        public SessionProperty(string sessionKey, T defaultValue) :base(sessionKey)
        {
            _defaultValue = defaultValue;
        }

        public SessionProperty(string sessionKey, Creator<T> defaultValueCreator) : base(sessionKey)
        {
            _defaultValueCreator = defaultValueCreator;
        }

 
        public T Value
        {
            get
            {
                if (WebAppContext.HttpContext.Session == null)
                    return default(T);

                if (WebAppContext.HttpContext.Session[SessionKey] == null)
                {
                    if (_defaultValueCreator != null)
                    {
                        T value = _defaultValueCreator();

                        Value = value;

                        return value;
                    }
                    else
                        return _defaultValue;
                }
                else
                    return (T) WebAppContext.HttpContext.Session[SessionKey];
            }
            set
            {
                if (WebAppContext.HttpContext.Session != null)
                    WebAppContext.HttpContext.Session[SessionKey] = value;
            }
        }

        internal override bool CreateNew
        {
            set
            {
                if (value)
                    _defaultValueCreator = delegate { return (T) Activator.CreateInstance(typeof(T)); };
                else
                    _defaultValueCreator = null;
            }
        }
    }
}
