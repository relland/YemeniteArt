using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Widgets.Wholesaler.Models;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Controllers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Customer;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.Wholesaler.Controllers
{
    public class WholesalerController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ICacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly MediaSettings _mediaSettings;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerService _customerService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IMeasureService _measureService;
        private readonly IPermissionService _permissionService;
        private readonly CatalogSettings _catalogSettings;
        
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDiscountService _discountService;
        private readonly IGiftCardService _giftCardService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPaymentService _paymentService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDownloadService _downloadService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly HttpContextBase _httpContext;
        
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly CustomerSettings _customerSettings;

        private readonly IReturnRequestService _returnRequestService;

        public WholesalerController(IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            IPictureService pictureService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService,
            IProductService productService,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            MediaSettings mediaSettings,
            IProductAttributeFormatter productAttributeFormatter,
            ICategoryService categoryService,
            ICustomerService customerService, 
            IProductAttributeService productAttributeService,
            IMeasureService measureService,
            IPermissionService permissionService,
            CatalogSettings catalogSettings,
            
            IShoppingCartService shoppingCartService,
            IProductAttributeParser productAttributeParser,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IOrderProcessingService orderProcessingService,
            IDiscountService discountService,
            IGiftCardService giftCardService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICheckoutAttributeService checkoutAttributeService,
            IPaymentService paymentService,
            IWorkflowMessageService workflowMessageService,
            IDownloadService downloadService,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            HttpContextBase httpContext,
            ShoppingCartSettings shoppingCartSettings,
            OrderSettings orderSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            CaptchaSettings captchaSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            CustomerSettings customerSettings,

            IReturnRequestService returnRequestService)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._localizationService = localizationService;
            this._productService = productService;
            this._priceCalculationService = priceCalculationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._mediaSettings = mediaSettings;
            this._productAttributeFormatter = productAttributeFormatter;
            this._categoryService = categoryService;
            this._customerService = customerService;
            this._productAttributeService = productAttributeService;
            this._measureService = measureService;
            this._permissionService = permissionService;
            this._catalogSettings = catalogSettings;
            
            this._shoppingCartService = shoppingCartService;
            this._productAttributeParser = productAttributeParser;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._orderProcessingService = orderProcessingService;
            this._discountService = discountService;
            this._giftCardService = giftCardService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._paymentService = paymentService;
            this._workflowMessageService = workflowMessageService;
            this._downloadService = downloadService;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._genericAttributeService = genericAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._httpContext = httpContext;
            
            this._shoppingCartSettings = shoppingCartSettings;
            this._orderSettings = orderSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
            this._captchaSettings = captchaSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._customerSettings = customerSettings;

            this._returnRequestService = returnRequestService;
        }

        private bool IsWholesaler(WholesalerSettings wholesalerSettings)
        {
            return _workContext.CurrentCustomer.CustomerRoles.Where(x => x.SystemName == wholesalerSettings.WholesalerRoleSystemName).Count() > 0;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ws = _settingService.LoadSetting<WholesalerSettings>(storeScope);
            var model = new ConfigurationModel()
            {
                EnableWidget = ws.EnableWidget,
                WholesalerRoleSystemName = ws.WholesalerRoleSystemName,
                PercentageAmount = ws.PercentageAmount,
                LastCalculationDate = ws.LastCalculationDate,
                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope > 0)
            {
                model.EnableWidget_OverrideForStore = _settingService.SettingExists(ws, x => x.EnableWidget, storeScope);
                model.WholesalerRoleSystemName_OverrideForStore = _settingService.SettingExists(ws, x => x.WholesalerRoleSystemName, storeScope);
                model.PercentageAmount_OverrideForStore = _settingService.SettingExists(ws, x => x.PercentageAmount, storeScope);
                model.LastCalculationDate_OverrideForStore = _settingService.SettingExists(ws, x => x.LastCalculationDate, storeScope);
            }
            return View("~/Plugins/Widgets.Wholesaler/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ws = _settingService.LoadSetting<WholesalerSettings>(storeScope);
            ws.EnableWidget = model.EnableWidget;
            ws.WholesalerRoleSystemName = model.WholesalerRoleSystemName;
            ws.PercentageAmount = model.PercentageAmount;
            ws.LastCalculationDate = model.LastCalculationDate;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(ws, x => x.EnableWidget, model.EnableWidget_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ws, x => x.WholesalerRoleSystemName, model.WholesalerRoleSystemName_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ws, x => x.PercentageAmount, model.PercentageAmount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ws, x => x.LastCalculationDate, model.LastCalculationDate_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        [AdminAuthorize]
        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult ReCalculate()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var wholesalerSettings = _settingService.LoadSetting<WholesalerSettings>(storeScope);
            var wholesalerCustomerRole = _customerService.GetAllCustomerRoles(true).FirstOrDefault(x => x.SystemName == wholesalerSettings.WholesalerRoleSystemName);

            if (wholesalerCustomerRole == null)
                throw new Exception("Wholesaler customer role doesn't exist. First create it and then re-calculate.");
            //var wholesalerRoleId = 
            var allProducts = _productService.SearchProducts();
            var updatedI = 0;
            var newI = 0;
            foreach (var product in allProducts.Where(x => x.Price > 0))
            {
                var existingWholesalerTierPrice = product
                    .TierPrices.FirstOrDefault(x => x.CustomerRoleId == wholesalerCustomerRole.Id);
                //UPDATE
                if (existingWholesalerTierPrice != null)
                {
                    existingWholesalerTierPrice.StoreId = storeScope;
                    existingWholesalerTierPrice.CustomerRoleId = wholesalerCustomerRole.Id;
                    existingWholesalerTierPrice.Quantity = 1;
                    existingWholesalerTierPrice.Price = product.Price * wholesalerSettings.PercentageAmount / 100;
                    _productService.UpdateTierPrice(existingWholesalerTierPrice);
                    updatedI++;
                }
                else
                {
                    var tierPrice = new TierPrice
                    {
                        ProductId = product.Id,
                        StoreId = storeScope,
                        CustomerRoleId = wholesalerCustomerRole.Id,
                        Quantity = 1,
                        Price = product.Price * wholesalerSettings.PercentageAmount / 100
                    };
                    _productService.InsertTierPrice(tierPrice);

                    //update "HasTierPrices" property
                    _productService.UpdateHasTierPricesProperty(product);
                    newI++;
                }
            }
            wholesalerSettings.LastCalculationDate = DateTime.UtcNow;
            _settingService.SaveSetting(wholesalerSettings, storeScope);
            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(string.Format(_localizationService.GetResource("Admin.Wholesaler.ReCalculatedSuccessfully"), updatedI, newI));
            return Configure();
        }

        #region Account Navigation
        [NopHttpsRequirement(SslRequirement.No)]
        [ChildActionOnly]
        public ActionResult AccountNavigation()
        {
            var accountNavigationModel = new AccountNavigationModel();

            var wholesalerSettings = _settingService.LoadSetting<WholesalerSettings>();
            accountNavigationModel.IsWholesaler = IsWholesaler(wholesalerSettings);


            return PartialView("~/Plugins/Widgets.Wholesaler/Views/AccountNavigation.cshtml", accountNavigationModel);


        }

        [ChildActionOnly]
        public ActionResult CustomerNavigation()
        {
            var model = new CustomerNavigationModel();

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerInfo",
                Title = _localizationService.GetResource("Account.CustomerInfo"),
                Tab = CustomerNavigationEnum.Info,
                ItemClass = "customer-info"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerAddresses",
                Title = _localizationService.GetResource("Account.CustomerAddresses"),
                Tab = CustomerNavigationEnum.Addresses,
                ItemClass = "customer-addresses"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerOrders",
                Title = _localizationService.GetResource("Account.CustomerOrders"),
                Tab = CustomerNavigationEnum.Orders,
                ItemClass = "customer-orders"
            });

            if (_orderSettings.ReturnRequestsEnabled &&
                _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id,
                    _workContext.CurrentCustomer.Id, 0, null, 0, 1).Any())
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerReturnRequests",
                    Title = _localizationService.GetResource("Account.CustomerReturnRequests"),
                    Tab = CustomerNavigationEnum.ReturnRequests,
                    ItemClass = "return-requests"
                });
            }

            if (!_customerSettings.HideDownloadableProductsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerDownloadableProducts",
                    Title = _localizationService.GetResource("Account.DownloadableProducts"),
                    Tab = CustomerNavigationEnum.DownloadableProducts,
                    ItemClass = "downloadable-products"
                });
            }

            if (!_customerSettings.HideBackInStockSubscriptionsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerBackInStockSubscriptions",
                    Title = _localizationService.GetResource("Account.BackInStockSubscriptions"),
                    Tab = CustomerNavigationEnum.BackInStockSubscriptions,
                    ItemClass = "back-in-stock-subscriptions"
                });
            }

            if (_rewardPointsSettings.Enabled)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerRewardPoints",
                    Title = _localizationService.GetResource("Account.RewardPoints"),
                    Tab = CustomerNavigationEnum.RewardPoints,
                    ItemClass = "reward-points"
                });
            }

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerChangePassword",
                Title = _localizationService.GetResource("Account.ChangePassword"),
                Tab = CustomerNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerAvatar",
                    Title = _localizationService.GetResource("Account.Avatar"),
                    Tab = CustomerNavigationEnum.Avatar,
                    ItemClass = "customer-avatar"
                });
            }

            //if (_forumSettings.ForumsEnabled && _forumSettings.AllowCustomersToManageSubscriptions)
            //{
            //    model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            //    {
            //        RouteName = "CustomerForumSubscriptions",
            //        Title = _localizationService.GetResource("Account.ForumSubscriptions"),
            //        Tab = CustomerNavigationEnum.ForumSubscriptions,
            //        ItemClass = "forum-subscriptions"
            //    });
            //}
            if (_catalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerProductReviews",
                    Title = _localizationService.GetResource("Account.CustomerProductReviews"),
                    Tab = CustomerNavigationEnum.ProductReviews,
                    ItemClass = "customer-reviews"
                });
            }
            //if (_vendorSettings.AllowVendorsToEditInfo && _workContext.CurrentVendor != null)
            //{
            //    model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            //    {
            //        RouteName = "CustomerVendorInfo",
            //        Title = _localizationService.GetResource("Account.VendorInfo"),
            //        Tab = CustomerNavigationEnum.VendorInfo,
            //        ItemClass = "customer-vendor-info"
            //    });
            //}

            //model.SelectedTab = (CustomerNavigationEnum)selectedTabId;

            return PartialView("~/Plugins/Widgets.Wholesaler/Views/CustomerNavigation.cshtml", model);
        }
        #endregion

        #region product list

        public ActionResult Products()
        {
            var wholesalerSettings = _settingService.LoadSetting<WholesalerSettings>();
            if (!IsWholesaler(wholesalerSettings))
                return null;

            var model = new ProductListModel();
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            ////manufacturers
            //model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
            //    model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            ////warehouses
            //model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var wh in _shippingService.GetAllWarehouses())
            //    model.AvailableWarehouses.Add(new SelectListItem { Text = wh.Name, Value = wh.Id.ToString() });

            ////vendors
            //model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var v in _vendorService.GetAllVendors(showHidden: true))
            //    model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            //rel
            //VisibleIndividually
            model.AvailableVisibleIndividuallyOnlyOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All"), Value = "1" });
            model.AvailableVisibleIndividuallyOnlyOptions.Add(new SelectListItem { Text = _localizationService.GetResource("ya.VisibleIndividuallyOnly"), Value = "2" });

            return View("~/Plugins/Widgets.Wholesaler/Views/Products.cshtml", model);
        }

        public ActionResult ProductFiltering()
        {
            var wholesalerSettings = _settingService.LoadSetting<WholesalerSettings>();
            if (!IsWholesaler(wholesalerSettings))
                return null;

            var model = new ProductListModel();
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            ////manufacturers
            //model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
            //    model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            ////warehouses
            //model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var wh in _shippingService.GetAllWarehouses())
            //    model.AvailableWarehouses.Add(new SelectListItem { Text = wh.Name, Value = wh.Id.ToString() });

            ////vendors
            //model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var v in _vendorService.GetAllVendors(showHidden: true))
            //    model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            //rel
            //VisibleIndividually
            model.AvailableVisibleIndividuallyOnlyOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All"), Value = "1" });
            model.AvailableVisibleIndividuallyOnlyOptions.Add(new SelectListItem { Text = _localizationService.GetResource("ya.VisibleIndividuallyOnly"), Value = "2" });

            return View("~/Plugins/Widgets.Wholesaler/Views/ProductFiltering.cshtml", model);
        }

        [HttpPost]
        public ActionResult ProductList([DataSourceRequest] DataSourceRequest command, ProductFilterModel model)
        {
            var wholesalerSettings = _settingService.LoadSetting<WholesalerSettings>();

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                //.LimitPerStore(_storeContext.CurrentStore.Id)
                .Where(x => x.StoreId == _storeContext.CurrentStore.Id)
                .ToList();
            //var model = new ShoppingCartModel();
            //PrepareShoppingCartModel(model, cart);
            
            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (true && model.SearchCategoryId > 0)//model.SearchIncludeSubCategories
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));
            var searchSku = !string.IsNullOrWhiteSpace(model.GoDirectlyToSku);
            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                //manufacturerId: model.SearchManufacturerId,
                //storeId: model.SearchStoreId,
                //vendorId: model.SearchVendorId,
                //warehouseId: model.SearchWarehouseId,
                //productType: ProductType.SimpleProduct,// model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                searchSku: searchSku,
                keywords: searchSku ? model.GoDirectlyToSku : model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: false,
                overridePublished: true,
                //rel
                visibleIndividuallyOnly: true
            );
            var gridModel = new DataSourceResult();
            var data = products.Select(x =>
            {
                //picture
                var defaultProductPicture = _pictureService.GetPicturesByProductId(x.Id, 1).FirstOrDefault();
                decimal priceWithDefaultAttributes;
                var wholesalerPriceWithDefaultAttributes = WholesalerPriceWithDifaultAttributes(x, out priceWithDefaultAttributes);
                var productModel = new ProductModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    SeName = x.GetSeName(_workContext.WorkingLanguage.Id),
                    Sku = x.Sku,
                    RetailPrice = priceWithDefaultAttributes,
                    WholesalerPrice = wholesalerPriceWithDefaultAttributes,
                    PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultProductPicture, 75, true)
                };
                
                return productModel;
            });
            //var actualData = data.ToList();

            data = data.SortBy("Name", model.SortByName)
                .SortBy("Sku", model.SortBySku)
                .SortBy("RetailPrice", model.SortByRetailPrice)
                .SortBy("WholesalerPrice", model.SortByWholesalerPrice);
            gridModel.Data = data;
            gridModel.Total = products.TotalCount;

            //return Json(gridModel.Data.ToDataSourceResult(command), JsonRequestBehavior.AllowGet);
            return Json(gridModel);
        }


        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Cart()
        {
            var wholesalerSettings = _settingService.LoadSetting<WholesalerSettings>();

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new ShoppingCartModel();
            PrepareShoppingCartModel(model, cart);

            var gridModel = new DataSourceResult();
            gridModel.Data = model.Items.Select(x =>
            {
                //picture
                var picture = x.Picture;
                var hasPicture = picture != null;
                var productModel = new WShoppintCartItemModel()
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    ProductSeName = x.ProductSeName,
                    Quantity = x.Quantity,
                    SubTotal = x.SubTotal,
                    UnitPrice = x.UnitPrice,
                    Sku = x.Sku,
                    ImageUrl = hasPicture ? picture.ImageUrl : "",
                    AlternateText = hasPicture ? picture.AlternateText : "",
                    Title = hasPicture ? picture.Title : ""
                };

                return productModel;
            });
            gridModel.Total = model.Items.Count();

            return Json(gridModel);
        }

        /// <summary>
        /// Prepare shopping cart model
        /// </summary>
        /// <param name="model">Model instance</param>
        /// <param name="cart">Shopping cart</param>
        /// <param name="isEditable">A value indicating whether cart is editable</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether we should validate checkout attributes when preparing the model</param>
        /// <param name="prepareEstimateShippingIfEnabled">A value indicating whether we should prepare "Estimate shipping" model</param>
        /// <param name="setEstimateShippingDefaultAddress">A value indicating whether we should prefill "Estimate shipping" model with the default customer address</param>
        /// <param name="prepareAndDisplayOrderReviewData">A value indicating whether we should prepare review data (such as billing/shipping address, payment or shipping data entered during checkout)</param>
        /// <returns>Model</returns>
        [NonAction]
        protected virtual void PrepareShoppingCartModel(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            if (!cart.Any())
                return;

            #region Simple properties

            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowProductSku;
            var checkoutAttributesXml = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
            model.CheckoutAttributeInfo = _checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, _workContext.CurrentCustomer);
            bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
            if (!minOrderSubtotalAmountOk)
            {
                decimal minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                model.MinOrderSubtotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"), _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false));
            }
            model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoShoppingCart;

            //gift card and gift card boxes
            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            var discountCouponCode = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.DiscountCouponCode);
            var discount = _discountService.GetDiscountByCouponCode(discountCouponCode);
            if (discount != null &&
                discount.RequiresCouponCode &&
                _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer).IsValid)
                model.DiscountBox.CurrentCode = discount.CouponCode;
            model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion

            

            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
                {
                    Id = sci.Id,
                    Sku = sci.Product.FormatSku(sci.AttributesXml, _productAttributeParser),
                    ProductId = sci.Product.Id,
                    ProductName = sci.Product.GetLocalized(x => x.Name),
                    ProductSeName = sci.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                };

                //allow editing?
                //1. setting enabled?
                //2. simple product?
                //3. has attribute or gift card?
                //4. visible individually?
                cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                    sci.Product.ProductType == ProductType.SimpleProduct &&
                    (!String.IsNullOrEmpty(cartItemModel.AttributeInfo) || sci.Product.IsGiftCard) &&
                    sci.Product.VisibleIndividually;

                //allowed quantities
                var allowedQuantities = sci.Product.ParseAllowedQuantities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }

                //recurring info
                if (sci.Product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.Product.RecurringCycleLength, sci.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (sci.Product.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                    List<Discount> scDiscounts;
                    decimal shoppingCartItemDiscountBase;
                    decimal taxRate;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out shoppingCartItemDiscountBase, out scDiscounts), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.ShoppingCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion            
        }

        [NonAction]
        protected virtual PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci,
            int pictureSize, bool showDefaultPicture, string productName)
        {
            var pictureCacheKey = string.Format(ModelCacheEventConsumer.CART_PICTURE_MODEL_KEY, sci.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            var model = _cacheManager.Get(pictureCacheKey,
                //as we cache per user (shopping cart item identifier)
                //let's cache just for 3 minutes
                3, () =>
                {
                    //shopping cart item picture
                    //var sciPicture = sci.Product.GetProductPicture(sci.AttributesXml, _pictureService, _productAttributeParser);
                    //rel
                    var sciPicture = sci.Product.GetProductPictureIgnoreAttribute(_pictureService);
                    return new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(sciPicture, pictureSize, showDefaultPicture),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
                    };
                });
            return model;
        }





        //public ActionResult Products()
        //{
        //    return View("~/Plugins/Widgets.Wholesaler/Views/Products.cshtml");
        //}

        //[HttpPost]
        //[NopHttpsRequirement(SslRequirement.No)]
        //public ActionResult ProductList(DataSourceRequest command, WholesaleProductsModel model)//int searchCategoryId, int pageSize, int page, string sku, string type, string val)
        //{
        //    if (!String.IsNullOrWhiteSpace(model.sku))
        //    {
        //        var productBySku = _productService.GetProductBySku(sku);
        //        if (productBySku != null)
        //        {
        //            var plProduct = new List<Product>();
        //            plProduct.Add(productBySku);
        //            model.ItemDisplayed = 1;
        //            model.PageNumber = 1;
        //            model.PageSize = pageSize;
        //            model.TotalPages = 1;
        //            model.TotalProducts = 1;
        //            model.Products = PrepareWholesalerProductOverviewModels(plProduct, 70);
        //        }
        //    }
        //    else if (type == "q")
        //    {
        //        var categoryIds = new List<int>() { searchCategoryId };
        //        //include subcategories
        //        if (searchCategoryId > 0)
        //            categoryIds.AddRange(GetChildCategoryIds(searchCategoryId));

        //        var products = _productService.SearchProducts(//.SearchProductsRel(
        //            categoryIds: categoryIds,
        //            searchSku: !String.IsNullOrWhiteSpace(sku),
        //            //keywords: model.SearchProductName,
        //            pageIndex: page - 1,
        //            pageSize: pageSize,
        //            //orderByRel: type,
        //            //orderByDirectionRel: val,                    
        //            showHidden: false
        //        );
        //        decimal totalPages = products.TotalCount / pageSize;

        //        if (products.TotalCount < (page - 1) * pageSize)
        //        {
        //            page = (int)Math.Floor((decimal)products.TotalCount / pageSize);
        //            if (page < 1)
        //                page = 1;
        //        }

        //        model.Products = PrepareWholesalerProductOverviewModels(products, 70, val);

        //        model.PageSize = pageSize;
        //        model.PageNumber = page;

        //        model.TotalProducts = products.TotalCount;
        //        model.TotalPages = totalPages > 0 ? (int)totalPages + 1 : (int)totalPages;
        //        model.ItemDisplayed = products.Count;
        //    }
        //    else
        //    {
        //        var categoryIds = new List<int>() { searchCategoryId };
        //        //include subcategories
        //        if (searchCategoryId > 0)
        //            categoryIds.AddRange(GetChildCategoryIds(searchCategoryId));

        //        var products = _productService.SearchProducts(//.SearchProductsRel(
        //            categoryIds: categoryIds,
        //            searchSku: !String.IsNullOrWhiteSpace(sku),
        //            //keywords: model.SearchProductName,
        //            pageIndex: page - 1,
        //            pageSize: pageSize,
        //            //orderByRel: type,
        //            //orderByDirectionRel: val,
        //            showHidden: false
        //        );
        //        decimal totalPages = products.TotalCount / pageSize;
        //        model.PageSize = pageSize;
        //        model.PageNumber = page;
        //        model.Products = PrepareWholesalerProductOverviewModels(products, 70);
        //        model.TotalProducts = products.TotalCount;
        //        model.TotalPages = totalPages > 0 ? (int)totalPages + 1 : (int)totalPages;
        //        model.ItemDisplayed = products.Count;
        //    }

        //    ViewBag.Type = type;
        //    ViewBag.Val = val;
        //    return View("~/Plugins/Widgets.Wholesaler/Views/ProductList.cshtml", model);
        //}

        [NonAction]
        protected IList<WholesalerProductOverviewModel> PrepareWholesalerProductOverviewModels(IEnumerable<Product> products,
            int? productThumbPictureSize = null, string direction = "")
        {
            if (products == null)
                throw new ArgumentNullException("products");

            var models = new List<WholesalerProductOverviewModel>();
            foreach (var product in products)
            {
                var model = new WholesalerProductOverviewModel()
                {
                    Id = product.Id,
                    Name = product.GetLocalized(x => x.Name),
                    ShortDescription = product.GetLocalized(x => x.ShortDescription),
                    SeName = product.GetSeName(),
                    //rel
                    Sku = product.Sku,
                    //RetalPrice = _priceCalculationService.GetFinalPrice(product, true)
                };
                //price
                #region Prepare product price

                var wholesalerProductPrice = _priceCalculationService.GetFinalPrice(product,
                                        _workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);
                //calculate prices
                decimal taxRate = decimal.Zero;
                decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out taxRate);
                decimal finalPriceBase = _taxService.GetProductPrice(product, product.Price, out taxRate);
                decimal finalWholesalerPriceBase = _taxService.GetProductPrice(product, wholesalerProductPrice, out taxRate);

                decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);
                decimal finalWholesalerPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalWholesalerPriceBase, _workContext.WorkingCurrency);

                //do we have tier prices configured?
                var tierPrices = new List<TierPrice>();
                if (product.HasTierPrices)
                {
                    tierPrices.AddRange(product.TierPrices
                        .OrderBy(tp => tp.Quantity)
                        .ToList()
                        .FilterByStore(_storeContext.CurrentStore.Id)
                        .FilterForCustomer(_workContext.CurrentCustomer)
                        .RemoveDuplicatedQuantities());
                }
                //When there is just one tier (with  qty 1), 
                //there are no actual savings in the list.
                //bool displayFromMessage = tierPrices.Count > 0 &&
                //    !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
                if (finalWholesalerPrice < finalPrice)
                {
                    if (oldPriceBase != decimal.Zero)
                        model.OldPrice = _priceFormatter.FormatPrice(oldPrice);
                    else
                        model.OldPrice = null;
                    model.Price = _priceFormatter.FormatPrice(finalPrice);
                    model.WholesalerPrice = _priceFormatter.FormatPrice(finalWholesalerPrice);
                }
                else
                {
                    if (oldPriceBase != decimal.Zero)
                        model.OldPrice = _priceFormatter.FormatPrice(oldPrice);
                    else
                        model.OldPrice = null;
                    model.Price = _priceFormatter.FormatPrice(finalPrice);
                    model.WholesalerPrice = _priceFormatter.FormatPrice(finalPrice);
                }

                #endregion

                //picture
                #region Prepare product picture

                //If a size has been set in the view, we use it in priority
                int pictureSize = productThumbPictureSize.HasValue ? productThumbPictureSize.Value : _mediaSettings.ProductThumbPictureSize;
                //prepare picture model
                var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                if (picture != null)
                {
                    model.PictureThumbnailUrl = _pictureService.GetPictureUrl(picture, pictureSize);
                    model.Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name);
                    model.AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name);
                    model.PictureBigUrl = _pictureService.GetPictureUrl(picture, 300);
                }

                #endregion

                //total price and quantity
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                            .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                            .Where(sci => sci.StoreId == _storeContext.CurrentStore.Id)
                            .ToList();
                var addedProducts = cart.Where(x => x.ProductId == product.Id).ToList();
                if (addedProducts.Count > 0)
                {
                    var hasDiferentProductAttributes = addedProducts.Count > 1;
                    model.HasDiferentProductAttributes = hasDiferentProductAttributes;
                    //if (hasDiferentProductAttributes)
                    //    model.ProductAttributesModels = new List<WholesalerProductAttributeOverviewModel>();
                    decimal totalPrice = decimal.Zero;
                    foreach (var p in addedProducts)
                    {
                        model.StockQuantity += p.Quantity;

                        if (hasDiferentProductAttributes)
                        {
                            var itemPrice = "";
                            //unit prices
                            if (p.Product.CallForPrice)
                            {
                                itemPrice = _localizationService.GetResource("Products.CallForPrice");
                            }
                            else
                            {
                                decimal taxRateB = decimal.Zero;
                                decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(p.Product, _priceCalculationService.GetUnitPrice(p, true), out taxRateB);
                                decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                                totalPrice += shoppingCartUnitPriceWithDiscount * p.Quantity;
                                itemPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                            }
                            var tableRow = "<tr class='product-attributes'><td role='gridcell'>";
                            tableRow += "<a href='/" + p.Product.GetSeName() + "?updatecartitemid=" + p.Id + "' class='edit-product-link'>";
                            tableRow += _localizationService.GetResource("Common.Edit") + "</a></td>";
                            tableRow += "<td class='attributes'>" + _productAttributeFormatter.FormatAttributes(p.Product, p.AttributesXml) + "</td>";
                            tableRow += "<td role='gridcell'>" + _localizationService.GetResource("Products.Price") + ": " + itemPrice + "</td>";
                            tableRow += "<td role='gridcell'>" + _localizationService.GetResource("Products.Quantity") + ": " + p.Quantity + "</td>";
                            tableRow += "</tr>";
                            model.ProductAttributes += tableRow;
                            //var pam = new WholesalerProductAttributeOverviewModel() 
                            //{ 
                            //    CartItemId = p.Id,
                            //    Name = p.Product.Name,
                            //    SeName = p.Product.GetSeName(),
                            //    Price = _priceFormatter.FormatPrice(p.Product.Price),
                            //    StockQuantity = p.Quantity,
                            //    TotalPrice = _priceFormatter.FormatPrice(p.Product.Price * p.Quantity),
                            //};
                            //model.ProductAttributesModels.Add(pam);
                        }
                        else
                        {
                            if (finalWholesalerPrice < finalPrice && finalWholesalerPrice != decimal.Zero)
                                totalPrice += finalWholesalerPrice * p.Quantity;
                            //model.TotalPrice = _priceFormatter.FormatPrice(finalWholesalerPrice * addedProduct.Quantity);
                            else
                                totalPrice += finalPrice * p.Quantity;
                            //model.TotalPrice = _priceFormatter.FormatPrice(finalPrice * addedProduct.Quantity);
                        }
                    }
                    model.TotalPrice = _priceFormatter.FormatPrice(totalPrice);
                }
                else
                {
                    model.StockQuantity = 0;
                    model.TotalPrice = _priceFormatter.FormatPrice(0);
                }
                models.Add(model);
            }
            if (direction == "up")
            {
                models.OrderBy(x => x.StockQuantity);
            }
            else if (direction == "down")
            {
                models.OrderByDescending(x => x.StockQuantity);
            }
            return models;
        }


        [NonAction]
        protected List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var customerRolesIds = _workContext.CurrentCustomer.CustomerRoles
                .Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY, parentCategoryId, string.Join(",", customerRolesIds), _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            });
        }

        #endregion


        public decimal WholesalerPriceWithDifaultAttributes(Product product, out decimal price)
        {
            price = 0;
            decimal wPriceD = 0;
            //OldPrice Price PriceWithDiscount PriceValue DisplayTaxShippingInfo 
            //BasePricePAngV CurrencyCode

            var dummyCustomer = new Customer();

            price = GetMainPrice(product, dummyCustomer);
            wPriceD = GetWholesalerPrice(product);


            //performance optimization
            //We cache a value indicating whether a product has attributes
            IList<ProductAttributeMapping> productAttributeMapping = null;
            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY, product.Id);
            var hasProductAttributesCache = _cacheManager.Get<bool?>(cacheKey);
            if (!hasProductAttributesCache.HasValue)
            {
                //no value in the cache yet
                //let's load attributes and cache the result (true/false)
                productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                hasProductAttributesCache = productAttributeMapping.Any();
                _cacheManager.Set(cacheKey, hasProductAttributesCache, 60);
            }
            if (hasProductAttributesCache.Value && productAttributeMapping == null)
            {
                //cache indicates that the product has attributes
                //let's load them
                productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            }
            if (productAttributeMapping == null)
            {
                productAttributeMapping = new List<ProductAttributeMapping>();
            }
            foreach (var attribute in productAttributeMapping.Where(x => x.IsRequired))
            {
                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                    var attributeValue = attributeValues.FirstOrDefault(x => x.IsPreSelected);
                    if (attributeValue == null)
                        attributeValue = attributeValues.OrderBy(x => x.DisplayOrder).FirstOrDefault();
                    //foreach (var attributeValue in attributeValues.Where(x => x.IsPreSelected)
                    //{
                    if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    {
                        var pAttribute = _productService.GetProductById(attributeValue.AssociatedProductId);

                        //decimal taxRate;
                        //decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out taxRate);
                        //decimal finalPriceWithoutDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: false), out taxRate);
                        //decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: true), out taxRate);

                        //decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                        //decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                        //decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                        //if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                        //    oldPriceS = _priceFormatter.FormatPrice(oldPrice);

                        //priceS = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                        //if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                        //    priceWithDiscountS = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                        if (pAttribute != null)
                        {
                            wPriceD += GetWholesalerPrice(pAttribute);
                            price += GetMainPrice(pAttribute, dummyCustomer);
                        }
                    }
                    else
                    {
                        //var valueModel = new ProductDetailsModel.ProductAttributeValueModel
                        //{
                        //    Id = attributeValue.Id,
                        //    Name = attributeValue.GetLocalized(x => x.Name),
                        //    ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                        //    IsPreSelected = attributeValue.IsPreSelected,
                        //    StoneNameId = attributeValue.StoneNameId,
                        //    ZIndex = attributeValue.ZIndex
                        //};
                        //attributeModel.Values.Add(valueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            //PriceAdjustment PriceAdjustmentValue
                            decimal taxRate;
                            decimal attributeValuePriceAdjustment = _priceCalculationService.GetProductAttributeValuePriceAdjustment(attributeValue);
                            decimal priceAdjustmentBase = _taxService.GetProductPrice(product, attributeValuePriceAdjustment, out taxRate);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            //if (priceAdjustmentBase > decimal.Zero)
                            //    valueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment, false, false);
                            //else if (priceAdjustmentBase < decimal.Zero)
                            //    valueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment, false, false);

                            wPriceD += priceAdjustment;
                            price += priceAdjustment;
                        }
                    }
                }
            }
            
            return wPriceD;
        }

        private decimal GetMainPrice(Product product, Customer dummyCustomer)
        {
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices) && !(product.CustomerEntersPrice || product.CallForPrice))
            {
                decimal taxRate;
                decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out taxRate);
                decimal finalPriceWithoutDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, dummyCustomer, includeDiscounts: false), out taxRate);
                decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, dummyCustomer, includeDiscounts: true), out taxRate);

                decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                //if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                //    oldPriceS = _priceFormatter.FormatPrice(oldPrice);

                //priceS = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                //if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                //    priceWithDiscountS = _priceFormatter.FormatPrice(finalPriceWithDiscount);

                return finalPriceWithDiscount;

            }
            return decimal.Zero;
        }

        private decimal GetWholesalerPrice(Product product)
        {
            if (product.HasTierPrices && _permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                return _priceCalculationService.GetFinalPrice(product,
                                        _workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);
                //model.TierPrices = product.TierPrices
                //    .OrderBy(x => x.Quantity)
                //    .ToList()
                //    .FilterByStore(_storeContext.CurrentStore.Id)
                //    .FilterForCustomer(_workContext.CurrentCustomer)
                //    .RemoveDuplicatedQuantities()
                //    .Select(tierPrice =>
                //    {
                //        var m = new ProductDetailsModel.TierPriceModel
                //        {
                //            Quantity = tierPrice.Quantity,
                //        };
                //        decimal taxRate;
                //        decimal priceBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts, tierPrice.Quantity), out taxRate);
                //        decimal price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase, _workContext.WorkingCurrency);
                //        m.Price = _priceFormatter.FormatPrice(price, false, false);
                //        return m;
                //    })
                //    .ToList();
            }
            return decimal.Zero;
        }
    }

    public static class Sort
    {
        public static IEnumerable<ProductModel> SortBy(this IEnumerable<ProductModel> products, string sortName, string sortDirection)
        {
            if (products.Count() > 1 && products != null && sortDirection != "none")
            {
                switch (sortName)
                {
                    case "Name":
                        if (sortDirection == "top")
                            return products.OrderBy(x => x.Name).ToList();
                        else if (sortDirection == "bottomn")
                            return products.OrderByDescending(x => x.Name).ToList();
                        break;
                    case "Sku":
                        if (sortDirection == "top")
                            return products.OrderBy(x => x.Sku).ToList();
                        else if (sortDirection == "bottomn")
                            return products.OrderByDescending(x => x.Sku).ToList();
                        break;
                    case "RetailPrice":
                        if (sortDirection == "top")
                            return products.OrderBy(x => x.RetailPrice).ToList();
                        else if (sortDirection == "bottomn")
                            return products.OrderByDescending(x => x.RetailPrice).ToList();
                        break;
                    case "WholesalerPrice":
                        if (sortDirection == "top")
                            return products.OrderBy(x => x.WholesalerPrice).ToList();
                        else if (sortDirection == "bottomn")
                            return products.OrderByDescending(x => x.WholesalerPrice).ToList();
                        break;
                }
            }
            return products;
        }
    }
}