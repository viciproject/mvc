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
using System.Reflection;
using System.Text;

namespace Vici.Mvc
{
    public static class ResourceHelper
    {
        public static byte[] GetResourceBytes(Assembly assembly , string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                byte[] data = new byte[stream.Length];

                stream.Read(data, 0, (int)stream.Length);

                return data;
            }
        }

        public static string GetResourceText(Assembly assembly, string resourceName)
        {
            byte[] data = GetResourceBytes(assembly, resourceName);

            int startIndex = 0;

            if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
                startIndex = 3;

            return Encoding.UTF8.GetString(data, startIndex, data.Length - startIndex);
        }

        internal static byte[] StripEncodingBOM(byte[] data, out Encoding contentEncoding)
        {
            int startIndex = 0;

            contentEncoding = Encoding.UTF8;

            if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
            {
                contentEncoding = Encoding.UTF8;

                startIndex = 3;
            }
            else if (data.Length >= 2 && data[0] == 0xFF && data[1] == 0xFE)
            {
                contentEncoding = Encoding.Unicode;

                startIndex = 2;
            }

            if (startIndex > 0)
            {
                byte[] newData = new byte[data.Length - startIndex];

                Array.Copy(data,startIndex,newData,0,newData.Length);

                return newData;
            }

            return data;
        }

        public static string CreateResourceUrl(Assembly assembly, string resourceName, string contentType)
        {
            return PathHelper.TranslateAbsolutePath("~/_$res$_.axd/" + UrlHelper.EncodeToUrl(contentType) + "/" + UrlHelper.EncodeToUrl(assembly.GetName().Name) + '/' + UrlHelper.EncodeToUrl(resourceName));
        }
    }
}
