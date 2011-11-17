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
    public static class SessionManager
    {
        private static SessionProperty CreateSessionProperty( FieldInfo field, string key , object defaultValue, bool createNew)
        {
            ConstructorInfo constructor;
            Type genericType = field.FieldType.GetGenericArguments()[0];

            if (createNew)
            {
                constructor = field.FieldType.GetConstructor(new[] { typeof(string) });

                SessionProperty value = (SessionProperty)constructor.Invoke(new object[] { key });

                value.CreateNew = true;

                return value;
            }
            else if (defaultValue != null)
            {
                constructor = field.FieldType.GetConstructor(new[] { typeof(string), genericType });

                return (SessionProperty) constructor.Invoke(new[] { key, defaultValue });
            }
            else
            {
                constructor = field.FieldType.GetConstructor(new[] {typeof (string)});

                return (SessionProperty) constructor.Invoke(new[] { key });
            }
        }


        public static void ClearSessionProperties(Type type)
        {
            for (; type != typeof(object); type = type.BaseType)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

                foreach (FieldInfo field in fields)
                {
                    Type fieldType = field.FieldType;

                    if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(SessionProperty<>))
                    {
                        string key = null;

                        if (field.IsDefined(typeof(SessionProperty.KeyAttribute), false))
                        {
                            SessionProperty.KeyAttribute attr = ((SessionProperty.KeyAttribute[])field.GetCustomAttributes(typeof(SessionProperty.KeyAttribute), false))[0];

                            key = attr.Key;
                        }

                        if (key == null)
                        {
                            key = BuildSessionKey(field);
                        }

                        WebAppContext.HttpContext.Session.Remove(key);
                    }
                }
            }
        }

        public static void CreateSessionProperties(Type type)
        {
            CreateSessionProperties(null, type);
        }

        public static void CreateSessionProperties(object obj)
        {
            CreateSessionProperties(obj, obj.GetType());
        }

        private static void CreateSessionProperties(object obj, Type type)
        {
            for (; type != typeof(object) ; type = type.BaseType )
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                foreach (FieldInfo field in fields)
                {
                    Type fieldType = field.FieldType;

                    if (obj == null && !field.IsStatic)
                        continue;

                    if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(SessionProperty<>))
                    {
                        if (field.GetValue(field.IsStatic ? null : obj) == null)
                        {
                            string key = null;
                            object defaultValue = null;
                            bool createNew = false;

                            if (field.IsDefined(typeof(SessionProperty.KeyAttribute), false))
                            {
                                SessionProperty.KeyAttribute attr = ((SessionProperty.KeyAttribute[])field.GetCustomAttributes(typeof(SessionProperty.KeyAttribute), false))[0];

                                key = attr.Key;
                            }

                            if (field.IsDefined(typeof(SessionProperty.DefaultValueAttribute), false))
                            {
                                SessionProperty.DefaultValueAttribute attr = ((SessionProperty.DefaultValueAttribute[])field.GetCustomAttributes(typeof(SessionProperty.DefaultValueAttribute), false))[0];

                                defaultValue = attr.Value;
                            }

                            if (field.IsDefined(typeof(SessionProperty.AutoCreateNewAttribute), false))
                            {
                                createNew = true;
                            }

                            if (key == null)
                            {
                                key = BuildSessionKey(field);
                            }

                            field.SetValue(field.IsStatic ? null : obj, CreateSessionProperty(field, key, defaultValue, createNew));
                        }
                    }
                }
            }
        }

        private static string BuildSessionKey(FieldInfo field)
        {
            return field.DeclaringType.FullName + "." + field.Name;
        }
    }
}
