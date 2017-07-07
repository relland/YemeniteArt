using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Models.Admin
{
    public class BulkEditSessionListModel
    {
        [NopResourceDisplayName("ya.Calendar.SearchFrom")]
        public DateTime SearchFrom { get; set; }
        [NopResourceDisplayName("ya.Calendar.SearchTo")]
        public DateTime SearchTo { get; set; }
    }
}
