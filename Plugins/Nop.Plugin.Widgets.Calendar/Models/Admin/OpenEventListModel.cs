using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Models.Admin
{
    public partial class OpenEventListModel : BaseNopEntityModel
    {
        public OpenEventListModel()
        {
            Events = new List<EventModel>();
        }

        public List<EventModel> Events { get; set; }

        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }

        public string StartsAt { get; set; }
        public string EndsAt { get; set; }

        public bool Active { get; set; }
        public bool Deleted { get; set; }

        public string StringA { get; set; }
        public string StringB { get; set; }
        public string StringC { get; set; }
        public string StringD { get; set; }
        //public int EventsSimultaneously { get; set; }
        public int IntA { get; set; }
        public int IntB { get; set; }
        public int IntC { get; set; }
        public int IntD { get; set; }
    }
}