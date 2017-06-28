using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.Wholesaler
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class WholesalerPlugin : BasePlugin, IWidgetPlugin
    {

        private readonly ISettingService _settingService;


        public WholesalerPlugin(ISettingService settingService)
        {

            this._settingService = settingService;

        }

        public bool Authenticate()
        {
            return true;
        }

                
        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string>() { "account_navigation_after" };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "Wholesaler";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.Wholesaler.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "AccountNavigation";
            controllerName = "Wholesaler";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", " Nop.Plugin.Widgets.Wholesaler.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new WholesalerSettings()
            {
                EnableWidget = true,
                WholesalerRoleSystemName = "wholesaler",
                PercentageAmount = 50,
                LastCalculationDate = DateTime.Now
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("admin.Wholesaler.configuare", "Configuare Wholesaler Widget");
            this.AddOrUpdatePluginLocaleResource("admin.Wholesaler.save", "Save Wholesaler Widget");
            this.AddOrUpdatePluginLocaleResource("admin.Wholesaler.FirstSaveChangesAndThenReCalculate", "First Save Changes And Then Re-Calculate");
            this.AddOrUpdatePluginLocaleResource("admin.Wholesaler.ReCalculateAllProductsWithNewPercentage", "Re-Calculate All Products With New Percentage");
            this.AddOrUpdatePluginLocaleResource("admin.Wholesaler.EnableWidget", "Enable Wholesaler Widget"); 
            this.AddOrUpdatePluginLocaleResource("admin.Wholesaler.PercentageAmount", "Percentage Amount");
            this.AddOrUpdatePluginLocaleResource("admin.Wholesaler.WholesalerRoleSystemName", "Wholesaler Role SystemName"); 
            this.AddOrUpdatePluginLocaleResource("admin.Wholesaler.LastCalculationDate", "Last Calculation Date");
            this.AddOrUpdatePluginLocaleResource("admin.Wholesaler.ReCalculatedSuccessfully", "Re Calculated Successfully. {0} Updated, {1} Added.");
            
            this.AddOrUpdatePluginLocaleResource("Wholesaler.ProductList", "Wholesaler Product List");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.Products", "Wholesaler Products");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.PictureThumbnailUrl", "Picture");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.Sku", "Sku");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.RetailPrice", "Retail Price");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.WholesalerPrice", "Wholesaler Price");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.Quantity", "Quantity");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.TotalPrice", "Total Price");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.ID", "ID");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.SeName", "SeName");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.WholesalerQuantity", "Wholesaler Quantity");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.viewproducts", "View Products");

            this.AddOrUpdatePluginLocaleResource("Wholesaler.unitprice", "Unit Price");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.subtotal", "Subtotal");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.catalog", "Catalog");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.cart", "Cart");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.SearchBySku", "Search By Sku");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.SearchProducts", "Search Products");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.SearchProductName", "Search Product Name");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.GoDirectlyToSku", "Go Directly To Sku");
            this.AddOrUpdatePluginLocaleResource("Wholesaler.SearchCategory", "Search Category");



            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {

            this.DeletePluginLocaleResource("admin.Wholesaler.configuare");
            this.DeletePluginLocaleResource("admin.Wholesaler.save");
            this.DeletePluginLocaleResource("admin.Wholesaler.ReCalculateAllProductsWithNewPercentage");
            this.DeletePluginLocaleResource("admin.Wholesaler.EnableWidget");
            this.DeletePluginLocaleResource("admin.Wholesaler.PercentageAmount");
            this.DeletePluginLocaleResource("admin.Wholesaler.WholesalerRoleSystemName");
            this.DeletePluginLocaleResource("admin.Wholesaler.LastCalculationDate");
            
            this.DeletePluginLocaleResource("Wholesaler.ProductList");
            this.DeletePluginLocaleResource("Wholesaler.Products");
            this.DeletePluginLocaleResource("Wholesaler.PictureThumbnailUrl");
            this.DeletePluginLocaleResource("Wholesaler.Name");
            this.DeletePluginLocaleResource("Wholesaler.Sku");
            this.DeletePluginLocaleResource("Wholesaler.RetailPrice");
            this.DeletePluginLocaleResource("Wholesaler.WholesalerPrice");
            this.DeletePluginLocaleResource("Wholesaler.Quantity");
            this.DeletePluginLocaleResource("Wholesaler.TotalPrice");
            this.DeletePluginLocaleResource("Wholesaler.ID");
            this.DeletePluginLocaleResource("Wholesaler.SeName");
            this.DeletePluginLocaleResource("Wholesaler.WholesalerQuantity");
            this.DeletePluginLocaleResource("Wholesaler.viewproducts");

            this.DeletePluginLocaleResource("Wholesaler.unitprice");
            this.DeletePluginLocaleResource("Wholesaler.subtotal");
            this.DeletePluginLocaleResource("Wholesaler.catalog");
            this.DeletePluginLocaleResource("Wholesaler.cart");
            this.DeletePluginLocaleResource("Wholesaler.SearchBySku");
            this.DeletePluginLocaleResource("Wholesaler.SearchProducts");

            this.DeletePluginLocaleResource("Wholesaler.SearchProductName");
            this.DeletePluginLocaleResource("Wholesaler.GoDirectlyToSku");
            this.DeletePluginLocaleResource("Wholesaler.SearchCategory");



            base.Uninstall();
        }
    }
}
