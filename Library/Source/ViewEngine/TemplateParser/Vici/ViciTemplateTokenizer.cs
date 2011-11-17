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
using Vici.Core.Parser;

namespace Vici.Mvc
{
    internal static class TokenID
    {
        public const string ExpressionOld = "EXPROLD";
        public const string ExpressionNew = "EXPRNEW";
        public const string Translation = "TRANSLATION";
        public const string Control = "CONTROL";
        public const string Component = "COMPONENT";
    }

    internal class ViciTemplateTokenizer : TemplateTokenizer
    {
        private class ForeachMatcher : WrappedExpressionMatcher
        {
            public ForeachMatcher(string start, string end) : base(false, start,"foreach",end)
            {
            }

            protected override string TranslateToken(string originalToken, WrappedExpressionMatcher tokenProcessor)
            {
                string s = base.TranslateToken(originalToken, tokenProcessor);

                int inIdx = s.IndexOf(" in ");

                if (inIdx < 0)
                    return "\0" + s;
                else
                    return s.Substring(0, inIdx).Trim() + "\0" + s.Substring(inIdx + 4).Trim();
            }
        }
        private static WrappedExpressionMatcher SquareMatcher(string keyword)
        {
            if (keyword == null)
                return new WrappedExpressionMatcher("$[", "]");
            else
                return new WrappedExpressionMatcher(false, "$[", keyword, "]");
        }

        private static WrappedExpressionMatcher CurlyMatcher(string keyword)
        {
            if (keyword == null)
                return new WrappedExpressionMatcher("{{", "}}");
            else
                return new WrappedExpressionMatcher(false, "{{", keyword, "}}");
        }

        private static WrappedExpressionMatcher SquareCommentMatcher(string keyword)
        {
            if (keyword == null)
                return new WrappedExpressionMatcher("<!--$[", "]-->");
            else
                return new WrappedExpressionMatcher(false, "<!--$[", keyword, "]-->");
        }

        private static WrappedExpressionMatcher CurlyCommentMatcher(string keyword)
        {
            if (keyword == null)
                return new WrappedExpressionMatcher("<!--{{", "}}-->");
            else
                return new WrappedExpressionMatcher(false, "<!--{{", keyword , "}}-->");
        }


        public ViciTemplateTokenizer()
        {
            /*
             * ProMesh 2.0 Tags
             * ================
             * {{ tag }}
             * {{ tag ` formatString }}
             * ## translation ##
             * ## translation(param1,param2,...) ##
             * [[ control ]]
             * [[ control : cssClass : errorClass ]]
             * {{ INCLUDE template }}
             * {{ RENDER template }}
             * {{ RENDER template, @var1=var1, @var2=var2, ... }}
             * {% component, @var1=var1, @var2=var2, ... %}
             * 
             * Deprecated
             * ================
             * $[tag]
             * $[tag:formatString]
             * $[[control]]
             * $[[control:cssClass:errorClass]]
             * ${translation}
             * ${translation(param1,param2,...)}
             * $[RENDER template]
             * $[RENDER template, @var1=var1, @var2=var2]
             * $[INCLUDE template]
             */

            AddTokenMatcher(TemplateTokenType.MacroDefinition, CurlyCommentMatcher("macro"));
            AddTokenMatcher(TemplateTokenType.MacroCall, CurlyCommentMatcher("call"));

            AddTokenMatcher(TemplateTokenType.ForEach, new ForeachMatcher("<!--$[","]-->"), true);
            AddTokenMatcher(TemplateTokenType.ForEach, new ForeachMatcher("<!--{{", "}}-->"), true);

            AddTokenMatcher(TemplateTokenType.EndBlock, SquareCommentMatcher("endfor"), true);
            AddTokenMatcher(TemplateTokenType.EndBlock, SquareCommentMatcher("endif"), true);
            AddTokenMatcher(TemplateTokenType.EndBlock, CurlyCommentMatcher("endfor"), true);
            AddTokenMatcher(TemplateTokenType.EndBlock, CurlyCommentMatcher("endif"), true);
            AddTokenMatcher(TemplateTokenType.EndBlock, CurlyCommentMatcher("end"), true);
            
            AddTokenMatcher(TemplateTokenType.If, SquareCommentMatcher("if"), true);
            AddTokenMatcher(TemplateTokenType.If, CurlyCommentMatcher("if"), true);

            AddTokenMatcher(TemplateTokenType.ElseIf, SquareCommentMatcher("elseif"), true);
            AddTokenMatcher(TemplateTokenType.ElseIf, CurlyCommentMatcher("elseif"), true);

            AddTokenMatcher(TemplateTokenType.Else, CurlyCommentMatcher("else"), true);
            AddTokenMatcher(TemplateTokenType.Else, SquareCommentMatcher("else"), true);

            AddTokenMatcher(TemplateTokenType.ParseFile, SquareCommentMatcher("render"));
            AddTokenMatcher(TemplateTokenType.IncludeFile, SquareCommentMatcher("include"));
            AddTokenMatcher(TemplateTokenType.ParseFile, CurlyCommentMatcher("render"));
            AddTokenMatcher(TemplateTokenType.IncludeFile, CurlyCommentMatcher("include"));

            AddTokenMatcher(TemplateTokenType.ParseFile, SquareMatcher("render"));
            AddTokenMatcher(TemplateTokenType.IncludeFile, SquareMatcher("include"));
            AddTokenMatcher(TemplateTokenType.ParseFile, CurlyMatcher("render"));
            AddTokenMatcher(TemplateTokenType.IncludeFile, CurlyMatcher("include"));

            AddTokenMatcher(TemplateTokenType.Expression, new WrappedExpressionMatcher("{%", "%}"), TokenID.Component);

            AddTokenMatcher(TemplateTokenType.Expression, new WrappedExpressionMatcher("$[[", "]]"), TokenID.Control);
            AddTokenMatcher(TemplateTokenType.Expression, new WrappedExpressionMatcher("[[", "]]"), TokenID.Control);
            
            AddTokenMatcher(TemplateTokenType.Statement, CurlyCommentMatcher(null), true);
            AddTokenMatcher(TemplateTokenType.Statement, SquareCommentMatcher(null), true);

            AddTokenMatcher(TemplateTokenType.Expression, CurlyMatcher(null), TokenID.ExpressionNew);
            AddTokenMatcher(TemplateTokenType.Expression, SquareMatcher(null), TokenID.ExpressionOld);

            AddTokenMatcher(TemplateTokenType.Expression, new WrappedExpressionMatcher("${", "}"), TokenID.Translation);
            AddTokenMatcher(TemplateTokenType.Expression, new WrappedExpressionMatcher("##", "##"), TokenID.Translation);

        }
    }
}