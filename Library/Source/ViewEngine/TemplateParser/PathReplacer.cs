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
using System.IO;
using System.Text.RegularExpressions;

namespace Vici.Mvc
{
	internal class PathReplacer
	{
		private readonly string _fromPath;
		private readonly string _toPath;
	    private readonly bool _onSameDrive;

        private static readonly Regex _regex = new Regex(@"(?<quote>['""])(?<url>~/.*?)\k<quote>|((?<=((href|src|action)\s*=\s*))(?<quote>['""])(?<url>[^\${#][^:]+?)\k<quote>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

		public PathReplacer(string fromPath , string toPath)
		{
			if (Path.GetPathRoot(fromPath).ToUpper() != Path.GetPathRoot(toPath).ToUpper())
			    _onSameDrive = false;
            else
			    _onSameDrive = true;

			if (Path.IsPathRooted(fromPath))
			{
				_fromPath = fromPath.Substring(Path.GetPathRoot(fromPath).Length-1);
				_toPath = toPath.Substring(Path.GetPathRoot(toPath).Length-1);
			}
			else
			{
				_fromPath = fromPath;
				_toPath = toPath;
			}
		}

		internal string ReplaceUrls(string inputString)
		{
            if (!_onSameDrive)
                return inputString;

		    return _regex.Replace(inputString, new MatchEvaluator(ReplaceHref));
		}

		private static string SetLastChar(string s , string c)
		{
			if (s.EndsWith(c))
				return s;
			else
				return (s + c);
		}

		private static string RemoveLastPart(string path)
		{
			int i = path.LastIndexOf("/");

			if (i >= 0)
				return path.Substring(0,i);
			else
				return path;
		}

		private string ReplaceHref(Match match)
		{
			string quote = match.Groups["quote"].Value;
			string url = match.Groups["url"].Value;

		    if (url.StartsWith("~/") || url.StartsWith("/"))
                return quote + PathHelper.TranslateAbsolutePath(url) + quote;

			string toPath = _toPath.Replace('\\','/');
			string fromPath = _fromPath.Replace('\\','/');

			toPath = SetLastChar(toPath, "/");
			fromPath = SetLastChar(fromPath, "/");

			string filePath;

			if (url.StartsWith("/"))
				filePath = url;
			else
				filePath = fromPath + url;

			toPath = SetLastChar(toPath , "/");

			if (!filePath.StartsWith("/") && !filePath.StartsWith("\\") && !Path.IsPathRooted(filePath))
				filePath = toPath + filePath;


			string[] pieces = filePath.Split('/');

			filePath = "";

			foreach(string piece in pieces)
				if (piece.Length > 0)
					if (piece == "..")
						filePath = RemoveLastPart(filePath);
					else
						if (piece != ".")
							filePath += "/" + piece;

			pieces = filePath.Split('/');
			string[] piecesTo = toPath.Split('/');

			int firstMismatch = 0;

			for (int i = 0; i < pieces.Length && i < piecesTo.Length;i++ )
			{
				if (pieces[i].ToUpper() != piecesTo[i].ToUpper())
				{
					firstMismatch = 0;

					for (int j = 0; j < i; j++)
						firstMismatch += pieces[j].Length + 1;

					break;
				}
			}

			if (firstMismatch <= filePath.Length)
				filePath = filePath.Substring(firstMismatch, filePath.Length - firstMismatch);

			int pos = firstMismatch;

			while (pos < toPath.Length)
			{
				if (toPath.Substring(pos, 1) == "/")
					filePath = "../" + filePath;

				pos++;
			}

			return quote + filePath + quote;
		}
	}
}
