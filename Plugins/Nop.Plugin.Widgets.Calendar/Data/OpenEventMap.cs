using Nop.Data.Mapping;
using Nop.Plugin.Widgets.Calendar.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Data
{
    public partial class OpenEventMap : NopEntityTypeConfiguration<OpenEvent>
    {
        public OpenEventMap()
        {
            this.ToTable("OpenEvents");
            this.HasKey(oe => oe.Id);

            this.HasMany(e => e.AppliedEvents)
                .WithMany()
                .Map(m => m.ToTable("OpenEvent_Events"));
        }
    }
}
