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
    public static class PropertyHelper
    {
        public static object GetObjectProperty(object item,string property)
        {
            if (item == null)
                return null;

            int dotIdx = property.IndexOf('.');

            if (dotIdx > 0)
            {
                object obj = GetObjectProperty(item,property.Substring(0,dotIdx));

                return GetObjectProperty(obj,property.Substring(dotIdx+1));
            }

            if (item is ViewDataContainer)
            {
                ViewDataContainer dic = (ViewDataContainer)item;

                return dic[property];
            }

            PropertyInfo propInfo = null;
            Type objectType = item.GetType();

            while (propInfo == null && objectType != null)
            {
                propInfo = objectType.GetProperty(property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                objectType = objectType.BaseType;
            }

            if (propInfo != null)
                return propInfo.GetValue(item, null);

            FieldInfo fieldInfo = item.GetType().GetField(property, BindingFlags.Public | BindingFlags.Instance);

            if (fieldInfo != null)
                return fieldInfo.GetValue(item);

            propInfo = item.GetType().GetProperty("Item", new [] { typeof(string) });

            if (propInfo != null)
                return propInfo.GetValue(item, new object[] { property });

            return null;
        }
    }
}
