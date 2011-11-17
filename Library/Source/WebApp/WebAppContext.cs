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
using System.Web.Caching;
using Vici.Core.Json;

namespace Vici.Mvc
{
	public static class WebAppContext
	{
	    private static readonly ClientDataCollection _getParameters = new ClientDataCollection(false);
        private static readonly ClientDataCollection _postParameters = new ClientDataCollection(true);

	    public static SessionBase          Session     { get { return HttpContextBase.Current.SessionObject; } }
        public static HttpContextBase      HttpContext { get { return HttpContextBase.Current;               } }
		public static IHttpResponse        Response    { get { return HttpContextBase.Current.Response;      } }
		public static IHttpRequest         Request     { get { return HttpContextBase.Current.Request;       } }
		public static IHttpServerUtility   Server      { get { return HttpContextBase.Current.Server;        } }
		public static Cache                WebCache    { get { return HttpContextBase.Current.Cache;         } }
        public static ClientDataCollection Parameters  { get { return _getParameters; } }
        public static ClientDataCollection FormData    { get { return _postParameters; } }

        [Obsolete("Use WebAppContext.Parameters")]
        public static ClientDataCollection GetData     { get { return _getParameters; } }
        [Obsolete("Use WebAppContext.FormData")]
        public static ClientDataCollection PostData    { get { return _postParameters; } }

	    public static View RootView
        {
            get { return (View)HttpContext.Items["RootView"]; }
            internal set { HttpContext.Items["RootView"] = value; }
        }

	    public static AjaxContext AjaxContext
	    {
            get { return (AjaxContext) HttpContext.Items["AjaxContext"]; }
            internal set { HttpContext.Items["AjaxContext"] = value; }
	    }

        public static object GetContextItem(object key)
        {
            return HttpContext.Items[key];
        }

        public static void SetContextItem(object key, object value)
        {
            HttpContext.Items[key] = value;
        }

        internal static NameValueCollection QueryParameters
	    {
	        get
	        {
	            NameValueCollection value =  (NameValueCollection) HttpContext.Items["GetParameters"];

                return value ?? Request.QueryString;
	        }
            private set
            {
                HttpContext.Items["GetParameters"] = value;
            }
	    }

        internal static void AddControllerParameters(NameValueCollection parameters)
        {
            NameValueCollection newParameters = new NameValueCollection(QueryParameters);

            newParameters.Add(parameters);

            QueryParameters = newParameters;
        }

	    public static bool Offline
	    {
            get { return (HttpContext is OfflineHttpContext); }
	    }

        public static bool IsPost()
        {
            return Request.RequestType == "POST";
        }

        public static bool IsPost(string buttonName)
        {
            return IsPost() && (!string.IsNullOrEmpty(FormData[buttonName]));
        }
	}
}
