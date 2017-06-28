using Nop.Core.Infrastructure;
using System;
using System.Data.Entity;

namespace Nop.Plugin.Widgets.AllInOne.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public int Order
        {
            get
            {
                return 0;
            }
        }

        public EfStartUpTask()
        {
        }

        public void Execute()
        {
            Database.SetInitializer<AllInOneObjectContext>(null);
        }
    }
}