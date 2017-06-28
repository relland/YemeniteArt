using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public class PriceCalculationSettings : ISettings
    {
        public decimal RetailSilverFactor { get; set; } //cost * RetailSilverFactor
        public decimal WholesaleSilverFactor { get; set; }
        public decimal RetailGoldPlatedFactor { get; set; } //(cost * RetailSilverFactor) * RetailGoldPlatedFactor
        public decimal WholesaleGoldPlatedFactor { get; set; }
        public decimal RetailSolidGold14KFactor { get; set; } //weight * RetailSolidGold14KFactor
        public decimal WholesaleSolidGold14KFactor { get; set; }
        public decimal RetailSolidGold18KFactor { get; set; }
        public decimal WholesaleSolidGold18KFactor { get; set; }

    }
}
