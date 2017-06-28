using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Data
{
    public class CalendarContext : DbContext, IDbContext
    {
        public CalendarContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        #region Implementation of IDbContext
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
        
        public void Detach(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new SessionMap());
            //modelBuilder.Configurations.Add(new OpenEventMap());

            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public void Install()
        {
            //It's required to set initializer to null (for SQL Server Compact).
            //otherwise, you'll get something like "The model backing the 'your context name' context has changed since the database was created. Consider using Code First Migrations to update the database"
            Database.SetInitializer<CalendarContext>(null);

            Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
            SaveChanges();
        }

        public void Uninstall()
        {
            var dbScript = "DROP TABLE Sessions";
            Database.ExecuteSqlCommand(dbScript);
            //var dbScript2 = "DROP TABLE OpenEvents";
            //Database.ExecuteSqlCommand(dbScript2);
            SaveChanges();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public System.Collections.Generic.IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}
