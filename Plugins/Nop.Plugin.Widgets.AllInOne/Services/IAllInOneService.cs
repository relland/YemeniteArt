using Nop.Core;
using Nop.Plugin.Widgets.AllInOne.Domain;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.AllInOne.Services
{
    public interface IAllInOneService
    {
        void DeleteAllInOne(AllInOneObject allinOne);

        AllInOneObject GetAllInOneById(int allInOneId);

        IPagedList<AllInOneObject> GetAllInOnes(string name = "", int pageIndex = 0, int pageSize = 2147483647);

        IList<AllInOneObject> GetAllInOnesByWidgetZones(string widgetZones);

        IList<string> GetAllInOnesWidgetZones();

        void InsertAllInOne(AllInOneObject allInOne);

        void UpdateAllInOne(AllInOneObject allInOne);
    }
}