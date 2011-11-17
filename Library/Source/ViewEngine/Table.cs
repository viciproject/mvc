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

namespace Vici.Mvc
{
	public class Table : List<ViewDataContainer>
	{
        [Obsolete("Use ViewDataContainer")]
        public class Row : ViewDataContainer
        {
            public Row()
            {
            }

            public Row(params object[] dataObjects) : base(dataObjects)
            {
            }
        }

		public int AddRow(ViewDataContainer rec)
		{
			Add(rec);

			return Count-1;
		}

        public ViewDataContainer AddRow(object rec)
        {
            return base[AddRow(new ViewDataContainer(rec))];
        }

		public ViewDataContainer AddRow()
		{
			return base[ AddRow(new ViewDataContainer()) ];
		}

		public ViewDataContainer LastRow
		{
			get
			{
				if (Count > 0)
					return base[Count-1];
				else
					return null;
			}
		}
	}
}
