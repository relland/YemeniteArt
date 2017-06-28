using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Localization;

namespace Nop.Plugin.Widgets.Wholesaler
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
            routes.MapRoute("Wholesaler.Products", 
                 "Wholesaler/Products/",
                 new { controller = "Wholesaler", action = "Products" },
                 new[] { "Nop.Plugin.Widgets.Wholesaler.Controllers" }
            );
            routes.MapRoute("Wholesaler.ProductList",
                 "Wholesaler/ProductList/",
                 new { controller = "Wholesaler", action = "ProductList" },
                 new[] { "Nop.Plugin.Widgets.Wholesaler.Controllers" }
            );
            routes.MapRoute("Wholesaler.ReCalculate",
                 "Wholesaler/ReCalculate/",
                 new { controller = "Wholesaler", action = "ReCalculate" },
                 new[] { "Nop.Plugin.Widgets.Wholesaler.Controllers" }
            );
            routes.MapRoute("Wholesaler.Cart",
                 "Wholesaler/Cart/",
                 new { controller = "Wholesaler", action = "Cart" },
                 new[] { "Nop.Plugin.Widgets.Wholesaler.Controllers" }
            );
        }
    }
}
