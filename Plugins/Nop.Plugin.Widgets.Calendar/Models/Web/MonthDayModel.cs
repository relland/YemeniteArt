using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.Calendar.Models.Web
{
    public partial class MonthDayModel : BaseNopModel
    {
        public MonthDayModel()
        {
            OpenEvents = new List<OpenEventModel>();
            FrontDisplayOpenEvents = new List<FrontDisplayOpenEventsModel>();
        }
        public int DayOfMonth { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public bool HasOpenEvents { get; set; }
        public List<OpenEventModel> OpenEvents { get; set; }
        public List<FrontDisplayOpenEventsModel> FrontDisplayOpenEvents { get; set; }
        public int FromHour { get; set; }
        public int ToHour { get; set; }
        public string DayName { get; set; }
        public string MonthName { get; set; }
        public int Year14 { get; set; }

        public AddEventModel AddEvent { get; set; }
    }

    public partial class OpenEventModel : BaseNopModel
    {
        public OpenEventModel()
        {
            Events = new List<EventModel>();
        }
        public List<EventModel> Events { get; set; }

        public DateTime StartsAt { get; set; }
        public int NumberOfSessions { get; set; }
        public int EachSessionLength { get; set; }
        public List<int> MySessions { get; set; }
        public DateTime EndsAt { get; set; }//remove

        public int FrontId { get; set; }//number of quorter hours

    }

    public partial class FrontDisplayOpenEventsModel : BaseNopModel
    {
        public bool HasOpenEvent { get; set; }
        public bool HasEvent { get; set; }
        public bool MyEvent { get; set; }
        public int DisplayOrder { get; set; }
    }

    public partial class EventModel : BaseNopModel
    {
        public DateTime StartsAt { get; set; }
        public int SessionLength { get; set; }
        public bool MyEvent { get; set; }
        public string AdminComment { get; set; }
        public string CustomerComment { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Language { get; set; }

        public int FrontId { get; set; }
    }

    public partial class AddEventModel : BaseNopModel
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        [NopResourceDisplayName("rel.AddEventModel.FromHour")]
        [AllowHtml]
        public int FromHour { get; set; }
        [NopResourceDisplayName("rel.AddEventModel.FromMinute")]
        [AllowHtml]
        public int FromMinute { get; set; }
        [NopResourceDisplayName("rel.AddEventModel.ToHour")]
        [AllowHtml]
        public int ToHour { get; set; }
        [NopResourceDisplayName("rel.AddEventModel.ToMinute")]
        [AllowHtml]
        public int ToMinute { get; set; }
        [NopResourceDisplayName("rel.AddEventModel.AvailabeleHours")]
        [AllowHtml]
        public string AvailabeleHours { get; set; }

        public int MinHour { get; set; }
        public int MaxHour { get; set; }
        public int MinMinute { get; set; }
        public int MaxMinute { get; set; }

        [NopResourceDisplayName("rel.AddEventModel.CustomerComment")]
        [AllowHtml]
        public string CustomerComment { get; set; }
        [NopResourceDisplayName("rel.AddEventModel.Language")]
        [AllowHtml]
        public string Language { get; set; }
        [NopResourceDisplayName("rel.AddEventModel.Phone")]
        [AllowHtml]
        public string Phone { get; set; }
    }
}