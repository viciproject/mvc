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
    public class RadioButtonControl : Control
    {
        private string _group;
        private bool _checked;
        private string _onClick;

        public RadioButtonControl(string name) : base(name)
        {
        }

        public RadioButtonControl(string name, string group) : base(name)
        {
            _group = group;
        }

        public bool Checked
        {
            get { return _checked; }
            set { _checked = value; }
        }

        public string OnClick
        {
            get { return _onClick; }
            set { _onClick = value; }
        }

        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        protected override string GenerateHtml(View view, string className, string classNameError)
        {
            string s = "<input type=\"radio\" value=\"" + HttpUtility.HtmlAttributeEncode(Name) + "\"";
            
            s = AddIdAttribute(s);

            s += " name=\"" + HttpUtility.HtmlAttributeEncode(Group) + "\"";

            s = AddClassAttribute(s, className, classNameError);
            s = AddEnabledAttribute(s);

            if (!string.IsNullOrEmpty(_onClick))
                s += " onclick=\"" + _onClick + "\"";

            if (Checked)
                s += " checked=\"checked\"";

            return s + " />";
        }

        protected override void HandlePostback(ClientDataCollection postData)
        {
            Checked = (postData.Get(Group) == Name);
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
                SetDefaultCssClass<RadioButtonControl>(value, null);
            }
        }

        public static string DefaultCssClassError
        {
            set
            {
                SetDefaultCssClass<RadioButtonControl>(null, value);
            }
        }


    }
}
