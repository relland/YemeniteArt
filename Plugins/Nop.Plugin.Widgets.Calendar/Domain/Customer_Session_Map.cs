using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Domain
{
    public class Customer_Session_Map : BaseEntity
    {
        public int CustomerId { get; set; }
        public int SessionId { get; set; }
        public bool Deleted { get; set; }
        public string CustomerComment { get; set; }
        public string AdminComment { get; set; }

        public string ContactEmail { get; set; }
        public string ContactPhoneNumbet { get; set; }
        public string Language { get; set; }
    }
}
