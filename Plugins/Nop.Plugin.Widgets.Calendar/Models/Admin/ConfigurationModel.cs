using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Models.Admin
{
    public class ConfigurationModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("ya.Calendar.EnableWidget")]
        public bool EnableWidget { get; set; }
        [NopResourceDisplayName("ya.Calendar.CalanderAccessRoleSystemName")]
        public string CalanderAccessRoleSystemName { get; set; }

        [NopResourceDisplayName("ya.Calendar.CanOverlapOtherSessions")]
        public bool CanOverlapOtherSessions { get; set; } //if "true" => don't check existing sessions, just add. if "false" => create only sessions that don't overlap

        [NopResourceDisplayName("ya.Calendar.PermittedOverlappingThreshold")]
        public int PermittedOverlappingThreshold { get; set; } // default - one minute...

        [NopResourceDisplayName("ya.Calendar.DisabledDays")]
        public string DisabledDays { get; set; } //"1,6,7"

        [NopResourceDisplayName("ya.Calendar.DisabledDates")]
        public string DisabledDates { get; set; } //"1/1/2015, 12/22/2019"

        [NopResourceDisplayName("ya.Calendar.SessionsDayAndTimeRange")]
        public string SessionsDayAndTimeRange { get; set; } //"1=10:00-14:30, 1=15:30-20:00, 2=09:00-20:30"

        [NopResourceDisplayName("ya.Calendar.SessionLengthByMinutes")]
        public int SessionLengthByMinutes { get; set; }

        [NopResourceDisplayName("ya.Calendar.SessionAvailablilityCustomerCount")]
        public int SessionAvailablilityCustomerCount { get; set; }

        [NopResourceDisplayName("ya.Calendar.LastSessionCreationDate")]
        public DateTime? LastSessionCreationDate { get; set; }

        [NopResourceDisplayName("ya.Calendar.From")]
        public DateTime? From { get; set; }

        [NopResourceDisplayName("ya.Calendar.To")]
        public DateTime? To { get; set; }

        [NopResourceDisplayName("ya.Calendar.LastSessionDate")]
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
