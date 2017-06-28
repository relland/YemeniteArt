using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Models.Admin
{
    public class ConfigurationModel
    {
        public bool EnableWidget { get; set; }
        public string CalanderAccessRoleSystemName { get; set; }

        public bool CanOverlapOtherSessions { get; set; } //if "true" => don't check existing sessions, just add. if "false" => create only sessions that don't overlap
        public int PermittedOverlappingThreshold { get; set; } // default - one minute...
        public string DisabledDays { get; set; } //"1,6,7"
        public string DisabledDates { get; set; } //"1/1/2015, 12/22/2019"
        public string SessionsDayAndTimeRange { get; set; } //"1=10:00-14:30, 1=15:30-20:00, 2=09:00-20:30"
        public int SessionLengthByMinutes { get; set; }
        public int SessionAvailablilityCustomerCount { get; set; }
        public DateTime? LastSessionCreationDate { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public DateTime? LastSessionDate { get; set; }




        public bool EnableWidget_OverrideForStore { get; set; }
        public bool CalanderAccessRoleSystemName_OverrideForStore { get; set; }

        public bool CanOverlapOtherSessions_OverrideForStore { get; set; } //if "true" => don't check existing sessions, just add. if "false" => create only sessions that don't overlap
        public bool PermittedOverlappingThreshold_OverrideForStore { get; set; } // default - one minute...
        public bool DisabledDays_OverrideForStore { get; set; } //"1,6,7"
        public bool DisabledDates_OverrideForStore { get; set; } //"1/1/2015, 12/22/2019"
        public bool SessionsDayAndTimeRange_OverrideForStore { get; set; } //"1=10:00-14:30, 1=15:30-20:00, 2=09:00-20:30"
        public bool SessionLengthByMinutes_OverrideForStore { get; set; }
        public bool SessionAvailablilityCustomerCount_OverrideForStore { get; set; }
        public bool LastSessionCreationDate_OverrideForStore { get; set; }
        public bool From_OverrideForStore { get; set; }
        public bool To_OverrideForStore { get; set; }
        public bool LastSessionDate_OverrideForStore { get; set; }
    }
}
