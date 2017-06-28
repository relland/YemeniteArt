using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using Nop.Web;

namespace Nop.Plugin.BrainStation.QuickView.Models
{
    public class BsQuickViewModel : BaseNopEntityModel
    {
        public BsQuickViewModel()
        {
            ProductDetailsModel = new ProductDetailsModel();
            BsQuickViewSettingsModel = new BsQuickViewSettingsModel();
        }

        public ProductDetailsModel ProductDetailsModel { get; set; }
        public BsQuickViewSettingsModel BsQuickViewSettingsModel { get; set; }


        

    }
}