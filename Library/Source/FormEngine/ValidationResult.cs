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
    public class ValidationResult
    {
        private readonly Dictionary<string, ValidationErrorMessage> _byMessage = new Dictionary<string, ValidationErrorMessage>();
        private readonly Dictionary<string, ValidationErrorControl> _byControl = new Dictionary<string, ValidationErrorControl>();

        public ICollection<ValidationErrorMessage> Messages { get { return _byMessage.Values; } }
        public ICollection<ValidationErrorControl> Controls { get { return _byControl.Values; } }

        public IEnumerable<string> MessagesForControl(string controlName)
        {
            ValidationErrorControl validationError;

            if (_byControl.TryGetValue(controlName, out validationError))
                return validationError.Messages;
            else
                return new string[0];
        }

        public IEnumerable<string> ControlsForMessage(string message)
        {
            ValidationErrorMessage validationError;

            if (_byMessage.TryGetValue(message, out validationError))
                return validationError.ControlNames;
            else
                return new string[0];
        }

        public void Add(string name, string message)
        {
            ValidationErrorMessage validationErrorMessage;

            if (message != null)
            {

                if (_byMessage.TryGetValue(message, out validationErrorMessage))
                    validationErrorMessage.ControlNames.Add(name);
                else
                    _byMessage[message] = new ValidationErrorMessage(name, message);
            }

            if (name != null)
            {
                ValidationErrorControl validationErrorControl;

                if (_byControl.TryGetValue(name, out validationErrorControl))
                {
                    if (message != null)
                        validationErrorControl.Messages.Add(message);
                }
                else
                    _byControl[name] = new ValidationErrorControl(name, message);
            }
        }

        public bool Success
        {
            get { return _byControl.Count == 0 && _byMessage.Count == 0; }
        }

        public bool HasErrors(string controlName)
        {
            return _byControl.ContainsKey(controlName);
        }

        public bool HasMessages(string controlName)
        {
            return HasErrors(controlName) && _byControl[controlName].Messages.Count > 0;
        }
    }
}