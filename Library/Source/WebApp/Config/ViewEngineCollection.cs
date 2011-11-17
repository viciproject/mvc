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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vici.Mvc
{
    public class ViewEngineCollection : IEnumerable<IViewEngine>
    {
        private class ViewEngineDefinition
        {
            public Regex Regex;
            public string Extension;
            public IViewEngine ViewEngine;
        }

        private static List<ViewEngineDefinition> _viewEngines = new List<ViewEngineDefinition>();

        public IEnumerator<IViewEngine> GetEnumerator()
        {
            return _viewEngines.Select(ved => ved.ViewEngine).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IViewEngine viewEngine, params string[] extensions)
        {
            foreach (string extension in extensions)
            {
                string ext = extension;

                if (!ext.StartsWith("."))
                    ext = '.' + ext;

                _viewEngines.Add(new ViewEngineDefinition
                                     {
                                         Regex = new Regex(ext.Replace(".", "\\.") + "$",RegexOptions.IgnoreCase), 
                                         Extension = ext, 
                                         ViewEngine = viewEngine
                                     });
            }
        }

        public void Add(IViewEngine viewEngine, Regex regex, params string[] extensions)
        {
            foreach (string extension in extensions)
            {
                string ext = extension;

                if (!ext.StartsWith("."))
                    ext = '.' + ext;

                _viewEngines.Add(new ViewEngineDefinition
                                     {
                                         Regex = regex,
                                         Extension = ext,
                                         ViewEngine = viewEngine
                                     });
            }
        }

        public IViewEngine Find(string fileName)
        {
            return
                (
                    from
                        ved
                        in
                        _viewEngines
                    where
                        ved.Regex.IsMatch(fileName) && ved.ViewEngine.CanParse(fileName)
                    select
                        ved.ViewEngine
                )
                    .FirstOrDefault();
            
        }

        public IEnumerable<string> Extensions
        {
            get
            {
                return _viewEngines.Select(ve => ve.Extension);
            }
            
        }
    }
}