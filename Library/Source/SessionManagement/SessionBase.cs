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
using System.Web;

namespace Vici.Mvc
{
    public interface IVisitorRecord
    {
        
    }

    public interface ISessionRecord
    {
        
    }

    public class SessionBase<TSessionRecord,TVisitorRecord> : SessionBase 
        where TSessionRecord: class, ISessionRecord
        where TVisitorRecord: class, IVisitorRecord
    {
        private TSessionRecord _sessionRecord;
        private TVisitorRecord _visitorRecord;

        public TSessionRecord SessionRecord
        {
            get
            {
                if (_sessionRecord == null)
                    _sessionRecord = WebAppConfig.SessionLoggingProvider.GetSessionObject<TSessionRecord>(SessionID);

                return _sessionRecord;
            }
        }

        public TVisitorRecord VisitorRecord
        {
            get
            {
                if (_visitorRecord == null)
                    _visitorRecord = WebAppConfig.VisitorProvider.GetVisitorObject<TVisitorRecord>(VisitorID);

                return _visitorRecord;
            }
        }
    }

	public class SessionBase 
	{
		private readonly IHttpSessionState _httpSession = null;

		private string _sessionID = null;
		private string _visitorID = null;
		private string _userID    = null;
		private string _languageCode = null;

		public static IHttpResponse      Response    { get { return WebAppContext.Response; } }
		public static IHttpRequest       Request     { get { return WebAppContext.Request;  } }
		public static IHttpServerUtility Server      { get { return WebAppContext.Server;   } }

		public static SessionBase CurrentSession
		{
			get { return WebAppContext.Session; }
		}

		public SessionBase()
		{
            _httpSession = WebAppContext.HttpContext.Session;

            SessionManager.CreateSessionProperties(this);


			if (IsNewSession)
			{
				SessionID = CreateSessionDataObject();

		        this["_START_TIME_"] = DateTime.Now;

				if (VisitorID == null)
					VisitorID = CreateVisitor();

                WebAppConfig.SessionLoggingProvider.AssignVisitorToSession(SessionID,VisitorID);
			}

			DetermineLanguage();
		}

	    private void DetermineLanguage()
	    {
	        string urlLanguage = UrlHelper.GetLanguageFromUrl();

	        string lang = null;

	        if (urlLanguage.Length > 0)
	        {
	            lang = urlLanguage;

	            //TODO: check if language is defined. If it isn't, set urlLanguage = ""
	        }
	        else
	        {
	            string langParam = WebAppContext.Parameters["lang"];
			
	            if (!string.IsNullOrEmpty(langParam))
	            {
	                lang = langParam;
	            }
	        }

	        if (lang != null)
	        {
	            LanguageCode = lang;
	        }
	    }

	    public object this[string key]
		{
			get
			{
                if (WebAppConfig.SessionSerializer != null)
                    return WebAppConfig.SessionSerializer.GetSessionVariable(SessionID,key);
                
			    return _httpSession[key];
			}
            set
            {
                if (WebAppConfig.SessionSerializer != null)
                    WebAppConfig.SessionSerializer.SetSessionVariable(SessionID, key, value);
                else
                    _httpSession[key] = value;
            }
		}

		public bool IsNewSession
		{
			get { return _httpSession.IsNewSession; }
		}

		public string InternalSessionID
		{
			get { return _httpSession.SessionID; }
		}

		private static void SetCookie(string cookieName, string cookieValue , bool permanent)
		{
            if (Response.Cookies[cookieName] != null)
			    Response.Cookies[cookieName].Value = cookieValue;
            else 
                Response.Cookies.Add(new HttpCookie(cookieName,cookieValue));
			
			if (permanent)
				Response.Cookies[cookieName].Expires = DateTime.Now.AddYears(10);
		}

		private static string GetCookie(string cookieName)
		{
			HttpCookie httpCookie = Request.Cookies[cookieName];

			if (httpCookie == null)
				return null;
            else
                return httpCookie.Value;
		}

		public string SessionID
		{
			get
			{
				if (_sessionID == null)
                    _sessionID = GetCookie("SESSIONID");

				return _sessionID;
			}
			set
			{
                SetCookie("SESSIONID",value,false);

				_sessionID = value;
			}

		}

		public string LanguageCode
		{
			get
			{
				if (_languageCode == null)
				{
					_languageCode = GetCookie("LANG");

					if (_languageCode == null || _languageCode.Length < 2)
					{
                        return (string) this["_LANG_"] ?? WebAppConfig.DefaultLanguage;
					}
				}

				return _languageCode;
			}
			set
			{
				this["_LANG_"] = value;

				SetCookie("LANG" , value , true);

				_languageCode = value;
			}
		}

		public string VisitorID
		{
			get
			{
				if (_visitorID == null)
				    _visitorID = GetCookie("VISITORID");

				return _visitorID;
			}
			set
			{
                if (value != null)
                {
                    SetCookie("VISITORID", value, true);

                    WebAppConfig.SessionLoggingProvider.AssignVisitorToSession(SessionID, value);
                }

			    _visitorID = value;
			}
		}

		public string UserID
		{
			get
			{
				if (_userID == null)
				{
					if (this["_USER_ID_"] is string)
						_userID = (string) this["_USER_ID_"];
				}

				return _userID;
			}
			set
			{
				this["_USER_ID_"] = value;

				if (value != null)
                    WebAppConfig.SessionLoggingProvider.AssignUserToSession(SessionID,UserID);

				_userID = value;
			}
		}

		public DateTime LogonTime
		{
			get { return (DateTime) this["_START_TIME_"]; }
		}

		private static string CreateSessionDataObject()
		{
            return WebAppConfig.SessionLoggingProvider.CreateSession(Request.UrlReferrer == null ? "" : Request.UrlReferrer.OriginalString, Request.UserHostAddress, Request.UserAgent);
		}

		private static string CreateVisitor()
		{
		    return WebAppConfig.VisitorProvider.CreateVisitor();
		}

	    public virtual void OnSessionCreated()
	    {
	        
	    }
	}
}
