using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace customApiApp_3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "get by index api",
                "{st}({index})",
                new { controller = "MainApi", action = "GetByIndex" }
                );
            // for apis
            routes.MapRoute(
                "Normal apis",
                "{*url}",
                new { controller = "MainApi", action = "NormalApiCall" }
                );
            // for main pages
            routes.MapRoute(
                name: "Default",
                url: "{*url}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
