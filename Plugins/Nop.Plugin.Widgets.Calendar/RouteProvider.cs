using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Localization;

namespace Nop.Plugin.Widgets.Calendar
{
    public partial class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 0;
            }
        }
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Calendar.Products",
                 "Calendar/Products/",
                 new { controller = "Calendar", action = "Products" },
                 new[] { "Nop.Plugin.Widgets.Calendar.Controllers" }
            );
            routes.MapRoute("Calendar.ProductList",
                 "Calendar/ProductList/",
                 new { controller = "Calendar", action = "ProductList" },
                 new[] { "Nop.Plugin.Widgets.Calendar.Controllers" }
            );
            routes.MapRoute("Calendar.ReCalculate",
                 "Calendar/ReCalculate/",
                 new { controller = "Calendar", action = "ReCalculate" },
                 new[] { "Nop.Plugin.Widgets.Calendar.Controllers" }
            );
            routes.MapRoute("Calendar.Cart",
                 "Calendar/Cart/",
                 new { controller = "Calendar", action = "Cart" },
                 new[] { "Nop.Plugin.Widgets.Calendar.Controllers" }
            );
        }
    }
}
