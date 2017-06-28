using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Widgets.AllInOne.Domain;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Nop.Plugin.Widgets.AllInOne.Services
{
    public class AllInOneService : IAllInOneService
    {
        private const string ALLINONES_BY_ID_KEY = "Nop.allinone.id-{0}";

        private const string ALLINONES_PATTERN_KEY = "Nop.allinone.";

        private readonly IRepository<AllInOneObject> _allInOneRepository;

        private readonly IRepository<AclRecord> _aclRepository;

        private readonly IRepository<StoreMapping> _storeMappingRepository;

        private readonly IWorkContext _workContext;

        private readonly IStoreContext _storeContext;

        private readonly IEventPublisher _eventPublisher;

        private readonly ICacheManager _cacheManager;

        private readonly IStoreMappingService _storeMappingService;

        private readonly IAclService _aclService;

        public AllInOneService(ICacheManager cacheManager, IRepository<AllInOneObject> allInOneRepository, IRepository<AclRecord> aclRepository, IRepository<StoreMapping> storeMappingRepository, IWorkContext workContext, IStoreContext storeContext, IEventPublisher eventPublisher, IStoreMappingService storeMappingService, IAclService aclService)
        {
            this._cacheManager = cacheManager;
            this._allInOneRepository = allInOneRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
        }

        public virtual void DeleteAllInOne(AllInOneObject allInOne)
        {
            if (allInOne == null)
            {
                throw new ArgumentNullException("allInOne");
            }
            this._allInOneRepository.Delete(allInOne);
            this._eventPublisher.EntityDeleted<AllInOneObject>(allInOne);
        }

        public virtual AllInOneObject GetAllInOneById(int allInOneId)
        {
            AllInOneObject allInOneObject;
            if (allInOneId != 0)
            {
                string str = string.Format("Nop.allinone.id-{0}", allInOneId);
                allInOneObject = this._cacheManager.Get<AllInOneObject>(str, () => this._allInOneRepository.GetById(allInOneId));
            }
            else
            {
                allInOneObject = null;
            }
            return allInOneObject;
        }

        public virtual IPagedList<AllInOneObject> GetAllInOnes(string name = "", int pageIndex = 0, int pageSize = 2147483647)
        {
            IQueryable<AllInOneObject> table = this._allInOneRepository.Table;
            if (!string.IsNullOrWhiteSpace(name))
            {
                table =
                    from s in table
                    where s.Name.Contains(name)
                    select s;
            }
            table =
                from s in table
                orderby s.DisplayOrder
                select s;
            return new PagedList<AllInOneObject>(table.ToList<AllInOneObject>(), pageIndex, pageSize);
        }

        public virtual IList<AllInOneObject> GetAllInOnesByWidgetZones(string widgetZones)
        {
            IOrderedQueryable<AllInOneObject> table =
                from s in this._allInOneRepository.Table
                where s.WidgetZone == widgetZones && s.Published
                orderby s.DisplayOrder
                select s;
            return table.ToList<AllInOneObject>();
        }

        public virtual IList<string> GetAllInOnesWidgetZones()
        {
            string[] array = (
                from x in this._allInOneRepository.Table
                where x.Published
                select x.WidgetZone).Distinct<string>().ToArray<string>();
            return array;
        }

        public virtual void InsertAllInOne(AllInOneObject allInOne)
        {
            if (allInOne == null)
            {
                throw new ArgumentNullException("allInOne");
            }
            this._allInOneRepository.Insert(allInOne);
            this._eventPublisher.EntityInserted<AllInOneObject>(allInOne);
        }

        public virtual void UpdateAllInOne(AllInOneObject allInOne)
        {
            if (allInOne == null)
            {
                throw new ArgumentNullException("allInOne");
            }
            this._allInOneRepository.Update(allInOne);
            this._eventPublisher.EntityUpdated<AllInOneObject>(allInOne);
        }
    }
}