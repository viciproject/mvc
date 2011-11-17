using System;
using System.Collections;
using System.Collections.Generic;

namespace Vici.Mvc
{
    public class FormCheckBoxListAttribute : FormFieldAttribute
    {
        private Type _keyType;

        public object DataSource { get; set; }

        public string KeyMember { get; set; }
        public string ValueMember { get; set; }

        public Type ValueType { get; set; }

        public CheckBoxListStyle Style { get; set; }

        public FormCheckBoxListAttribute()
        {
            ValueType = typeof (string);
        }

        public FormCheckBoxListAttribute(string[] items) 
            : this()
        {
            DataSource = items;
        }

        protected internal override object GetControlValue(Control control)
        {
            var checkBoxListControl = ((CheckBoxListControl) control);

            if (FieldType.IsArray)
            {
                Array array = Array.CreateInstance(FieldType.GetElementType(), checkBoxListControl.Values.Count);

                for (int i = 0; i < checkBoxListControl.Values.Count; i++)
                    array.SetValue(checkBoxListControl.Values[i], i);

                return array;
            }

            // If it's not an array, it must be an IList

            IList listObject = (IList) Activator.CreateInstance(FieldType);

            foreach (object value in checkBoxListControl.Values)
                listObject.Add(value);

            return listObject;
        }

        protected internal override void SetControlValue(Control control, object value)
        {
            var checkBoxListControl = (CheckBoxListControl)control;

            if (checkBoxListControl.Values == null)
                checkBoxListControl.Values = new List<object>();
            else
                checkBoxListControl.Values.Clear();

            foreach (object valueObject in (IEnumerable) value)
                checkBoxListControl.Values.Add(valueObject);
        }

        protected internal override Control CreateControl(string name)
        {
            if (FieldType.IsArray)
            {
                _keyType = FieldType.GetElementType();
            }
            else
            {
                foreach (Type type in FieldType.GetInterfaces())
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (IList<>))
                        _keyType = type.GetGenericArguments()[0];
                }
            }

            CheckBoxListControl control = new CheckBoxListControl(name, _keyType, Style)
                                              {
                                                  DataSource = DataSource,
                                                  KeyMember = KeyMember,
                                                  ValueMember = ValueMember
                                              };

            return control;
        }

        protected internal override bool IsRightType()
        {
            if (FieldType.IsArray)
                return true;

            foreach (Type type in FieldType.GetInterfaces())
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
                    return true;
            }

            return false;
        }

        protected internal override bool Validate(Control control)
        {
            return true;
        }
    }
}
