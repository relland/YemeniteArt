using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.Wholesaler.Models
{
    public partial class ProductModel : BaseNopEntityModel, ILocalizedModel<ProductLocalizedModel>
    {
        public ProductModel()
        {
            Locales = new List<ProductLocalizedModel>();
        }

        [NopResourceDisplayName("wholesaler.ID")]
        public override int Id { get; set; }

        //picture thumbnail
        [NopResourceDisplayName("wholesaler.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        
        [NopResourceDisplayName("wholesaler.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("wholesaler.SeName")]
        public string SeName { get; set; }

        [NopResourceDisplayName("wholesaler.Sku")]
        public string Sku { get; set; }

        [NopResourceDisplayName("wholesaler.RetailPrice")]
        public decimal RetailPrice { get; set; }

        [NopResourceDisplayName("wholesaler.WholesalerPrice")]
        
        public decimal WholesalerPrice { get; set; }
        //tier price per quantity...
        [NopResourceDisplayName("wholesaler.WholesalerQuantity")]
        public int WholesalerQuantity { get; set; }

        [NopResourceDisplayName("wholesaler.Quantity")]
        public int Quantity { get; set; }

        [NopResourceDisplayName("wholesaler.TotalPrice")]
        public decimal TotalPrice { get; set; }
        public IList<ProductLocalizedModel> Locales { get; set; }
    }

    public partial class ProductLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}
