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
using System.Collections.Specialized;
using System.Web;
using System.Web.SessionState;

namespace Vici.Mvc
{
    public class OnlineHttpSessionState : IHttpSessionState
    {
        private readonly HttpSessionState _session;

        public OnlineHttpSessionState(HttpSessionState session)
        {
            _session = session;
        }

        public bool IsNewSession
        {
            get { return _session.IsNewSession; }
        }

        public SessionStateMode Mode
        {
            get { return _session.Mode; }
        }

        public bool IsCookieless
        {
            get { return _session.IsCookieless; }
        }

        public HttpCookieMode CookieMode
        {
            get { return _session.CookieMode; }
        }

        public int LCID
        {
            get { return _session.LCID; }
            set { _session.LCID = value; }
        }

        public int CodePage
        {
            get { return _session.CodePage; }
            set { _session.CodePage = value; }
        }

        public IHttpSessionState Contents
        {
            get { return new OnlineHttpSessionState(_session.Contents); }
        }

        public HttpStaticObjectsCollection StaticObjects
        {
            get { return _session.StaticObjects; }
        }

        public NameObjectCollectionBase.KeysCollection Keys
        {
            get { return _session.Keys; }
        }

        public bool IsReadOnly
        {
            get { return _session.IsReadOnly; }
        }

        public void Abandon()
        {
            _session.Abandon();
        }

        public void Add(string name, object value)
        {
            _session.Add(name,value);
        }

        public string SessionID
        {
            get { return _session.SessionID; }
        }

        public int Timeout
        {
            get { return _session.Timeout; }
            set { _session.Timeout = value; }
        }

        public object this[string name]
        {
            get { return _session[name]; }
            set { _session[name] = value; }
        }

        public object this[int index]
        {
            get { return _session[index]; }
            set { _session[index] = value; }
        }

        public void Clear()
        {
            _session.Clear();
        }

        public void RemoveAll()
        {
            _session.RemoveAll();
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }

        public void RemoveAt(int index)
        {
            _session.RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            _session.CopyTo(array, index);
        }

        public int Count
        {
            get { return _session.Count; }
        }

        public object SyncRoot
        {
            get { return _session.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return _session.IsSynchronized; }
        }

        public IEnumerator GetEnumerator()
        {
            return _session.GetEnumerator();
        }
    }
}