using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.AllInOne.Data;
using Nop.Plugin.Widgets.AllInOne.Services;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.AllInOne
{
    public class AllInOnePlugin : BasePlugin, IWidgetPlugin, IPlugin, IAdminMenuPlugin
    {
        private readonly IAllInOneService _allInOneService;

        private readonly ISettingService _settingService;

        private readonly IWebHelper _webHelper;

        private readonly AllInOneObjectContext _objectContext;

        private readonly ILocalizationService _localizationService;

        private readonly IWorkContext _workContext;

        public AllInOnePlugin(IAllInOneService allInOneService, AllInOneObjectContext objectContext, ISettingService settingService, IWebHelper webHelper, ILocalizationService localizationService, IWorkContext workContext)
        {
            this._allInOneService = allInOneService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._objectContext = objectContext;
            this._localizationService = localizationService;
            this._workContext = workContext;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsAllInOne";
            RouteValueDictionary routeValueDictionaries = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.Widgets.AllInOne.Controllers" },
                { "area", null }
            };
            routeValues = routeValueDictionaries;
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsAllInOne";
            RouteValueDictionary routeValueDictionaries = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.Widgets.AllInOne.Controllers" },
                { "area", null },
                { "widgetZone", widgetZone }
            };
            routeValues = routeValueDictionaries;
        }

        public IList<string> GetWidgetZones()
        {
            return this._allInOneService.GetAllInOnesWidgetZones();
        }

        public override void Install()
        {
            this._objectContext.Install();
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Manage", "Manage AllInOnes", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.Name", "Name", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.Name.Required", "Please provide a name.", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.Name.hint", "AllInOne name.", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCode", "Html Code", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCode.Required", "Please provide a HTML code.", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCode.hint", "HTML code.", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCodeExtra", "Html additional  Code", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCodeExtra.Required", "Please provide a HTML code.", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCodeExtra.hint", "HTML additional code not supported by TinyMCE Editor.", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.WidgetZone", "Widget Zone", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.WidgetZone.Required", "Please provide a Widget Zone", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.WidgetZone.hint", "Widget Zone", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.Published", "Published", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.Published.Hint", "Published", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.DisplayOrder", "Display order", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.DisplayOrder.Hint", "Display order", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.jsFileList", "Javascript files", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.jsFileList.Hint", "Add javascript files", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.CssFileList", "CSS files", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.CssFileList.Hint", "Add CSS files", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Allinone.Editdetails", "Edit details", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Allinone.Editdetails", "Edit details", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Allinone.BackToList", "back to list", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Allinone.Info", "Info", null);
            this.AddOrUpdatePluginLocaleResource("plugins.widgets.allinone.cssscripts", "CSS JavaScript files", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Allinone.Addnew", "Add new", null);
            this.AddOrUpdatePluginLocaleResource("plugins.widgets.allinone.updated", "Record Updated", null);
            this.AddOrUpdatePluginLocaleResource("plugins.widgets.allinone.added", "Record Added", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Allinone.Deleted", "Record Deleted", null);
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AllInOne.AdminMenuText", "All in one", null);
            base.Install();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            SiteMapNode siteMapNode = new SiteMapNode()
            {
                SystemName = "AllInOne",
                Title = this._localizationService.GetResource("Plugins.Widgets.AllInOne.AdminMenuText"),
                ControllerName = "WidgetsAllInOne",
                ActionName = "List",
                Visible = true,
                RouteValues = new RouteValueDictionary()
                {
                    { "area", null }
                }
            };
            SiteMapNode siteMapNode1 = siteMapNode;
            SiteMapNode siteMapNode2 = rootNode.ChildNodes.FirstOrDefault<SiteMapNode>((SiteMapNode x) => x.SystemName == "Third party plugins");
            if (siteMapNode2 == null)
            {
                rootNode.ChildNodes.Add(siteMapNode1);
            }
            else
            {
                siteMapNode2.ChildNodes.Add(siteMapNode1);
            }
        }

        public override void Uninstall()
        {
            this._objectContext.Uninstall();
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Manage");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.Name");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.Name.Required");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.Name.hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCode");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCode.Required");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCode.hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCodeExtra");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCodeExtra.Required");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.HtmlCodeExtra.hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.WidgetZone");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.WidgetZone.hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.WidgetZone.Required");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.Published");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.Published.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.DisplayOrder");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.DisplayOrder.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.jsFileList");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.jsFileList.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.CssFileList");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.Fields.CssFileList.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Allinone.Editdetails");
            this.DeletePluginLocaleResource("Plugins.Widgets.Allinone.Editdetails");
            this.DeletePluginLocaleResource("Plugins.Widgets.Allinone.BackToList");
            this.DeletePluginLocaleResource("Plugins.Widgets.Allinone.Info");
            this.DeletePluginLocaleResource("Plugins.Widgets.Allinone.cssscripts");
            this.DeletePluginLocaleResource("Plugins.Widgets.Allinone.Addnew");
            this.DeletePluginLocaleResource("plugins.widgets.allinone.updated");
            this.DeletePluginLocaleResource("plugins.widgets.allinone.added");
            this.DeletePluginLocaleResource("Plugins.Widgets.Allinone.Deleted");
            this.DeletePluginLocaleResource("Plugins.Widgets.AllInOne.AdminMenuText");
            base.Uninstall();
        }
    }
}