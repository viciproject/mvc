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
    public class FormRadioButtonListAttribute : FormFieldAttribute
    {
        public string KeyMember { get; set; }
        public string ValueMember { get; set; }
        public object DataSource { get; set; }
        public string ValueFormatString { get; set; }

        public FormRadioButtonListAttribute()
        {
        }

        public FormRadioButtonListAttribute(string[] items)
        {
            DataSource = items;
        }

        protected internal override object GetControlValue(Control control)
        {
            return ((RadioButtonListControl) control).Value;
        }

        protected internal override void SetControlValue(Control control, object value)
        {
            ((RadioButtonListControl) control).Value = value;
        }

        protected internal override Control CreateControl(string name)
        {
            RadioButtonListControl control = new RadioButtonListControl(name, FieldType)
                                                 {
                                                     DataSource = DataSource,
                                                     KeyMember = KeyMember,
                                                     ValueMember = ValueMember,
                                                     ValueFormatString = ValueFormatString
                                                 };

            return control;
        }

        protected internal override bool IsRightType()
        {
            return true;
        }

        protected internal override bool Validate(Control control)
        {
            return true;
        }
    }
}
