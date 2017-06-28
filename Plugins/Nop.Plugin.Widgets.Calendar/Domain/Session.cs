using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Domain
{
    public partial class Session : BaseEntity
    {
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public DateTime StartsAtUtc { get; set; }
        //public DateTime EndsAtUtc { get; set; }

        public int SessionLengthByMinutes { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool Taken { get; set; }
        public int CustomerId { get; set; }
        public int SessionAvailablilityCustomerCount { get; set; }
        public string CustomerComment { get; set; }
        public string AdminComment { get; set; }

        public string ContactEmail { get; set; }
        public string ContactPhoneNumbet { get; set; }
        public string Language { get; set; }

        public string StringA { get; set; }
        public string StringB { get; set; }
        public string StringC { get; set; }
        public string StringD { get; set; }
        public int IntA { get; set; } //NumberOfSessions
        public int IntB { get; set; } //EachSessionLength
        public int IntC { get; set; } //SessionId
        public int IntD { get; set; }
    }
}
