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
using System.IO;
using System.Reflection;
using Vici.Core;

namespace Vici.Mvc
{
    internal static class WebAppHelper
    {
        internal static ActionResult RunControllerAction(ControllerAction controllerAction)
        {
            WebAppContext.AddControllerParameters(controllerAction.Parameters);

            ControllerClass controllerClass = controllerAction.ControllerClass;

            if (!controllerClass.IsActionMethod(controllerAction.ActionMethod))
                return null;

            using (Controller controller = controllerClass.CreateController()) // Dispose() will call [AfterAction] methods
            {
                try
                {
                    WebAppContext.RootView = controller.View;

                    controllerClass.SetupController(controller, null);

                    object returnValue = controllerClass.Run(controller, controllerAction.ActionMethod, null);

                    if (returnValue is ActionResult)
                    {
                        ActionResult actionResult = (ActionResult) returnValue;

                        if (actionResult.Final)
                            controller.SkipTearDown = true;

                        return actionResult;
                    }

                    return new RenderViewActionResult(controller.View);
                }
                catch
                {
                    controller.SkipTearDown = true;

                    throw;
                }
            }
        }

        internal static string RunViewComponent(ControllerAction controllerAction, TemplateParserContext context)
        {
            ControllerClass controllerClass = controllerAction.ControllerClass;

            if (!controllerClass.IsActionMethod(controllerAction.ActionMethod))
                return null;

            using (ViewComponent component = controllerClass.CreateViewComponent(context))
            {
                try
                {
                    controllerClass.SetupController(component, context);

                    object returnValue = controllerClass.Run(component, "Run", context);

                    if (returnValue is string)
                        return (string) returnValue;

                    return TemplateUtil.ExtractBody(component.View.Render());
                }
                catch
                {
                    component.SkipTearDown = true;

                    throw;
                }
            }
        }


        internal static Template GetTemplate(string templateName, bool isLayout)
        {
            if (string.IsNullOrEmpty(templateName))
                return null;

            string path = WebAppContext.Request.Path;

            string physicalPagePath = WebAppContext.Server.MapPath(path);

            string templateFileName = null;

            if (WebAppConfig.TemplateResolver != null)
            {
                templateFileName = WebAppConfig.TemplateResolver.ResolveTemplate(templateName, isLayout);
            }

            if (templateFileName == null)
            {
                if (Path.IsPathRooted(WebAppConfig.TemplatePath))
                    templateFileName = Path.GetFullPath(Path.Combine(WebAppConfig.TemplatePath, templateName));
                else
                    templateFileName = Path.GetFullPath(Path.Combine(Path.Combine(WebAppContext.Request.PhysicalApplicationPath, WebAppConfig.TemplatePath.Replace('/', '\\')), templateName));
            }

            foreach (string ext in WebAppConfig.ViewEngines.Extensions)
            {
                if (File.Exists(templateFileName + ext))
                    return Template.CreateTemplate(templateFileName + ext, physicalPagePath, false);
            }

            throw new TemplateNotFoundException(templateFileName);
        }

        internal static object GetClientValue(ClientDataAttribute attribute, Type type)
        {
            if (attribute.UseGet && WebAppContext.Parameters.Has(attribute.Name))
                return WebAppContext.Parameters.Get(attribute.Name,type);
            else if (attribute.UsePost && WebAppContext.FormData.Has(attribute.Name))
                return WebAppContext.FormData.Get(attribute.Name,type);

            return null;
        }

        internal static SessionBase CreateSessionObject()
        {
            return (SessionBase) Activator.CreateInstance(WebAppConfig.SessionType);
        }

        internal static object[] CreateParameters(MethodInfo method)
        {
            return CreateParameters(method, null);
        }

        internal static object[] CreateParameters(MethodInfo method, TemplateParserContext context)
        {
            ParameterInfo[] parameters = method.GetParameters();

            object[] parameterValues = new object[parameters.Length];

            int i = 0;
            foreach (ParameterInfo parameter in parameters)
            {
                ClientDataAttribute[] attributes = (ClientDataAttribute[])parameter.GetCustomAttributes(typeof(ClientDataAttribute), true);

                ClientDataAttribute mapAttribute;

                if (attributes.Length > 0)
                    mapAttribute = attributes[0];
                else
                    mapAttribute = new GetOrPostAttribute(parameter.Name);

                object value;
                Type type;

                if (context != null && context.Get(parameter.Name, out value, out type))
                    parameterValues[i++] = value.Convert(parameter.ParameterType);
                else
                    parameterValues[i++] = GetClientValue(mapAttribute, parameter.ParameterType); //TODO: add IObjectBinder support for complex objects
            }

            return parameterValues;
        }

        internal static string RunAjaxMethod(MethodInfo ajaxMethod, object obj, bool useXml)
        {
            WebAppContext.AjaxContext = new AjaxContext();
            WebAppContext.AjaxContext.ViewName = WebAppContext.FormData["_VIEWNAME_"];

            try
            {
                object result = ajaxMethod.Invoke(ajaxMethod.IsStatic ? null : obj, CreateParameters(ajaxMethod));

                return useXml ? (string) result : AjaxHelper.GenerateJSONReturnValue(result);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                    ex = ExceptionHelper.ResolveTargetInvocationException((TargetInvocationException) ex);

                return useXml ? AjaxHelper.GenerateXmlError(ex.Message) : AjaxHelper.GenerateJSONError(ex.Message);
            }
        }

        internal static ControllerAction GetViewComponent(string name)
        {
            ControllerClass controllerClass = WebAppConfig.GetViewComponentClass(name);

            if (controllerClass == null)
                return null;

            return new ControllerAction(controllerClass);
        }

        internal static ControllerAction GetControllerAction(string url)
        {
            RouteResult routeResult = WebAppConfig.Router.Resolve(url);

            if (routeResult == null)
                return null;

            ControllerClass controllerClass = routeResult.CreateControllerClass();

            if (controllerClass != null)
            {
                ControllerAction controllerAction = new ControllerAction(controllerClass, routeResult.Action);

                foreach (KeyValuePair<string, string> param in routeResult.Parameters)
                    controllerAction.Parameters.Add(param.Key, param.Value);

                return controllerAction;
            }

            return null;
        }
    }
}
