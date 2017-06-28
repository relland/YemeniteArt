using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Models.Admin
{
    public partial class OpenEventModel : BaseNopEntityModel
    {
        public OpenEventModel()
        {
            Events = new List<EventModel>();
        }

        [NopResourceDisplayName("admin.calendar.Events")]
        public List<EventModel> Events { get; set; }
        [NopResourceDisplayName("admin.calendar.EventsCount")]
        public int EventsCount { get; set; }

        [NopResourceDisplayName("admin.calendar.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [NopResourceDisplayName("admin.calendar.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        [NopResourceDisplayName("admin.calendar.StartsAt")]
        public DateTime StartsAt { get; set; }
        [NopResourceDisplayName("admin.calendar.EndsAt")]
        public DateTime EndsAt { get; set; }

        [NopResourceDisplayName("admin.calendar.Active")]
        public bool Active { get; set; }
        [NopResourceDisplayName("admin.calendar.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("admin.calendar.StringA")]
        public string StringA { get; set; }
        [NopResourceDisplayName("admin.calendar.StringB")]
        public string StringB { get; set; }
        [NopResourceDisplayName("admin.calendar.StringC")]
        public string StringC { get; set; }
        [NopResourceDisplayName("admin.calendar.StringD")]
        public string StringD { get; set; }

        //public int EventsSimultaneously { get; set; }
        [NopResourceDisplayName("admin.calendar.IntA")]
        public int IntA { get; set; }
        [NopResourceDisplayName("admin.calendar.IntB")]
        public int IntB { get; set; }
        [NopResourceDisplayName("admin.calendar.IntC")]
        public int IntC { get; set; }
        [NopResourceDisplayName("admin.calendar.IntD")]
        public int IntD { get; set; }

        //rel new
        [NopResourceDisplayName("admin.calendar.StartsAtHour")]
        public int StartsAtHour { get; set; }
        [NopResourceDisplayName("admin.calendar.StartsAtMinutes")]
        public int StartsAtMinutes { get; set; }
        [NopResourceDisplayName("admin.calendar.NumberOfSessions")]
        public int NumberOfSessions { get; set; }
        [NopResourceDisplayName("admin.calendar.SessionLength")]
        public int SessionLength { get; set; }
        [NopResourceDisplayName("admin.calendar.BookedSessionsIndex")]
        public List<int> BookedSessionsIndex { get; set; }

        [NopResourceDisplayName("admin.calendar.AllowedFromHour")]
        public int AllowedFromHour { get; set; }
        [NopResourceDisplayName("admin.calendar.AllowedToHour")]
        public int AllowedToHour { get; set; }
    }
}