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
using System.Globalization;
using Vici.Core.Parser;

namespace Vici.Mvc
{
    internal class ViciTemplateConfig : TemplateParserConfig<ViciTemplateTokenizer>
    {
        protected override string OnEvalExpression(ExpressionParser parser, TemplateToken token, IParserContext context)
        {
            TemplateParserContext parserContext = (TemplateParserContext)context;

            switch (token.TokenId)
            {
                case TokenID.Control: return EvalControlExpression(parser, token, parserContext);
                case TokenID.Component: return EvalComponentExpression(parser, token, parserContext);
                case TokenID.Translation: return EvalTranslationExpression(parser, token, parserContext);
                default: return EvalExpression(parser, token, parserContext);
            }
        }

        protected override string OnEvalIncludeFile(ExpressionParser parser, string fileName, TemplateToken token, IParserContext context)
        {
            return ((TemplateParserContext)context).Template.ReadSubTemplate(fileName);
        }

        protected override CompiledTemplate OnEvalParseFile(ExpressionParser parser, TemplateParser templateParser, string fileName, TemplateToken token, IParserContext context, out Dictionary<string, IValueWithType> parameters)
        {
            TemplateParserContext parserContext = (TemplateParserContext) context;

            TemplateToken.ParameterizedExpression pExpr = token.ExtractParameters();

            parameters = new Dictionary<string, IValueWithType>(StringComparer.InvariantCultureIgnoreCase);

            foreach (KeyValuePair<string, string> var in pExpr.Parameters)
            {
                parameters[var.Key] = parser.Evaluate(var.Value, context, token.TokenPosition);
            }

            ICompiledTemplate subTemplate = parserContext.Template.CompileSubTemplate(parser.Evaluate<string>(pExpr.MainExpression, context, token.TokenPosition));

            //TODO : fix this. It should be possible to include templates with different syntax
            return ((ViciCompiledTemplate)subTemplate).CompiledTemplate;
        }

        private static string EvalControlExpression(ExpressionParser parser, TemplateToken token, TemplateParserContext context)
        {
            int colon1 = token.Text.LastIndexOf(':');
            int colon2 = -1;

            if (colon1 > 0)
                colon2 = token.Text.LastIndexOf(':', colon1-1);

            string expr = token.Text;
            string className = null;
            string classError = null;

            if (colon2 >= 0)
            {
                classError = expr.Substring(colon1 + 1).Trim();
                className = expr.Substring(colon2 + 1, colon1 - colon2 - 1);
                expr = expr.Substring(0, colon2).Trim();
            }
            else if (colon1 >= 0)
            {
                className = expr.Substring(colon1 + 1).Trim();
                expr = expr.Substring(0, colon1).Trim();
            }

            Control control = parser.Evaluate<Control>(expr, context, token.TokenPosition);

            if (control != null)
            {
                return control.Render(context.View, className, classError);
                //return context.View.ParseTranslations(html);
            }
            else
            {
                return "[?[" + expr + "]?]";
            }
        }

        private static string EvalTranslationExpression(ExpressionParser parser, TemplateToken token, TemplateParserContext context)
        {
            int parenIdx = token.Text.IndexOf('(');

            string tag = token.Text;
            string parameters = null;

            if (parenIdx > 0)
            {
                parameters = token.Text.Substring(parenIdx + 1);
                tag = token.Text.Substring(0, parenIdx).Trim();
            }

            string translation = TranslationHelper.GetTranslation(context.View.ViewName, tag);

            if (translation == null)
                return "{?{" + tag + "}?}";

            if (parameters != null)
            {
                IParserContext localContext = context.CreateLocal();

                localContext.Set("__Translation__", translation);

                translation = parser.Evaluate<string>("string.Format(__Translation__," + parameters,localContext, token.TokenPosition);
            }

            return translation;
        }

        private static string EvalComponentExpression(ExpressionParser parser, TemplateToken token, TemplateParserContext context)
        {
            TemplateToken.ParameterizedExpression pExpr = token.ExtractParameters();

            string componentName = pExpr.MainExpression;

            if (pExpr.MainExpression.StartsWith("(") && pExpr.MainExpression.EndsWith(")"))
            {
                componentName = parser.Evaluate<string>(pExpr.MainExpression, context, token.TokenPosition);
            }

            TemplateParserContext newContext = (TemplateParserContext) context.CreateLocal();

            foreach (string varName in pExpr.Parameters.Keys)
                newContext.SetLocal(varName, parser.Evaluate(pExpr.Parameters[varName], context, token.TokenPosition));

            ControllerAction viewComponent = WebAppHelper.GetViewComponent(componentName);

            if (viewComponent == null)
                throw new TemplateRenderingException("View component " + componentName + " not found", token.TokenPosition);

            try
            {
                return WebAppHelper.RunViewComponent(viewComponent, newContext);
            }
            catch (Exception e)
            {
                throw new TemplateRenderingException("Error rendering view component " + componentName, e, token.TokenPosition);
            }
        }


        private static string EvalExpression(ExpressionParser parser, TemplateToken token, TemplateParserContext context)
        {
            char q = token.TokenId == TokenID.ExpressionNew ? '`' : ':';

            int quoteIdx = token.Text.LastIndexOf(q);

            string expr = quoteIdx >= 0 ? token.Text.Substring(0, quoteIdx).Trim() : token.Text;

            IValueWithType result = parser.Evaluate(expr, context, token.TokenPosition);

            if (token.TokenType == TemplateTokenType.Statement)
                return "";

            if (result.Value is Control)
                return context.View.ParseTranslations(((Control)result.Value).Render(context.View));

            string fmt = quoteIdx >= 0 ? token.Text.Substring(quoteIdx + 1).Trim() : null;

            if (fmt != null)
                return String.Format(WebAppConfig.FormatProvider, "{0:" + fmt + "}", result.Value);
            else if (result.Value is string)
                return (string) result.Value;
            else
                return result.Value == null ? "" : Convert.ToString(result.Value,WebAppConfig.FormatProvider);
        }


    }
}