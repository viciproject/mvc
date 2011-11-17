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
using Vici.Core.Parser;

namespace Vici.Mvc
{
    internal class TemplateParserContext : CSharpContext , ITemplateContext
    {
        private readonly View _view;
        private readonly Template _template;

        public TemplateParserContext(View view, Template template) : base(ParserContextBehavior.Falsy|ParserContextBehavior.ReturnNullWhenNullReference)
        {
            FormatProvider = WebAppConfig.FormatProvider;

            _template = template;
            _view = view;
        }

        public TemplateParserContext(TemplateParserContext parentContext, Template template) : base(parentContext)
        {
            _template = template;
            _view = parentContext.View;
        }

        private TemplateParserContext(TemplateParserContext parentContext) : base(parentContext)
        {
            _template = parentContext.Template;
            _view = parentContext.View;
        }

        public override bool Get(string varName, out object value, out Type type)
        {
            if (base.Get(varName, out value, out type))
                return true;

            if (View.ViewData.TryGetValue(varName, out value, out type))
                return true;

            value = null;
            type = typeof (object);

            return false;
        }

        public override bool Exists(string varName)
        {
            return View.ViewData.Contains(varName) || base.Exists(varName);
        }

        public override IParserContext CreateLocal()
        {
            return new TemplateParserContext(this);
        }

        public Template Template
        {
            get { return _template; }
        }

        public View View
        {
            get { return _view; }
        }
    }
}