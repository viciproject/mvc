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
    public class CheckBoxControl : Control
    {
        public bool Checked { get; set; }
        public string OnClick { get; set; }
        public string Value { get; set; }

        public CheckBoxControl(string name) : base(name)
        {
            Value = "1";
        }

        protected override string GenerateHtml(View view, string className, string classNameError)
        {
            string s = "<input type=\"checkbox\"";

            if (!string.IsNullOrEmpty(Value))
                s += " value=\"" + HttpUtility.HtmlEncode(Value) + "\"";

            s = AddIdAttribute(s);
            s = AddNameAttribute(s);
            s = AddClassAttribute(s, className, classNameError);
            s = AddEnabledAttribute(s);
            s = AddOnChangeAttribute(s);
            s = AddTabIndexAttribute(s);

            if (!string.IsNullOrEmpty(OnClick))
                s += " onclick=\"" + OnClick + "\"";

            if (Checked)
                s += " checked=\"checked\"";

            return s + " />";
        }

        protected override void HandlePostback(ClientDataCollection postData)
        {
            string[] values = postData.GetValues(Name);

            if (values == null)
            {
                Checked = false;
            }
            else
            {
                Checked = Array.Find(values, s => Value == s) != null;
            }
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
                SetDefaultCssClass<CheckBoxControl>(value, null);
            }
        }

        public static string DefaultCssClassError
        {
            set
            {
                SetDefaultCssClass<CheckBoxControl>(null, value);
            }
        }

    }
}
