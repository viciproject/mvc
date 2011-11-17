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
	public class FormDropDownAttribute : FormFieldAttribute
	{
        public bool ShowBlank;
		public  string BlankValue;
		public  object BlankKey;
        private string _valueFormatString;

		private object _dataSource;
		private string _keyMember;
		private string _valueMember;

		public FormDropDownAttribute()
		{
		}

    	public FormDropDownAttribute(string [] items)
		{
			DataSource = items;
		}

	    public string KeyMember
	    {
	        get { return _keyMember; }
	        set { _keyMember = value; }
	    }

	    public string ValueMember
	    {
	        get { return _valueMember; }
	        set { _valueMember = value; }
	    }

	    public object DataSource
	    {
	        get { return _dataSource; }
	        set { _dataSource = value; }
	    }

	    public string ValueFormatString
	    {
	        get { return _valueFormatString; }
	        set { _valueFormatString = value; }
	    }


	    protected internal override bool IsRightType()
		{
			return true;
		}

		
	    protected internal override object GetControlValue(Control control)
	    {
	        return ((DropdownControl) control).Value;
	    }

        protected internal override void SetControlValue(Control control,object value)
        {
            ((DropdownControl) control).Value = value;
        }

	    protected internal override Control CreateControl(string name)
	    {
	        DropdownControl control = new DropdownControl(name, FieldType);

            control.ShowBlank = ShowBlank;
	        control.BlankKey = BlankKey;
	        control.BlankValue = BlankValue;
	        control.DataSource = DataSource;
            control.KeyMember = KeyMember;
            control.ValueMember = ValueMember;
            control.ValueFormatString = ValueFormatString;

	        return control;
	    }

	    protected internal override bool Validate(Control control)
	    {
	        return true;
	    }
	}
}