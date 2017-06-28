using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Wholesaler.Models
{
    public class ProductFilterModel
    {
        public string SearchProductName { get; set; }
        public int SearchCategoryId { get; set; }
        public string SortByName { get; set; }
        public string SortBySku { get; set; }
        public string SortByRetailPrice { get; set; }
        public string SortByWholesalerPrice { get; set; }
        
        public string GoDirectlyToSku { get; set; }
    }
}
