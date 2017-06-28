using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Wholesaler.Models
{
    public class ConfigurationModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("admin.Wholesaler.EnableWidget")]
        public bool EnableWidget { get; set; }
        public bool EnableWidget_OverrideForStore { get; set; }

        [NopResourceDisplayName("admin.Wholesaler.WholesalerRoleSystemName")]
        public string WholesalerRoleSystemName { get; set; }
        public bool WholesalerRoleSystemName_OverrideForStore { get; set; }

        [NopResourceDisplayName("admin.Wholesaler.PercentageAmount")]
        public decimal PercentageAmount { get; set; }
        public bool PercentageAmount_OverrideForStore { get; set; }

        [NopResourceDisplayName("admin.Wholesaler.LastCalculationDate")]
        public DateTime? LastCalculationDate { get; set; }
        public bool LastCalculationDate_OverrideForStore { get; set; }
    }
}
