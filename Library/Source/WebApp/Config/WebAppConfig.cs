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
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Vici.Core;
using Vici.Core.Json;
using Vici.Core.Parser;

namespace Vici.Mvc
{
    public static class WebAppConfig
    {
        private static Type                          _sessionType = typeof(SessionBase);

        private static ITranslationProvider          _translationProvider;
        private static ISessionLoggingProvider       _sessionLoggingProvider = new MinimalSessionLoggingProvider();
        private static IVisitorProvider              _visitorProvider = new MinimalVisitorProvider();
        private static IPageLoggingProvider          _loggingProvider;
        private static IAjaxProvider                 _ajaxProvider = new JQueryAjaxProvider();
        private static ITemplateResolver             _templateResolver;
        private static ISessionSerializer            _sessionSerializer;
        private static IFormatProvider               _formatProvider = CultureInfo.InvariantCulture;
        private static JsonDateFormat                _jsonDateFormat = JsonDateFormat.UtcISO;

        private static Converter<string, string>     _mapPathHandler;

        private static Dictionary<string, ControllerClass> _controllerTypesByName;
        private static Dictionary<string, ControllerClass> _viewComponentsByName;
        private static Dictionary<Type, ControllerClass> _controllerTypesByType;
        private static readonly ViewEngineCollection _viewEngines = new ViewEngineCollection();

        private static readonly List<Assembly>                 _registeredAssemblies = new List<Assembly>();

        private static string _templatePath          = "templates";
        private static string _defaultLayout         = "master";
        private static string _defaultLanguage       = "en";
        private static bool   _useLanguagePath;

        private static string _version;

        private static ObjectBinder _objectBinder = new ObjectBinder();

        public static event Action<Exception> ExceptionOccurred;
        public static event Action<string> TemplateNotFound;
        public static event Action<SessionBase> SessionCreated;

        private static readonly Router _router = new Router();

        private static readonly object _initLock = new object();
        private static bool _initialized;

        private class ObjectBinder : IStringConverter
        {
            private static readonly List<IObjectBinder> _customObjectBinders = new List<IObjectBinder>();

            public void Register(IObjectBinder objectBinder)
            {
                _customObjectBinders.Add(objectBinder);
            }

            public bool TryConvert(string s, Type targetType, out object value)
            {
                foreach (var objectBinder in _customObjectBinders)
                {
                    if (objectBinder.TryConvert(s,targetType,out value))
                        return true;
                }

                value = null;

                return false;
            }
        }


        public static void Init()
        {
            lock (_initLock)
            {
                if (_initialized)
                    return;

                _initialized = true;

                Regex.CacheSize = 100;

                _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                string applicationClassName = null;

                foreach (string configKey in ConfigurationManager.AppSettings.Keys)
                {
                    string configValue = ConfigurationManager.AppSettings[configKey];

                    switch (configKey.ToLower())
                    {
                        case "promesh.defaultlayout":
                        case "mvc.defaultlayout":
                            DefaultLayout = configValue;
                            break;
                        case "promesh.templatepath":
                        case "mvc.templatepath":
                            TemplatePath = configValue;
                            break;
                        case "promesh.uselanguagepath":
                        case "mvc.uselanguagepath":
                            UseLanguagePath = (configValue.ToLower() == "true");
                            break;
                        case "promesh.defaultlanguage":
                        case "mvc.defaultlanguage":
                            DefaultLanguage = configValue;
                            break;
                        case "promesh.applicationclass":
                        case "mvc.applicationclass":
                            applicationClassName = configValue;
                            break;
                    }

                    if (configKey.ToLower().StartsWith("mvc.viewengine"))
                    {
                        IViewEngine viewEngine = (IViewEngine) Activator.CreateInstance(Type.GetType(configValue));

                        string ext = configKey.Length > 15 ? configKey.Substring(14) : null;

                        ViewEngines.Add(viewEngine, ext);
                    }
                }

                if (applicationClassName == null)
                    throw new Exception("No Mvc.ApplicationClass defined in web.config");

                Type appType = Type.GetType(applicationClassName, false);

                if (appType == null)
                    throw new Exception("Application class {" + applicationClassName + "} could not be loaded");

                MethodInfo initMethod = appType.GetMethod("Init", new Type[0]);

                if (initMethod == null || !initMethod.IsStatic)
                    throw new Exception("No \"public static void Init()\" method defined for class " + appType.Name);

                RegisterAssembly(appType.Assembly);
                RegisterAssembly(Assembly.GetExecutingAssembly());

                initMethod.Invoke(null, null);

                ViewEngines.Add(new ViciViewEngine(),"htm");

                LoadControllerClasses();

                StringConverter.RegisterStringConverter(_objectBinder);
            }
        }

        public static void Init(Converter<string,string> mapPathHandler)
        {
            _mapPathHandler = mapPathHandler;

            Init();
        }

        public static string MapPath(string url)
        {
            if (_mapPathHandler != null)
                return _mapPathHandler(url);
            else
                return HttpContext.Current.Server.MapPath(url);
        }

        public static Type SessionType
        {
            get
            {
                return _sessionType;
            }
            set
            {
                if (!typeof(SessionBase).IsAssignableFrom(value))
                    throw new Exception("SessionType should be derived from SessionBase");

                _sessionType = value;
            }
        }

        public static ITranslationProvider TranslationProvider
        {
            get { return _translationProvider; }
            set { _translationProvider = value; }
        }

        public static ISessionLoggingProvider SessionLoggingProvider
        {
            get { return _sessionLoggingProvider; }
            set { _sessionLoggingProvider = value; }
        }

        public static IPageLoggingProvider LoggingProvider
        {
            get { return _loggingProvider; }
            set { _loggingProvider = value; }
        }

        public static IVisitorProvider VisitorProvider
        {
            get { return _visitorProvider; }
            set { _visitorProvider = value; }
        }

        public static IAjaxProvider AjaxProvider
        {
            get { return _ajaxProvider; }
            set { _ajaxProvider = value; }
        }

        public static ITemplateResolver TemplateResolver
        {
            get { return _templateResolver; }
            set { _templateResolver = value; }
        }

        public static ISessionSerializer SessionSerializer
        {
            get { return _sessionSerializer; }
            set { _sessionSerializer = value; }
        }

        public static string TemplatePath
        {
            get { return _templatePath; }
            set { _templatePath = value; }
        }

        public static string DefaultLayout
        {
            get { return _defaultLayout; }
            set { _defaultLayout = value; }
        }

        public static string DefaultLanguage
        {
            get { return _defaultLanguage; }
            set { _defaultLanguage = value.ToLower(); }
        }

        public static bool UseLanguagePath
        {
            get { return _useLanguagePath; }
            set { _useLanguagePath = value; }
        }

        public static string Version
        {
            get { return _version; }
        }

        public static Router Router
        {
            get { return _router; }
        }

        public static IFormatProvider FormatProvider
        {
            get { return _formatProvider; }
            set { _formatProvider = value; }
        }

        public static void RegisterAssembly(Assembly assembly)
        {
            // We don't need to load anything, because if the assembly object is here, the assembly is automatically loaded

            if (_registeredAssemblies.Find(a => a.FullName == assembly.FullName) == null)
            {
                _registeredAssemblies.Add(assembly);
            }
        }

        public static void RegisterAssembly(string assemblyPath)
        {
            RegisterAssembly(Assembly.LoadFrom(assemblyPath));
        }

        internal static void LoadControllerClasses() // only used in unit testing
        {
            _controllerTypesByName = new Dictionary<string, ControllerClass>(StringComparer.OrdinalIgnoreCase);
            _viewComponentsByName = new Dictionary<string, ControllerClass>(StringComparer.OrdinalIgnoreCase);
            _controllerTypesByType = new Dictionary<Type, ControllerClass>();

            List<Type> controllerTypes = new List<Type>();

            foreach (Assembly assembly in _registeredAssemblies)
            {
                controllerTypes.AddRange(TypeHelper.FindCompatibleTypes(assembly, typeof (Controller)));
            }

            foreach (Type type in controllerTypes)
            {
                ControllerClass controllerClass = new ControllerClass(type);

                if (controllerClass.IsViewComponent)
                {
                    _viewComponentsByName.Add(controllerClass.Name, controllerClass);
                }
                else
                {
                    _controllerTypesByName.Add(controllerClass.Name, controllerClass);
                    _controllerTypesByType.Add(type, controllerClass);

                    foreach (Route route in controllerClass.Routes)
                        Router.InsertRoute(route);
                }
            }
        }

        internal static ControllerClass GetControllerClass(string controllerName)
        {
            ControllerClass controllerClass;

            if (_controllerTypesByName.TryGetValue(controllerName, out controllerClass))
                return controllerClass;

            return null;
        }

        internal static ControllerClass GetControllerClass(Type controllerType)
        {
            ControllerClass controllerClass;

            if (_controllerTypesByType.TryGetValue(controllerType, out controllerClass))
                return controllerClass;

            return null;
        }

        internal static ControllerClass GetViewComponentClass(string viewComponentName)
        {
            ControllerClass controllerClass;

            if (_viewComponentsByName.TryGetValue(viewComponentName, out controllerClass))
                return controllerClass;

            return null;
        }

        [Obsolete("Use StringConverter.RegisterStringConverter()")]
        public static void RegisterCustomObjectCreator(IObjectBinder objectBinder)
        {
            _objectBinder.Register(objectBinder);
        }

        internal static void Fire_ExceptionHandler(Exception ex)
        {
            if (ExceptionOccurred != null)
            {
                ExceptionOccurred(ex);
            }
            else
            {
                IHttpResponse response = WebAppContext.Response;

                response.Write("<pre>");

                for (Exception currentException = ex; currentException != null; currentException = currentException.InnerException)
                {
                    if (currentException is IPositionedException)
                    {
                        TokenPosition position = ((IPositionedException)currentException).Position;

                        response.Write("<em>&nbsp;&nbsp;At line " + position.Line + ", column " + position.Column + "</em><br/>");
                    }

                    response.Write("<em><b>" + currentException.Message + "</b></em><br/>");

                    response.Write(currentException.StackTrace);
                    response.Write("<hr>");
                }

                response.Write("</pre>");
            }
        }

        internal static bool Fire_TemplateNotFound(string pageUrl)
        {
            if (TemplateNotFound != null)
            {
                TemplateNotFound(pageUrl);

                return true;
            }

            return false;
        }

        internal static bool Fire_SessionCreated(SessionBase session)
        {
            session.OnSessionCreated();

            if (SessionCreated != null)
            {
                SessionCreated(session);

                return true;
            }

            return false;
        }

        public static JsonDateFormat JsonDateFormat
        {
            get { return _jsonDateFormat;  } 
            set { _jsonDateFormat = value; }
        }

        public static ViewEngineCollection ViewEngines
        {
            get { return _viewEngines; }
        }
    }
}
