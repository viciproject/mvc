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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web;
using Vici.Core;
using StringConverter=Vici.Core.StringConverter;

namespace Vici.Mvc
{
    public class RadioButtonListControl<TKey> : RadioButtonListControl where TKey : IComparable<TKey>
    {
        public RadioButtonListControl(string name) : base(name, typeof (TKey))
        {
        }

        public new TKey Value
        {
            get { return (TKey) base.Value; }
            set { base.Value = value; }
        }
    }

    public class RadioButtonListControl : Control
    {
        public class Item
        {
            public Item()
            {
            }

            public Item(object key, object value)
            {
                Key = key;
                Value = value;
            }

            public object Key;
            public object Value;
        }

        private readonly Type _keyType;

        private bool _isBound;

        private object _dataSource;

        private string _keyMember;
        private string _valueMember;

        private bool _useLineBreaks;

        private string _valueFormatString;

        private readonly List<Item> _items = new List<Item>();

        public object Value { get; set; }

        public override object DataSource
        {
            set
            {
                _dataSource = value;

                // Set the bound to false because we set a new datasource
                _isBound = false;

                if (_dataSource != null)
                    Bind();
            }
            get
            {
                return _dataSource;
            }
        }

        public string KeyMember
        {
            get
            {
                return _keyMember;
            }
            set
            {
                _keyMember = value;
                _isBound = false;
            }
        }

        public string ValueMember
        {
            get
            {
                return _valueMember;
            }
            set
            {
                _valueMember = value;
                _isBound = false;
            }
        }

        public bool UseLineBreaks
        {
            get { return _useLineBreaks; }
            set { _useLineBreaks = value; }
        }

        public string ValueFormatString
        {
            get { return _valueFormatString; }
            set { _valueFormatString = value; }
        }

        public List<Item> Items
        {
            get
            {
                if (!_isBound)
                    Bind();

                return _items;
            }
        }

        public static string DefaultCsClass
        {
            set
            {
                SetDefaultCssClass<RadioButtonListControl>(value, null);
            }
        }

        public static string DefaultCssErrorClass
        {
            set
            {
                SetDefaultCssClass<RadioButtonListControl>(null, value);
            }
        }

        public RadioButtonListControl(string name) : base(name)
        {
            _keyType = typeof (string);
        }

        protected internal RadioButtonListControl(string name, Type keyType) : base(name)
        {
            _keyType = keyType;
        }

        protected override string GenerateHtml(View view, string className, string classNameError)
        {
            if (!_isBound)
                Bind();

            StringBuilder output = new StringBuilder();

            // Create the opening tag
            //output.Append(AddIdAttribute(""));
            //output.Append(AddEnabledAttribute(""));

            string formatString = _valueFormatString ?? "{1}";

            // Append the elements of the list
            foreach (Item t in Items)
            {
                bool isCurrent = Equals(Value, t.Key);

                string value = view.ParseTranslations(String.Format(formatString, t.Key, t.Value));

                output.Append("<input type=\"radio\" " + AddOnChangeAttribute("") + " " +
                              AddClassAttribute("", className, classNameError) + " " + AddNameAttribute("") + " value=\"" +
                              HttpUtility.HtmlEncode(t.Key.ToString()) + "\"" + (isCurrent ? " checked" : "") + ">" + HttpUtility.HtmlEncode(value) + 
                              (_useLineBreaks ? "<br />" : "") + "\r\n");
            }

            return output.ToString();
        }

        protected override void HandlePostback(ClientDataCollection postData)
        {
            Value = postData[Name].Convert(_keyType);
        }

        private void Bind()
        {
            _isBound = true;

            _items.Clear();

            if (_dataSource is ITypedList)
            {
                ITypedList typedList = (ITypedList) _dataSource;
                IList list = (IList) _dataSource;

                PropertyDescriptorCollection props = typedList.GetItemProperties(null);

                PropertyDescriptor propKey = props.Find(_keyMember, true);
                PropertyDescriptor propValue = props.Find(_valueMember, true);

                if (propKey == null)
                    throw new ViciMvcException(_keyMember + " property does not exist in datasource " + _dataSource);

                if (propValue == null)
                    throw new ViciMvcException(_valueMember + " property does not exist in datasource " + _dataSource);

                foreach (object obj in list)
                {
                    _items.Add(new Item(propKey.GetValue(obj).Convert(_keyType), propValue.GetValue(obj)));
                }
            }
            else if (_dataSource is NameValueCollection)
            {
                NameValueCollection nv = (NameValueCollection) _dataSource;

                for (int i = 0; i < nv.Count; i++)
                {
                    _items.Add(new Item(nv.Keys[i].Convert(_keyType), nv[i]));
                }
            }
            else if (_dataSource is ICollection)
            {
                ICollection col = (ICollection) _dataSource;

                foreach (object obj in col)
                {
                    object key = _keyMember == null
                                     ? obj.Convert(_keyType)
                                     : PropertyHelper.GetObjectProperty(obj, KeyMember).Convert(_keyType);

                    object v = _valueMember == null ? obj : PropertyHelper.GetObjectProperty(obj, ValueMember);

                    _items.Add(new Item(key, v));
                }
            }
        }
    }
}
