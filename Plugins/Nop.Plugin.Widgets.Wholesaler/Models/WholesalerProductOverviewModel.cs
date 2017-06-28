using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Wholesaler.Models
{
    public partial class WholesalerProductOverviewModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string SeName { get; set; }

        //price
        public string OldPrice { get; set; }
        public string Price { get; set; }
        public string WholesalerPrice { get; set; }

        public string TotalPrice { get; set; }
        //picture
        public string PictureThumbnailUrl { get; set; }
        //rel
        public string PictureBigUrl { get; set; }
        public string Title { get; set; }
        public string AlternateText { get; set; }
        //rel
        public string Sku { get; set; }
        public int StockQuantity { get; set; }

        public bool HasDiferentProductAttributes { get; set; }
        //public List<WholesalerProductAttributeOverviewModel> ProductAttributesModels { get; set; }
        public string ProductAttributes { get; set; }


    }
    public partial class WholesalerProductAttributeOverviewModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        //public string ShortDescription { get; set; }
        public string SeName { get; set; }

        //price
        //public string OldPrice { get; set; }
        public string Price { get; set; }
        //public string WholesalerPrice { get; set; }

        public string TotalPrice { get; set; }
        //picture
        //public string PictureThumbnailUrl { get; set; }
        //public string Title { get; set; }
        //public string AlternateText { get; set; }
        //rel
        //public string Sku { get; set; }
        public int StockQuantity { get; set; }
        public int CartItemId { get; set; }
    }
}
