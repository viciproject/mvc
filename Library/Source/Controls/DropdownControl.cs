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
using System.Web;
using Vici.Core;

namespace Vici.Mvc
{
    public class DropdownControl<TKey> : DropdownControl where TKey:IComparable<TKey>
    {
        public DropdownControl(string name) 
            : base(name, typeof(TKey))
        {
        }

        public new TKey Value
        {
            get { return (TKey) base.Value; }
            set { base.Value = value; }
        }

        public new TKey BlankKey
        {
            get { return (TKey) base.BlankKey; }
            set { base.BlankKey = value; }
        }
    }

    public class DropdownControl : Control
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

        public object Value;

        private bool _showBlank;
        private string _blankValue;
        private object _blankKey;

        private bool _isBound;

        private object _dataSource;

        private string _keyMember;
        private string _valueMember;

        private string _valueFormatString;

        private readonly List<Item> _items = new List<Item>();

        public DropdownControl(string name) : base(name)
        {
            _keyType = typeof(string);
        }

        protected internal DropdownControl(string name, Type keyType) : base(name)
        {
            _keyType = keyType;
        }

        public static string DefaultCssClass
        {
            set { SetDefaultCssClass<DropdownControl>(value,null); }
        }

        public static string DefaultCssClassError
        {
            set { SetDefaultCssClass<DropdownControl>(null,value); }
        }

        public override object DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;

                _isBound = false;
            }
        }

        private void Bind()
        {
            _isBound = true;

            Items.Clear();

            if (_dataSource is ITypedList)
            {
                ITypedList typedList = (ITypedList)_dataSource;
                IList list = (IList)_dataSource;

                PropertyDescriptorCollection props = typedList.GetItemProperties(null);

                PropertyDescriptor propKey = props.Find(KeyMember, true);
                PropertyDescriptor propValue = props.Find(ValueMember, true);

                if (propKey == null)
                    throw new ViciMvcException(KeyMember + " property does not exist in datasource " + _dataSource);

                if (propValue == null)
                    throw new ViciMvcException(ValueMember + " property does not exist in datasource " + _dataSource);

                foreach (object obj in list)
                {
                    Items.Add(new Item(propKey.GetValue(obj).Convert(_keyType), propValue.GetValue(obj)));
                }
            }
            else if (_dataSource is NameValueCollection)
            {
                NameValueCollection nv = (NameValueCollection)_dataSource;

                for (int i = 0; i < nv.Count; i++)
                {
                    Items.Add(new Item(nv.Keys[i].Convert(_keyType), nv[i]));
                }
            }
            else if (_dataSource is object[,])
            {
                object[,] array = (object[,])_dataSource;

                for (int i = 0; i <= array.GetUpperBound(0); i++)
                {
                    Items.Add(new Item(array[i, 0].Convert(_keyType), array[i, 1]));
                }
            }
            else if (_dataSource is IEnumerable)
            {
                foreach (object obj in (IEnumerable)_dataSource)
                {
                    object key;
                    object v;

                    if (KeyMember == null)
                        key = obj.Convert(_keyType);
                    else
                        key = PropertyHelper.GetObjectProperty(obj, KeyMember).Convert(_keyType);

                    if (ValueMember == null)
                        v = obj;
                    else
                        v = PropertyHelper.GetObjectProperty(obj, ValueMember);

                    Items.Add(new Item(key,v));
                }
            }

            if (_showBlank)
            {
                Items.Insert(0, new Item(_blankKey,_blankValue));
            }
        }

        public string KeyMember
        {
            get { return _keyMember; }
            set { _keyMember = value; _isBound = false; }
        }

        public string ValueMember
        {
            get { return _valueMember; }
            set { _valueMember = value; _isBound = false; }
        }

        public bool ShowBlank
        {
            get { return _showBlank; }
            set { _showBlank = value; _isBound = false; }
        }

        public string BlankValue
        {
            get { return _blankValue; }
            set { _blankValue = value; _isBound = false; }
        }

        public object BlankKey
        {
            get { return _blankKey; }
            set { _blankKey = value; _isBound = false; }
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

        protected override string GenerateHtml(View view, string className, string classNameError)
        {
            if (!_isBound)
                Bind();

            string s = "<select";

            s = AddIdAttribute(s);
            s = AddNameAttribute(s);
            s = AddClassAttribute(s, className , classNameError);
            s = AddEnabledAttribute(s);
            s = AddOnChangeAttribute(s);
            s = AddTabIndexAttribute(s);

            s += ">\r\n";

            string formatString = _valueFormatString ?? "{1}";

            for (int i = 0; i < Items.Count; i++)
            {
                bool isCurrent = Equals(Value, Items[i].Key);

                string value = view.ParseTranslations(String.Format(formatString, Items[i].Key, Items[i].Value));

                s += "<option value='" + HttpUtility.HtmlEncode(Items[i].Key.ToString()) + "'" + (isCurrent ? " selected='selected'" : "") + ">" + HttpUtility.HtmlEncode(value) + "</option>\r\n";
            }

            s += "</select>";

            return s;

        }

        protected override void HandlePostback(ClientDataCollection postData)
        {
            Value = postData[Name].Convert(_keyType);
        }
    }
}
