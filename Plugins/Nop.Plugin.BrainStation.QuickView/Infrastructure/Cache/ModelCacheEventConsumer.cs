using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Events;

namespace Nop.Plugin.BrainStation.QuickView.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        IConsumer<EntityInserted<Setting>>,
        IConsumer<EntityUpdated<Setting>>,
        IConsumer<EntityDeleted<Setting>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// </remarks>
        public const string PICTURE_URL_MODEL_KEY = "Nop.plugins.widgets.nivosrlider.pictureurl-{0}";
        public const string PICTURE_URL_PATTERN_KEY = "Nop.plugins.widgets.nivosrlider";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTATTRIBUTE_PICTURE_MODEL_KEY = "Nop.pres.productattribute.picture-{0}-{1}-{2}";
        public const string PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY = "Nop.pres.productattribute.picture";
        /// <summary>
        /// Key for ProductBreadcrumbModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : comma separated list of customer roles
        /// {3} : current store ID
        /// </remarks>
        public const string PRODUCT_BREADCRUMB_MODEL_KEY = "Nop.pres.product.breadcrumb-{0}-{1}-{2}-{3}";
        public const string PRODUCT_BREADCRUMB_PATTERN_KEY = "Nop.pres.product.breadcrumb";


        /// <summary>
        /// Key for ProductTagModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTTAG_BY_PRODUCT_MODEL_KEY = "Nop.pres.producttag.byproduct-{0}-{1}-{2}";
        public const string PRODUCTTAG_BY_PRODUCT_PATTERN_KEY = "Nop.pres.producttag.byproduct";


        /// <summary>
        /// Key for ProductTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : product template id
        /// </remarks>
        public const string PRODUCT_TEMPLATE_MODEL_KEY = "Nop.pres.producttemplate-{0}";
        public const string PRODUCT_TEMPLATE_PATTERN_KEY = "Nop.pres.producttemplate";


        /// <summary>
        /// Key for product picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string PRODUCT_DETAILS_PICTURES_MODEL_KEY = "Nop.pres.product.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string PRODUCT_DETAILS_TPICTURES_PATTERN_KEY = "Nop.pres.product.picture";


        /// <summary>
        /// Key for caching of a value indicating whether a product has product attributes
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY = "Nop.pres.product.hasproductattributes-{0}";
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY = "Nop.pres.product.hasproductattributes";


        /// <summary>
        /// Key for ProductManufacturers model caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string PRODUCT_MANUFACTURERS_MODEL_KEY = "Nop.pres.product.manufacturers-{0}-{1}-{2}-{3}";
        public const string PRODUCT_MANUFACTURERS_PATTERN_KEY = "Nop.pres.product.manufacturers";

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTS_RELATED_IDS_KEY = "Nop.pres.related-{0}-{1}";
        public const string PRODUCTS_RELATED_IDS_PATTERN_KEY = "Nop.pres.related";

        /// <summary>
        /// Key for "also purchased" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTS_ALSO_PURCHASED_IDS_KEY = "Nop.pres.alsopuchased-{0}-{1}";
        public const string PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY = "Nop.pres.alsopuchased";

        //rel
        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_MODEL_KEY = "Nop.pres.productattribute.imagesquare.picture-{0}-{1}-{2}";
        public const string PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_PATTERN_KEY = "Nop.pres.productattribute.imagesquare.picture";
        //rel
        private readonly ICacheManager _cacheManager;

        public ModelCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }

        public void HandleEvent(EntityInserted<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(PICTURE_URL_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(PICTURE_URL_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(PICTURE_URL_PATTERN_KEY);
        }
    }
}
