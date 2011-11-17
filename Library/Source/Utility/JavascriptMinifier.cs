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

// Based on the original jsmin.c minifier by Douglas Crockford:

/* jsmin.c
   2007-05-22

Copyright (c) 2002 Douglas Crockford  (www.crockford.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

The Software shall be used for Good, not Evil.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion

using System;
using System.Text;

namespace Vici.Mvc
{
    public class JavaScriptMinifier
    {
        private const char EOF = Char.MaxValue;

        private char _charA;
        private char _charB;
        private char _lookAhead = EOF;

        private readonly StringBuilder _output = new StringBuilder();
        private readonly string _input;
        private int _inputIdx;

        private JavaScriptMinifier(string input)
        {
            _input = input;
        }

        private string Output
        {
            get { return _output.ToString(); }
        }

        public static string Minify(string input)
        {
            var minifier = new JavaScriptMinifier(input);

            minifier.Minify();

            return minifier.Output;
        }

       private void Minify()
        {
            _charA = '\n';

            Action(JsMinAction.GetNextB);

            while (_charA != EOF)
            {
                switch (_charA)
                {
                    case ' ':
                        {
                            if (IsAlphanum(_charB))
                                Action(JsMinAction.OutputA);
                            else
                                Action(JsMinAction.CopyBtoA);

                            break;
                        }
                    case '\n':
                        {
                            switch (_charB)
                            {
                                case '{':
                                case '[':
                                case '(':
                                case '+':
                                case '-':
                                    {
                                        Action(JsMinAction.OutputA);
                                        break;
                                    }
                                case ' ':
                                    {
                                        Action(JsMinAction.GetNextB);
                                        break;
                                    }
                                default:
                                    {
                                        if (IsAlphanum(_charB))
                                        {
                                            Action(JsMinAction.OutputA);
                                        }
                                        else
                                        {
                                            Action(JsMinAction.CopyBtoA);
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            switch (_charB)
                            {
                                case ' ':
                                    {
                                        if (IsAlphanum(_charA))
                                        {
                                            Action(JsMinAction.OutputA);
                                            break;
                                        }

                                        Action(JsMinAction.GetNextB);
                                        
                                        break;
                                    }
                                case '\n':
                                    {
                                        switch (_charA)
                                        {
                                            case '}':
                                            case ']':
                                            case ')':
                                            case '+':
                                            case '-':
                                            case '"':
                                            case '\'':
                                                {
                                                    Action(JsMinAction.OutputA);
                                                    break;
                                                }
                                            default:
                                                {
                                                    if (IsAlphanum(_charA))
                                                        Action(JsMinAction.OutputA);
                                                    else
                                                        Action(JsMinAction.GetNextB);

                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        Action(JsMinAction.OutputA);

                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }

        public enum JsMinAction
        {
            OutputA = 1,
            CopyBtoA = 2,
            GetNextB = 3
        }

        /* action -- do something! What you do is determined by the argument:
                1   Output A. Copy B to A. Get the next B.
                2   Copy B to A. Get the next B. (Delete A).
                3   Get the next B. (Delete B).
           action treats a string as a single character. Wow!
           action recognizes a regular expression if it is preceded by ( or , or =.
        */

        private void Action(JsMinAction d)
        {
            if (d == JsMinAction.OutputA)
            {
                WriteChar(_charA);
            }

            if (d ==JsMinAction.OutputA || d == JsMinAction.CopyBtoA)
            {
                _charA = _charB;

                if (_charA == '\'' || _charA == '"')
                {
                    for (;;)
                    {
                        WriteChar(_charA);

                        _charA = Get();

                        if (_charA == _charB)
                            break;

                        if (_charA <= '\n' || _charA == EOF)
                            throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", _charA));

                        if (_charA == '\\')
                        {
                            WriteChar(_charA);

                            _charA = Get();
                        }
                    }
                }
            }
                _charB = Next();

                if (_charB == '/' && (_charA == '(' || _charA == ',' || _charA == '=' ||
                                      _charA == '[' || _charA == '!' || _charA == ':' ||
                                      _charA == '&' || _charA == '|' || _charA == '?' ||
                                      _charA == '{' || _charA == '}' || _charA == ';' ||
                                      _charA == '\n'))
                {
                    WriteChar(_charA);
                    WriteChar(_charB);

                    for (;;)
                    {
                        _charA = Get();

                        if (_charA == '/')
                            break;
                        
                        if (_charA == '\\')
                        {
                            WriteChar(_charA);

                            _charA = Get();
                        }
                        else if (_charA <= '\n' || _charA == EOF)
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.\n", _charA));
                        }

                        WriteChar(_charA);
                    }

                    _charB = Next();
                }

        }

        /* next -- get the next character, excluding comments. peek() is used to see
                if a '/' is followed by a '/' or '*'.
        */

        private char Next()
        {
            char c = Get();

            if (c == '/')
            {
                switch (Peek())
                {
                    case '/':
                        {
                            for (;;)
                            {
                                c = Get();

                                if (c <= '\n' || c == EOF)
                                    return c;
                            }
                        }
                    case '*':
                        {
                            Get();

                            for (;;)
                            {
                                switch (Get())
                                {
                                    case '*':
                                        {
                                            if (Peek() == '/')
                                            {
                                                Get();

                                                return ' ';
                                            }

                                            break;
                                        }
                                    case EOF:
                                        {
                                            throw new Exception("Error: JSMIN Unterminated comment.\n");
                                        }
                                }
                            }
                        }
                    default:
                        {
                            return c;
                        }
                }
            }

            return c;
        }

        /* peek -- get the next character without getting it.
        */

        private char Peek()
        {
            _lookAhead = Get();

            return _lookAhead;
        }

        /* get -- return the next character from stdin. Watch out for lookahead. If
                the character is a control character, translate it to a space or
                linefeed.
        */

        private char Get()
        {
            char c = _lookAhead;

            _lookAhead = EOF;

            if (c == EOF)
                c = ReadChar();

            if (c >= ' ' || c == '\n' || c == EOF)
                return c;

            if (c == '\r')
                return '\n';

            return ' ';
        }

        private char ReadChar()
        {
            return _inputIdx >= _input.Length ? EOF : _input[_inputIdx++];
        }

        private void WriteChar(char c)
        {
            _output.Append(c);
        }

        private static bool IsAlphanum(char c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
                    c > 126);
        }
    }
}
