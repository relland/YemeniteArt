using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Nop.Plugin.Widgets.AllInOne.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public List<string> CssFiles
        {
            get;
            set;
        }

        public List<string> HtmlCodes
        {
            get;
            set;
        }

        public List<string> JFiles
        {
            get;
            set;
        }

        public PublicInfoModel()
        {
            this.HtmlCodes = new List<string>();
            this.CssFiles = new List<string>();
            this.JFiles = new List<string>();
        }
    }
}