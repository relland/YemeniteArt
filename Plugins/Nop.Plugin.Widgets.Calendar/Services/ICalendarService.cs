using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.Calendar.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Services
{
    public interface ICalendarService
    {
        Session GetSessionById(int id);

        IList<Session> GetAllSessionsInCurrentDay(DateTime date = new DateTime());

        IList<Session> GetSessions(DateTime from, DateTime to);
        void InsertSession(Session session);
        void InsertSessions(List<Session> sessions);

        void UpdateSession(Session session);

        void DeleteSession(Session session);
    }
}
