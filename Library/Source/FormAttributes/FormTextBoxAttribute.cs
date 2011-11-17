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
using System.Globalization;
using System.Text.RegularExpressions;
using Vici.Core;
using Vici.Mvc;

namespace Vici.Mvc
{
	public class FormTextBoxAttribute : FormFieldAttribute
	{
		public int Width;
        public int MaxLength = Int32.MaxValue;
	    public string FormatString;
        public bool AutoComplete;
        public string OnKeyDown;
        public string OnKeyUp;
        public string OnKeyPress;
	    public bool ReadOnly;

	    protected internal override bool IsRightType()
		{
            Type type = Nullable.GetUnderlyingType(FieldType) ?? FieldType;

	        return (
                   type == typeof(string)
                || type == typeof(Int16)
                || type == typeof(Int32)
                || type == typeof(Int64)
                || type == typeof(UInt16)
                || type == typeof(UInt32)
                || type == typeof(UInt64)
                || type == typeof(Double)
                || type == typeof(Single)
                || type == typeof(Decimal)
                );
		}

	    protected internal override bool Validate(Control control)
		{
            string value = ((TextBoxControl)control).Value ?? "";

	        bool valid = (value.Length <= MaxLength);

            if (valid && value.Length > 0 && FieldType != typeof(string))
            {
                return Regex.Match(value, @"^\-?\d+([\.,]\d+)?$").Success;
            }

	        return valid;
		}

	    protected internal override Control CreateControl(string name)
	    {
	        TextBoxControl control = new TextBoxControl(name);

            control.MaxLength = MaxLength;
            control.AutoComplete = AutoComplete;
            control.Size = Width;
            control.OnKeyDown = OnKeyDown;
            control.OnKeyUp = OnKeyUp;
            control.OnKeyPress = OnKeyPress;
	        control.ReadOnly = ReadOnly;

            return control;
	    }

	    protected internal override object GetControlValue(Control control)
	    {
            string stringValue = ((TextBoxControl) control).Value;

	        return stringValue.Convert(FieldType);
	    }

        protected internal override void SetControlValue(Control control, object value)
        {
            if (FormatString == null)
                ((TextBoxControl) control).Value = (value != null) ? value.ToString() : "";
            else
                ((TextBoxControl) control).Value = (value != null) ? String.Format("{0:" + FormatString + "}",value) : "";
        }
    }
}