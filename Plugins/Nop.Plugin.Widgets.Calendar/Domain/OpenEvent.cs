using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Domain
{
    public partial class OpenEvent : BaseEntity
    {
        private ICollection<Session> _appliedEvents;

        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public DateTime StartsAt { get; set; }//Utc
        public DateTime EndsAt { get; set; }//Utc
        //public int NumberOfSessions { get; set; }
        //public int EachSessionLength { get; set; }

        public bool Active { get; set; }
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the collection of applied Events
        /// </summary>
        public virtual ICollection<Session> AppliedEvents
        {
            get { return _appliedEvents ?? (_appliedEvents = new List<Session>()); }
            protected set { _appliedEvents = value; }
        }

        public string StringA { get; set; }
        public string StringB { get; set; }
        public string StringC { get; set; }
        public string StringD { get; set; }
        public int IntA { get; set; } //NumberOfSessions
        public int IntB { get; set; } //EachSessionLength
        public int IntC { get; set; }
        public int IntD { get; set; }
    }
}
