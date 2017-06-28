using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.Calendar.Data;
using Nop.Plugin.Widgets.Calendar.Domain;
using Nop.Plugin.Widgets.Calendar.Services;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_calander";

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CalendarService>().As<ICalendarService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<CalendarContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<Session>>()
                .As<IRepository<Session>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<OpenEvent>>()
                .As<IRepository<OpenEvent>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
