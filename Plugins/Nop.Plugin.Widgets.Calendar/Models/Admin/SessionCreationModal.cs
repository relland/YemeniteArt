using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Models.Admin
{
    public class SessionCreationModal
    {
        [NopResourceDisplayName("ya.Calendar.From")]
        public DateTime From { get; set; }
        [NopResourceDisplayName("ya.Calendar.To")]
        public DateTime To { get; set; }
        [NopResourceDisplayName("ya.Calendar.SuHours")]
        public string SuHours { get; set; }//"10:00,15:30,09:00"
        [NopResourceDisplayName("ya.Calendar.MoHours")]
        public string MoHours { get; set; }
        [NopResourceDisplayName("ya.Calendar.TuHours")]
        public string TuHours { get; set; }
        [NopResourceDisplayName("ya.Calendar.WeHours")]
        public string WeHours { get; set; }
        [NopResourceDisplayName("ya.Calendar.ThHours")]
        public string ThHours { get; set; }
        [NopResourceDisplayName("ya.Calendar.FrHours")]
        public string FrHours { get; set; }
        [NopResourceDisplayName("ya.Calendar.SaHours")]
        public string SaHours { get; set; }
        [NopResourceDisplayName("ya.Calendar.SessionLengthByMinutes")]
        public int SessionLengthByMinutes { get; set; }
        [NopResourceDisplayName("ya.Calendar.SessionAvailablilityCustomerCount")]
        public int SessionAvailablilityCustomerCount { get; set; }
    }
}
