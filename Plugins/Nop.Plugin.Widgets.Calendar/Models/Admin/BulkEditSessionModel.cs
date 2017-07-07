using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Models.Admin
{
    public class BulkEditSessionModel
    {
        public int Id { get; set; }

        [NopResourceDisplayName("ya.Calendar.CreatedOnUtc")]
        public string CreatedOnUtc { get; set; }
        [NopResourceDisplayName("ya.Calendar.UpdatedOnUtc")]
        public string UpdatedOnUtc { get; set; }
        [NopResourceDisplayName("ya.Calendar.StartsAtLocalTime")]
        public string StartsAtLocalTime { get; set; }
        [NopResourceDisplayName("ya.Calendar.SessionLengthByMinutes")]
        public int SessionLengthByMinutes { get; set; }
        [NopResourceDisplayName("ya.Calendar.Active")]
        public bool Active { get; set; }
        [NopResourceDisplayName("ya.Calendar.Taken")]
        public bool Taken { get; set; }
        [NopResourceDisplayName("ya.Calendar.TakenAdminOverride")]
        public bool TakenAdminOverride { get; set; }
        [NopResourceDisplayName("ya.Calendar.SessionAvailablilityCustomerCount")]
        public int SessionAvailablilityCustomerCount { get; set; }
    }
}
