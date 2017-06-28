using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.AllInOne
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public RouteProvider()
        {
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Nop.Plugin.Widgets.AllInOne.Configure", 
                "Plugins/WidgetsAllInOne/Configure", 
                new { controller = "WidgetsAllInOne", action = "Configure" }, 
                new string[] { "Nop.Plugin.Widgets.AllInOne.Controllers" });
            routes.MapRoute("Plugin.Widgets.AllInOne.List", 
                "Plugins/WidgetsAllInOne/List", 
                new { controller = "WidgetsAllInOne", action = "List" }, 
                new string[] { "Nop.Plugin.Widgets.AllInOne.Controllers" });
        }
    }
}