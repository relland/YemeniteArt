using Nop.Core;
using Nop.Data;
using Nop.Plugin.Widgets.AllInOne.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Nop.Plugin.Widgets.AllInOne.Data
{
    public class AllInOneObjectContext : DbContext, IDbContext
    {
        public virtual bool AutoDetectChangesEnabled
        {
            get
            {
                return base.Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                base.Configuration.AutoDetectChangesEnabled = value;
            }
        }

        public virtual bool ProxyCreationEnabled
        {
            get
            {
                return base.Configuration.ProxyCreationEnabled;
            }
            set
            {
                base.Configuration.ProxyCreationEnabled = value;
            }
        }

        public AllInOneObjectContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public void Detach(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
        where TEntity : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public void Install()
        {
            string str = this.CreateDatabaseScript();
            base.Database.ExecuteSqlCommand(str, new object[0]);
            this.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add<AllInOneObject>(new AllInOneMap());
            base.OnModelCreating(modelBuilder);
        }

        public new IDbSet<TEntity> Set<TEntity>()
        where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Uninstall()
        {
            this.DropPluginTable("AllInOne");
        }
    }
}