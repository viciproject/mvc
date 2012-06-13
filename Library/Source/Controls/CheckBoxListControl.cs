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
    public enum CheckBoxListStyle
    {
        Div,
        Span
    }

    public class CheckBoxListControl<TKey> : CheckBoxListControl where TKey : IComparable<TKey>
    {
        public CheckBoxListControl(string name)
            : base(name, typeof(TKey))
        {
        }

        public CheckBoxListControl(string name, CheckBoxListStyle style)
            : base(name, typeof(TKey), style)
        {
        }

        public void Check(TKey key)
        {
            if (!Values.Contains(key))
                Values.Add(key);
        }

        public void UnCheck(TKey key)
        {
            if (Values.Contains(key))
                Values.Remove(key);
        }

        public bool IsChecked(TKey key)
        {
            return Values.Contains(key);
        }
    }

    public class CheckBoxListControl : Control
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

        private CheckBoxListStyle _style = CheckBoxListStyle.Span;

        private readonly Type _keyType;

        public List<object> Values;

        private bool _isBound;

        private object _dataSource;

        private string _keyMember;
        private string _valueMember;

        private readonly List<Item> _items = new List<Item>();

        public CheckBoxListControl(string name) : base(name)
        {
            _keyType = typeof(string);
        }

        public CheckBoxListControl(string name, CheckBoxListStyle style) : this(name)
        {
            Style = style;
        }

        protected internal CheckBoxListControl(string name,Type keyType) : base(name)
        {
            _keyType = keyType;
        }

        protected internal CheckBoxListControl(string name,Type keyType, CheckBoxListStyle style) : this(name,keyType)
        {
            Style = style;
        }

        public static string DefaultCssClass
        {
            set { SetDefaultCssClass<CheckBoxListControl>(value, null); }
        }

        public static string DefaultCssClassError
        {
            set { SetDefaultCssClass<CheckBoxListControl>(null, value); }
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

                    Items.Add(new Item(key, v));
                }
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

        public List<Item> Items
        {
            get
            {
                if (!_isBound)
                    Bind();

                return _items;
            }
        }

        public CheckBoxListStyle Style
        {
            get { return _style; }
            set { _style = value; }
        }

        protected override string GenerateHtml(View view, string className, string classNameError)
        {
            if (!_isBound)
                Bind();

            string s = "<div";

            s = AddIdAttribute(s);
            s = AddClassAttribute(s, className, classNameError);

            s += ">\r\n";

            for (int i = 0; i < Items.Count; i++)
            {
                if (Style == CheckBoxListStyle.Div)
                    s += "<div>";

                if (Style == CheckBoxListStyle.Span)
                    s += "<span>";

                bool isChecked = Values.Contains(Items[i].Key);

                s += "<input ";

                s = AddNameAttribute(s);
                s = AddEnabledAttribute(s);
                s = AddOnChangeAttribute(s);
                s = AddTabIndexAttribute(s);

                string value = view.ParseTranslations(Items[i].Value.ToString());

                s += " type='checkbox' value='" + HttpUtility.HtmlEncode(Items[i].Key.ToString()) + "'" + (isChecked  ? " checked='checked'" : "") + " />" + HttpUtility.HtmlEncode(value);

                if (Style == CheckBoxListStyle.Div)
                    s += "</div>";

                if (Style == CheckBoxListStyle.Span)
                    s += "</span>";
            }

            s += "</div>";

            return s;
        }

        protected override void HandlePostback(ClientDataCollection postData)
        {
            Array array = (Array) postData.Get(Name, _keyType.MakeArrayType());

            Values.Clear();

            foreach (object obj in array)
                Values.Add(obj);
        }
    }
}
