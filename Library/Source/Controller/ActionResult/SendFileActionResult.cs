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

namespace Vici.Mvc
{
    public class SendFileActionResult : ActionResult
    {
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
        public string RemoteFileName { get; set; }
        public string LocalFileName { get; set; }

        public SendFileActionResult(string localFileName, string contentType)
            : base(true)
        {
            ContentType = contentType;
            LocalFileName = localFileName;
        }

        public SendFileActionResult(byte[] fileData, string contentType)
            : base(true)
        {
            ContentType = contentType;
            FileData = fileData;
        }

        public SendFileActionResult(string localFileName, string contentType, string remoteFileName)
            : base(true)
        {
            ContentType = contentType;
            LocalFileName = localFileName;
            RemoteFileName = remoteFileName;
        }

        public SendFileActionResult(byte[] fileData, string contentType, string remotefileName)
            : base(true)
        {
            RemoteFileName = remotefileName;
            ContentType = contentType;
            FileData = fileData;
        }

        public override void Execute(HttpContextBase httpContext)
        {
            httpContext.Response.ContentType = ContentType;

            if (RemoteFileName != null)
                httpContext.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + RemoteFileName + "\"");

            if (FileData != null)
                httpContext.Response.BinaryWrite(FileData);
            else
                httpContext.Response.WriteFile(LocalFileName);
        }
    }
}