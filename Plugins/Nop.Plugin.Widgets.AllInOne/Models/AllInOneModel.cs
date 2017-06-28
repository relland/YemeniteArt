using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.AllInOne.Models
{
    [Validator(typeof(AllInOneValidator))]
    public class AllInOneModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Widgets.AllInOne.Fields.cssFileList")]
        public string cssFileList
        {
            get;
            set;
        }

        [NopResourceDisplayName("Plugins.Widgets.AllInOne.Fields.DisplayOrder")]
        public int DisplayOrder
        {
            get;
            set;
        }

        [AllowHtml]
        [NopResourceDisplayName("Plugins.Widgets.AllInOne.Fields.HtmlCode")]
        public string HtmlCode
        {
            get;
            set;
        }

        [AllowHtml]
        [NopResourceDisplayName("Plugins.Widgets.AllInOne.Fields.HtmlCodeExtra")]
        public string HtmlCodeExtra
        {
            get;
            set;
        }

        [NopResourceDisplayName("Plugins.Widgets.AllInOne.Fields.jsFileList")]
        public string jsFileList
        {
            get;
            set;
        }

        [NopResourceDisplayName("Plugins.Widgets.AllInOne.Fields.Name")]
        public string Name
        {
            get;
            set;
        }

        [NopResourceDisplayName("Plugins.Widgets.AllInOne.Fields.Published")]
        public bool Published
        {
            get;
            set;
        }

        [NopResourceDisplayName("Plugins.Widgets.AllInOne.Fields.WidgetZone")]
        public string WidgetZone
        {
            get;
            set;
        }

        public AllInOneModel()
        {
        }
    }
}