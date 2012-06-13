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
using System.Web;

namespace Vici.Mvc
{
    public class MemoControl : TextBoxControl
    {
        private int _width = 40;
        private int _height = 5;
        private bool _translate = false;

        public MemoControl(string name) : base(name)
        {
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public bool Translate
        {
            get { return _translate;  }
            set { _translate = value; }
        }

        protected override string GenerateHtml(View view, string className, string clasError)
        {
            string s = "<textarea";

            s = AddIdAttribute(s);
            s = AddNameAttribute(s);
            s = AddClassAttribute(s, className,clasError);
            s = AddEnabledAttribute(s);
            s = AddOnChangeAttribute(s);
            s = AddReadOnlyAttribute(s);
            s = AddTabIndexAttribute(s);

            s += " rows=\"" + Height + "\"";
            s += " cols=\"" + Width + "\"";

            if (!string.IsNullOrEmpty(OnKeyDown))
                s += " onkeydown=\"" + OnKeyDown + "\"";

            if (!string.IsNullOrEmpty(OnKeyUp))
                s += " onkeyup=\"" + OnKeyUp + "\"";

            if (!string.IsNullOrEmpty(OnKeyPress))
                s += " onkeypress=\"" + OnKeyPress + "\"";

            string value = Value ?? "";

            if (_translate)
                value = view.ParseTranslations(value);

            return s + ">" + HttpUtility.HtmlEncode(value) + "</textarea>";
        }

        public static new string DefaultCssClass
        {
            set
            {
                SetDefaultCssClass<MemoControl>(value, null);
            }
        }

        public static new string DefaultCssClassError
        {
            set
            {
                SetDefaultCssClass<MemoControl>(null, value);
            }
        }

    }
}
