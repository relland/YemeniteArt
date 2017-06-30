using Nop.Admin.Controllers;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Widgets.Calendar.Domain;
using Nop.Plugin.Widgets.Calendar.Models.Admin;
using Nop.Plugin.Widgets.Calendar.Services;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.Calendar.Controllers
{
    public class CalanderAdminController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerReportService _customerReportService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly CustomerSettings _customerSettings;
        private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        private readonly IVendorService _vendorService;
        private readonly IStoreContext _storeContext;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderService _orderService;
        private readonly IExportManager _exportManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPermissionService _permissionService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ForumSettings _forumSettings;
        private readonly IForumService _forumService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly AddressSettings _addressSettings;
        private readonly IStoreService _storeService;
        private readonly ICalendarService _calanderService;
        private readonly StoreInformationSettings _storeInformationSettings;

        private readonly ISettingService _settingService;

        #endregion

        #region Constructors

        public CalanderAdminController(ICustomerService customerService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerReportService customerReportService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            RewardPointsSettings rewardPointsSettings,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IAddressService addressService,
            CustomerSettings customerSettings,
            ITaxService taxService,
            IWorkContext workContext,
            IVendorService vendorService,
            IStoreContext storeContext,
            IPriceFormatter priceFormatter,
            IOrderService orderService,
            IExportManager exportManager,
            ICustomerActivityService customerActivityService,
            IPriceCalculationService priceCalculationService,
            IPermissionService permissionService,
            IQueuedEmailService queuedEmailService,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            ForumSettings forumSettings,
            IForumService forumService,
            IOpenAuthenticationService openAuthenticationService,
            AddressSettings addressSettings,
            IStoreService storeService,
            ICalendarService calanderService,
            StoreInformationSettings storeInformationSettings)
        {
            this._customerService = customerService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._customerReportService = customerReportService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._customerSettings = customerSettings;
            this._taxService = taxService;
            this._workContext = workContext;
            this._vendorService = vendorService;
            this._storeContext = storeContext;
            this._priceFormatter = priceFormatter;
            this._orderService = orderService;
            this._exportManager = exportManager;
            this._customerActivityService = customerActivityService;
            this._priceCalculationService = priceCalculationService;
            this._permissionService = permissionService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailAccountService = emailAccountService;
            this._forumSettings = forumSettings;
            this._forumService = forumService;
            this._openAuthenticationService = openAuthenticationService;
            this._addressSettings = addressSettings;
            this._storeService = storeService;
            this._calanderService = calanderService;
            this._storeInformationSettings = storeInformationSettings;
        }

        #endregion

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var calendarSettings = _settingService.LoadSetting<CalendarSettings>(storeScope);
            var model = new ConfigurationModel()
            {
                CalanderAccessRoleSystemName = calendarSettings.CalanderAccessRoleSystemName,
                CanOverlapOtherSessions = calendarSettings.CanOverlapOtherSessions,
                DisabledDates = calendarSettings.DisabledDates,
                EnableWidget = calendarSettings.EnableWidget,
                SessionAvailablilityCustomerCount = calendarSettings.SessionAvailablilityCustomerCount,
                SessionsDayAndTimeRange = calendarSettings.SessionsDayAndTimeRange,
                From = calendarSettings.From,
                To = calendarSettings.To,
                LastSessionCreationDate = calendarSettings.LastSessionCreationDate,
                LastSessionDate = calendarSettings.LastSessionDate,
                PermittedOverlappingThreshold = calendarSettings.PermittedOverlappingThreshold,
                SessionLengthByMinutes = calendarSettings.SessionLengthByMinutes
            };
            if (storeScope > 0)
            {
                model.EnableWidget_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.EnableWidget, storeScope);
                model.CalanderAccessRoleSystemName_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.CalanderAccessRoleSystemName, storeScope);
                model.CanOverlapOtherSessions_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.CanOverlapOtherSessions, storeScope);
                model.PermittedOverlappingThreshold_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.PermittedOverlappingThreshold, storeScope);
                model.DisabledDays_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.DisabledDays, storeScope);
                model.DisabledDates_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.DisabledDates, storeScope);
                model.SessionsDayAndTimeRange_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.SessionsDayAndTimeRange, storeScope);
                model.SessionLengthByMinutes_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.SessionLengthByMinutes, storeScope);
                model.SessionAvailablilityCustomerCount_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.SessionAvailablilityCustomerCount, storeScope);
                model.LastSessionCreationDate_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.LastSessionCreationDate, storeScope);
                model.From_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.From, storeScope);
                model.To_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.To, storeScope);
                model.LastSessionDate_OverrideForStore = _settingService.SettingExists(calendarSettings, x => x.LastSessionDate, storeScope);
            }
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var calendarSettings = _settingService.LoadSetting<CalendarSettings>(storeScope);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.EnableWidget, model.EnableWidget_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.CalanderAccessRoleSystemName, model.CalanderAccessRoleSystemName_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.CanOverlapOtherSessions, model.CanOverlapOtherSessions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.PermittedOverlappingThreshold, model.PermittedOverlappingThreshold_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.DisabledDays, model.DisabledDays_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.DisabledDates, model.DisabledDates_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.SessionsDayAndTimeRange, model.SessionsDayAndTimeRange_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.SessionLengthByMinutes, model.SessionLengthByMinutes_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.SessionAvailablilityCustomerCount, model.SessionAvailablilityCustomerCount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.LastSessionCreationDate, model.LastSessionCreationDate_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.From, model.From_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.To, model.To_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(calendarSettings, x => x.LastSessionDate, model.LastSessionDate_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        [HttpPost]
        public ActionResult AddSessions(SessionCreationModal model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var calendarSettings = _settingService.LoadSetting<CalendarSettings>(storeScope);
            if (ModelState.IsValid)
            {
                var sessions = new List<Session>();
                sessions.AddRange(model.SuHours.AddSessions(DayOfWeek.Sunday, model.From, model.To, model.SessionLengthByMinutes, model.SessionAvailablilityCustomerCount));
                sessions.AddRange(model.MoHours.AddSessions(DayOfWeek.Monday, model.From, model.To, model.SessionLengthByMinutes, model.SessionAvailablilityCustomerCount));
                sessions.AddRange(model.TuHours.AddSessions(DayOfWeek.Tuesday, model.From, model.To, model.SessionLengthByMinutes, model.SessionAvailablilityCustomerCount));
                sessions.AddRange(model.WeHours.AddSessions(DayOfWeek.Wednesday, model.From, model.To, model.SessionLengthByMinutes, model.SessionAvailablilityCustomerCount));
                sessions.AddRange(model.ThHours.AddSessions(DayOfWeek.Thursday, model.From, model.To, model.SessionLengthByMinutes, model.SessionAvailablilityCustomerCount));
                sessions.AddRange(model.FrHours.AddSessions(DayOfWeek.Friday, model.From, model.To, model.SessionLengthByMinutes, model.SessionAvailablilityCustomerCount));
                sessions.AddRange(model.SaHours.AddSessions(DayOfWeek.Saturday, model.From, model.To, model.SessionLengthByMinutes, model.SessionAvailablilityCustomerCount));
                _calanderService.InsertSessions(sessions);
            }

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        //[NonAction]
        //private List<EventModel> PrepareEventsModel(ICollection<Session> events)
        //{
        //    var appliedEvents = new List<EventModel>();
        //    if (events != null || events.Count > 0)
        //    {
        //        foreach (var evnt in events)
        //        {
        //            var appliedEventModel = new EventModel()
        //            {
        //                Id = evnt.Id,
        //                Active = evnt.Active,
        //                AdminComment = evnt.AdminComment,
        //                ContactEmail = evnt.ContactEmail,
        //                ContactPhoneNumbet = evnt.ContactPhoneNumbet,
        //                CustomerComment = evnt.CustomerComment,
        //                CreatedOn = evnt.CreatedOnUtc,
        //                Deleted = evnt.Deleted,
        //                //EndsAt = evnt.EndsAtUtc,
        //                Language = evnt.Language,
        //                StartsAt = evnt.StartsAtUtc,
        //                UpdatedOn = evnt.UpdatedOnUtc,
        //                IntA = evnt.IntA,
        //                IntB = evnt.IntB,
        //                IntC = evnt.IntC,
        //                IntD = evnt.IntD,
        //                StringA = evnt.StringA,
        //                StringB = evnt.StringB,
        //                StringC = evnt.StringC,
        //                StringD = evnt.StringD,
        //                CustomerId = evnt.CustomerId
        //            };
        //            appliedEvents.Add(appliedEventModel);
        //        }
        //    }
        //    return appliedEvents;
        //}

        //[NonAction]
        //protected OpenEventModel PrepareOpenEventsModelForList(OpenEvent openEvent)
        //{
        //    var model = new OpenEventModel()
        //    {
        //        Id = openEvent.Id,
        //        Active = openEvent.Active,
        //        CreatedOn = openEvent.CreatedOnUtc,
        //        Deleted = openEvent.Deleted,
        //        UpdatedOn = openEvent.UpdatedOnUtc,
        //        EndsAt = openEvent.EndsAt,
        //        StartsAt = openEvent.StartsAt,
        //        IntA = openEvent.IntA,
        //        IntB = openEvent.IntB,
        //        IntC = openEvent.IntC,
        //        IntD = openEvent.IntD,
        //        StringA = openEvent.StringA,
        //        StringB = openEvent.StringB,
        //        StringC = openEvent.StringC,
        //        StringD = openEvent.StringD,
        //        Events = PrepareEventsModel(_calanderService.GetEventsByOpenEventId(openEvent.Id))
        //    };
        //    model.EventsCount = model.Events.Count;
        //    return model;
        //}

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //public ActionResult List(int year = 0, int month = 0)
        //{
        //    var d = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        //    if (month > 0 && month < 13 && year > 0)
        //        d = new DateTime(year, month, 1);


        //    var model = new NewOpenEventsListModel();
        //    PrepareOpenEventsModel(model);
        //    model.DayOfWeek = (int)d.DayOfWeek;// zero based...
        //    model.NumOfDays = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
        //    model.EmptyDays = model.DayOfWeek - 1;
        //    model.MonthName = d.ToString("MMM", CultureInfo.CurrentCulture);// .InvariantCulture);
        //    model.Month = d.Month;
        //    model.Year = d.Year;
        //    //model.DayOfWeek = (int)d.DayOfWeek;// zero based...
        //    //model.NumOfDays = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
        //    //model.MonthDays = new List<MonthDayModel>();
        //    //var openEvents = _eventService.GetAllOpenEvents(d).OrderBy(x => x.StartsAt);
        //    //var day = 1;
        //    //foreach (var openEvent in openEvents)
        //    //{
        //    //    if (openEvent.StartsAt.Day < day)
        //    //        day = openEvent.StartsAt.Day;
        //    //    while (openEvent.StartsAt.Day > day)
        //    //    {
        //    //        var monthDay = new MonthDayModel()
        //    //        {
        //    //            Day = day,
        //    //            Month = d.Month,
        //    //            Year = d.Year,
        //    //            HasOpenEvents = false,
        //    //            HasBookedOpenEvents = false
        //    //        };
        //    //        model.MonthDays.Add(monthDay);
        //    //        day++;
        //    //    }
        //    //    var monthDay2 = new MonthDayModel() 
        //    //    { 
        //    //        Day = openEvent.StartsAt.Day,
        //    //        Month = openEvent.StartsAt.Month,
        //    //        Year = openEvent.StartsAt.Year,
        //    //        HasOpenEvents = true,
        //    //        HasBookedOpenEvents = openEvent.AppliedEvents.Count > 0
        //    //    };
        //    //    model.MonthDays.Add(monthDay2);
        //    //    day++;
        //    //}
        //    //var daysInMonth = DateTime.DaysInMonth(d.Year, d.Month);
        //    //if (daysInMonth > day)
        //    //{
        //    //    while (day <= daysInMonth)
        //    //    {
        //    //        var monthDay = new MonthDayModel()
        //    //        {
        //    //            Day = day,
        //    //            Month = d.Month,
        //    //            Year = d.Year,
        //    //            HasOpenEvents = false,
        //    //            HasBookedOpenEvents = false
        //    //        };
        //    //        model.MonthDays.Add(monthDay);
        //    //        day++;
        //    //    }
        //    //}
        //    var fromHour = _storeInformationSettings.OpenEventsFromHour;
        //    var toHour = _storeInformationSettings.OpenEventsToHour;

        //    model.MonthDays = new List<NewMonthDayModel>();
        //    //var currentMonthFirstDay = new DateTime(year, month, 1);
        //    //var totalMonthDays = DateTime.DaysInMonth(year, month);
        //    //for (var i = 1; i <= totalMonthDays; i++)
        //    //{
        //    //    var mdModel = GetMonthDayModel(year, month, i, fromHour, toHour);
        //    //    model.Add(mdModel);
        //    //}
        //    //ViewBag.FirstDayOfWeek = (int)currentMonthFirstDay.DayOfWeek;
        //    //ViewBag.TotalMonthDays = totalMonthDays;
        //    //ViewBag.TotalHoursInDay = toHour - fromHour + 1;
        //    //ViewBag.FromHour = fromHour;
        //    //ViewBag.ToHour = toHour;
        //    return View(model);
        //}

        //[NonAction]
        //private void PrepareOpenEventsModel(NewOpenEventsListModel model, int month = 0, int year = 0)
        //{
        //    if (month == 0 && year == 0)
        //    {
        //        month = DateTime.UtcNow.Month;
        //        year = DateTime.UtcNow.Year;
        //    }
        //    var date = new DateTime(year, month, 1);

        //    var totalMonthDays = DateTime.DaysInMonth(year, month);

        //    for (var day = 1; day <= totalMonthDays; day++)
        //    {
        //        var monthDay = new NewMonthDayModel()
        //        {
        //            DayOfMonth = day,
        //            Month = month,
        //            Year = year,
        //        };
        //        var openEvents = _calanderService.GetAllOpenEvents(date);
        //        monthDay.HasOpenEvents = openEvents.Count > 0;
        //        foreach (var oe in openEvents)
        //        {
        //            var oem = new NewOpenEventModel()
        //            {
        //                StartsAt = oe.StartsAt,
        //                NumberOfSessions = oe.IntA,
        //                EachSessionLength = oe.IntB,
        //                //MySessions = _eventService.GetCustomerSessionsForCurrentOpenEvent(customer, oe)
        //            };
        //            monthDay.OpenEvents.Add(oem);
        //        }
        //        model.MonthDays.Add(monthDay);
        //    }
        //}

        ////rel - ajax
        //public ActionResult MonthEvents(int year, int month)
        //{
        //    //tourguide role
        //    //var toureguideRole = _customerService.GetCustomerRoleBySystemName("TourGuide");
        //    //var customer = _workContext.CurrentCustomer;

        //    var fromHour = _storeInformationSettings.OpenEventsFromHour;
        //    var toHour = _storeInformationSettings.OpenEventsToHour;

        //    var model = new List<NewMonthDayModel>();
        //    var currentMonthFirstDay = new DateTime(year, month, 1);
        //    var totalMonthDays = DateTime.DaysInMonth(year, month);
        //    for (var i = 1; i <= totalMonthDays; i++)
        //    {
        //        var mdModel = GetMonthDayModel(year, month, i, fromHour, toHour);
        //        model.Add(mdModel);
        //    }
        //    ViewBag.FirstDayOfWeek = (int)currentMonthFirstDay.DayOfWeek;
        //    ViewBag.TotalMonthDays = totalMonthDays;
        //    ViewBag.TotalHoursInDay = toHour - fromHour + 1;
        //    ViewBag.FromHour = fromHour;
        //    ViewBag.ToHour = toHour;
        //    ViewBag.Year = year;
        //    ViewBag.Month = month;
        //    return View(model);
        //}

        ////rel
        ////OpenEvent
        ////public int IntA { get; set; } //NumberOfSessions
        ////public int IntB { get; set; } //EachSessionLength
        //private NewMonthDayModel GetMonthDayModel(int year, int month, int day, int fromHour, int toHour)
        //{
        //    var currentDate = new DateTime(year, month, day);
        //    var model = new NewMonthDayModel()
        //    {
        //        DayOfMonth = day,
        //        Month = month,
        //        Year = year,
        //        FromHour = fromHour,
        //        ToHour = toHour,
        //        MonthName = currentDate.ToString("MMMM"),
        //        DayName = currentDate.DayOfWeek.ToString(),
        //        Year14 = year - 2000
        //    };

        //    var openEventsList = _calanderService.GetAllOpenEventsInCurrentDay(currentDate);
        //    if (openEventsList.Count > 0)
        //    {
        //        model.HasOpenEvents = true;
        //        foreach (var openEvent in openEventsList)
        //        {
        //            var oeModel = new NewOpenEventModel()
        //            {
        //                StartsAt = openEvent.StartsAt,
        //                EachSessionLength = openEvent.IntB,
        //                EndsAt = openEvent.EndsAt,
        //                NumberOfSessions = openEvent.IntA,
        //                Events = GetOpenEventsEvents(openEvent.Id),
        //                FrontId = openEvent.Id
        //            };
        //            model.OpenEvents.Add(oeModel);
        //        }
        //    }
        //    else
        //    {
        //        model.HasOpenEvents = false;
        //    }
        //    GetMiniMonthDayInfo(model, new DateTime(year, month, day, fromHour, 0, 0), fromHour, toHour);//(9am to 8pm)
        //    return model;
        //}

        ////rel
        //private List<NewEventModel> GetOpenEventsEvents(int openEventId)
        //{
        //    var list = new List<NewEventModel>();
        //    var events = _calanderService.GetEventsByOpenEventId(openEventId);
        //    foreach (var evnt in events)
        //    {
        //        //var currentCustomerEvent = evnt.Customer.Id == customer.Id;
        //        var eventModel = new NewEventModel()
        //        {
        //            Id = evnt.Id,
        //            CustomerComment = evnt.CustomerComment,
        //            Email = evnt.ContactEmail,
        //            Language = evnt.Language,
        //            PhoneNumber = evnt.ContactPhoneNumbet,
        //            EndsAt = evnt.EndsAtUtc,
        //            CustomerName = evnt.Customer.GetFullName(),
        //            AdminComment = evnt.AdminComment,
        //            //SessionLength = currentCustomerEvent ? evnt : "",
        //            //MyEvent = currentCustomerEvent,
        //            StartsAt = evnt.StartsAtUtc
        //        };
        //        list.Add(eventModel);
        //    }
        //    return list;
        //}

        ////rel datetime -> open event -> event 
        //private void GetMiniMonthDayInfo(NewMonthDayModel model, DateTime startTime, int fromHour, int toHour)
        //{
        //    var block = 15;
        //    var numBlocks = 4 * (toHour - fromHour + 1);//quorte hour - 4 (hour) * 12 hours (9am to 8pm)
        //    var endTime = startTime.AddMinutes((double)block);
        //    for (int i = 0; i < numBlocks; i++)
        //    {
        //        var currentOpenEvent = model.OpenEvents.Where(x => x.StartsAt <= startTime && x.EndsAt >= endTime).ToList();
        //        var currentEvents = new List<EventModel>();
        //        var myEvent = false;
        //        var hasEvent = false;
        //        foreach (var oe in currentOpenEvent)
        //        {
        //            if (oe.Events.Count > 0)
        //            {
        //                foreach (var e in oe.Events.Where(x => x.StartsAt <= startTime && startTime.AddMinutes(x.SessionLength) >= startTime).ToList())
        //                {
        //                    hasEvent = true;
        //                    myEvent = myEvent ? myEvent : e.MyEvent;
        //                }
        //            }
        //        }
        //        var frontModel = new FrontDisplayOpenEventsModel()
        //        {
        //            DisplayOrder = i + 1,
        //            HasOpenEvent = currentOpenEvent.Count > 0,
        //            HasEvent = hasEvent,
        //            MyEvent = myEvent
        //        };
        //        model.FrontDisplayOpenEvents.Add(frontModel);
        //        startTime = startTime.AddMinutes((double)block);
        //        endTime = endTime.AddMinutes((double)block);
        //    }
        //    model.FrontDisplayOpenEvents = model.FrontDisplayOpenEvents.OrderBy(x => x.DisplayOrder).ToList();
        //}
        ////[HttpPost]//, GridAction(EnableCustomBinding = true)
        ////public ActionResult OpenEventsList(OpenEventsListModel model)//GridCommand command, 
        ////{
        ////    //we use own own binder for searchCustomerRoleIds property 
        ////    if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
        ////        return AccessDeniedView();

        ////    int searchDateMonth = 0;
        ////    int searchDateDay = 0;
        ////    int searchDateYear = 0;
        ////    int searchFromDateMonth = 0;
        ////    int searchFromDateDay = 0;
        ////    int searchFromDateYear = 0;
        ////    int searchToDateMonth = 0;
        ////    int searchToDateDay = 0;
        ////    int searchToDateYear = 0;

        ////    int searchMonthNext = 0;
        ////    int searchMonthPast = 0;
        ////    if (!String.IsNullOrWhiteSpace(model.SearchDateMonth))
        ////        searchDateMonth = Convert.ToInt32(model.SearchDateMonth);
        ////    if (!String.IsNullOrWhiteSpace(model.SearchDateDay))
        ////        searchDateDay = Convert.ToInt32(model.SearchDateDay);
        ////    if (!String.IsNullOrWhiteSpace(model.SearchDateYear))
        ////        searchDateYear = Convert.ToInt32(model.SearchDateYear);

        ////    if (!String.IsNullOrWhiteSpace(model.SearchFromDateMonth))
        ////        searchFromDateMonth = Convert.ToInt32(model.SearchFromDateMonth);
        ////    if (!String.IsNullOrWhiteSpace(model.SearchFromDateDay))
        ////        searchFromDateDay = Convert.ToInt32(model.SearchFromDateDay);
        ////    if (!String.IsNullOrWhiteSpace(model.SearchFromDateYear))
        ////        searchFromDateYear = Convert.ToInt32(model.SearchFromDateYear);

        ////    if (!String.IsNullOrWhiteSpace(model.SearchToDateMonth))
        ////        searchToDateMonth = Convert.ToInt32(model.SearchToDateMonth);
        ////    if (!String.IsNullOrWhiteSpace(model.SearchToDateDay))
        ////        searchToDateDay = Convert.ToInt32(model.SearchToDateDay);
        ////    if (!String.IsNullOrWhiteSpace(model.SearchToDateYear))
        ////        searchToDateYear = Convert.ToInt32(model.SearchToDateYear);

        ////    if (!String.IsNullOrWhiteSpace(model.SearchMonthNext))
        ////        searchMonthNext = Convert.ToInt32(model.SearchMonthNext);
        ////    if (!String.IsNullOrWhiteSpace(model.SearchMonthPast))
        ////        searchMonthPast = Convert.ToInt32(model.SearchMonthPast);

        ////    var openEvents = _eventService.GetAllOpenEvents(
        ////        searchDateMonth: searchDateMonth,
        ////        searchDateDay: searchDateDay,
        ////        searchDateYear: searchDateYear,
        ////        searchFromDateMonth: searchFromDateMonth,
        ////        searchFromDateDay: searchFromDateDay,
        ////        searchFromDateYear: searchFromDateYear,
        ////        searchToDateMonth: searchToDateMonth,
        ////        searchToDateDay: searchToDateDay,
        ////        searchToDateYear: searchToDateYear,
        ////        monthPast: searchMonthPast, monthNext: searchMonthNext);//pageIndex: command.Page - 1, pageSize: command.PageSize

        ////    //var gridModel = new GridModel<OpenEventModel>
        ////    //{
        ////    //    Data = openEvents.Select(PrepareOpenEventsModelForList),
        ////    //    Total = openEvents.TotalCount
        ////    //};
        ////    return new JsonResult
        ////    {
        ////        //Data = gridModel
        ////    };
        ////}

        //public ActionResult Create(int year, int month, int day)
        //{
        //    var d = new DateTime(year, month, day, _storeInformationSettings.OpenEventsFromHour, 0, 0);
        //    var model = new OpenEventModel()
        //    {
        //        StartsAt = d,
        //        Active = true,
        //        SessionLength = _storeInformationSettings.OpenEventsSessionLength,
        //        AllowedFromHour = _storeInformationSettings.OpenEventsFromHour,
        //        AllowedToHour = _storeInformationSettings.OpenEventsToHour,
        //        NumberOfSessions = 1
        //    };
        //    return View(model);
        //}

        //[HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        //[FormValueRequired("save", "save-continue")]
        //public ActionResult Create(OpenEventModel model, bool continueEditing)
        //{
        //    //validate 
        //    var allowedFromHour = _storeInformationSettings.OpenEventsFromHour;
        //    var allowedToHour = _storeInformationSettings.OpenEventsToHour;
        //    var hours = (model.SessionLength * model.NumberOfSessions) / 60;
        //    var minutes = (model.SessionLength * model.NumberOfSessions) - (hours * 60);
        //    var ts = new TimeSpan(hours, minutes, 0);
        //    if (model.StartsAt.Hour >= allowedFromHour && model.StartsAt.Hour + hours <= allowedToHour && model.NumberOfSessions > 0)
        //    {
        //        var openEvent = new OpenEvent()
        //        {
        //            Active = model.Active,
        //            CreatedOnUtc = DateTime.UtcNow,
        //            UpdatedOnUtc = DateTime.UtcNow,
        //            StartsAt = model.StartsAt,
        //            EndsAt = model.StartsAt.Add(ts),
        //            IntA = model.NumberOfSessions,//NumberOfSessions
        //            IntB = model.SessionLength,//EachSessionLength
        //            IntC = model.IntC,
        //            IntD = model.IntD,
        //            StringA = model.StringA,
        //            StringB = model.StringB,
        //            StringC = model.StringC,
        //            StringD = model.StringD
        //        };
        //        _calanderService.InsertOpenEvent(openEvent);
        //        //activity log
        //        //_customerActivityService.InsertActivity("AddNewCustomer", _localizationService.GetResource("ActivityLog.AddNewCustomer"), customer.Id);

        //        SuccessNotification(_localizationService.GetResource("rel.OpenEvent.Added"));
        //        return continueEditing ? RedirectToAction("Edit", new { id = openEvent.Id }) : RedirectToAction("List");
        //    }
        //    else
        //    {
        //        ErrorNotification(_localizationService.GetResource("notAdded" + hours.ToString()));
        //        return RedirectToAction("Create", new { model.StartsAt.Year, model.StartsAt.Month, model.StartsAt.Day });
        //    }
        //}

        //public ActionResult Edit(int id)
        //{
        //    var openEvent = _calanderService.GetOpenEventById(id);
        //    if (openEvent == null)
        //        return RedirectToAction("List");

        //    var model = new OpenEventModel()
        //    {
        //        Id = openEvent.Id,
        //        Active = openEvent.Active,
        //        CreatedOn = openEvent.CreatedOnUtc,
        //        Deleted = openEvent.Deleted,
        //        UpdatedOn = openEvent.UpdatedOnUtc,
        //        EndsAt = openEvent.EndsAt,
        //        StartsAt = openEvent.StartsAt,
        //        //IntA = openEvent.IntA,//NumberOfSessions
        //        //IntB = openEvent.IntB,//EachSessionLength
        //        IntC = openEvent.IntC,
        //        IntD = openEvent.IntD,
        //        StringA = openEvent.StringA,
        //        StringB = openEvent.StringB,
        //        StringC = openEvent.StringC,
        //        StringD = openEvent.StringD,
        //        EventsCount = openEvent.AppliedEvents.Count,
        //        SessionLength = openEvent.IntB,
        //        NumberOfSessions = openEvent.IntA,
        //        AllowedFromHour = _storeInformationSettings.OpenEventsFromHour,
        //        AllowedToHour = _storeInformationSettings.OpenEventsToHour,
        //    };
        //    //var events = _eventService.GetAllEventsByOpenEventId(openEvent);
        //    foreach (var evnt in openEvent.AppliedEvents)
        //    {
        //        if (!evnt.Deleted)
        //        {
        //            var eModel = new EventModel()
        //            {
        //                Id = evnt.Id,
        //                StartsAt = evnt.StartsAtUtc,
        //                EndsAt = evnt.EndsAtUtc,
        //                Active = evnt.Active,
        //                AdminComment = evnt.AdminComment,
        //                ContactEmail = evnt.ContactEmail,
        //                ContactPhoneNumbet = evnt.ContactPhoneNumbet,
        //                CreatedOn = evnt.CreatedOnUtc,
        //                CustomerComment = evnt.CustomerComment,
        //                Language = evnt.Language
        //            };
        //            model.Events.Add(eModel);
        //        }
        //    }


        //    return View(model);
        //}

        //[HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        //[FormValueRequired("save", "save-continue")]
        //public ActionResult Edit(OpenEventModel model, bool continueEditing)
        //{
        //    var openEvent = _calanderService.GetOpenEventById(model.Id);
        //    if (openEvent == null)
        //        return RedirectToAction("List");
        //    var allowedFromHour = _storeInformationSettings.OpenEventsFromHour;
        //    var allowedToHour = _storeInformationSettings.OpenEventsToHour;
        //    var hours = (model.SessionLength * model.NumberOfSessions) / 60;
        //    var minutes = (model.SessionLength * model.NumberOfSessions) - (hours * 60);
        //    var ts = new TimeSpan(hours, minutes, 0);
        //    if (model.StartsAt.Hour >= allowedFromHour && model.StartsAt.Hour + hours <= allowedToHour && model.NumberOfSessions > 0)
        //    {
        //        openEvent.Active = model.Active;
        //        //openEvent.CreatedOnUtc = DateTime.UtcNow;
        //        openEvent.UpdatedOnUtc = DateTime.UtcNow;
        //        openEvent.StartsAt = model.StartsAt;
        //        openEvent.EndsAt = model.StartsAt.Add(ts);
        //        openEvent.IntA = model.NumberOfSessions;
        //        openEvent.IntB = model.SessionLength;
        //        openEvent.IntC = model.IntC;
        //        openEvent.IntD = model.IntD;
        //        openEvent.StringA = model.StringA;
        //        openEvent.StringB = model.StringB;
        //        openEvent.StringC = model.StringC;
        //        openEvent.StringD = model.StringD;

        //        _calanderService.UpdateOpenEvent(openEvent);
        //        //activity log
        //        //_customerActivityService.InsertActivity("AddNewCustomer", _localizationService.GetResource("ActivityLog.AddNewCustomer"), customer.Id);

        //        SuccessNotification(_localizationService.GetResource("rel.OpenEvent.Added"));
        //        return continueEditing ? RedirectToAction("Edit", new { id = openEvent.Id }) : RedirectToAction("List");
        //    }
        //    else
        //    {
        //        ErrorNotification(_localizationService.GetResource("notAdded" + hours.ToString()));
        //        return RedirectToAction("Edit", new { id = model.Id });
        //    }

        //}

        //public ActionResult EventsList(int id)
        //{
        //    var openEvent = _calanderService.GetOpenEventById(id);
        //    if (openEvent == null)
        //        return RedirectToAction("List");

        //    var model = PrepareEventsModel(openEvent.AppliedEvents);
        //    ViewBag.StartsAt = openEvent.StartsAt.ToString();
        //    ViewBag.EndsAt = openEvent.EndsAt.ToString();
        //    return View(model);
        //}

        //public ActionResult CreateEvent(int id)
        //{
        //    var openEvent = _calanderService.GetOpenEventById(id);
        //    if (openEvent == null)
        //        return RedirectToAction("List");

        //    //tourguide role
        //    var toureguideRole = _customerService.GetCustomerRoleBySystemName("TourGuide");

        //    int[] customerTourguideId = new int[1];
        //    if (toureguideRole != null)
        //        customerTourguideId[0] = toureguideRole.Id;
        //    var model = new EventModel()
        //    {
        //        Active = true,
        //        OpenEventId = id,
        //        OpenEventStartsAt = openEvent.StartsAt,
        //        OpenEventEndsAt = openEvent.EndsAt,
        //        //AdminComment = evnt.AdminComment,
        //        //ContactEmail = evnt.ContactEmail,
        //        //ContactPhoneNumbet = evnt.ContactPhoneNumbet,
        //        //CustomerComment = evnt.CustomerComment,
        //        //CreatedOn = evnt.CreatedOnUtc,
        //        //Deleted = evnt.Deleted,
        //        EndsAt = openEvent.EndsAt,
        //        //Language = evnt.Language,
        //        StartsAt = openEvent.StartsAt,
        //        //UpdatedOn = evnt.UpdatedOnUtc,
        //        //IntA = evnt.IntA,
        //        //IntB = evnt.IntB,
        //        //IntC = evnt.IntC,
        //        //IntD = evnt.IntD,
        //        //StringA = evnt.StringA,
        //        //StringB = evnt.StringB,
        //        //StringC = evnt.StringC,
        //        //StringD = evnt.StringD,
        //        //CustomerId = evnt.CustomerId
        //    };
        //    var customersList = _customerService.GetAllCustomers(null, null, 0, 0, customerTourguideId);
        //    foreach (var c in customersList)
        //        model.CustomersList.Add(new SelectListItem()
        //        {
        //            Value = c.Id.ToString(),
        //            Text = c.GetFullName(),
        //            Selected = false
        //        });

        //    return View(model);
        //}

        //[HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        //[FormValueRequired("save", "save-continue")]
        //public ActionResult CreateEvent(EventModel model, bool continueEditing)
        //{
        //    var openEvent = _calanderService.GetOpenEventById(model.OpenEventId);
        //    if (openEvent == null)
        //        return RedirectToAction("List");
        //    //validate only hours
        //    if (model.StartsAt.Hour >= openEvent.StartsAt.Hour && model.StartsAt.Hour < model.EndsAt.Hour
        //        && model.EndsAt.Hour > model.StartsAt.Hour && model.EndsAt.Hour <= openEvent.EndsAt.Hour)
        //    {
        //        var eventStartsAt = new DateTime(openEvent.StartsAt.Year, openEvent.StartsAt.Month, openEvent.StartsAt.Day, model.StartsAt.Hour, model.StartsAt.Minute, 0);
        //        var eventEndsAt = new DateTime(openEvent.StartsAt.Year, openEvent.StartsAt.Month, openEvent.StartsAt.Day, model.EndsAt.Hour, model.EndsAt.Minute, 0);
        //        var evnt = new Session();

        //        evnt.Active = model.Active;
        //        evnt.AdminComment = model.AdminComment;

        //        evnt.EndsAtUtc = eventEndsAt;
        //        evnt.Language = model.Language;
        //        evnt.StartsAtUtc = eventStartsAt;
        //        evnt.CreatedOnUtc = DateTime.UtcNow;
        //        evnt.UpdatedOnUtc = DateTime.UtcNow;
        //        evnt.IntA = model.IntA;
        //        evnt.IntB = model.IntB;
        //        evnt.IntC = model.IntC;
        //        evnt.IntD = model.IntD;
        //        evnt.StringA = model.StringA;
        //        evnt.StringB = model.StringB;
        //        evnt.StringC = model.StringC;
        //        evnt.StringD = model.StringD;
        //        evnt.CustomerId = model.CustomerId;
        //        evnt.Customer = _customerService.GetCustomerById(model.CustomerId);
        //        evnt.ContactEmail = _customerService.GetCustomerById(model.CustomerId).Email;
        //        evnt.ContactPhoneNumbet = _customerService.GetCustomerById(model.CustomerId).GetAttribute<string>(SystemCustomerAttributeNames.Phone);

        //        openEvent.AppliedEvents.Add(evnt);
        //        openEvent.UpdatedOnUtc = DateTime.UtcNow;

        //        _calanderService.InsertEvent(evnt);
        //        _calanderService.UpdateOpenEvent(openEvent);

        //        //activity log
        //        //_customerActivityService.InsertActivity("AddNewCustomer", _localizationService.GetResource("ActivityLog.AddNewCustomer"), customer.Id);

        //        SuccessNotification(_localizationService.GetResource("rel.eventAdded"));
        //        return continueEditing ? RedirectToAction("EditEvent", new { id = evnt.Id }) : RedirectToAction("List");
        //    }
        //    ErrorNotification(_localizationService.GetResource("rel.errorcreatingeventtimeanddatearewrong"));
        //    //something failled

        //    return RedirectToAction("CreateEvent", new { id = model.OpenEventId });
        //}

        //public ActionResult EditEvent(int id)
        //{
        //    var evnt = _calanderService.GetSessionById(id);
        //    if (evnt == null)
        //        return RedirectToAction("List");
        //    var openEvent = _calanderService.GetOpenEventByEventId(evnt.Id);
        //    if (openEvent == null)
        //        return RedirectToAction("List");
        //    //tourguide role
        //    var toureguideRole = _customerService.GetCustomerRoleBySystemName("TourGuide");

        //    int[] customerTourguideId = new int[1];
        //    if (toureguideRole != null)
        //        customerTourguideId[0] = toureguideRole.Id;
        //    var model = new EventModel()
        //    {
        //        Id = evnt.Id,
        //        Active = evnt.Active,
        //        AdminComment = evnt.AdminComment,
        //        ContactEmail = evnt.ContactEmail,
        //        ContactPhoneNumbet = evnt.ContactPhoneNumbet,
        //        CustomerComment = evnt.CustomerComment,
        //        CreatedOn = evnt.CreatedOnUtc,
        //        Deleted = evnt.Deleted,
        //        EndsAt = evnt.EndsAtUtc,
        //        Language = evnt.Language,
        //        StartsAt = evnt.StartsAtUtc,
        //        UpdatedOn = evnt.UpdatedOnUtc,
        //        IntA = evnt.IntA,
        //        IntB = evnt.IntB,
        //        IntC = evnt.IntC,
        //        IntD = evnt.IntD,
        //        StringA = evnt.StringA,
        //        StringB = evnt.StringB,
        //        StringC = evnt.StringC,
        //        StringD = evnt.StringD,
        //        CustomerId = evnt.CustomerId,
        //        OpenEventId = openEvent.Id
        //    };

        //    var customersList = _customerService.GetAllCustomers(null, null, 0, 0, customerTourguideId);
        //    foreach (var c in customersList)
        //        model.CustomersList.Add(new SelectListItem()
        //        {
        //            Value = c.Id.ToString(),
        //            Text = c.GetFullName(),
        //            Selected = c.Id == evnt.CustomerId
        //        });

        //    return View(model);

        //}

        //[HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        //[FormValueRequired("save", "save-continue")]
        //public ActionResult EditEvent(EventModel model, bool continueEditing)
        //{
        //    var evnt = _calanderService.GetSessionById(model.Id);
        //    var openEvent = _calanderService.GetOpenEventById(model.OpenEventId);
        //    if (openEvent != null && evnt != null)
        //        //validate event -> open event
        //        if (openEvent.AppliedEvents.Contains(evnt) && openEvent.StartsAt.Day == evnt.StartsAtUtc.Day && openEvent.StartsAt.Month == evnt.StartsAtUtc.Month && openEvent.StartsAt.Year == evnt.StartsAtUtc.Year
        //            //validate event -> event
        //            && evnt.StartsAtUtc.Year == evnt.EndsAtUtc.Year && evnt.StartsAtUtc.Month == evnt.EndsAtUtc.Month && evnt.StartsAtUtc.Day == evnt.EndsAtUtc.Day
        //            && evnt.StartsAtUtc >= openEvent.StartsAt && evnt.EndsAtUtc < openEvent.EndsAt && evnt.EndsAtUtc > evnt.EndsAtUtc && evnt.EndsAtUtc <= openEvent.EndsAt)
        //        {
        //            var fromHour = _storeInformationSettings.OpenEventsFromHour;
        //            var toHour = _storeInformationSettings.OpenEventsToHour;
        //            var oeStart = openEvent.StartsAt.TimeOfDay;
        //            var oeEnds = openEvent.EndsAt.TimeOfDay;
        //            if (model.StartsAt.TimeOfDay >= openEvent.StartsAt.TimeOfDay && model.EndsAt.TimeOfDay <= openEvent.EndsAt.TimeOfDay &&
        //                model.StartsAt.Hour >= fromHour && model.EndsAt.Hour <= toHour && model.StartsAt != model.EndsAt)
        //            {
        //                evnt.Active = model.Active;
        //                evnt.AdminComment = model.AdminComment;

        //                evnt.EndsAtUtc = model.EndsAt;
        //                evnt.Language = model.Language;
        //                evnt.StartsAtUtc = model.StartsAt;
        //                evnt.UpdatedOnUtc = DateTime.UtcNow;
        //                evnt.IntA = model.IntA;
        //                evnt.IntB = model.IntB;
        //                evnt.IntC = model.IntC;
        //                evnt.IntD = model.IntD;
        //                evnt.StringA = model.StringA;
        //                evnt.StringB = model.StringB;
        //                evnt.StringC = model.StringC;
        //                evnt.StringD = model.StringD;
        //                //update customer
        //                if (model.CustomerId != evnt.CustomerId)
        //                {
        //                    var toureguideRole = _customerService.GetCustomerRoleBySystemName("TourGuide");
        //                    var customer = _customerService.GetCustomerById(model.CustomerId);
        //                    if (customer.CustomerRoles.Contains(toureguideRole))
        //                    {
        //                        evnt.CustomerId = model.CustomerId;
        //                        evnt.CustomerComment = "";
        //                        evnt.ContactEmail = customer.Email;
        //                        evnt.ContactPhoneNumbet = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
        //                    }
        //                }
        //                evnt.Customer = _customerService.GetCustomerById(model.CustomerId);
        //                _calanderService.UpdateEvent(evnt);

        //                //activity log
        //                //_customerActivityService.InsertActivity("AddNewCustomer", _localizationService.GetResource("ActivityLog.AddNewCustomer"), customer.Id);

        //                SuccessNotification(_localizationService.GetResource("rel.eventAdded"));
        //                return continueEditing ? RedirectToAction("EditEvent", new { id = evnt.Id }) : RedirectToAction("List");
        //            }
        //            ErrorNotification(_localizationService.GetResource("rel.eventwrongtimeordate"));
        //            return RedirectToAction("EditEvent", new { id = evnt.Id });
        //        }
        //    //something failled

        //    return RedirectToAction("List");
        //}

        //public ActionResult Delete(int id)//Event
        //{
        //    var evnt = _calanderService.GetSessionById(id);
        //    if (evnt == null)
        //        //No Event found with the specified id
        //        return RedirectToAction("List");

        //    try
        //    {
        //        _calanderService.DeleteEvent(evnt);


        //        //activity log
        //        //_customerActivityService.InsertActivity("DeleteCustomer", _localizationService.GetResource("ActivityLog.DeleteCustomer"), customer.Id);

        //        SuccessNotification(_localizationService.GetResource("rel.eventDeleted"));
        //        return RedirectToAction("List");
        //    }
        //    catch (Exception exc)
        //    {
        //        ErrorNotification(exc.Message);
        //        return RedirectToAction("EditEvent", new { id = evnt.Id });
        //    }
        //}

        //public ActionResult DeleteOpenEvent(int id)
        //{
        //    var openEvent = _calanderService.GetOpenEventById(id);
        //    if (openEvent == null)
        //        //No Event found with the specified id
        //        return RedirectToAction("List");

        //    try
        //    {
        //        _calanderService.DeleteOpenEvent(openEvent);


        //        //activity log
        //        //_customerActivityService.InsertActivity("DeleteCustomer", _localizationService.GetResource("ActivityLog.DeleteCustomer"), customer.Id);

        //        SuccessNotification(_localizationService.GetResource("rel.openEventDeleted"));
        //        return RedirectToAction("List");
        //    }
        //    catch (Exception exc)
        //    {
        //        ErrorNotification(exc.Message);
        //        return RedirectToAction("Edit", new { id = openEvent.Id });
        //    }
        //}

    }
}