using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.Calendar.Models.Admin
{
    public partial class EventModel : BaseNopEntityModel
    {
        public EventModel()
        {
            CustomersList = new List<SelectListItem>();
        }

        [NopResourceDisplayName("admin.calendar.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [NopResourceDisplayName("admin.calendar.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        [NopResourceDisplayName("admin.calendar.StartsAt")]
        public DateTime StartsAt { get; set; }
        [NopResourceDisplayName("admin.calendar.EndsAt")]
        public DateTime EndsAt { get; set; }

        [NopResourceDisplayName("admin.calendar.OpenEventStartsAt")]
        public DateTime OpenEventStartsAt { get; set; }
        [NopResourceDisplayName("admin.calendar.OpenEventEndsAt")]
        public DateTime OpenEventEndsAt { get; set; }

        [NopResourceDisplayName("admin.calendar.Active")]
        public bool Active { get; set; }
        [NopResourceDisplayName("admin.calendar.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("admin.calendar.CustomerId")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("admin.calendar.CustomerComment")]
        public string CustomerComment { get; set; }
        [NopResourceDisplayName("admin.calendar.AdminComment")]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("admin.calendar.ContactEmail")]
        public string ContactEmail { get; set; }
        [NopResourceDisplayName("admin.calendar.ContactPhoneNumbet")]
        public string ContactPhoneNumbet { get; set; }
        [NopResourceDisplayName("admin.calendar.Language")]
        public string Language { get; set; }

        [NopResourceDisplayName("admin.calendar.EventsStringA")]
        public string StringA { get; set; }
        [NopResourceDisplayName("admin.calendar.EventsStringB")]
        public string StringB { get; set; }
        [NopResourceDisplayName("admin.calendar.EventsStringC")]
        public string StringC { get; set; }
        [NopResourceDisplayName("admin.calendar.EventsStringD")]
        public string StringD { get; set; }
        //amount of people
        [NopResourceDisplayName("admin.calendar.EventsIntA")]
        public int IntA { get; set; }
        [NopResourceDisplayName("admin.calendar.EventsIntB")]
        public int IntB { get; set; }
        [NopResourceDisplayName("admin.calendar.EventsIntC")]
        public int IntC { get; set; }
        [NopResourceDisplayName("admin.calendar.EventsIntD")]
        public int IntD { get; set; }

        [NopResourceDisplayName("admin.calendar.CustomersList")]
        public List<SelectListItem> CustomersList { get; set; }

        [NopResourceDisplayName("admin.calendar.OpenEventId")]
        public int OpenEventId { get; set; }

    }
}