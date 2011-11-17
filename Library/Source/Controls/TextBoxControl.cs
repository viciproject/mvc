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
    public class TextBoxControl : Control
    {
        private string _value = "";
        private int _maxLength = Int32.MaxValue;
        private bool _autoComplete;
        private int _size;
        private string _onKeyPress;
        private string _onKeyDown;
        private string _onKeyUp;
        private bool _readOnly;

        public TextBoxControl(string name) : base(name)
        {
        }

        public string Value
        {
            get { return _value; }
            set { _value = value ?? ""; }
        }

        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }

        public bool AutoComplete
        {
            get { return _autoComplete; }
            set { _autoComplete = value; }
        }

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public string OnKeyPress
        {
            get { return _onKeyPress; }
            set { _onKeyPress = value; }
        }

        public string OnKeyDown
        {
            get { return _onKeyDown; }
            set { _onKeyDown = value; }
        }

        public string OnKeyUp
        {
            get { return _onKeyUp; }
            set { _onKeyUp = value; }
        }

        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        protected override string GenerateHtml(View view, string className, string clasError)
        {
            string s = "<input type=\"text\" value=\"" + HttpUtility.HtmlEncode(_value) + "\"";

            s = AddIdAttribute(s);
            s = AddNameAttribute(s);
            s = AddClassAttribute(s, className, clasError);
            s = AddEnabledAttribute(s);
            s = AddOnChangeAttribute(s);
            s = AddReadOnlyAttribute(s);

            if (Size != 0)
                s += " size=\"" + Size + "\"";

            if (MaxLength != Int32.MaxValue)
                s += " maxlength=\"" + MaxLength + "\"";

            if (!AutoComplete)
                s += " autocomplete=\"off\"";

            if (!string.IsNullOrEmpty(OnKeyDown))
                s += " onkeydown=\"" + OnKeyDown + "\"";

            if (!string.IsNullOrEmpty(OnKeyUp))
                s += " onkeyup=\"" + OnKeyUp + "\"";

            if (!string.IsNullOrEmpty(OnKeyPress))
                s += " onkeypress=\"" + OnKeyPress + "\"";

            return s + "/>";
        }

        protected string AddReadOnlyAttribute(string html)
        {
            if (ReadOnly)
                html += " readonly=\"readonly\"";

            return html;
        }

        protected override void HandlePostback(ClientDataCollection postData)
        {
            if (postData.Has(Name))
                _value = postData.Get(Name);
        }

        public override object DataSource
        {
            get { return null; }
            set { }
        }

        public static string DefaultCssClass
        {
            set
            {
                SetDefaultCssClass<TextBoxControl>(value, null);
            }
        }

        public static string DefaultCssClassError
        {
            set
            {
                SetDefaultCssClass<TextBoxControl>(null, value);
            }
        }
    }
}
