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
using System.Collections.Specialized;
using System.Web;
using System.Web.SessionState;

namespace Vici.Mvc
{
    public class OfflineHttpSessionState : IHttpSessionState
    {
        private readonly OfflineWebSession _webSession;

        public OfflineHttpSessionState(OfflineWebSession webSession)
        {
            _webSession = webSession;
        }

        public bool IsNewSession
        {
            get { return _webSession.IsNewSession; }
        }

        public SessionStateMode Mode
        {
            get { return SessionStateMode.InProc; }
        }

        public bool IsCookieless
        {
            get { return false; }
        }

        public HttpCookieMode CookieMode
        {
            get { return HttpCookieMode.UseCookies; }
        }

        public int LCID
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int CodePage
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IHttpSessionState Contents
        {
            get { throw new NotImplementedException(); }
        }

        public HttpStaticObjectsCollection StaticObjects
        {
            get { throw new NotImplementedException(); }
        }

        public NameObjectCollectionBase.KeysCollection Keys
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Abandon()
        {
            
        }

        public void Add(string name, object value)
        {
            _webSession.SetSessionData(name, value);
        }

        public string SessionID
        {
            get { return _webSession.SessionID; }
        }

        public int Timeout
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public object this[string name]
        {
            get { return _webSession.GetSessionData(name); }
            set { _webSession.SetSessionData(name,value); }
        }

        public object this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Clear()
        {
            _webSession.ClearSessionData();
        }

        public void RemoveAll()
        {
            _webSession.ClearSessionData();
        }

        public void Remove(string key)
        {
            _webSession.SetSessionData(key,null);
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}