using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.AllInOne.Controllers;
using Nop.Plugin.Widgets.AllInOne.Data;
using Nop.Plugin.Widgets.AllInOne.Domain;
using Nop.Plugin.Widgets.AllInOne.Services;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Plugin.Widgets.AllInOne
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public DependencyRegistrar()
        {
        }

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<WidgetsAllInOneController>()
                .WithParameter<WidgetsAllInOneController, ConcreteReflectionActivatorData, SingleRegistrationStyle>(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));

            //RegistrationExtensions.InstancePerHttpRequest<AllInOneService, ConcreteReflectionActivatorData, SingleRegistrationStyle>(builder.RegisterType<AllInOneService>()
            //    .As<IAllInOneService>(), new object[0]);
            //rel
            builder.RegisterType<AllInOneService>().As<IAllInOneService>().InstancePerLifetimeScope();
            //end rel
            this.RegisterPluginDataContext<AllInOneObjectContext>(builder, "nop_object_context_allinone");

            //RegistrationExtensions.InstancePerHttpRequest<EfRepository<AllInOneObject>, ConcreteReflectionActivatorData, SingleRegistrationStyle>(builder.RegisterType<EfRepository<AllInOneObject>>()
            //    .As<IRepository<AllInOneObject>>().WithParameter<EfRepository<AllInOneObject>, ConcreteReflectionActivatorData, SingleRegistrationStyle>(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_allinone")), 
            //    new object[0]);
            //rel
            builder.RegisterType<EfRepository<AllInOneObject>>()
                .As<IRepository<AllInOneObject>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_allinone"))
                .InstancePerLifetimeScope();
            //end rel


            //builder.RegisterType<GoogleService>().As<IGoogleService>().InstancePerLifetimeScope();

            ////data context
            //this.RegisterPluginDataContext<GoogleProductObjectContext>(builder, "nop_object_context_google_product");

            ////override required repository with our custom context
            //builder.RegisterType<EfRepository<GoogleProductRecord>>()
            //    .As<IRepository<GoogleProductRecord>>()
            //    .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_google_product"))
            //    .InstancePerLifetimeScope();
        }
    }
}