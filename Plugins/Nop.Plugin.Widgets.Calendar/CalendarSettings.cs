using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar
{
    class CalendarSettings : ISettings
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
    }
}