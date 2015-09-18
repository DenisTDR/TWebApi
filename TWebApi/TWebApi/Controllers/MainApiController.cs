using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using customApiApp_3.Responses;

namespace customApiApp_3.Controllers
{
    public class MainApiController : Controller
    {
        public string GetByIndex()
        {
            return ProcessRequest(RequestType.GetById);
        }

        public string NormalApiCall()
        {
            return ProcessRequest(RequestType.NormalApiCall);
        }

        public string ProcessRequest(RequestType requestType)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Response.AddHeader("Content-Type", "application/json");
            var responseDictionary = new Dictionary<string, object>();

            try
            {
                var response = requestType == RequestType.NormalApiCall
                    ? ProcessNormalApiRequest(Request)
                    : ProcessGetByIndex(Request);
                if (response is Response)
                {
                    responseDictionary.Add("StatusCode", ((Response) response).StatusCode);
                    responseDictionary.Add("ErrorMessage", ((Response) response).CustomMessage);
                }
                else
                {
                    if (response is IEnumerable<object>)
                    {
                        if (response is IQueryable<object>)
                        {
                            try
                            {
                                response = (response as IQueryable<object>).ToArray();
                                responseDictionary.Add("StatusCode", 200);
                                responseDictionary["Count"] = (response as IEnumerable<object>).Count();
                                responseDictionary.Add("Data", response);
                            }
                            catch (System.Data.Entity.Core.ProviderIncompatibleException exc)
                            {
                                responseDictionary.Add("StatusCode", 500);
                                responseDictionary.Add("ErrorMessage", "Internal server error! Can't connect to Database!");
                                responseDictionary.Add("WhatToDo", "Blame the Database for this...");
                            }
                        }
                        else
                        {
                            responseDictionary.Add("StatusCode", 200);
                            responseDictionary["Count"] = (response as IEnumerable<object>).Count();
                            responseDictionary.Add("Data", response);
                        }
                    }
                    else
                    {
                        responseDictionary.Add("StatusCode", 200);
                        responseDictionary.Add("Data", response);
                    }
                }
            }
            catch (Exception exc)
            {
                responseDictionary.Add("StatusCode", 500);
                responseDictionary.Add("ErrorMessage", "Internal server error!");
                responseDictionary.Add("WhatToDo", "Blame Denis for this...");
                responseDictionary.Add("Exception", exc.ToString());
            }
            stopwatch.Stop();
            responseDictionary.Add("Request process duration", stopwatch.ElapsedMilliseconds);
            return Utilis.JsonSerializer(responseDictionary);
        }

        private static object ProcessGetByIndex(System.Web.HttpRequestBase request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (request.Url == null)
                throw new ArgumentNullException("request.Url");
            string uri = request.Url.LocalPath;
            while (uri.EndsWith("/")) uri = uri.Substring(0, uri.Length - 1);
            uri = uri.Split('/').Last();
            uri = uri.Substring(0, uri.Length - 1);
            string id = uri.Split('(').Skip(1).First();

            var controllerName = uri.Split('(').First();
            var controllerType = Utilis.GetTypeByNameAndAncestor<ITApiController>(controllerName + "Controller");
            if (controllerType == null)
            {
                controllerType = Utilis.GetTypeByNameAndAncestor<ITApiController>(controllerName + "Controller");
                if (controllerType == null)
                    return new NotFound("No controller with name '" + controllerName + "' or '" + controllerName + "Controller' found!");
            }
            controllerName = controllerType.Name;
            var methods =
                controllerType.GetMethodsByName("Get")
                    .Where(
                        method =>
                            method.GetCustomAttributes()
                                .Any(
                                    attribute =>
                                        attribute is IHttpVerbAttribute &&
                                        ((IHttpVerbAttribute) attribute).HttpVerb ==
                                        Utilis.ParseEnum<HttpVerbs>(request.HttpMethod)) &&
                            method.GetParameters().Count() == 1)
                    .ToArray();
            if (methods.Length == 0)
            {
                return
                    new NotFound(controllerType.Name + " doesn't have Get method allowing '" + request.HttpMethod +
                                 "' with one parameter.");
            }
            foreach (var method in methods.Reverse())
            {
                if (method.GetParameters().First().ParameterType == typeof(int))
                {
                    int intVal;
                    if (int.TryParse(id, out intVal))
                    {
                        return method.Invoke(controllerType.MakeInstance(), new object[] {intVal});
                    }
                }
                else
                {
                    return method.Invoke(controllerType.MakeInstance(), new object[] {id});
                }
            }

            return
                new NotFound(controllerType.Name + " doesn't have Get method allowing '" + request.HttpMethod +
                             "' with one parameter of type [" + Utilis.GetPossibleTypesFromStringList(new[] {id}) + "]");
        }
        private static object ProcessNormalApiRequest(System.Web.HttpRequestBase request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (request.Url == null)
                throw new ArgumentNullException("request.Url");
            var requestVerb = Utilis.ParseEnum<HttpVerbs>(request.HttpMethod);
            var urlParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                .AddDictionary(
                    request.Url.LocalPath.Split('/')
                        .Where(x => x.Length != 0 && x != "api")
                        .Select((paramValue, index) => new {Key = "paramNo" + index, Value = paramValue})
                        .ToDictionary(t => t.Key, t => t.Value))
                .AddDictionary(request.QueryString.ToDictionary());

            if (urlParams.Count == 0)
                return new BadRequest("Api controller type not found!");

            var controllerName = urlParams.ValueAtIndex(0);
            var controllerType = Utilis.GetTypeByNameAndAncestor<ITApiController>(controllerName + "Controller");
            if (controllerType == null)
            {
                controllerType = Utilis.GetTypeByNameAndAncestor<ITApiController>(controllerName + "Controller");
                if (controllerType == null)
                    return new NotFound("No controller with name '"+ controllerName+"' or '" + controllerName + "Controller' found!");
            }
            if (urlParams.Count == 1) // url like "ControllerName/",  so, call the default Get method
            {
                var getMethod =
                    controllerType.GetMethodsByName("Get")
                        .FirstOrDefault(
                            method =>
                                method.GetCustomAttributes()
                                    .Any(
                                        attribute =>
                                            attribute is IHttpVerbAttribute &&
                                            ((IHttpVerbAttribute) attribute).HttpVerb == requestVerb));
                if (getMethod == null)
                {
                    return
                        new NotFound(controllerType.Name + " doesn't have Get method allowing '" + request.HttpMethod +
                                     "' with no parameters.");
                }
                return getMethod.Invoke(controllerType.MakeInstance(), null);
            }
            if (urlParams.Count == 2)
                // url like "ControllerName/something",  so, search the Get(something) method
            {
                var getMethodAndMatchedParamsValues = controllerType.GetFirstMethodWhichMatchParameters("Get",
                    requestVerb, new Dictionary<string, string> {{"Id", urlParams.ValueAtIndex(1)}},
                    request.Form.ToDictionary(), 1);
                if (getMethodAndMatchedParamsValues != null)
                    return getMethodAndMatchedParamsValues.Item1.Invoke(controllerType.MakeInstance(),
                        getMethodAndMatchedParamsValues.Item2);
                /*return
                    new NotFound(controllerType.Name + " doesn't have Get method allowing '" + request.HttpMethod +
                                 "' with parameters that match your request.");
                                 */
            }
            string methodName = urlParams.ValueAtIndex(1);

            var methodAndMatchedParamsValues = controllerType.GetFirstMethodWhichMatchParameters(methodName,
               requestVerb, urlParams.SkipD(2), request.Form.ToDictionary());
            if (methodAndMatchedParamsValues == null)
            {
                return
                    new NotFound("No method with name '" + methodName + "' allowing '" + request.HttpMethod +
                                 "' with " + (urlParams.Count - 2) + " parameters of type [" +
                                 string.Join(", ", Utilis.GetPossibleTypesFromStringList(urlParams.SkipD(2).Values))
                                 + "] found in " + controllerType.Name);
            }
            return methodAndMatchedParamsValues.Item1.Invoke(controllerType.MakeInstance(),
                methodAndMatchedParamsValues.Item2);
        }
    }

    public enum RequestType
    {
        NormalApiCall,
        GetById
    }
}
