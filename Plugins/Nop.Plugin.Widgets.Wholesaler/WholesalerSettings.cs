using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Wholesaler
{
    class WholesalerSettings : ISettings
    {
        public bool EnableWidget { get; set; }
        public string WholesalerRoleSystemName { get; set; }
        public decimal PercentageAmount { get; set; }
        public DateTime? LastCalculationDate { get; set; }
        //public bool ShowAlsoPurchased { get; set; }
        //public bool ShowRelatedProducts { get; set; }
        //public bool EnableEnlargePicture { get; set; }
        //public string ButtonContainerName { get; set; }
    }
}