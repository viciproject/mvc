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
using System.Text;

namespace Vici.Mvc
{
    internal static class UrlHelper
    {
        internal static string EncodeToUrl(string s)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s)).Replace('+', '_').Replace('/', '-').Replace('=','$');
        }

        internal static string DecodeFromUrl(string s)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(s.Replace('_', '+').Replace('-', '/').Replace('$','=')));
        }

        internal static string GetLanguageFromUrl()
        {
            if (!WebAppConfig.UseLanguagePath)
                return "";

            string url = SessionBase.Request.AppRelativeCurrentExecutionFilePath.Substring(2);

            int idx1 = url.LastIndexOf('/');
			
            if (idx1 > 0)
            {
                int idx2 = url.Substring(0,idx1).LastIndexOf('/');

                if (idx2 >= 0)
                    return url.Substring(idx2+1,idx1-idx2-1).ToUpper();
                else
                    return url.Substring(0,idx1);
            }

            return "";
        }

        internal static string GetUrlPath(string relativePath, string pathInfo)
        {
            string path = relativePath.Substring(2);

            if (WebAppConfig.UseLanguagePath)
                if (path.Split('/').Length > 0 && path.Split('/')[0].Length == 2)
                    path = path.Substring(3);

            return path + pathInfo;
        }
    }
}
