using Nop.Core;
using Nop.Data.Mapping;
using Nop.Plugin.Widgets.AllInOne.Domain;
using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;

namespace Nop.Plugin.Widgets.AllInOne.Data
{
    public class AllInOneMap : NopEntityTypeConfiguration<AllInOneObject>
    {
        public AllInOneMap()
        {
            base.ToTable("AllInOne");
            base.HasKey<int>((AllInOneObject a) => a.Id);
            base.Property((AllInOneObject a) => a.Name).IsRequired().HasMaxLength(new int?(400));
            base.Property((AllInOneObject a) => a.WidgetZone).IsRequired().HasMaxLength(new int?(100));
        }
    }
}