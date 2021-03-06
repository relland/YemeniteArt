﻿using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.Calendar.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Calendar.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IEventPublisher _eventPublisher;

        public CalendarService(IRepository<Session> sessionRepository, IEventPublisher eventPublisher)
        {
            _sessionRepository = sessionRepository;
            _eventPublisher = eventPublisher;
        }
        
        public virtual Session GetSessionById(int id)
        {
            var session = _sessionRepository.GetById(id);
            return session;
        }

        public virtual IList<Session> GetAllSessionsInCurrentDay(DateTime date = new DateTime())
        {
            if (date == DateTime.MinValue)
                date = DateTime.UtcNow;
            var intDate = date.ToIntDate();
            var query = _sessionRepository.Table;
            query = query.Where(x => x.Active && !x.Deleted && x.IntDate == intDate);
            return query.ToList();
        }

        public IList<Session> GetSessions(DateTime from, DateTime to)
        {
            if (from == DateTime.MinValue)
                from = DateTime.UtcNow;
            if (to == DateTime.MinValue)
                to = from.AddMonths(1);
            var intFrom = from.ToIntDate();
            var intTo = to.ToIntDate();
            var query = _sessionRepository.Table;
            query = query.Where(x => x.Active && !x.Deleted && x.IntDate >= intFrom && x.IntDate <= intTo);
            return query.ToList();
        }
        public virtual void InsertSession(Session session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            _sessionRepository.Insert(session);

            //event notification
            _eventPublisher.EntityInserted(session);
        }

        public virtual void InsertSessions(List<Session> sessions)
        {

            if (sessions.Count() == 0)
                throw new ArgumentNullException("sessions");

            _sessionRepository.Insert(sessions);

            foreach (var session in sessions)
            //event notification
                _eventPublisher.EntityInserted(session);
        }
        public virtual void UpdateSession(Session session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            _sessionRepository.Update(session);

            //event notification
            _eventPublisher.EntityUpdated(session);
        }

        public virtual void DeleteSession(Session session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            _sessionRepository.Delete(session);

            //event notification
            _eventPublisher.EntityDeleted(session);
        }        
    }
}
