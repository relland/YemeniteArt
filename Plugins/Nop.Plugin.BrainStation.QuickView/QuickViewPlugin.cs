using System.Collections.Generic;
using System.IO;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework;

namespace Nop.Plugin.BrainStation.QuickView 
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class QuickViewPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        
        private readonly ISettingService _settingService;
        

        public QuickViewPlugin(ISettingService settingService)
        {
           
            this._settingService = settingService;
            
        }

        public bool Authenticate()
        {
            return true;
        }

       
        public SiteMapNode BuildMenuItem() // SiteMapNode is Class in Nop.Web.Framework.Menu 
        {
            var menuItemBuilder = new SiteMapNode()
            {
                 Visible = true,
                Title = "Bs Quick View",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            };
            menuItemBuilder.ChildNodes.Add(new SiteMapNode
            {
                Title = "Settings",
                Url = "/Admin/Plugin/BrainStation/QuickView/Settings",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            });
            menuItemBuilder.ChildNodes.Add(new SiteMapNode
            {
               
                Title = "Help",
                Url = "/Admin/Plugin/BrainStation/QuickView/Help",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            });
            var SubMenuItem = new SiteMapNode()   // add child Custom menu 
            {
                Title = "Configure", //   Title for your Sub Menu item
                ControllerName = "BsQuickViewAdmin", // Your controller Name
                ActionName = "BsQuickViewConfigure", // Action Name
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } },
            };
            menuItemBuilder.ChildNodes.Add(SubMenuItem);


            return menuItemBuilder;

        }
        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string>() { "footer" };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "BsQuickViewConfigure";
            controllerName = "BsQuickViewAdmin";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.BrainStation.QuickView.Controllers" }, { "area", null } };
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
            actionName = "AddButton";
            controllerName = "BsQuickView";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "Nop.Plugin.BrainStation.QuickView.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new QuickViewSettings()
            {
                EnableWidget = true,
                ShowAlsoPurchased = true,
                EnableEnlargePicture = true,
                ShowRelatedProducts = true,
                ButtonContainerName = ".buttons,.overlay-content",
                
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("admin.plugin.brainstation.quickview.configuare", "Configuare Quick View");
            this.AddOrUpdatePluginLocaleResource("admin.plugin.brainstation.quickview.save", "Save Quick View");
            this.AddOrUpdatePluginLocaleResource("Plugins.BsQuickView.Fields.ShowAlsoPurchased", "Show Also Purchased Products");
            this.AddOrUpdatePluginLocaleResource("Plugins.BsQuickView.Fields.ShowRelatedProducts", "Show Related Products");
            this.AddOrUpdatePluginLocaleResource("Plugins.BsQuickView.Fields.EnableWidget", "Enable QuickView Widget");
            this.AddOrUpdatePluginLocaleResource("Plugins.BsQuickView.Fields.EnableEnlargePicture", "Enable Enlarging Product Pictures");
            this.AddOrUpdatePluginLocaleResource("Plugins.BsQuickView.Fields.ButtonContainerName", "Button Container Class (eg: .buttons)");
            this.AddOrUpdatePluginLocaleResource("admin.plugin.brainstation.quickview.help", "Help");
            this.AddOrUpdatePluginLocaleResource("admin.plugin.brainstation.quickview.settings", "Settings");
            
            

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {

            this.DeletePluginLocaleResource("admin.plugin.brainstation.quickview.configuare");
            this.DeletePluginLocaleResource("admin.plugin.brainstation.quickview.save");
            this.DeletePluginLocaleResource("Plugins.BsQuickView.Fields.ShowAlsoPurchased");
            this.DeletePluginLocaleResource("Plugins.BsQuickView.Fields.ShowRelatedProducts");
            this.DeletePluginLocaleResource("Plugins.BsQuickView.Fields.EnableWidget");
            this.DeletePluginLocaleResource("Plugins.BsQuickView.Fields.ButtonContainerName");
            this.DeletePluginLocaleResource("admin.plugin.brainstation.quickview.help");
            this.DeletePluginLocaleResource("admin.plugin.brainstation.quickview.settings");
            this.DeletePluginLocaleResource("Plugins.BsQuickView.Fields.EnableEnlargePicture");

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItemBuilder = new SiteMapNode()
            {
                Visible = true,
                Title = "Bs Quick View",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            };
            menuItemBuilder.ChildNodes.Add(new SiteMapNode
            {
                Visible = true,
                Title = "Settings",
                Url = "/Admin/Plugin/BrainStation/QuickView/Settings",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            });
            menuItemBuilder.ChildNodes.Add(new SiteMapNode
            {
                Visible = true,
                Title = "Help",
                Url = "/Admin/Plugin/BrainStation/QuickView/Help",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            });
            var SubMenuItem = new SiteMapNode()   // add child Custom menu 
            {
                Visible = true,
                Title = "Configure", //   Title for your Sub Menu item
                Url="/Admin/Widget/ConfigureWidget?systemName=BrainStation.QuickView",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } },
            };
            menuItemBuilder.ChildNodes.Add(SubMenuItem);
            rootNode.ChildNodes.Add(menuItemBuilder);
        }
    }
}
