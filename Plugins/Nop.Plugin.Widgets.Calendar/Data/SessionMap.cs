using Nop.Plugin.Widgets.Calendar.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Data
{
    public class SessionMap : NopEntityTypeConfiguration<Session>
    {
        public SessionMap()
        {
            this.ToTable("Sessions");
            this.HasKey(e => e.Id);
        }
    }
}
