using System.Web.Mvc;
using System.Web.Routing;
using Nop.Plugin.BrainStation.QuickView.ViewEngines;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.BrainStation.QuickView
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            System.Web.Mvc.ViewEngines.Engines.Insert(0, new CustomViewEngine());


            routes.MapRoute("Nop.Plugin.BrainStation.QuickView.ProductDetails", "bs_product_details",
                new { controller = "BsQuickView", action = "ProductDetails"},
                 new[] { "Nop.Plugin.BrainStation.QuickView.Controllers" });

            routes.MapRoute("Admin.Plugin.BrainStation.QuickView.Settings", "Admin/Plugin/BrainStation/QuickView/Settings",
                   new { controller = "BsQuickViewAdmin", action = "BsQuickViewSettings" },
                   new[] { "Nop.Plugin.BrainStation.QuickView.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Admin.Plugin.BrainStation.QuickView.Help", "Admin/Plugin/BrainStation/QuickView/Help",
                   new { controller = "BsQuickViewAdmin", action = "BsQuickViewHelp" },
                   new[] { "Nop.Plugin.BrainStation.QuickView.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Admin.Plugin.BrainStation.QuickView.SaveSettings", "Admin/Plugin/QuickView/Settings/Save",
                   new { controller = "BsQuickViewAdmin", action = "BsQuickViewSettings" },
                   new[] { "Nop.Plugin.BrainStation.QuickView.Controllers" }).DataTokens.Add("area", "admin");

           

        }

        public int Priority
        {
            get { return 10; }
        }

    }
}
