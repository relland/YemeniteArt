using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Plugin.Widgets.AllInOne.Domain;
using Nop.Plugin.Widgets.AllInOne.Models;
using Nop.Plugin.Widgets.AllInOne.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.AllInOne.Controllers
{
    public class WidgetsAllInOneController : BasePluginController
    {
        private readonly IAllInOneService _allInOneService;

        private readonly IWorkContext _workContext;

        private readonly IStoreContext _storeContext;

        private readonly IStoreService _storeService;

        private readonly ISettingService _settingService;

        private readonly ILocalizationService _localizationService;

        private readonly IWebHelper _webHelper;

        public WidgetsAllInOneController(IAllInOneService allInOneService, IWorkContext workContext, IStoreContext storeContext, IStoreService storeService, ISettingService settingService, ILocalizationService localizationService, IWebHelper webHelper)
        {
            this._allInOneService = allInOneService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            return base.View("~/Plugins/Widgets.AllInOne/Views/Configure.cshtml");
        }

        public ActionResult Create()
        {
            AllInOneModel allInOneModel = new AllInOneModel()
            {
                Published = true
            };
            return base.View("~/Plugins/Widgets.AllInOne/Views/Create.cshtml", allInOneModel);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(AllInOneModel model, bool continueEditing)
        {
            ActionResult actionResult;
            if (!base.ModelState.IsValid)
            {
                actionResult = base.View(model);
            }
            else
            {
                AllInOneObject allInOneObject = new AllInOneObject()
                {
                    Id = model.Id,
                    Name = model.Name,
                    WidgetZone = model.WidgetZone,
                    Published = model.Published,
                    DisplayOrder = model.DisplayOrder,
                    HtmlCode = model.HtmlCode,
                    HtmlCodeExtra = model.HtmlCodeExtra,
                    jsFileList = model.jsFileList,
                    cssFileList = model.cssFileList
                };
                AllInOneObject allInOneObject1 = allInOneObject;
                this._allInOneService.InsertAllInOne(allInOneObject1);
                this.SuccessNotification(this._localizationService.GetResource("Plugins.Widgets.AllInOne.Added"), true);
                actionResult = (continueEditing ? base.RedirectToAction("Edit", new { id = allInOneObject1.Id }) : base.RedirectToAction("List"));
            }
            return actionResult;
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            ActionResult action;
            AllInOneObject allInOneById = this._allInOneService.GetAllInOneById(id);
            if (allInOneById != null)
            {
                this._allInOneService.DeleteAllInOne(allInOneById);
                this.SuccessNotification(this._localizationService.GetResource("Plugins.Widgets.AllInOne.Deleted"), true);
                action = base.RedirectToAction("List");
            }
            else
            {
                action = base.RedirectToAction("List");
            }
            return action;
        }

        public ActionResult Edit(int id)
        {
            ActionResult action;
            AllInOneObject allInOneById = this._allInOneService.GetAllInOneById(id);
            if (allInOneById != null)
            {
                action = base.View("~/Plugins/Widgets.AllInOne/Views/Edit.cshtml", this.PrepareAllInOneModel(allInOneById));
            }
            else
            {
                action = base.RedirectToAction("List");
            }
            return action;
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(AllInOneModel model, bool continueEditing)
        {
            ActionResult action;
            AllInOneObject allInOneById = this._allInOneService.GetAllInOneById(model.Id);
            if (allInOneById == null)
            {
                action = base.RedirectToAction("List");
            }
            else if (!base.ModelState.IsValid)
            {
                action = base.View(model);
            }
            else
            {
                allInOneById.Name = model.Name;
                allInOneById.WidgetZone = model.WidgetZone;
                allInOneById.Published = model.Published;
                allInOneById.DisplayOrder = model.DisplayOrder;
                allInOneById.HtmlCode = model.HtmlCode;
                allInOneById.HtmlCodeExtra = model.HtmlCodeExtra;
                allInOneById.jsFileList = model.jsFileList;
                allInOneById.cssFileList = model.cssFileList;
                this._allInOneService.UpdateAllInOne(allInOneById);
                this.SuccessNotification(this._localizationService.GetResource("Plugins.Widgets.AllInOne.Updated"), true);
                action = (continueEditing ? base.RedirectToAction("Edit", allInOneById.Id) : base.RedirectToAction("List"));
            }
            return action;
        }

        public ActionResult Index()
        {
            return base.RedirectToAction("List");
        }

        public ActionResult List()
        {
            return base.View("~/Plugins/Widgets.AllInOne/Views/List.cshtml");
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, AllInOneModel model)
        {
            IPagedList<AllInOneObject> allInOnes = this._allInOneService.GetAllInOnes("", command.Page - 1, command.PageSize);
            List<AllInOneModel> allInOneModels = new List<AllInOneModel>();
            foreach (AllInOneObject allInOne in allInOnes)
            {
                allInOneModels.Add(this.PrepareAllInOneModel(allInOne));
            }
            IEnumerable<AllInOneModel> allInOneModels1 = allInOneModels.PagedForCommand<AllInOneModel>(command);
            DataSourceResult dataSourceResult = new DataSourceResult()
            {
                Data = allInOneModels1,
                Total = allInOneModels1.Count<AllInOneModel>()
            };
            return new JsonResult()
            {
                Data = dataSourceResult
            };
        }

        protected AllInOneModel PrepareAllInOneModel(AllInOneObject allInOne)
        {
            AllInOneModel allInOneModel = new AllInOneModel()
            {
                Id = allInOne.Id,
                Name = allInOne.Name,
                HtmlCode = allInOne.HtmlCode,
                HtmlCodeExtra = allInOne.HtmlCodeExtra,
                WidgetZone = allInOne.WidgetZone,
                Published = allInOne.Published,
                DisplayOrder = allInOne.DisplayOrder,
                cssFileList = allInOne.cssFileList,
                jsFileList = allInOne.jsFileList
            };
            return allInOneModel;
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone)
        {
            IList<AllInOneObject> allInOnesByWidgetZones = this._allInOneService.GetAllInOnesByWidgetZones(widgetZone);
            PublicInfoModel publicInfoModel = new PublicInfoModel();
            foreach (AllInOneObject allInOnesByWidgetZone in allInOnesByWidgetZones)
            {
                string htmlCode = allInOnesByWidgetZone.HtmlCode;
                if (!string.IsNullOrEmpty(allInOnesByWidgetZone.HtmlCodeExtra))
                {
                    htmlCode = string.Concat(htmlCode, allInOnesByWidgetZone.HtmlCodeExtra);
                }
                publicInfoModel.HtmlCodes.Add(htmlCode);
                if (!string.IsNullOrEmpty(allInOnesByWidgetZone.cssFileList))
                {
                    List<string> strs = new List<string>(allInOnesByWidgetZone.cssFileList.Split(new char[] { ',' }));
                    bool rtl = this._workContext.WorkingLanguage.Rtl;
                    foreach (string str in strs)
                    {
                        string str1 = string.Concat(str, (rtl ? ".rtl.css" : ".css"));
                        if (System.IO.File.Exists(Path.Combine(CommonHelper.MapPath("~/Plugins/Widgets.AllInOne/Content/"), str1)))
                        {
                            publicInfoModel.CssFiles.Add(str1);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(allInOnesByWidgetZone.jsFileList))
                {
                    foreach (string str2 in new List<string>(allInOnesByWidgetZone.jsFileList.Split(new char[] { ',' })))
                    {
                        if (System.IO.File.Exists(Path.Combine(CommonHelper.MapPath("~/Plugins/Widgets.AllInOne/Content/"), str2)))
                        {
                            publicInfoModel.JFiles.Add(str2);
                        }
                    }
                }
            }
            return base.View("~/Plugins/Widgets.AllInOne/Views/PublicInfo.cshtml", publicInfoModel);
        }
    }
}