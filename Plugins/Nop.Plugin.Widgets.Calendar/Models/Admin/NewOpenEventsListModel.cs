using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Models.Admin
{
    public class NewOpenEventsListModel : BaseNopModel
    {
        public NewOpenEventsListModel()
        {
            MonthDays = new List<NewMonthDayModel>();
        }
        public List<NewMonthDayModel> MonthDays { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }
        public int NumOfDays { get; set; }
        public int DayOfWeek { get; set; }
        public int EmptyDays { get; set; }
    }

    public partial class NewMonthDayModel : BaseNopModel
    {
        public NewMonthDayModel()
        {
            OpenEvents = new List<NewOpenEventModel>();
            FrontDisplayOpenEvents = new List<FrontDisplayOpenEventsModel>();
        }
        public int DayOfMonth { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public bool HasOpenEvents { get; set; }
        public List<NewOpenEventModel> OpenEvents { get; set; }
        public List<FrontDisplayOpenEventsModel> FrontDisplayOpenEvents { get; set; }
        public int FromHour { get; set; }
        public int ToHour { get; set; }
        public string DayName { get; set; }
        public string MonthName { get; set; }
        public int Year14 { get; set; }
    }

    public partial class NewOpenEventModel : BaseNopModel
    {
        public NewOpenEventModel()
        {
            Events = new List<NewEventModel>();
        }
        public List<NewEventModel> Events { get; set; }

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

    public partial class NewEventModel : BaseNopModel
    {
        public int Id { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public int SessionLength { get; set; }
        public bool MyEvent { get; set; }
        public string AdminComment { get; set; }
        public string CustomerComment { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Language { get; set; }
        public string CustomerName { get; set; }

        public int FrontId { get; set; }
    }
}