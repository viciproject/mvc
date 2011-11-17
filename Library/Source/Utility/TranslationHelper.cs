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
using System.Text.RegularExpressions;

namespace Vici.Mvc
{
    public static class TranslationHelper
    {
        public static string GetTranslation(string viewName, string tag)
        {
            if (WebAppConfig.TranslationProvider != null)
                return WebAppConfig.TranslationProvider.GetTranslation(WebAppContext.Session.LanguageCode, viewName, tag);
            else 
                return null;
        }

        public static string ParseTranslations(string inputString, string viewName)
        {
            if (inputString.IndexOf("${") < 0 && inputString.IndexOf("##") < 0)
                return inputString;

            return Regex.Replace(inputString, @"(\$\{(?<tag>[^\}]+)\})|(##(?<tag>.+?)##)", 
                                 delegate(Match match)
                                     {
                                         string tag = match.Groups["tag"].Value;

                                         return GetTranslation(viewName, tag) ?? ("{?{" + tag + "}?}");
                                     },
                                 RegexOptions.Singleline);
        }
    }
}
