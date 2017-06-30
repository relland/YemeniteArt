using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Domain
{
    public partial class Session : BaseEntity
    {
        public Session(DateTime startDate, int sessionAvailablilityCustomerCount, int sessionLengthByMinutes)
        {
            IntDate = startDate.ToIntDate();
            CreatedOnUtc = DateTime.UtcNow;
            UpdatedOnUtc = DateTime.UtcNow;
            StartsAtUtc = startDate;
            Active = true;
            SessionAvailablilityCustomerCount = sessionAvailablilityCustomerCount;
            SessionLengthByMinutes = sessionLengthByMinutes;
        }
        [Index]
        public int IntDate { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public DateTime StartsAtUtc { get; set; }
        //public DateTime EndsAtUtc { get; set; }

        public int SessionLengthByMinutes { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool Taken { get; set; }
        public int SessionAvailablilityCustomerCount { get; set; }

        public string StringA { get; set; }
        public string StringB { get; set; }
        public string StringC { get; set; }
        public string StringD { get; set; }
        public int IntA { get; set; } //NumberOfSessions
        public int IntB { get; set; } //EachSessionLength
        public int IntC { get; set; } //SessionId
        public int IntD { get; set; }
    }

    public static class IntDate
    {
        public static int ToIntDate(this DateTime date)
        {
            return date.Year * 10000 + date.Month * 100 + date.Day;
        }
    }
}
