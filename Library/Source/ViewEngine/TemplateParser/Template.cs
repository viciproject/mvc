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
using System.Text.RegularExpressions;
using System.Web.Caching;
using Vici.Core.Parser;

namespace Vici.Mvc
{
    public class Template
    {
        private readonly ICompiledTemplate _parsedTemplate;
        private readonly ICompiledTemplate _pageTitle;
        private List<ICompiledTemplate> _headSections;

        private readonly string _destinationPath;
        private readonly string _fileName;

        private static readonly object _templateCacheLock = new object();

        private readonly ITemplateParser _templateParser;

        //private static readonly ITemplateParser _templateParser = new ViciTemplateParser();


        internal static Template CreateTemplate(string templateFile, string destinationPath, bool onlyBody)
        {
            if (WebAppContext.Offline)
                return new Template(templateFile,destinationPath, onlyBody);

            string key = "TPL: " + destinationPath + ";" + templateFile;

            Template template = (Template) WebAppContext.WebCache[key];

            if (template != null)
                return template;

            lock (_templateCacheLock)
            {
                template = (Template) WebAppContext.WebCache[key];

                if (template != null)
                    return template;

                template = new Template(templateFile, destinationPath, onlyBody);

                WebAppContext.WebCache.Insert(key, template, new CacheDependency(templateFile), Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
            }

            return template;
        }

        internal static Template CreateTemplate(Template callingTemplate, string templateName, bool onlyBody)
        {
            return CreateTemplate(templateName, callingTemplate._destinationPath, onlyBody);
        }

        private Template(string templateFile, string destinationPath, bool onlyBody)
        {
            _destinationPath = destinationPath;
            _fileName = templateFile;

            _templateParser = WebAppConfig.ViewEngines.Find(templateFile).Parser;

            string contents = TemplateUtil.ReadTemplateContents(_fileName, _destinationPath);

            Match matchTitle = Regex.Match(contents, @"<title\s*>(?<title>.*?)</title>");
            Match matchHead = Regex.Match(contents, @"<!--\s*#STARTCOPY#\s*-->(?<text>.*?)<!--\s*#ENDCOPY#\s*-->", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (matchTitle.Success)
                _pageTitle = CompileTemplate(matchTitle.Groups["title"].Value);

            if (matchHead.Success)
            {
                if (_headSections == null)
                    _headSections = new List<ICompiledTemplate>();

                _headSections.Add(CompileTemplate(matchHead.Groups["text"].Value));
            }

            if (onlyBody)
                contents = TemplateUtil.ExtractBody(contents);

            _parsedTemplate = CompileTemplate(contents);
            _parsedTemplate.FileName = templateFile;
        }

        internal string RenderTitle(View view)
        {
            try
            {
                if (_pageTitle == null)
                    return null;

                return _templateParser.Render(_pageTitle, new TemplateParserContext(view, this));
            }
            catch (TemplateParserException ex)
            {
                throw new ViciMvcException("Error rendering template " + Path.GetFileName(_fileName), ex);
            }
            
        }

        internal string RenderHeadSection(View view)
        {
            try
            {
                if (_headSections == null || _headSections.Count == 0)
                    return null;

                StringBuilder s = new StringBuilder();

                _headSections.ForEach(section => s.Append(_templateParser.Render(section, new TemplateParserContext(view, this))));

                return s.ToString();
            }
            catch (TemplateParserException ex)
            {
                throw new ViciMvcException("Error rendering template " + Path.GetFileName(_fileName), ex);
            }
        }

        internal string Render(View view)
        {
            try
            {
                return _templateParser.Render(_parsedTemplate, new TemplateParserContext(view, this));
            }
            catch (TemplateParserException ex)
            {
                throw new ViciMvcException("Error rendering template " + Path.GetFileName(_fileName), ex);
            }
        }

        internal string GetSubTemplatePath(string templateName)
        {
            if (templateName.StartsWith("~/") || templateName.StartsWith("/"))
                return WebAppContext.Server.MapPath(templateName);
            else
                return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(_fileName), templateName));
        }

        internal string ReadSubTemplate(string templateName)
        {
            string templatePath = GetSubTemplatePath(templateName);

            return TemplateUtil.ExtractBody(TemplateUtil.ReadTemplateContents(templatePath, _destinationPath));
        }

        internal ICompiledTemplate CompileSubTemplate(string templateName)
        {
            string templatePath = GetSubTemplatePath(templateName);

            Template innerTemplate = CreateTemplate(this, templatePath, true);

            if (innerTemplate._headSections != null)
            {
                if (_headSections == null)
                    _headSections = new List<ICompiledTemplate>();

                _headSections.AddRange(innerTemplate._headSections);
            }

            return innerTemplate._parsedTemplate;
        }

        private ICompiledTemplate CompileTemplate(string text)
        {
            try
            {
                return _templateParser.Parse(text);
            }
            catch (TemplateParserException ex)
            {
                throw new ViciMvcException("Error parsing template " + Path.GetFileName(_fileName), ex);
            }
        }

    }
}
