using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Wholesaler.Models
{
    public partial class WholesaleProductsModel : BaseNopEntityModel
    {
        public int TotalProducts { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int ItemDisplayed { get; set; }
        public IList<WholesalerProductOverviewModel> Products { get; set; }
    }
}
