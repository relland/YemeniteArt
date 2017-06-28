using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Models.Admin
{
    public class SessionCreationModal
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string SuHours { get; set; }
        public string MoHours { get; set; }
        public string TuHours { get; set; }
        public string WeHours { get; set; }
        public string ThHours { get; set; }
        public string FrHours { get; set; }
        public string SaHours { get; set; }
        public int SessionLengthByMinutes { get; set; }
        public int SessionAvailablilityCustomerCount { get; set; }
    }
}
