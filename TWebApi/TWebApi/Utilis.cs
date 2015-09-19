using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using customApiApp_3.Controllers;
using log4net;
using Newtonsoft.Json;
using System.Runtime.Caching;

namespace customApiApp_3
{
    public static class Utilis
    {
        private static ILog Logger = log4net.LogManager.GetLogger(typeof(Utilis));
        private static MemoryCache _typeCache = new MemoryCache("typeCache");
        /// <summary>
        /// Gets a all Type instances matching the specified class name with just non-namespace qualified class name.
        /// </summary>
        /// <param name="className">Name of the class sought.</param>
        /// <param name="ancestorType">Has an ancestor with type.</param>
        /// <returns>Types that have the class name specified. They may not be in the same namespace.</returns>
        public static Type[] GetTypesByName(string className, Type ancestorType)
        {
            string cacheKey = className + "_" + ancestorType.FullName;
            List<Type> returnVal;
            if (_typeCache[cacheKey] != null)
            {
                return (Type[]) _typeCache[cacheKey];
            }
            returnVal = new List<Type>();
            var asss = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var assemblyTypes = a.GetTypes();
                    returnVal.AddRange(
                        assemblyTypes.Where(
                            t =>
                                ancestorType.IsAssignableFrom(t) &&
                                (string.Equals(t.Name, className, StringComparison.CurrentCultureIgnoreCase)) ||
                                t.GetCustomAttributes()
                                    .Any(x => x is TypeAliases && ((TypeAliases) x).Contains(className))));
                }
                catch
                {
                    Logger.Debug("Cant load types from assemblei: " + a.FullName);
                }
            }
            _typeCache.Add(cacheKey, returnVal.ToArray(), DateTimeOffset.Now.AddDays(2));
            return returnVal.ToArray();
        }

        /// <summary>
        /// Gets a Type instances matching the specified class name with just non-namespace qualified class name.
        /// </summary>
        /// <param name="className">Name of the class sought.</param>
        /// <returns>Type that have the class name specified. They may not be in the same namespace.</returns>
        /// <exception cref="System.Exception">Thrown when more than one Type was found</exception>


        public static Type GetTypeByNameAndAncestor(string className, Type ancestorType)
        {
            var types = GetTypesByName(className, ancestorType);
            if (types.Length > 1)
                throw new Exception("More than one type found!");
            return types.Length == 0 ? null : types[0];
        }

        public static Type GetTypeByNameAndAncestor<TAncestor>(string className)
        {
            var types = GetTypesByName(className, typeof (TAncestor));
            if (types.Length > 1)
                throw new Exception("More than one type found!");
            return types.Length == 0 ? null : types.First();
        }

        public static Type GetTypeByNameAndAncestorWithCustomAttribute<TAncestor>(string className, Type attribute)
        {
            var types =
                GetTypesByName(className, typeof (TAncestor))
                    .Where(x => x.GetCustomAttributes().Any(attribute.IsInstanceOfType)).ToArray();
            if (types.Length > 1)
                throw new Exception("More than one type found!");
            return types.Length == 0 ? null : types.First();
        }

        public static MethodInfo[] GetMethodsByName(this Type type, string methodName)
        {
            return
                type.GetMethods()
                    .Where(method => String.Equals(method.Name, methodName, StringComparison.CurrentCultureIgnoreCase))
                    .ToArray();
        }
        public static Tuple<MethodInfo, object[]> GetFirstMethodWhichMatchParameters(this Type controllerType, string methodName,
           HttpVerbs httpVerb, Dictionary<string ,string> urlParams , Dictionary<string, string> postParams, int minimumRequiredParametersCount=0)
        { 
            var methods =
                controllerType.GetMethodsByName(methodName)
                    .Where(
                        method =>
                            method.GetCustomAttributes()
                                .Any(
                                    attribute =>
                                        attribute is IHttpVerbAttribute &&
                                        ((IHttpVerbAttribute)attribute).HttpVerb == httpVerb))
                    .ToArray();

            foreach (var method in methods.Reverse())
            {
                if (method.GetParameters().Length < minimumRequiredParametersCount)
                    continue;
                var matchedParamsValues = new List<object>();
                switch (httpVerb)
                {
                    case HttpVerbs.Get:
                        if (!TryMatchParameters(method, urlParams, httpVerb, matchedParamsValues))
                        {
                            continue;
                        }
                        if (matchedParamsValues.Count == method.GetParameters().Length &&
                            (urlParams.Count == 0 || matchedParamsValues.Count > 0))
                        {
                            return new Tuple<MethodInfo, object[]>(method, matchedParamsValues.ToArray());
                        }
                        break;
                    case HttpVerbs.Post:
                        if (method.GetParameters().Length != 1)
                            throw new NotSupportedException(
                                "There are allowed only Post methods with one parameter.");
                        var parameterType = method.GetParameters().First().ParameterType;

                        var parameterObject = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(postParams),
                            parameterType,
                            new JsonSerializerSettings {Error = (sender, args) => { args.ErrorContext.Handled = true; }});

                        return new Tuple<MethodInfo, object[]>(method, new[] {parameterObject});
                    case HttpVerbs.Delete:
                        if (!TryMatchParameters(method, urlParams, httpVerb, matchedParamsValues))
                        {
                            continue;
                        }
                        if (matchedParamsValues.Count == method.GetParameters().Length &&
                            (urlParams.Count == 0 || matchedParamsValues.Count > 0))
                        {
                            return new Tuple<MethodInfo, object[]>(method, matchedParamsValues.ToArray());
                        }
                        break;
                }
            }
            return null;
        }

        private static bool TryMatchParameters(MethodInfo method, Dictionary<string, string> urlParams,
            HttpVerbs requestVerb, List<object> matchedParamsValues)
        {
            if (matchedParamsValues == null)
                return false;
            else
                matchedParamsValues.Clear();

            for (int i = 0; i < method.GetParameters().Length; i++)
            {
                ParameterInfo parameter = method.GetParameters()[i];
                string gotParam;
                if (urlParams.TryGetValue(parameter.Name, out gotParam))

                {
                    if (parameter.ParameterType == typeof (int))
                    {
                        int integerParamValue;
                        if (int.TryParse(matchedParamsValues[matchedParamsValues.Count - 1].ToString(),
                            out integerParamValue))
                        {
                            matchedParamsValues.Add(integerParamValue);
                        }
                    }
                    else
                    {
                        matchedParamsValues.Add(gotParam);
                    }
                }
                else if (urlParams.Count <= i)
                    continue;
                else if (parameter.ParameterType == typeof (int))
                {
                    int paramValue;
                    if (int.TryParse(urlParams.ValueAtIndex(i), out paramValue))
                    {
                        matchedParamsValues.Add(paramValue);
                    }
                    else
                    {
                        break;
                    }
                }
                else if (parameter.ParameterType == typeof (string))
                {
                    matchedParamsValues.Add(urlParams.ValueAtIndex(i));
                }
                else
                {
                    throw new NotSupportedException(
                        "An allowing '" + requestVerb + "' method cannot have parameters type other than string and int");
                }
            }
            return true;
        }

        public static string JsonSerializer(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof (T), value, true);
        }
        public static IEnumerable<string> GetPossibleTypesFromStringList(IEnumerable<string> values)
        {
            int intVal;
            return values.Select(value => int.TryParse(value, out intVal) ? "int" : "string");
        }
    }
}
