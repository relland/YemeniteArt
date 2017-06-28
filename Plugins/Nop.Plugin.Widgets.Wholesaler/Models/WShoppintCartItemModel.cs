using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Wholesaler.Models
{
    public class WShoppintCartItemModel : BaseNopModel
    {
        public int ProductId { get; set; }

        public string Sku { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public string UnitPrice { get; set; }

        public string SubTotal { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
    }
}
