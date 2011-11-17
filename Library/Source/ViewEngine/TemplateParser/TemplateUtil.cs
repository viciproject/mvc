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
using System.IO;
using System.Text;

namespace Vici.Mvc
{
	internal static class TemplateUtil
	{
		internal static string ReadTemplateContents(string templateFileName , string destinationPath)
		{
			string templatePath = Path.GetDirectoryName(templateFileName);

            try
            {
                using (StreamReader stream = new StreamReader(templateFileName, Encoding.GetEncoding("windows-1252")))
                {
                    return ReplaceUrls(stream.ReadToEnd(), templatePath, destinationPath);
                }
            }
            catch (IOException)
            {
                throw new TemplateNotFoundException(Path.GetFileName(templateFileName));
            }
		}

		internal static string ExtractBody(string s)
		{
            int start = s.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
            int end = s.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);

            if (start >= 0)
                start = s.IndexOf('>',start);

            if (start >= 0 && end > start)
            {
                return s.Substring(start + 1, end - start - 1);
            }
            else
            {
                return s;
            }
        }

        internal static string ReplaceUrls(string inputString, string fromPath, string toPath)
        {
            return new PathReplacer(fromPath, toPath).ReplaceUrls(inputString);
        }
	}
}
