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
using System.Reflection;

namespace Vici.Mvc
{
    public class FormField<T> where T:class
    {
        private readonly MemberInfo _memberInfo;
        private readonly FormFieldAttribute _attribute;
        private readonly List<ValidationAttribute> _validationAttributes;
        private readonly WebForm<T> _webForm;
        private readonly T _dataObject;

        private Control _control;

        public event FieldValidateHandler Validate;

        internal FormField(WebForm<T> webForm, T dataObject, MemberInfo memberInfo, FormFieldAttribute formFieldAttribute)
        {
            _webForm = webForm;
            _memberInfo = memberInfo;
            _attribute = formFieldAttribute;

            _dataObject = dataObject;

            _validationAttributes = new List<ValidationAttribute>((ValidationAttribute[]) memberInfo.GetCustomAttributes(typeof(ValidationAttribute), false));

            _validationAttributes.Sort((att1, att2) => att1.Priority - att2.Priority);
        }

        internal void CreateControl()
        {
            _control = _attribute.CreateControl();
        }

        public Control Control
        {
            get { return _control; }
        }

        internal MemberInfo MemberInfo
        {
            get { return _memberInfo; }
        }

        public FormFieldAttribute Attribute
        {
            get { return _attribute; }
        }

        public bool Error
        {
            get { return _control.Error; }
            set { _control.Error = value; }
        }

        public bool Enabled
        {
            get { return _control.Enabled; }
            set { _control.Enabled = value; }
        }

        public object DataSource
        {
            set { _control.DataSource = value; }
            get { return _control.DataSource; }
        }

        public WebForm<T> Form
        {
            get { return _webForm; }
        }

        internal void SetPropertyFromControl()
        {
            object value = _attribute.GetControlValue(_control);

            if (MemberInfo is FieldInfo)
                ((FieldInfo)MemberInfo).SetValue(_dataObject, value);

            if (MemberInfo is PropertyInfo)
                ((PropertyInfo)MemberInfo).SetValue(_dataObject, value, null);
        }

        internal void SetControlFromProperty()
        {
            if (MemberInfo is FieldInfo)
                _attribute.SetControlValue(_control, ((FieldInfo)MemberInfo).GetValue(_dataObject));

            if (MemberInfo is PropertyInfo)
                _attribute.SetControlValue(_control, ((PropertyInfo)MemberInfo).GetValue(_dataObject, null));
        }

        internal object GetValue()
        {
            if (_memberInfo is FieldInfo)
                return ((FieldInfo)_memberInfo).GetValue(_dataObject);

            if (_memberInfo is PropertyInfo)
                return ((PropertyInfo)_memberInfo).GetValue(_dataObject, null);

            return null;
        }

        internal bool ValidateField(Dictionary<string,object> validationContext, out string validationErrorMessage)
        {
            if (!_attribute.Validate(Control))
            {
                validationErrorMessage = _attribute.ValidationErrorMsg;

                return false;
            }

            object value = GetValue();

            foreach (ValidationAttribute validationAttribute in _validationAttributes)
                if (!validationAttribute.Validate(validationContext, value, _attribute.FieldType))
                {
                    validationErrorMessage = validationAttribute.Message;

                    return false;
                }

            if (Validate != null && !Validate(_attribute.Name, value, out validationErrorMessage))
                return false;

            validationErrorMessage = null;

            return true;
        }
    }
}