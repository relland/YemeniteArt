using Nop.Core;
using Nop.Core.Domain.Localization;
using System;
using System.Runtime.CompilerServices;

namespace Nop.Plugin.Widgets.AllInOne.Domain
{
    public class AllInOneObject : BaseEntity, ILocalizedEntity
    {
        public string cssFileList
        {
            get;
            set;
        }

        public int DisplayOrder
        {
            get;
            set;
        }

        public string HtmlCode
        {
            get;
            set;
        }

        public string HtmlCodeExtra
        {
            get;
            set;
        }

        public string jsFileList
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public bool Published
        {
            get;
            set;
        }

        public string WidgetZone
        {
            get;
            set;
        }

        public AllInOneObject()
        {
        }
    }
}