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
using System.IO;
using System.Reflection;

namespace Vici.Mvc
{
    public class OfflineWebSession
    {
        private bool _isNewSession = true;
        private readonly string _sessionID;

        private readonly Dictionary<string, object> _sessionData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private readonly NameValueCollection _postData = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);

        private View _view;

        private readonly string _rootPath;
        private string _redirectedPage;
        private string _currentPage;
        private bool _followRedirects;

        public event EventHandler<ContextCreatedEventArgs> ContextCreated;

        public OfflineWebSession(string rootPath)
        {
            _rootPath = rootPath;
            _sessionID = Guid.NewGuid().ToString("N");
        }

        public void Reset()
        {
            _sessionData.Clear();
        }

        public static void ResetSession()
        {
        }

        internal object GetSessionData(string key)
        {
            if (_sessionData.ContainsKey(key))
                return _sessionData[key];
            else 
                return null;
        }

        internal void SetSessionData(string key, object value)
        {
            _sessionData[key] = value;
        }

        public void ClearSessionData()
        {
            _sessionData.Clear();
        }

        private string RunPage(string url, bool post)
        {
            for (; ; )
            {
                _redirectedPage = null;
                _currentPage = url;

                try
                {
                    HttpContextBase.CreateContext(this, post ? "POST" : "GET", url, post ? _postData : null);

                    if (ContextCreated != null)
                        ContextCreated(this, new ContextCreatedEventArgs(this, WebAppContext.HttpContext));

                    IHttpRequest httpRequest = WebAppContext.Request;

                    string path = UrlHelper.GetUrlPath(httpRequest.AppRelativeCurrentExecutionFilePath,
                                                       httpRequest.PathInfo);

                    ControllerAction controllerAction = WebAppHelper.GetControllerAction(path);

                    MvcPageHandler pageHandler = new MvcPageHandler(controllerAction);

                    pageHandler.ProcessRequest(WebAppContext.HttpContext);

                    _isNewSession = false;

                    _view = WebAppContext.HttpContext.Response.RenderedView;
                }
                catch(TargetInvocationException ex)
                {
                    Exception ex2 = ExceptionHelper.ResolveTargetInvocationException(ex);

                    if (!(ex2 is EndResponseException))
                        throw ex2;
                }
                catch(EndResponseException)
                {
                }

                _redirectedPage = ((OfflineHttpResponse)WebAppContext.HttpContext.Response).RedirectedUrl;

                PostData.Clear();

                if (!_followRedirects || _redirectedPage == null)
                    return WebAppContext.Response.Output;

                post = false;
                url = _redirectedPage;
            }
        }

        public bool FollowRedirects
        {
            get { return _followRedirects; }
            set { _followRedirects = value; }
        }

        public string PageGet(string url)
        {
            return RunPage(url, false);
        }

        public string PagePost(string url)
        {
            return RunPage(url, true);
        }

        public View View
        {
            get { return _view; }
        }

        public ViewDataContainer ViewData
        {
            get { return _view.ViewData; }
        }

        public bool IsNewSession
        {
            get { return _isNewSession; }
        }

        public string RootPath
        {
            get { return _rootPath; }
        }

        public string SessionID
        {
            get { return _sessionID; }
        }

        public string RedirectedPage
        {
            get { return _redirectedPage; }
        }

        public string CurrentPage
        {
            get { return _currentPage; }
        }

        public NameValueCollection PostData
        {
            get { return _postData; }
        }

        public void PushButton(string buttonName)
        {
            PostData[buttonName] = "*";
        }

        public string MapPath(string path)
        {
            if (path.StartsWith("~"))
                path = path.Substring(1);

            if (!path.StartsWith("/"))
                return RootPath;

            return Path.Combine(RootPath, path.Substring(1).Replace('/', '\\'));
        }

    }
}
