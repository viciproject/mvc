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
using System.Threading;

namespace Vici.Mvc
{
    public class WebForm : WebForm<object>
    {
        public WebForm()
        {
        }

        public WebForm(object dataObject) : base(dataObject)
        {
            
        }
    }

    public delegate bool FieldValidateHandler<T>(T dataObject, string fieldName, object fieldValue, out string validationErrorMessage);
    public delegate bool FieldValidateHandler(string fieldName,object fieldValue, out string validationErrorMessage);
    public delegate void WebFormPostHandler<T>(bool validated,T dataObject);

    public class WebForm<T> where T:class
    {
        public event Action<T> Fill;
        public event WebFormPostHandler<T> Post;
        public event Action<T> Binding;
        public event Action<T> Bound;
        public event Action<T> Initialized;
        public event Action<T> Initializing;
        public event Action<T> Validating;

        public event Predicate<T> Validate;

        public event FieldValidateHandler<T> FieldValidate;

        private bool _wasPosted;
        private bool _dataOk;
        private bool _bound;
        private readonly T _dataObject;

        private readonly ValidationResult _validationResult = new ValidationResult();

        private FormFieldCollection<T> _fieldList;

        private static readonly Dictionary<Type, AjaxMethod[]> _ajaxMethods = new Dictionary<Type, AjaxMethod[]>();
        private static readonly ReaderWriterLock _ajaxMethodsLock = new ReaderWriterLock();

        private void GetFields()
        {
            _fieldList = new FormFieldCollection<T>();

            MemberInfo[] classMembers = _dataObject.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MemberInfo memberInfo in classMembers)
            {
                FormFieldAttribute[] fieldAttributes = (FormFieldAttribute[])memberInfo.GetCustomAttributes(typeof(FormFieldAttribute), false);

                if (fieldAttributes.Length != 1)
                    continue;

                FormFieldAttribute fieldAttribute = fieldAttributes[0];

                if (fieldAttribute.Name == null)
                    fieldAttribute.Name = memberInfo.Name;

                fieldAttribute.FieldType = GetFieldType(memberInfo);
                fieldAttribute.FormData = _dataObject;

                if (!fieldAttribute.IsRightType())
                    throw new ViciMvcException("Property " + memberInfo.Name + " is not of the correct type");

                _fieldList.Add(memberInfo.Name, new FormField<T>(this,_dataObject, memberInfo, fieldAttribute));
            }
        }

        protected WebForm()
        {
            if (typeof(T).IsAssignableFrom(GetType()))
                _dataObject = (T) (object) this;

            GetFields();
        }

        public WebForm(T dataObject)
        {
            _dataObject = dataObject;

            GetFields();
        }

        [AjaxOk]
        public ValidationResult AjaxValidate()
        {
            foreach (FormField<T> formField in _fieldList)
            {
                formField.CreateControl();

                formField.SetControlFromProperty();

                formField.Control.HandlePostBack();
            }

            ValidateForm();

            foreach (ValidationErrorMessage error in ValidationResult.Messages)
            {
                if (WebAppContext.AjaxContext != null && WebAppContext.AjaxContext.ViewName != null)
                    error.Message = TranslationHelper.ParseTranslations(error.Message, WebAppContext.AjaxContext.ViewName);
            }

            return ValidationResult;
        }

        public void Bind(string formName)
        {
            BindInternal();

            ViewDataContainer viewData = WebAppContext.RootView.ViewData;

            if (!viewData.Contains("ValidationResult"))
                viewData["ValidationResult"] = new ViewDataContainer();

            ((ViewDataContainer)viewData["ValidationResult"])[formName] = new ViewDataContainer(ValidationResult);
        }

        public void Bind()
        {
            BindInternal();

            foreach (ValidationErrorMessage error in ValidationResult.Messages)
            {
                if (WebAppContext.RootView != null && WebAppContext.RootView.ViewName != null)
                    error.Message = TranslationHelper.ParseTranslations(error.Message, WebAppContext.RootView.ViewName);
            }

            ViewDataContainer viewData = WebAppContext.RootView.ViewData;

            viewData["ValidationErrors"] = ValidationResult.Messages; // for backwards compatibility

            if (viewData.Contains("ValidationResult"))
            {
                viewData["ValidationResult"] = new ViewDataContainer(ValidationResult, viewData["ValidationResult"]);
            }
            else
            {
                viewData["ValidationResult"] = new ViewDataContainer(ValidationResult);    
            }
        }

        private void BindInternal()
        {
            if (_bound)
                return;

            _bound = true;

            View view = WebAppContext.RootView;

            foreach (AjaxMethod ajaxMethod in AjaxMethods)
                ajaxMethod.Register(view);

            foreach (FormField<T> formField in _fieldList)
            {
                formField.CreateControl();

                formField.SetControlFromProperty();

                view.ViewData[formField.MemberInfo.Name] = formField.Control;

                formField.Control.HandlePostBack();
            }

            if (Initializing != null)
                Initializing(_dataObject);

            OnInit();

            if (Initialized != null)
                Initialized(_dataObject);

            if (WebAppContext.IsPost())
                HandlePost();
            else
                HandleFill();

            foreach (FormField<T> field in _fieldList)
                field.SetControlFromProperty();

            if (Binding != null)
                Binding(_dataObject);

            OnBind();

            if (Bound != null)
                Bound(_dataObject);
        }

        private void HandleFill()
        {
            if (Fill != null)
                Fill(_dataObject);

            OnFill();
        }

        private void HandlePost()
        {
            _wasPosted = true;

            // Set fields to correct values

            _dataOk = ValidateForm();

            if (Post != null)
                Post(_dataOk,_dataObject);

            OnPost(_dataOk);
        }

        private bool ValidateForm()
        {
            bool dataOk = true;

            foreach (FormField<T> field in _fieldList)
                field.SetPropertyFromControl();

            OnPreValidate();

            if (Validating != null)
                Validating(_dataObject);

            // Build validation context, which is used by the [ValidateExpression] attribute.
            Dictionary<string, object> validationContext = new Dictionary<string, object>(_fieldList.Count);

            foreach (FormField<T> field in _fieldList)
            {
                validationContext[field.Attribute.Name] = field.GetValue();
            }

            // Validate fields

            foreach (FormField<T> field in _fieldList)
            {
                string errMsg;

                bool valid = field.ValidateField(validationContext, out errMsg);

                if (valid)
                {
                    valid = OnValidateField(field.Attribute.Name, field.GetValue(), out errMsg);

                    if (valid && FieldValidate != null)
                        valid = FieldValidate(_dataObject, field.Attribute.Name, field.GetValue(), out errMsg);
                }

                if (!valid)
                {
                    ValidationResult.Add(field.Attribute.Name, errMsg);

                    field.Error = true;

                    dataOk = false;
                }
                
            }

            // Validate complete form

            if (dataOk)
                dataOk = OnValidate();

            if (dataOk && Validate != null)
                dataOk = Validate(_dataObject);

            return dataOk;
        }

        public bool Validated
        {
            get
            {
                if (!_bound)
                    throw new ViciMvcException("No Bind() called for " + GetType().Name);

                return _wasPosted && _dataOk;
            }
        }

        public FormFieldCollection<T> Fields
        {
            get { return _fieldList; }
        }

        public ValidationResult ValidationResult
        {
            get { return _validationResult; }
        }

        protected virtual void OnFill()
        {
        }

        protected virtual void OnPost(bool validated)
        {
        }

        protected virtual void OnBind()
        {
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnPreValidate()
        {
        }

        protected virtual bool OnValidate()
        {
            return true;
        }

        protected virtual bool OnValidateField(string fieldName, object fieldValue, out string errorMessage)
        {
            errorMessage = null;

            return true;
        }

        private static Type GetFieldType(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
                return ((FieldInfo)memberInfo).FieldType;

            if (memberInfo is PropertyInfo)
                return ((PropertyInfo)memberInfo).PropertyType;

            return null;
        }

        private AjaxMethod[] AjaxMethods
        {
            get
            {
                AjaxMethod[] returnValue;
                Type type = GetType();

                _ajaxMethodsLock.AcquireReaderLock(-1);

                try
                {
                    if (_ajaxMethods.TryGetValue(type, out returnValue))
                        return returnValue;
                }
                finally
                {
                    _ajaxMethodsLock.ReleaseReaderLock();
                }

                _ajaxMethodsLock.AcquireWriterLock(-1);

                try
                {
                    // double check because another thread could have changed the 
                    // cache between releasing and acquiring the lock

                    if (_ajaxMethods.TryGetValue(type, out returnValue))
                        return returnValue;

                    List<AjaxMethod> ajaxMethods = new List<AjaxMethod>();

                    if (type.IsDefined(typeof(AjaxValidatedAttribute),true))
                    {
                        AjaxValidatedAttribute[] attributes = (AjaxValidatedAttribute[])type.GetCustomAttributes(typeof(AjaxValidatedAttribute), true);

                        MethodInfo ajaxValidatedMethod = type.GetMethod("AjaxValidate", new Type[0]);

                        ajaxMethods.Add(new AjaxMethod(ajaxValidatedMethod, type, new AjaxAttribute(true,attributes[0].FunctionName)));
                    }

                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public|BindingFlags.Static|BindingFlags.Instance|BindingFlags.FlattenHierarchy))
                    {
                        AjaxAttribute[] ajaxAttributes = (AjaxAttribute[]) method.GetCustomAttributes(typeof(AjaxAttribute), true);

                        if (ajaxAttributes.Length > 0)
                        {
                            if (!method.IsStatic)
                                throw new ViciMvcException("Ajax methods on form objects should be static");

                            ajaxMethods.Add(new AjaxMethod(method, ajaxAttributes[0]));
                        }
                    }

                    returnValue = ajaxMethods.ToArray();

                    _ajaxMethods[type] = returnValue;

                    return returnValue;
                }
                finally
                {
                    _ajaxMethodsLock.ReleaseWriterLock();
                }
            }
        }

    }
}
