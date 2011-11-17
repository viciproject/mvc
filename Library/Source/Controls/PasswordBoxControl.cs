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
    public class PasswordBoxControl : TextBoxControl
    {
        public PasswordBoxControl(string name) : base(name)
        {
        }

        protected override string GenerateHtml(View view, string className, string clasError)
        {
            string s = "<input type=\"password\" value=\"" + HttpUtility.HtmlAttributeEncode(Value) + "\"";

            s = AddIdAttribute(s);
            s = AddNameAttribute(s);
            s = AddClassAttribute(s, className, clasError);
            s = AddEnabledAttribute(s);
            s = AddOnChangeAttribute(s);

            if (MaxLength != Int32.MaxValue)
                s += " maxlength=\"" + MaxLength + "\"";

            if (Size != 0)
                s += " size=\"" + Size + "\"";

            if (!string.IsNullOrEmpty(OnKeyDown))
                s += " onkeydown=\"" + OnKeyDown + "\"";

            if (!string.IsNullOrEmpty(OnKeyUp))
                s += " onkeyup=\"" + OnKeyUp + "\"";

            if (!string.IsNullOrEmpty(OnKeyPress))
                s += " onkeypress=\"" + OnKeyPress + "\"";

            s += " autocomplete=\"off\"";

            return s + " />";
        }

    }
}
