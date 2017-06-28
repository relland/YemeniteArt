using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using OfficeOpenXml;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreContext _storeContext;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IEncryptionService _encryptionService;
        private readonly IDataProvider _dataProvider;
        private readonly MediaSettings _mediaSettings;
        private readonly IVendorService _vendorService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IShippingService _shippingService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IMeasureService _measureService;
        private readonly CatalogSettings _catalogSettings;
        //rel
        private readonly IProductAttributeParser _productAttributeParser;

        #endregion

        #region Ctor

        public ImportManager(IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IStoreContext storeContext,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IEncryptionService encryptionService,
            IDataProvider dataProvider,
            MediaSettings mediaSettings,
            IVendorService vendorService,
            IProductTemplateService productTemplateService,
            IShippingService shippingService,
            ITaxCategoryService taxCategoryService,
            IMeasureService measureService,
            IProductAttributeService productAttributeService,
            CatalogSettings catalogSettings,
            IProductAttributeParser productAttributeParser)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._urlRecordService = urlRecordService;
            this._storeContext = storeContext;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._encryptionService = encryptionService;
            this._dataProvider = dataProvider;
            this._mediaSettings = mediaSettings;
            this._vendorService = vendorService;
            this._productTemplateService = productTemplateService;
            this._shippingService = shippingService;
            this._taxCategoryService = taxCategoryService;
            this._measureService = measureService;
            this._productAttributeService = productAttributeService;
            this._catalogSettings = catalogSettings;
            this._productAttributeParser = productAttributeParser;
        }

        #endregion

        #region Utilities

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string ConvertColumnToString(object columnValue)
        {
            if (columnValue == null)
                return null;

            return Convert.ToString(columnValue);
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            var mimeType = MimeMapping.GetMimeMapping(filePath);

            //little hack here because MimeMapping does not contain all mappings (e.g. PNG)
            if (mimeType == MimeTypes.ApplicationOctetStream)
                mimeType = MimeTypes.ImageJpeg;

            return mimeType;
        }

        /// <summary>
        /// Creates or loads the image
        /// </summary>
        /// <param name="picturePath">The path to the image file</param>
        /// <param name="name">The name of the object</param>
        /// <param name="picId">Image identifier, may be null</param>
        /// <returns>The image or null if the image has not changed</returns>
        protected virtual Picture LoadPicture(string picturePath, string name, int? picId = null)
        {
            if (String.IsNullOrEmpty(picturePath) || !File.Exists(picturePath))
                return null;

            var mimeType = GetMimeTypeFromFilePath(picturePath);
            var newPictureBinary = File.ReadAllBytes(picturePath);
            var pictureAlreadyExists = false;
            if (picId != null)
            {
                //compare with existing product pictures
                var existingPicture = _pictureService.GetPictureById(picId.Value);

                var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                //picture binary after validation (like in database)
                var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                    existingBinary.SequenceEqual(newPictureBinary))
                {
                    pictureAlreadyExists = true;
                }
            }

            if (pictureAlreadyExists) return null;

            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType,
                _pictureService.GetPictureSeName(name));
            return newPicture;
        }

        protected virtual void ImportProductImagesUsingServices(IList<ProductPictureMetadata> productPictureMetadata)
        {
            foreach (var product in productPictureMetadata)
            {
                //rel
                var index = 0;
                foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
                {
                    if (String.IsNullOrEmpty(picturePath))
                        continue;

                    var mimeType = GetMimeTypeFromFilePath(picturePath);
                    var newPictureBinary = File.ReadAllBytes(picturePath);
                    var pictureAlreadyExists = false;
                    if (!product.IsNew)
                    {
                        //compare with existing product pictures
                        var existingPictures = _pictureService.GetPicturesByProductId(product.ProductItem.Id);
                        foreach (var existingPicture in existingPictures)
                        {
                            var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                            //picture binary after validation (like in database)
                            var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                            if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                                !existingBinary.SequenceEqual(newPictureBinary))
                                continue;
                            //the same picture content
                            pictureAlreadyExists = true;
                            //rel
                            var exsistingProductImage = product.ProductItem.ProductPictures.FirstOrDefault(x => x.PictureId == existingPicture.Id);
                            if (exsistingProductImage != null)
                            {
                                exsistingProductImage.DisplayOrder = index++;
                                _productService.UpdateProduct(product.ProductItem);
                            }
                            //end rel
                            break;
                        }
                    }

                    if (pictureAlreadyExists)
                        continue;
                    var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                    product.ProductItem.ProductPictures.Add(new ProductPicture
                    {
                        //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                        //pictures are duplicated
                        //maybe because entity size is too large
                        PictureId = newPicture.Id,
                        DisplayOrder = index++,
                    });
                    _productService.UpdateProduct(product.ProductItem);
                }
            }
        }

        //rel test
        protected virtual void ImportProductImagesUsingServices_Test(IList<ProductPictureMetadata> productPictureMetadata, out string errors)
        {
            errors = "";
            foreach (var product in productPictureMetadata)
            {
                //rel
                var index = 0;
                foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
                {
                    if (String.IsNullOrEmpty(picturePath))
                        continue;
                    try
                    {
                        var mimeType = GetMimeTypeFromFilePath(picturePath);
                        var newPictureBinary = File.ReadAllBytes(picturePath);
                        var pictureAlreadyExists = false;
                        if (!product.IsNew)
                        {
                            //compare with existing product pictures
                            var existingPictures = _pictureService.GetPicturesByProductId(product.ProductItem.Id);
                            foreach (var existingPicture in existingPictures)
                            {
                                var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                                //picture binary after validation (like in database)
                                var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                                if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                                    !existingBinary.SequenceEqual(newPictureBinary))
                                    continue;
                                //the same picture content
                                pictureAlreadyExists = true;
                                //rel
                                var exsistingProductImage = product.ProductItem.ProductPictures.FirstOrDefault(x => x.PictureId == existingPicture.Id);
                                if (exsistingProductImage != null)
                                {
                                    exsistingProductImage.DisplayOrder = index++;
                                    _productService.UpdateProduct(product.ProductItem);
                                }
                                //end rel
                                break;
                            }
                        }

                        if (pictureAlreadyExists)
                            continue;
                        var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                        product.ProductItem.ProductPictures.Add(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = index++,
                        });
                        _productService.UpdateProduct(product.ProductItem);
                    }
                    catch (Exception e)
                    {
                        errors += "Picture path error: '" + picturePath + "' - " + e.Message;
                    }
                }
            }
        }
        //rel
        protected virtual void ImportProductAndAttributeValueImagesUsingServices(List<ProductAttributeValueMetadata> list)
        {
            foreach (var product in list)
            {
                var selectedProductPictureId = 0;
                if (!String.IsNullOrEmpty(product.BigPicture_ProductPicturePath))
                {
                    var mimeType = GetMimeTypeFromFilePath(product.BigPicture_ProductPicturePath);
                    var newPictureBinary = File.ReadAllBytes(product.BigPicture_ProductPicturePath);
                    var pictureAlreadyExists = false;
                    //compare with existing product pictures
                    var existingPictures = _pictureService.GetPicturesByProductId(product.ProductItem.Id);
                    foreach (var existingPicture in existingPictures)
                    {
                        var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                        //picture binary after validation (like in database)
                        var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                        if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                            !existingBinary.SequenceEqual(newPictureBinary))
                            continue;
                        //the same picture content
                        pictureAlreadyExists = true;
                        selectedProductPictureId = existingPicture.Id;
                        var exsistingProductImage = product.ProductItem.ProductPictures.FirstOrDefault(x => x.PictureId == existingPicture.Id);
                        if (exsistingProductImage != null)
                        {
                            exsistingProductImage.DisplayOrder = 10;//rel - so it won't be first
                            _productService.UpdateProduct(product.ProductItem);
                        }
                        break;
                    }

                    if (!pictureAlreadyExists)
                    {
                        var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                        product.ProductItem.ProductPictures.Add(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 10,//rel - so it won't be first
                        });
                        _productService.UpdateProduct(product.ProductItem);
                    }
                }

                if (!string.IsNullOrWhiteSpace(product.SmallPicture_PAV_BoxPicturePath))
                {
                    var mimeType = GetMimeTypeFromFilePath(product.SmallPicture_PAV_BoxPicturePath);
                    var newPictureBinary = File.ReadAllBytes(product.SmallPicture_PAV_BoxPicturePath);
                    var pictureAlreadyExists = false;
                    if (product.ProductAttributeValue.ImageSquaresPictureId > 0)
                    {
                        var existingPicture = _pictureService.GetPictureById(product.ProductAttributeValue.ImageSquaresPictureId);
                        var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                        //picture binary after validation (like in database)
                        var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                        if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                            !existingBinary.SequenceEqual(newPictureBinary))
                        { }
                        else
                        {
                            //the same picture content
                            pictureAlreadyExists = true;
                        }
                    }

                    if (!pictureAlreadyExists)
                    {
                        var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductAttributeValue.Name));
                        product.ProductAttributeValue.ImageSquaresPictureId = newPicture.Id;
                        _productAttributeService.UpdateProductAttributeValue(product.ProductAttributeValue);
                    }
                }
                if (product.ProductAttributeValue.PictureId != selectedProductPictureId)
                {
                    product.ProductAttributeValue.PictureId = selectedProductPictureId;
                    _productAttributeService.UpdateProductAttributeValue(product.ProductAttributeValue);
                }
            }

        }

        //rel - test
        protected virtual void ImportProductAndAttributeValueImagesUsingServices_Test(List<ProductAttributeValueMetadata> list, out string errors)
        {
            errors = "";
            foreach (var product in list)
            {
                var selectedProductPictureId = 0;
                if (!String.IsNullOrEmpty(product.BigPicture_ProductPicturePath))
                {
                    try
                    { 
                        var mimeType = GetMimeTypeFromFilePath(product.BigPicture_ProductPicturePath);
                        var newPictureBinary = File.ReadAllBytes(product.BigPicture_ProductPicturePath);
                        var pictureAlreadyExists = false;
                        //compare with existing product pictures
                        var existingPictures = _pictureService.GetPicturesByProductId(product.ProductItem.Id);
                        foreach (var existingPicture in existingPictures)
                        {
                            var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                            //picture binary after validation (like in database)
                            var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                            if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                                !existingBinary.SequenceEqual(newPictureBinary))
                                continue;
                            //the same picture content
                            pictureAlreadyExists = true;
                            selectedProductPictureId = existingPicture.Id;
                            var exsistingProductImage = product.ProductItem.ProductPictures.FirstOrDefault(x => x.PictureId == existingPicture.Id);
                            if (exsistingProductImage != null)
                            {
                                exsistingProductImage.DisplayOrder = 10;//rel - so it won't be first
                                _productService.UpdateProduct(product.ProductItem);
                            }
                            break;
                        }

                        if (!pictureAlreadyExists)
                        {
                            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                            product.ProductItem.ProductPictures.Add(new ProductPicture
                            {
                                //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                                //pictures are duplicated
                                //maybe because entity size is too large
                                PictureId = newPicture.Id,
                                DisplayOrder = 10,//rel - so it won't be first
                            });
                            _productService.UpdateProduct(product.ProductItem);
                        }
                    }
                    catch (Exception e)
                    {
                        errors += "BigPicture error: '" + product.BigPicture_ProductPicturePath + "' - " + e.Message;
                    }
                }

                if (!string.IsNullOrWhiteSpace(product.SmallPicture_PAV_BoxPicturePath))
                {
                    try
                    { 
                        var mimeType = GetMimeTypeFromFilePath(product.SmallPicture_PAV_BoxPicturePath);
                        var newPictureBinary = File.ReadAllBytes(product.SmallPicture_PAV_BoxPicturePath);
                        var pictureAlreadyExists = false;
                        if (product.ProductAttributeValue.ImageSquaresPictureId > 0)
                        {
                            var existingPicture = _pictureService.GetPictureById(product.ProductAttributeValue.ImageSquaresPictureId);
                            var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                            //picture binary after validation (like in database)
                            var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                            if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                                !existingBinary.SequenceEqual(newPictureBinary))
                            { }
                            else
                            {
                                //the same picture content
                                pictureAlreadyExists = true;
                            }
                        }

                        if (!pictureAlreadyExists)
                        {
                            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductAttributeValue.Name));
                            product.ProductAttributeValue.ImageSquaresPictureId = newPicture.Id;
                            _productAttributeService.UpdateProductAttributeValue(product.ProductAttributeValue);
                        }
                    }
                    catch (Exception e)
                    {
                        errors += "SmallPicture error: '" + product.SmallPicture_PAV_BoxPicturePath + "' - " + e.Message;
                    }
                }
                if (product.ProductAttributeValue.PictureId != selectedProductPictureId)
                {
                    product.ProductAttributeValue.PictureId = selectedProductPictureId;
                    _productAttributeService.UpdateProductAttributeValue(product.ProductAttributeValue);
                }
            }

        }
        protected virtual void ImportProductImagesUsingHash(IList<ProductPictureMetadata> productPictureMetadata, IList<Product> allProductsBySku)
        {
            //performance optimization, load all pictures hashes
            //it will only be used if the images are stored in the SQL Server database (not compact)
            var takeCount = _dataProvider.SupportedLengthOfBinaryHash() - 1;
            var productsImagesIds = _productService.GetProductsImagesIds(allProductsBySku.Select(p => p.Id).ToArray());
            var allPicturesHashes = _pictureService.GetPicturesHash(productsImagesIds.SelectMany(p => p.Value).ToArray());

            foreach (var product in productPictureMetadata)
            {
                var index = 0;
                foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
                {
                    if (String.IsNullOrEmpty(picturePath))
                        continue;

                    var mimeType = GetMimeTypeFromFilePath(picturePath);
                    var newPictureBinary = File.ReadAllBytes(picturePath);
                    var pictureAlreadyExists = false;
                    if (!product.IsNew)
                    {
                        var newImageHash = _encryptionService.CreateHash(newPictureBinary.Take(takeCount).ToArray());
                        var newValidatedImageHash = _encryptionService.CreateHash(_pictureService.ValidatePicture(newPictureBinary, mimeType).Take(takeCount).ToArray());

                        var imagesIds = productsImagesIds.ContainsKey(product.ProductItem.Id)
                            ? productsImagesIds[product.ProductItem.Id]
                            : new int[0];

                        pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key)).Select(p => p.Value).Any(p => p == newImageHash || p == newValidatedImageHash);
                        //rel TryUpdateExistingImageDisplayOrder(pictureAlreadyExists, imagesIds, allPicturesHashes, newImageHash, newValidatedImageHash, index);
                        if (pictureAlreadyExists)
                        {
                            var exsistingImage = allPicturesHashes.FirstOrDefault(p => imagesIds.Contains(p.Key) && (p.Value == newImageHash || p.Value == newValidatedImageHash));
                            var exsistingImageId = exsistingImage.Key;
                            var exsistingProductImage = product.ProductItem.ProductPictures.FirstOrDefault(x => x.PictureId == exsistingImageId);
                            if (exsistingProductImage != null)
                            {
                                exsistingProductImage.DisplayOrder = index++;
                                _productService.UpdateProduct(product.ProductItem);
                            }
                        }
                        //end rel
                    }

                    if (pictureAlreadyExists)
                        continue;
                    var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                    product.ProductItem.ProductPictures.Add(new ProductPicture
                    {
                        //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                        //pictures are duplicated
                        //maybe because entity size is too large
                        PictureId = newPicture.Id,
                        DisplayOrder = index++,
                    });
                    _productService.UpdateProduct(product.ProductItem);
                }
            }
        }

        //rel - test
        protected virtual void ImportProductImagesUsingHash_Test(IList<ProductPictureMetadata> productPictureMetadata, IList<Product> allProductsBySku, out string errors)
        {
            //performance optimization, load all pictures hashes
            //it will only be used if the images are stored in the SQL Server database (not compact)
            var takeCount = _dataProvider.SupportedLengthOfBinaryHash() - 1;
            var productsImagesIds = _productService.GetProductsImagesIds(allProductsBySku.Select(p => p.Id).ToArray());
            var allPicturesHashes = _pictureService.GetPicturesHash(productsImagesIds.SelectMany(p => p.Value).ToArray());
            errors = "";
            foreach (var product in productPictureMetadata)
            {
                var index = 0;
                foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
                {
                    if (String.IsNullOrEmpty(picturePath))
                        continue;
                    try
                    {
                        var mimeType = GetMimeTypeFromFilePath(picturePath);
                        var newPictureBinary = File.ReadAllBytes(picturePath);
                        var pictureAlreadyExists = false;
                        if (!product.IsNew)
                        {
                            var newImageHash = _encryptionService.CreateHash(newPictureBinary.Take(takeCount).ToArray());
                            var newValidatedImageHash = _encryptionService.CreateHash(_pictureService.ValidatePicture(newPictureBinary, mimeType).Take(takeCount).ToArray());

                            var imagesIds = productsImagesIds.ContainsKey(product.ProductItem.Id)
                                ? productsImagesIds[product.ProductItem.Id]
                                : new int[0];

                            pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key)).Select(p => p.Value).Any(p => p == newImageHash || p == newValidatedImageHash);
                            //rel TryUpdateExistingImageDisplayOrder(pictureAlreadyExists, imagesIds, allPicturesHashes, newImageHash, newValidatedImageHash, index);
                            if (pictureAlreadyExists)
                            {
                                var exsistingImage = allPicturesHashes.FirstOrDefault(p => imagesIds.Contains(p.Key) && (p.Value == newImageHash || p.Value == newValidatedImageHash));
                                var exsistingImageId = exsistingImage.Key;
                                var exsistingProductImage = product.ProductItem.ProductPictures.FirstOrDefault(x => x.PictureId == exsistingImageId);
                                if (exsistingProductImage != null)
                                {
                                    exsistingProductImage.DisplayOrder = index++;
                                    _productService.UpdateProduct(product.ProductItem);
                                }
                            }
                            //end rel
                        }

                        if (pictureAlreadyExists)
                            continue;
                        var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                        product.ProductItem.ProductPictures.Add(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = index++,
                        });
                        _productService.UpdateProduct(product.ProductItem);
                    }
                    catch (Exception e)
                    {
                        errors += "Picture path error: '" + picturePath + "' - " + e.Message;
                    }
                }
            }
        }
        //rel
        protected virtual void ImportProductAndAttributeValueImagesUsingHash(List<ProductAttributeValueMetadata> list, IList<Product> allProductsBySku)
        {
            //performance optimization, load all pictures hashes
            //it will only be used if the images are stored in the SQL Server database (not compact)
            var takeCount = _dataProvider.SupportedLengthOfBinaryHash() - 1;
            var productsImagesIds = _productService.GetProductsImagesIds(allProductsBySku.Select(p => p.Id).ToArray());
            var allPicturesHashes = _pictureService.GetPicturesHash(productsImagesIds.SelectMany(p => p.Value).ToArray());

            foreach (var product in list)
            {
                var selectedProductPictureId = 0;
                if (!string.IsNullOrEmpty(product.BigPicture_ProductPicturePath))
                {

                    var mimeType = GetMimeTypeFromFilePath(product.BigPicture_ProductPicturePath);
                    var newPictureBinary = File.ReadAllBytes(product.BigPicture_ProductPicturePath);
                    var pictureAlreadyExists = false;
                    if (!product.ProductIsNew)
                    {
                        var newImageHash = _encryptionService.CreateHash(newPictureBinary.Take(takeCount).ToArray());
                        var newValidatedImageHash = _encryptionService.CreateHash(_pictureService.ValidatePicture(newPictureBinary, mimeType).Take(takeCount).ToArray());

                        var imagesIds = productsImagesIds.ContainsKey(product.ProductItem.Id)
                            ? productsImagesIds[product.ProductItem.Id]
                            : new int[0];

                        pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key)).Select(p => p.Value).Any(p => p == newImageHash || p == newValidatedImageHash);
                        selectedProductPictureId = allPicturesHashes.Where(p => imagesIds.Contains(p.Key) && (p.Value == newImageHash || p.Value == newValidatedImageHash)).Count() > 0 ?
                            allPicturesHashes.FirstOrDefault(p => imagesIds.Contains(p.Key) && (p.Value == newImageHash || p.Value == newValidatedImageHash)).Key : 0;

                        var exsistingImage = allPicturesHashes.FirstOrDefault(p => imagesIds.Contains(p.Key) && (p.Value == newImageHash || p.Value == newValidatedImageHash));
                        var exsistingImageId = exsistingImage.Key;
                        var exsistingProductImage = product.ProductItem.ProductPictures.FirstOrDefault(x => x.PictureId == exsistingImageId);
                        if (exsistingProductImage != null)
                        {
                            exsistingProductImage.DisplayOrder = 10;//rel - so it won't be first
                            _productService.UpdateProduct(product.ProductItem);
                        }
                    }

                    if (!pictureAlreadyExists)
                    {
                        var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                        selectedProductPictureId = newPicture.Id;
                        product.ProductItem.ProductPictures.Add(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 10,//rel - so it won't be first
                        });
                        _productService.UpdateProduct(product.ProductItem);
                    }
                }
                if (!string.IsNullOrWhiteSpace(product.SmallPicture_PAV_BoxPicturePath))
                {
                    var mimeType = GetMimeTypeFromFilePath(product.SmallPicture_PAV_BoxPicturePath);
                    var newPictureBinary = File.ReadAllBytes(product.SmallPicture_PAV_BoxPicturePath);
                    var attrPictureAlreadyExists = false;
                    if (!product.AttributeValueIsNew)
                    {
                        var productAttributeValuePicturesHashe = _pictureService.GetPicturesHash(new[] { product.ProductAttributeValue.ImageSquaresPictureId });
                        var newImageHash = _encryptionService.CreateHash(newPictureBinary.Take(takeCount).ToArray());
                        var newValidatedImageHash = _encryptionService.CreateHash(_pictureService.ValidatePicture(newPictureBinary, mimeType).Take(takeCount).ToArray());

                        if (productAttributeValuePicturesHashe.Count() > 0)
                            attrPictureAlreadyExists = productAttributeValuePicturesHashe.FirstOrDefault().Value == newImageHash || productAttributeValuePicturesHashe.FirstOrDefault().Value == newValidatedImageHash;
                    }

                    if (!attrPictureAlreadyExists || selectedProductPictureId > 0)
                    {
                        if (!attrPictureAlreadyExists)
                        {
                            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductAttributeValue.Name));
                            product.ProductAttributeValue.ImageSquaresPictureId = newPicture.Id;
                        }
                        if (selectedProductPictureId > 0)
                            product.ProductAttributeValue.PictureId = selectedProductPictureId;
                        _productAttributeService.UpdateProductAttributeValue(product.ProductAttributeValue);
                    }
                }
                else if (selectedProductPictureId > 0)
                {
                    product.ProductAttributeValue.PictureId = selectedProductPictureId;
                    _productAttributeService.UpdateProductAttributeValue(product.ProductAttributeValue);
                }
            }
        }

        //rel - test
        protected virtual void ImportProductAndAttributeValueImagesUsingHash_Test(List<ProductAttributeValueMetadata> list, IList<Product> allProductsBySku, out string errors)
        {
            //performance optimization, load all pictures hashes
            //it will only be used if the images are stored in the SQL Server database (not compact)
            var takeCount = _dataProvider.SupportedLengthOfBinaryHash() - 1;
            var productsImagesIds = _productService.GetProductsImagesIds(allProductsBySku.Select(p => p.Id).ToArray());
            var allPicturesHashes = _pictureService.GetPicturesHash(productsImagesIds.SelectMany(p => p.Value).ToArray());
            errors = "";
            foreach (var product in list)
            {
                var selectedProductPictureId = 0;
                if (!string.IsNullOrEmpty(product.BigPicture_ProductPicturePath))
                {
                    try
                    { 
                        var mimeType = GetMimeTypeFromFilePath(product.BigPicture_ProductPicturePath);
                        var newPictureBinary = File.ReadAllBytes(product.BigPicture_ProductPicturePath);
                        var pictureAlreadyExists = false;
                        if (!product.ProductIsNew)
                        {
                            var newImageHash = _encryptionService.CreateHash(newPictureBinary.Take(takeCount).ToArray());
                            var newValidatedImageHash = _encryptionService.CreateHash(_pictureService.ValidatePicture(newPictureBinary, mimeType).Take(takeCount).ToArray());

                            var imagesIds = productsImagesIds.ContainsKey(product.ProductItem.Id)
                                ? productsImagesIds[product.ProductItem.Id]
                                : new int[0];

                            pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key)).Select(p => p.Value).Any(p => p == newImageHash || p == newValidatedImageHash);
                            selectedProductPictureId = allPicturesHashes.Where(p => imagesIds.Contains(p.Key) && (p.Value == newImageHash || p.Value == newValidatedImageHash)).Count() > 0 ?
                                allPicturesHashes.FirstOrDefault(p => imagesIds.Contains(p.Key) && (p.Value == newImageHash || p.Value == newValidatedImageHash)).Key : 0;

                            var exsistingImage = allPicturesHashes.FirstOrDefault(p => imagesIds.Contains(p.Key) && (p.Value == newImageHash || p.Value == newValidatedImageHash));
                            var exsistingImageId = exsistingImage.Key;
                            var exsistingProductImage = product.ProductItem.ProductPictures.FirstOrDefault(x => x.PictureId == exsistingImageId);
                            if (exsistingProductImage != null)
                            {
                                exsistingProductImage.DisplayOrder = 10;//rel - so it won't be first
                                _productService.UpdateProduct(product.ProductItem);
                            }
                        }

                        if (!pictureAlreadyExists)
                        {
                            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductItem.Name));
                            selectedProductPictureId = newPicture.Id;
                            product.ProductItem.ProductPictures.Add(new ProductPicture
                            {
                                //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                                //pictures are duplicated
                                //maybe because entity size is too large
                                PictureId = newPicture.Id,
                                DisplayOrder = 10,//rel - so it won't be first
                            });
                            _productService.UpdateProduct(product.ProductItem);
                        }
                    }
                    catch (Exception e)
                    {
                        errors += "BigPicture error: '" + product.BigPicture_ProductPicturePath + "' - " + e.Message;
                    }
                }
                if (!string.IsNullOrWhiteSpace(product.SmallPicture_PAV_BoxPicturePath))
                {
                    try
                    { 
                        var mimeType = GetMimeTypeFromFilePath(product.SmallPicture_PAV_BoxPicturePath);
                        var newPictureBinary = File.ReadAllBytes(product.SmallPicture_PAV_BoxPicturePath);
                        var attrPictureAlreadyExists = false;
                        if (!product.AttributeValueIsNew)
                        {
                            var productAttributeValuePicturesHashe = _pictureService.GetPicturesHash(new[] { product.ProductAttributeValue.ImageSquaresPictureId });
                            var newImageHash = _encryptionService.CreateHash(newPictureBinary.Take(takeCount).ToArray());
                            var newValidatedImageHash = _encryptionService.CreateHash(_pictureService.ValidatePicture(newPictureBinary, mimeType).Take(takeCount).ToArray());

                            if (productAttributeValuePicturesHashe.Count() > 0)
                                attrPictureAlreadyExists = productAttributeValuePicturesHashe.FirstOrDefault().Value == newImageHash || productAttributeValuePicturesHashe.FirstOrDefault().Value == newValidatedImageHash;
                        }

                        if (!attrPictureAlreadyExists || selectedProductPictureId > 0)
                        {
                            if (!attrPictureAlreadyExists)
                            {
                                var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.ProductAttributeValue.Name));
                                product.ProductAttributeValue.ImageSquaresPictureId = newPicture.Id;
                            }
                            if (selectedProductPictureId > 0)
                                product.ProductAttributeValue.PictureId = selectedProductPictureId;
                            _productAttributeService.UpdateProductAttributeValue(product.ProductAttributeValue);
                        }
                    }
                    catch (Exception e)
                    {
                        errors += "SmallPicture error: '" + product.SmallPicture_PAV_BoxPicturePath + "' - " + e.Message;
                    }
                }
                else if (selectedProductPictureId > 0)
                {
                    product.ProductAttributeValue.PictureId = selectedProductPictureId;
                    _productAttributeService.UpdateProductAttributeValue(product.ProductAttributeValue);
                }
            }
        }

        //rel
        private void AddConditions(List<Conditions> conditionsList)
        {
            foreach (var condition in conditionsList.Where(x => x.HasCondition))
            {
                var currentProductAttributeMapping = _productAttributeService.GetProductAttributeMappingById(condition.ProductAttributeMappingId);
                var productAttributeMappingConditionId = conditionsList.FirstOrDefault(x => x.RowValueId == condition.RowValueIdForCondition)?.ProductAttributeMappingId;
                var valueId = conditionsList.FirstOrDefault(x => x.RowValueId == condition.RowValueIdForCondition)?.AttributeValueId;
                if (productAttributeMappingConditionId.HasValue && productAttributeMappingConditionId > 0)
                {
                    var productAttributeMappingCondition = _productAttributeService.GetProductAttributeMappingById(productAttributeMappingConditionId.Value);
                    var value = condition.RowValueIsSelected ? valueId.HasValue ? valueId.ToString() : "" : "";
                    string attributesXml = _productAttributeParser.AddProductAttribute("", productAttributeMappingCondition, value);
                    currentProductAttributeMapping.ConditionAttributeXml = attributesXml;
                    _productAttributeService.UpdateProductAttributeMapping(currentProductAttributeMapping);
                }
            }
        }
        //rel end
        #endregion

        #region Methods

        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void REL_ImportProductsFromXlsx(Stream stream, string basePath)
        {
            //var start = DateTime.Now;
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = new List<PropertyByName<Product>>();
                var poz = 1;
                while (true)
                {
                    try
                    {
                        var cell = worksheet.Cells[1, poz];

                        if (cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()))
                            break;

                        poz += 1;
                        properties.Add(new PropertyByName<Product>(cell.Value.ToString()));
                    }
                    catch
                    {
                        break;
                    }
                }

                var manager = new PropertyManager<Product>(properties.ToArray());

                var attributProperties = new[]
                {
                    new PropertyByName<ExportProductAttribute>("AttributeId"),
                    new PropertyByName<ExportProductAttribute>("AttributeName"),
                    //rel                
                    new PropertyByName<ExportProductAttribute>("HasCondition"),
                    new PropertyByName<ExportProductAttribute>("RowValueIdForCondition"),
                    new PropertyByName<ExportProductAttribute>("RowValueIsSelected"),
                    new PropertyByName<ExportProductAttribute>("RowValueId"),
                    //rel
                    new PropertyByName<ExportProductAttribute>("AttributeTextPrompt"),
                    //rel                
                    new PropertyByName<ExportProductAttribute>("IsStoneAttribute"),
                    //rel
                    new PropertyByName<ExportProductAttribute>("AttributeIsRequired"),
                    new PropertyByName<ExportProductAttribute>("AttributeControlType")
                    {
                        DropDownElements = AttributeControlType.TextBox.ToSelectList(useLocalization: false)
                    },
                    new PropertyByName<ExportProductAttribute>("AttributeDisplayOrder"),
                    new PropertyByName<ExportProductAttribute>("ProductAttributeValueId"),
                    new PropertyByName<ExportProductAttribute>("ValueName"),
                    new PropertyByName<ExportProductAttribute>("AttributeValueType")
                    {
                        DropDownElements = AttributeValueType.Simple.ToSelectList(useLocalization: false)
                    },
                    new PropertyByName<ExportProductAttribute>("AssociatedProductId"),
                    new PropertyByName<ExportProductAttribute>("Quantity"),
                    //
                    new PropertyByName<ExportProductAttribute>("ColorSquaresRgb"),
                    new PropertyByName<ExportProductAttribute>("ImageSquaresPictureId"),
                    new PropertyByName<ExportProductAttribute>("PriceAdjustment"),
                    new PropertyByName<ExportProductAttribute>("WeightAdjustment"),
                    new PropertyByName<ExportProductAttribute>("Cost"),
                    new PropertyByName<ExportProductAttribute>("ZIndex"),
                    //new PropertyByName<ExportProductAttribute>("IsStoneProductAttributeValue"),
                    //rel
                    new PropertyByName<ExportProductAttribute>("IsPreSelected"),
                    new PropertyByName<ExportProductAttribute>("DisplayOrder"),
                    new PropertyByName<ExportProductAttribute>("PictureId"),
                    //
                    //rel                
                    new PropertyByName<ExportProductAttribute>("SKUCode"),
                    new PropertyByName<ExportProductAttribute>("StoneNameId"),
                    new PropertyByName<ExportProductAttribute>("BigPicture"),
                    new PropertyByName<ExportProductAttribute>("SmallPicture")

                };

                var conditionsList = new List<Conditions>();
                var managerProductAttribute = new PropertyManager<ExportProductAttribute>(attributProperties);

                var endRow = 2;
                var allCategoriesNames = new List<string>();
                var allSku = new List<string>();

                var tempProperty = manager.GetProperty("Categories");
                var categoryCellNum = tempProperty.Return(p => p.PropertyOrderPosition, -1);

                tempProperty = manager.GetProperty("SKU");
                var skuCellNum = tempProperty.Return(p => p.PropertyOrderPosition, -1);

                var allManufacturersNames = new List<string>();
                tempProperty = manager.GetProperty("Manufacturers");
                var manufacturerCellNum = tempProperty.Return(p => p.PropertyOrderPosition, -1);

                manager.SetSelectList("ProductType", ProductType.SimpleProduct.ToSelectList(useLocalization: false));
                manager.SetSelectList("GiftCardType", GiftCardType.Virtual.ToSelectList(useLocalization: false));
                manager.SetSelectList("DownloadActivationType", DownloadActivationType.Manually.ToSelectList(useLocalization: false));
                manager.SetSelectList("ManageInventoryMethod", ManageInventoryMethod.DontManageStock.ToSelectList(useLocalization: false));
                manager.SetSelectList("LowStockActivity", LowStockActivity.Nothing.ToSelectList(useLocalization: false));
                manager.SetSelectList("BackorderMode", BackorderMode.NoBackorders.ToSelectList(useLocalization: false));
                manager.SetSelectList("RecurringCyclePeriod", RecurringProductCyclePeriod.Days.ToSelectList(useLocalization: false));
                manager.SetSelectList("RentalPricePeriod", RentalPricePeriod.Days.ToSelectList(useLocalization: false));

                manager.SetSelectList("Vendor", _vendorService.GetAllVendors(showHidden: true).Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor).Return(v => v.Name, String.Empty)));
                manager.SetSelectList("ProductTemplate", _productTemplateService.GetAllProductTemplates().Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate).Return(pt => pt.Name, String.Empty)));
                manager.SetSelectList("DeliveryDate", _shippingService.GetAllDeliveryDates().Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate).Return(dd => dd.Name, String.Empty)));
                manager.SetSelectList("TaxCategory", _taxCategoryService.GetAllTaxCategories().Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory).Return(tc => tc.Name, String.Empty)));
                manager.SetSelectList("BasepriceUnit", _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight).Return(mw => mw.Name, String.Empty)));
                manager.SetSelectList("BasepriceBaseUnit", _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight).Return(mw => mw.Name, String.Empty)));

                var allAttributeIds = new List<int>();
                var attributeIdCellNum = managerProductAttribute.GetProperty("AttributeId").PropertyOrderPosition + ExportProductAttribute.ProducAttributeCellOffset;

                //find end of data
                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[endRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    if (new[] { 1, 2 }.Select(cellNum => worksheet.Cells[endRow, cellNum]).All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString())) && worksheet.Row(endRow).OutlineLevel == 0)
                    {
                        var cellValue = worksheet.Cells[endRow, attributeIdCellNum].Value;
                        try
                        {
                            var aid = cellValue.Return(Convert.ToInt32, -1);

                            var productAttribute = _productAttributeService.GetProductAttributeById(aid);

                            if (productAttribute != null)
                                worksheet.Row(endRow).OutlineLevel = 1;
                        }
                        catch (FormatException)
                        {
                            if (cellValue.Return(cv => cv.ToString(), String.Empty) == "AttributeId")
                                worksheet.Row(endRow).OutlineLevel = 1;
                        }
                    }
                    ////test
                    //var outlinelevel = worksheet.Row(endRow).OutlineLevel;
                    ////end test
                    if (worksheet.Row(endRow).OutlineLevel != 0)
                    {
                        managerProductAttribute.ReadFromXlsx(worksheet, endRow, ExportProductAttribute.ProducAttributeCellOffset);
                        if (!managerProductAttribute.IsCaption)
                        {
                            ////test
                            //var val = worksheet.Cells[endRow, attributeIdCellNum].Value;
                            ////end test
                            var aid = worksheet.Cells[endRow, attributeIdCellNum].Value.Return(Convert.ToInt32, -1);
                            allAttributeIds.Add(aid);
                        }

                        endRow++;
                        continue;
                    }

                    if (categoryCellNum > 0)
                    {
                        var categoryIds = worksheet.Cells[endRow, categoryCellNum].Value.Return(p => p.ToString(), string.Empty);

                        if (!categoryIds.IsEmpty())
                            allCategoriesNames.AddRange(categoryIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                    }

                    if (skuCellNum > 0)
                    {
                        var sku = worksheet.Cells[endRow, skuCellNum].Value.Return(p => p.ToString(), string.Empty);

                        if (!sku.IsEmpty())
                            allSku.Add(sku);
                    }

                    if (manufacturerCellNum > 0)
                    {
                        var manufacturerIds = worksheet.Cells[endRow, manufacturerCellNum].Value.Return(p => p.ToString(), string.Empty);
                        if (!manufacturerIds.IsEmpty())
                            allManufacturersNames.AddRange(manufacturerIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                    }

                    endRow++;
                }

                //performance optimization, the check for the existence of the categories in one SQL request
                var notExistingCategories = _categoryService.GetNotExistingCategories(allCategoriesNames.ToArray());
                if (notExistingCategories.Any())
                {
                    throw new ArgumentException(string.Format("The following category name(s) don't exist - {0}", string.Join(", ", notExistingCategories)));
                }

                //performance optimization, the check for the existence of the manufacturers in one SQL request
                var notExistingManufacturers = _manufacturerService.GetNotExistingManufacturers(allManufacturersNames.ToArray());
                if (notExistingManufacturers.Any())
                {
                    throw new ArgumentException(string.Format("The following manufacturer name(s) don't exist - {0}", string.Join(", ", notExistingManufacturers)));
                }

                //performance optimization, the check for the existence of the product attributes in one SQL request
                var notExistingProductAttributes = _productAttributeService.GetNotExistingAttributes(allAttributeIds.ToArray());
                if (notExistingProductAttributes.Any())
                {
                    throw new ArgumentException(string.Format("The following product attribute ID(s) don't exist - {0}", string.Join(", ", notExistingProductAttributes)));
                }

                //performance optimization, load all products by SKU in one SQL request
                var allProductsBySku = _productService.GetProductsBySku(allSku.ToArray());

                //performance optimization, load all categories IDs for products in one SQL request
                var allProductsCategoryIds = _categoryService.GetProductCategoryIds(allProductsBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all categories in one SQL request
                var allCategories = _categoryService.GetAllCategories(showHidden: true);

                //performance optimization, load all manufacturers IDs for products in one SQL request
                var allProductsManufacturerIds = _manufacturerService.GetProductManufacturerIds(allProductsBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all manufacturers in one SQL request
                var allManufacturers = _manufacturerService.GetAllManufacturers(showHidden: true);

                //product to import images
                var productPictureMetadata = new List<ProductPictureMetadata>();

                Product lastLoadedProduct = null;

                //rel
                var productAndAttrValuePictures = new List<ProductAttributeValueMetadata>();
                //rel

                for (var iRow = 2; iRow < endRow; iRow++)
                {
                    //imports product attributes
                    if (worksheet.Row(iRow).OutlineLevel != 0)
                    {
                        if (_catalogSettings.ExportImportProductAttributes)
                        {
                            managerProductAttribute.ReadFromXlsx(worksheet, iRow,
                                ExportProductAttribute.ProducAttributeCellOffset);
                            if (lastLoadedProduct == null || managerProductAttribute.IsCaption)
                                continue;

                            var productAttributeId = managerProductAttribute.GetProperty("AttributeId").IntValue;
                            var attributeControlTypeId = managerProductAttribute.GetProperty("AttributeControlType").IntValue;

                            //rel for conditions
                            var hasCondition = managerProductAttribute.GetProperty("HasCondition").BooleanValue;
                            var rowValueIdForCondition = managerProductAttribute.GetProperty("RowValueIdForCondition").IntValue;
                            var rowValueIsSelected = managerProductAttribute.GetProperty("RowValueIsSelected").BooleanValue;
                            var rowValueId = managerProductAttribute.GetProperty("RowValueId").IntValue;
                            //end rel

                            var productAttributeValueId = managerProductAttribute.GetProperty("ProductAttributeValueId").IntValue;
                            var associatedProductId = managerProductAttribute.GetProperty("AssociatedProductId").IntValue;
                            var valueName = managerProductAttribute.GetProperty("ValueName").StringValue;
                            var attributeValueTypeId = managerProductAttribute.GetProperty("AttributeValueType").IntValue;
                            var colorSquaresRgb = managerProductAttribute.GetProperty("ColorSquaresRgb").StringValue;
                            var imageSquaresPictureId = managerProductAttribute.GetProperty("ImageSquaresPictureId").IntValue;
                            var priceAdjustment = managerProductAttribute.GetProperty("PriceAdjustment").DecimalValue;
                            var weightAdjustment = managerProductAttribute.GetProperty("WeightAdjustment").DecimalValue;
                            var cost = managerProductAttribute.GetProperty("Cost").DecimalValue;
                            var quantity = managerProductAttribute.GetProperty("Quantity").IntValue;
                            var isPreSelected = managerProductAttribute.GetProperty("IsPreSelected").BooleanValue;
                            var displayOrder = managerProductAttribute.GetProperty("DisplayOrder").IntValue;
                            var pictureId = managerProductAttribute.GetProperty("PictureId").IntValue;
                            var textPrompt = managerProductAttribute.GetProperty("AttributeTextPrompt").StringValue;
                            var isRequired = managerProductAttribute.GetProperty("AttributeIsRequired").BooleanValue;
                            var isStoneAttribute = managerProductAttribute.GetProperty("IsStoneAttribute").BooleanValue;//rel
                            var attributeDisplayOrder = managerProductAttribute.GetProperty("AttributeDisplayOrder").IntValue;
                            //rel
                            //var isStoneProductAttributeValue = managerProductAttribute.GetProperty("IsStoneProductAttributeValue").BooleanValue;
                            var zIndex = managerProductAttribute.GetProperty("ZIndex").IntValue;
                            var skuCode = managerProductAttribute.GetProperty("SKUCode").StringValue;
                            var stoneNameId = managerProductAttribute.GetProperty("StoneNameId").StringValue;
                            var bigPicture = string.IsNullOrWhiteSpace(managerProductAttribute.GetProperty("BigPicture").StringValue) ? "" : basePath + managerProductAttribute.GetProperty("BigPicture").StringValue;
                            var smallPicture = string.IsNullOrWhiteSpace(managerProductAttribute.GetProperty("SmallPicture").StringValue) ? "" : basePath + managerProductAttribute.GetProperty("SmallPicture").StringValue;
                            //


                            var productAttributeMapping = lastLoadedProduct.ProductAttributeMappings.FirstOrDefault(pam => pam.ProductAttributeId == productAttributeId);

                            //rel
                            if (productAttributeMapping == null || productAttributeMapping.ProductId != lastLoadedProduct.Id)
                            {
                                //insert mapping
                                productAttributeMapping = new ProductAttributeMapping
                                {
                                    ProductId = lastLoadedProduct.Id,
                                    ProductAttributeId = productAttributeId,
                                    TextPrompt = textPrompt,
                                    IsRequired = isRequired,
                                    AttributeControlTypeId = attributeControlTypeId,
                                    DisplayOrder = attributeDisplayOrder,
                                    IsStoneAttribute = isStoneAttribute//rel
                                };
                                _productAttributeService.InsertProductAttributeMapping(productAttributeMapping);
                            }
                            else
                            {
                                productAttributeMapping.AttributeControlTypeId = attributeControlTypeId;
                                productAttributeMapping.TextPrompt = textPrompt;
                                productAttributeMapping.IsRequired = isRequired;
                                productAttributeMapping.DisplayOrder = attributeDisplayOrder;
                                productAttributeMapping.IsStoneAttribute = isStoneAttribute;
                                _productAttributeService.UpdateProductAttributeMapping(productAttributeMapping);
                            }

                            var pav = _productAttributeService.GetProductAttributeValueById(productAttributeValueId);
                            //if productAttributeValue is associated to product
                            if ((int)AttributeValueType.AssociatedToProduct == productAttributeValueId)
                            {
                                //first check by sku
                                pav = _productAttributeService.GetProductAttributeValueByProductAttributeMappingIdAndAttributeValueSKUCode(productAttributeMapping.Id, skuCode);
                                //rel
                                //if pav is null try checking by id
                                if (pav == null)
                                {
                                    pav = _productAttributeService.GetProductAttributeValueById(productAttributeValueId);
                                }
                            }
                            //end rel

                            var attributeControlType = (AttributeControlType)attributeControlTypeId;

                            //rel
                            if (pav == null || pav.ProductAttributeMappingId != productAttributeMapping.Id)
                            {
                                switch (attributeControlType)
                                {
                                    case AttributeControlType.Datepicker:
                                    case AttributeControlType.FileUpload:
                                    case AttributeControlType.MultilineTextbox:
                                    case AttributeControlType.TextBox:
                                        continue;
                                }

                                pav = new ProductAttributeValue
                                {
                                    ProductAttributeMappingId = productAttributeMapping.Id,
                                    AttributeValueType = (AttributeValueType)attributeValueTypeId,
                                    AssociatedProductId = associatedProductId,
                                    Name = valueName,
                                    PriceAdjustment = priceAdjustment,
                                    WeightAdjustment = weightAdjustment,
                                    Cost = cost,
                                    IsPreSelected = isPreSelected,
                                    DisplayOrder = displayOrder,
                                    ColorSquaresRgb = colorSquaresRgb,
                                    ImageSquaresPictureId = imageSquaresPictureId,
                                    Quantity = quantity,
                                    PictureId = pictureId,
                                    //rel
                                    SKUCode = skuCode,
                                    //IsStoneProductAttributeValue = isStoneProductAttributeValue,
                                    StoneNameId = stoneNameId,
                                    ZIndex = zIndex
                                    //rel
                                };

                                _productAttributeService.InsertProductAttributeValue(pav);

                                //rel
                                if (!string.IsNullOrWhiteSpace(bigPicture) || !string.IsNullOrWhiteSpace(smallPicture))
                                {
                                    productAndAttrValuePictures.Add(new ProductAttributeValueMetadata() {
                                        ProductItem = lastLoadedProduct,
                                        BigPicture_ProductPicturePath = bigPicture,
                                        SmallPicture_PAV_BoxPicturePath = smallPicture,
                                        ProductAttributeValue = pav,
                                        ProductIsNew = false,//allways check products pictures
                                        AttributeValueIsNew = true
                                    });
                                }
                                //rel
                            }
                            else
                            {
                                pav.AttributeValueTypeId = attributeValueTypeId;
                                pav.AssociatedProductId = associatedProductId;
                                pav.Name = valueName;
                                pav.ColorSquaresRgb = colorSquaresRgb;
                                //rel
                                if (imageSquaresPictureId > 0)
                                    pav.ImageSquaresPictureId = imageSquaresPictureId;
                                pav.PriceAdjustment = priceAdjustment;
                                pav.WeightAdjustment = weightAdjustment;
                                pav.Cost = cost;
                                pav.Quantity = quantity;
                                pav.IsPreSelected = isPreSelected;
                                pav.DisplayOrder = displayOrder;
                                //rel
                                if (pictureId > 0)
                                    pav.PictureId = pictureId;
                                //rel
                                pav.SKUCode = skuCode;
                                //pav.IsStoneProductAttributeValue = isStoneProductAttributeValue;
                                pav.StoneNameId = stoneNameId;
                                pav.ZIndex = zIndex;
                                //rel

                                _productAttributeService.UpdateProductAttributeValue(pav);

                                //rel
                                if (!string.IsNullOrWhiteSpace(bigPicture) || !string.IsNullOrWhiteSpace(smallPicture))
                                {
                                    productAndAttrValuePictures.Add(new ProductAttributeValueMetadata()
                                    {
                                        ProductItem = lastLoadedProduct,
                                        BigPicture_ProductPicturePath = bigPicture,
                                        SmallPicture_PAV_BoxPicturePath = smallPicture,
                                        ProductAttributeValue = pav,
                                        ProductIsNew = false,//allways check products pictures
                                        AttributeValueIsNew = false
                                    });
                                }
                                //rel
                            }
                            //rel conditions
                            conditionsList.Add(new Conditions()
                            {
                                AttributeValueId = pav.Id,
                                ProductAttributeMappingId = productAttributeMapping.Id,
                                ProductId = lastLoadedProduct.Id,
                                RowValueId = rowValueId,
                                RowValueIdForCondition = rowValueIdForCondition,
                                RowValueIsSelected = rowValueIsSelected,
                                HasCondition = hasCondition
                            });
                        }
                        continue;
                    }

                    manager.ReadFromXlsx(worksheet, iRow);

                    var product = skuCellNum > 0 ? allProductsBySku.FirstOrDefault(p => p.Sku == manager.GetProperty("SKU").StringValue) : null;

                    var isNew = product == null;

                    product = product ?? new Product();

                    if (isNew)
                        product.CreatedOnUtc = DateTime.UtcNow;

                    foreach (var property in manager.GetProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "ProductType":
                                product.ProductTypeId = property.IntValue;
                                break;
                            case "ParentGroupedProductId":
                                product.ParentGroupedProductId = property.IntValue;
                                break;
                            case "VisibleIndividually":
                                product.VisibleIndividually = property.BooleanValue;
                                break;
                            case "Name":
                                product.Name = property.StringValue;
                                break;
                            case "ShortDescription":
                                product.ShortDescription = property.StringValue;
                                break;
                            case "FullDescription":
                                product.FullDescription = property.StringValue;
                                break;
                            case "Vendor":
                                product.VendorId = property.IntValue;
                                break;
                            case "ProductTemplate":
                                product.ProductTemplateId = property.IntValue;
                                break;
                            case "ShowOnHomePage":
                                product.ShowOnHomePage = property.BooleanValue;
                                break;
                            case "MetaKeywords":
                                product.MetaKeywords = property.StringValue;
                                break;
                            case "MetaDescription":
                                product.MetaDescription = property.StringValue;
                                break;
                            case "MetaTitle":
                                product.MetaTitle = property.StringValue;
                                break;
                            case "AllowCustomerReviews":
                                product.AllowCustomerReviews = property.BooleanValue;
                                break;
                            case "Published":
                                product.Published = property.BooleanValue;
                                break;
                            case "SKU":
                                product.Sku = property.StringValue;
                                break;
                            case "ManufacturerPartNumber":
                                product.ManufacturerPartNumber = property.StringValue;
                                break;
                            case "Gtin":
                                product.Gtin = property.StringValue;
                                break;
                            case "IsGiftCard":
                                product.IsGiftCard = property.BooleanValue;
                                break;
                            case "GiftCardType":
                                product.GiftCardTypeId = property.IntValue;
                                break;
                            case "OverriddenGiftCardAmount":
                                product.OverriddenGiftCardAmount = property.DecimalValue;
                                break;
                            case "RequireOtherProducts":
                                product.RequireOtherProducts = property.BooleanValue;
                                break;
                            case "RequiredProductIds":
                                product.RequiredProductIds = property.StringValue;
                                break;
                            case "AutomaticallyAddRequiredProducts":
                                product.AutomaticallyAddRequiredProducts = property.BooleanValue;
                                break;
                            case "IsDownload":
                                product.IsDownload = property.BooleanValue;
                                break;
                            case "DownloadId":
                                product.DownloadId = property.IntValue;
                                break;
                            case "UnlimitedDownloads":
                                product.UnlimitedDownloads = property.BooleanValue;
                                break;
                            case "MaxNumberOfDownloads":
                                product.MaxNumberOfDownloads = property.IntValue;
                                break;
                            case "DownloadActivationType":
                                product.DownloadActivationTypeId = property.IntValue;
                                break;
                            case "HasSampleDownload":
                                product.HasSampleDownload = property.BooleanValue;
                                break;
                            case "SampleDownloadId":
                                product.SampleDownloadId = property.IntValue;
                                break;
                            case "HasUserAgreement":
                                product.HasUserAgreement = property.BooleanValue;
                                break;
                            case "UserAgreementText":
                                product.UserAgreementText = property.StringValue;
                                break;
                            case "IsRecurring":
                                product.IsRecurring = property.BooleanValue;
                                break;
                            case "RecurringCycleLength":
                                product.RecurringCycleLength = property.IntValue;
                                break;
                            case "RecurringCyclePeriod":
                                product.RecurringCyclePeriodId = property.IntValue;
                                break;
                            case "RecurringTotalCycles":
                                product.RecurringTotalCycles = property.IntValue;
                                break;
                            case "IsRental":
                                product.IsRental = property.BooleanValue;
                                break;
                            case "RentalPriceLength":
                                product.RentalPriceLength = property.IntValue;
                                break;
                            case "RentalPricePeriod":
                                product.RentalPricePeriodId = property.IntValue;
                                break;
                            case "IsShipEnabled":
                                product.IsShipEnabled = property.BooleanValue;
                                break;
                            case "IsFreeShipping":
                                product.IsFreeShipping = property.BooleanValue;
                                break;
                            case "ShipSeparately":
                                product.ShipSeparately = property.BooleanValue;
                                break;
                            case "AdditionalShippingCharge":
                                product.AdditionalShippingCharge = property.DecimalValue;
                                break;
                            case "DeliveryDate":
                                product.DeliveryDateId = property.IntValue;
                                break;
                            case "IsTaxExempt":
                                product.IsTaxExempt = property.BooleanValue;
                                break;
                            case "TaxCategory":
                                product.TaxCategoryId = property.IntValue;
                                break;
                            case "IsTelecommunicationsOrBroadcastingOrElectronicServices":
                                product.IsTelecommunicationsOrBroadcastingOrElectronicServices = property.BooleanValue;
                                break;
                            case "ManageInventoryMethod":
                                product.ManageInventoryMethodId = property.IntValue;
                                break;
                            case "UseMultipleWarehouses":
                                product.UseMultipleWarehouses = property.BooleanValue;
                                break;
                            case "WarehouseId":
                                product.WarehouseId = property.IntValue;
                                break;
                            case "StockQuantity":
                                product.StockQuantity = property.IntValue;
                                break;
                            case "DisplayStockAvailability":
                                product.DisplayStockAvailability = property.BooleanValue;
                                break;
                            case "DisplayStockQuantity":
                                product.DisplayStockQuantity = property.BooleanValue;
                                break;
                            case "MinStockQuantity":
                                product.MinStockQuantity = property.IntValue;
                                break;
                            case "LowStockActivity":
                                product.LowStockActivityId = property.IntValue;
                                break;
                            case "NotifyAdminForQuantityBelow":
                                product.NotifyAdminForQuantityBelow = property.IntValue;
                                break;
                            case "BackorderModeId":
                                product.BackorderModeId = property.IntValue;
                                break;
                            case "AllowBackInStockSubscriptions":
                                product.AllowBackInStockSubscriptions = property.BooleanValue;
                                break;
                            case "OrderMinimumQuantity":
                                product.OrderMinimumQuantity = property.IntValue;
                                break;
                            case "OrderMaximumQuantity":
                                product.OrderMaximumQuantity = property.IntValue;
                                break;
                            case "AllowedQuantities":
                                product.AllowedQuantities = property.StringValue;
                                break;
                            case "AllowAddingOnlyExistingAttributeCombinations":
                                product.AllowAddingOnlyExistingAttributeCombinations = property.BooleanValue;
                                break;
                            case "NotReturnable":
                                product.NotReturnable = property.BooleanValue;
                                break;
                            case "DisableBuyButton":
                                product.DisableBuyButton = property.BooleanValue;
                                break;
                            case "DisableWishlistButton":
                                product.DisableWishlistButton = property.BooleanValue;
                                break;
                            case "AvailableForPreOrder":
                                product.AvailableForPreOrder = property.BooleanValue;
                                break;
                            case "PreOrderAvailabilityStartDateTimeUtc":
                                product.PreOrderAvailabilityStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "CallForPrice":
                                product.CallForPrice = property.BooleanValue;
                                break;
                            case "Price":
                                product.Price = property.DecimalValue;
                                break;
                            case "OldPrice":
                                product.OldPrice = property.DecimalValue;
                                break;
                            case "ProductCost":
                                product.ProductCost = property.DecimalValue;
                                break;
                            case "SpecialPrice":
                                product.SpecialPrice = property.DecimalValueNullable;
                                break;
                            case "SpecialPriceStartDateTimeUtc":
                                product.SpecialPriceStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "SpecialPriceEndDateTimeUtc":
                                product.SpecialPriceEndDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "CustomerEntersPrice":
                                product.CustomerEntersPrice = property.BooleanValue;
                                break;
                            case "MinimumCustomerEnteredPrice":
                                product.MinimumCustomerEnteredPrice = property.DecimalValue;
                                break;
                            case "MaximumCustomerEnteredPrice":
                                product.MaximumCustomerEnteredPrice = property.DecimalValue;
                                break;
                            case "BasepriceEnabled":
                                product.BasepriceEnabled = property.BooleanValue;
                                break;
                            case "BasepriceAmount":
                                product.BasepriceAmount = property.DecimalValue;
                                break;
                            case "BasepriceUnit":
                                product.BasepriceUnitId = property.IntValue;
                                break;
                            case "BasepriceBaseAmount":
                                product.BasepriceBaseAmount = property.DecimalValue;
                                break;
                            case "BasepriceBaseUnit":
                                product.BasepriceBaseUnitId = property.IntValue;
                                break;
                            case "MarkAsNew":
                                product.MarkAsNew = property.BooleanValue;
                                break;
                            case "MarkAsNewStartDateTimeUtc":
                                product.MarkAsNewStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "MarkAsNewEndDateTimeUtc":
                                product.MarkAsNewEndDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "Weight":
                                product.Weight = property.DecimalValue;
                                break;
                            case "Length":
                                product.Length = property.DecimalValue;
                                break;
                            case "Width":
                                product.Width = property.DecimalValue;
                                break;
                            case "Height":
                                product.Height = property.DecimalValue;
                                break;
                            //rel
                            case "IsStoneProduct":
                                product.IsStoneProduct = property.BooleanValue;
                                break;
                            case "StoneNameId":
                                product.StoneNameId = property.StringValue;
                                break;
                            //end rel
                        }
                    }

                    //set default product type id
                    if (isNew && properties.All(p => p.PropertyName != "ProductTypeId"))
                        product.ProductType = ProductType.SimpleProduct;

                    product.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                    {
                        _productService.InsertProduct(product);
                    }
                    else
                    {
                        _productService.UpdateProduct(product);
                    }

                    tempProperty = manager.GetProperty("SeName");
                    if (tempProperty != null)
                    {
                        var seName = tempProperty.StringValue;
                        //search engine name
                        _urlRecordService.SaveSlug(product, product.ValidateSeName(seName, product.Name, true), 0);
                    }
                    //rel
                    else
                    {
                        _urlRecordService.SaveSlug(product, product.ValidateSeName(product.Name, product.Name, true), 0);
                    }
                    //end rel

                    tempProperty = manager.GetProperty("Categories");

                    if (tempProperty != null)
                    {
                        var categoryNames = tempProperty.StringValue;

                        //category mappings
                        var categories = isNew || !allProductsCategoryIds.ContainsKey(product.Id) ? new int[0] : allProductsCategoryIds[product.Id];
                        foreach (var categoryId in categoryNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => allCategories.First(c => c.Name == x.Trim()).Id))
                        {
                            if (categories.Any(c => c == categoryId))
                                continue;

                            var productCategory = new ProductCategory
                            {
                                ProductId = product.Id,
                                CategoryId = categoryId,
                                IsFeaturedProduct = false,
                                DisplayOrder = 1
                            };
                            _categoryService.InsertProductCategory(productCategory);
                        }
                    }

                    tempProperty = manager.GetProperty("Manufacturers");
                    if (tempProperty != null)
                    {
                        var manufacturerNames = tempProperty.StringValue;

                        //manufacturer mappings
                        var manufacturers = isNew || !allProductsManufacturerIds.ContainsKey(product.Id) ? new int[0] : allProductsManufacturerIds[product.Id];
                        foreach (var manufacturerId in manufacturerNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => allManufacturers.First(m => m.Name == x.Trim()).Id))
                        {
                            if (manufacturers.Any(c => c == manufacturerId))
                                continue;

                            var productManufacturer = new ProductManufacturer
                            {
                                ProductId = product.Id,
                                ManufacturerId = manufacturerId,
                                IsFeaturedProduct = false,
                                DisplayOrder = 1
                            };
                            _manufacturerService.InsertProductManufacturer(productManufacturer);
                        }
                    }

                    var picture1 = string.IsNullOrWhiteSpace(manager.GetProperty("Picture1").Return(p => p.StringValue, String.Empty)) ? "" : basePath + manager.GetProperty("Picture1").Return(p => p.StringValue, String.Empty);
                    var picture2 = string.IsNullOrWhiteSpace(manager.GetProperty("Picture2").Return(p => p.StringValue, String.Empty)) ? "" : basePath + manager.GetProperty("Picture2").Return(p => p.StringValue, String.Empty);
                    var picture3 = string.IsNullOrWhiteSpace(manager.GetProperty("Picture3").Return(p => p.StringValue, String.Empty)) ? "" : basePath + manager.GetProperty("Picture3").Return(p => p.StringValue, String.Empty);

                    productPictureMetadata.Add(new ProductPictureMetadata
                    {
                        ProductItem = product,
                        Picture1Path = picture1,
                        Picture2Path = picture2,
                        Picture3Path = picture3,
                        IsNew = isNew
                    });

                    lastLoadedProduct = product;

                    //update "HasTierPrices" and "HasDiscountsApplied" properties
                    //_productService.UpdateHasTierPricesProperty(product);
                    //_productService.UpdateHasDiscountsApplied(product);
                }

                if (_mediaSettings.ImportProductImagesUsingHash && _pictureService.StoreInDb && _dataProvider.SupportedLengthOfBinaryHash() > 0)
                {
                    ImportProductImagesUsingHash(productPictureMetadata, allProductsBySku);
                    //rel
                    ImportProductAndAttributeValueImagesUsingHash(productAndAttrValuePictures, allProductsBySku);
                    //rel
                }
                else
                { 
                    ImportProductImagesUsingServices(productPictureMetadata);
                    //rel
                    ImportProductAndAttributeValueImagesUsingServices(productAndAttrValuePictures);
                    //rel
                }
                if (conditionsList.Where(x => x.HasCondition).Count() > 0)
                    AddConditions(conditionsList);
            }
            //Trace.WriteLine(DateTime.Now-start);
        }

        public void REL_ImportProductsFromXlsx_Test(Stream stream, string basePath, out string errorMessage)
        {
            errorMessage = "";
            //var start = DateTime.Now;
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = new List<PropertyByName<Product>>();
                var poz = 1;
                while (true)
                {
                    try
                    {
                        var cell = worksheet.Cells[1, poz];

                        if (cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()))
                            break;

                        poz += 1;
                        properties.Add(new PropertyByName<Product>(cell.Value.ToString()));
                    }
                    catch
                    {
                        break;
                    }
                }

                var manager = new PropertyManager<Product>(properties.ToArray());

                var attributProperties = new[]
                {
                    new PropertyByName<ExportProductAttribute>("AttributeId"),
                    new PropertyByName<ExportProductAttribute>("AttributeName"),
                    //rel                
                    new PropertyByName<ExportProductAttribute>("HasCondition"),
                    new PropertyByName<ExportProductAttribute>("RowValueIdForCondition"),
                    new PropertyByName<ExportProductAttribute>("RowValueIsSelected"),
                    new PropertyByName<ExportProductAttribute>("RowValueId"),
                    //rel
                    new PropertyByName<ExportProductAttribute>("AttributeTextPrompt"),
                    //rel                
                    new PropertyByName<ExportProductAttribute>("IsStoneAttribute"),
                    //rel
                    new PropertyByName<ExportProductAttribute>("AttributeIsRequired"),
                    new PropertyByName<ExportProductAttribute>("AttributeControlType")
                    {
                        DropDownElements = AttributeControlType.TextBox.ToSelectList(useLocalization: false)
                    },
                    new PropertyByName<ExportProductAttribute>("AttributeDisplayOrder"),
                    new PropertyByName<ExportProductAttribute>("ProductAttributeValueId"),
                    new PropertyByName<ExportProductAttribute>("ValueName"),
                    new PropertyByName<ExportProductAttribute>("AttributeValueType")
                    {
                        DropDownElements = AttributeValueType.Simple.ToSelectList(useLocalization: false)
                    },
                    new PropertyByName<ExportProductAttribute>("AssociatedProductId"),
                    new PropertyByName<ExportProductAttribute>("Quantity"),
                    //
                    new PropertyByName<ExportProductAttribute>("ColorSquaresRgb"),
                    new PropertyByName<ExportProductAttribute>("ImageSquaresPictureId"),
                    new PropertyByName<ExportProductAttribute>("PriceAdjustment"),
                    new PropertyByName<ExportProductAttribute>("WeightAdjustment"),
                    new PropertyByName<ExportProductAttribute>("Cost"),
                    new PropertyByName<ExportProductAttribute>("ZIndex"),
                    //new PropertyByName<ExportProductAttribute>("IsStoneProductAttributeValue"),
                    //rel
                    new PropertyByName<ExportProductAttribute>("IsPreSelected"),
                    new PropertyByName<ExportProductAttribute>("DisplayOrder"),
                    new PropertyByName<ExportProductAttribute>("PictureId"),
                    //
                    //rel                
                    new PropertyByName<ExportProductAttribute>("SKUCode"),
                    new PropertyByName<ExportProductAttribute>("StoneNameId"),
                    new PropertyByName<ExportProductAttribute>("BigPicture"),
                    new PropertyByName<ExportProductAttribute>("SmallPicture")

                };

                var conditionsList = new List<Conditions>();
                var managerProductAttribute = new PropertyManager<ExportProductAttribute>(attributProperties);

                var endRow = 2;
                var allCategoriesNames = new List<string>();
                var allSku = new List<string>();

                var tempProperty = manager.GetProperty("Categories");
                var categoryCellNum = tempProperty.Return(p => p.PropertyOrderPosition, -1);

                tempProperty = manager.GetProperty("SKU");
                var skuCellNum = tempProperty.Return(p => p.PropertyOrderPosition, -1);

                var allManufacturersNames = new List<string>();
                tempProperty = manager.GetProperty("Manufacturers");
                var manufacturerCellNum = tempProperty.Return(p => p.PropertyOrderPosition, -1);

                manager.SetSelectList("ProductType", ProductType.SimpleProduct.ToSelectList(useLocalization: false));
                manager.SetSelectList("GiftCardType", GiftCardType.Virtual.ToSelectList(useLocalization: false));
                manager.SetSelectList("DownloadActivationType", DownloadActivationType.Manually.ToSelectList(useLocalization: false));
                manager.SetSelectList("ManageInventoryMethod", ManageInventoryMethod.DontManageStock.ToSelectList(useLocalization: false));
                manager.SetSelectList("LowStockActivity", LowStockActivity.Nothing.ToSelectList(useLocalization: false));
                manager.SetSelectList("BackorderMode", BackorderMode.NoBackorders.ToSelectList(useLocalization: false));
                manager.SetSelectList("RecurringCyclePeriod", RecurringProductCyclePeriod.Days.ToSelectList(useLocalization: false));
                manager.SetSelectList("RentalPricePeriod", RentalPricePeriod.Days.ToSelectList(useLocalization: false));

                manager.SetSelectList("Vendor", _vendorService.GetAllVendors(showHidden: true).Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor).Return(v => v.Name, String.Empty)));
                manager.SetSelectList("ProductTemplate", _productTemplateService.GetAllProductTemplates().Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate).Return(pt => pt.Name, String.Empty)));
                manager.SetSelectList("DeliveryDate", _shippingService.GetAllDeliveryDates().Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate).Return(dd => dd.Name, String.Empty)));
                manager.SetSelectList("TaxCategory", _taxCategoryService.GetAllTaxCategories().Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory).Return(tc => tc.Name, String.Empty)));
                manager.SetSelectList("BasepriceUnit", _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight).Return(mw => mw.Name, String.Empty)));
                manager.SetSelectList("BasepriceBaseUnit", _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight).Return(mw => mw.Name, String.Empty)));

                var allAttributeIds = new List<int>();
                var attributeIdCellNum = managerProductAttribute.GetProperty("AttributeId").PropertyOrderPosition + ExportProductAttribute.ProducAttributeCellOffset;

                //find end of data
                errorMessage += "starting while loop. ";
                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[endRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    if (new[] { 1, 2 }.Select(cellNum => worksheet.Cells[endRow, cellNum]).All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString())) && worksheet.Row(endRow).OutlineLevel == 0)
                    {
                        var cellValue = worksheet.Cells[endRow, attributeIdCellNum].Value;
                        try
                        {
                            var aid = cellValue.Return(Convert.ToInt32, -1);

                            var productAttribute = _productAttributeService.GetProductAttributeById(aid);

                            if (productAttribute != null)
                                worksheet.Row(endRow).OutlineLevel = 1;
                        }
                        catch (FormatException)
                        {
                            if (cellValue.Return(cv => cv.ToString(), String.Empty) == "AttributeId")
                                worksheet.Row(endRow).OutlineLevel = 1;
                        }
                    }
                    //test
                    var outlinelevel = worksheet.Row(endRow).OutlineLevel;
                    //end test
                    if (worksheet.Row(endRow).OutlineLevel != 0)
                    {
                        //A
                        errorMessage += "A. ";
                        managerProductAttribute.ReadFromXlsx(worksheet, endRow, ExportProductAttribute.ProducAttributeCellOffset);
                        if (!managerProductAttribute.IsCaption)
                        {
                            ////test
                            //var val = worksheet.Cells[endRow, attributeIdCellNum].Value;
                            ////end test
                            var aid = worksheet.Cells[endRow, attributeIdCellNum].Value.Return(Convert.ToInt32, -1);
                            allAttributeIds.Add(aid);
                        }

                        endRow++;
                        continue;
                    }

                    //B
                    errorMessage += "B. ";
                    if (categoryCellNum > 0)
                    {
                        var categoryIds = worksheet.Cells[endRow, categoryCellNum].Value.Return(p => p.ToString(), string.Empty);

                        if (!categoryIds.IsEmpty())
                            allCategoriesNames.AddRange(categoryIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                    }

                    //C
                    errorMessage += "C. ";
                    if (skuCellNum > 0)
                    {
                        var sku = worksheet.Cells[endRow, skuCellNum].Value.Return(p => p.ToString(), string.Empty);

                        if (!sku.IsEmpty())
                            allSku.Add(sku);
                    }

                    //D
                    errorMessage += "D. ";
                    if (manufacturerCellNum > 0)
                    {
                        var manufacturerIds = worksheet.Cells[endRow, manufacturerCellNum].Value.Return(p => p.ToString(), string.Empty);
                        if (!manufacturerIds.IsEmpty())
                            allManufacturersNames.AddRange(manufacturerIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                    }

                    endRow++;
                }

                //E
                errorMessage += "E. ";
                //performance optimization, the check for the existence of the categories in one SQL request
                var notExistingCategories = _categoryService.GetNotExistingCategories(allCategoriesNames.ToArray());
                if (notExistingCategories.Any())
                {
                    throw new ArgumentException(string.Format("The following category name(s) don't exist - {0}", string.Join(", ", notExistingCategories)));
                }

                //performance optimization, the check for the existence of the manufacturers in one SQL request
                var notExistingManufacturers = _manufacturerService.GetNotExistingManufacturers(allManufacturersNames.ToArray());
                if (notExistingManufacturers.Any())
                {
                    throw new ArgumentException(string.Format("The following manufacturer name(s) don't exist - {0}", string.Join(", ", notExistingManufacturers)));
                }

                //performance optimization, the check for the existence of the product attributes in one SQL request
                var notExistingProductAttributes = _productAttributeService.GetNotExistingAttributes(allAttributeIds.ToArray());
                if (notExistingProductAttributes.Any())
                {
                    throw new ArgumentException(string.Format("The following product attribute ID(s) don't exist - {0}", string.Join(", ", notExistingProductAttributes)));
                }

                //performance optimization, load all products by SKU in one SQL request
                var allProductsBySku = _productService.GetProductsBySku(allSku.ToArray());

                //performance optimization, load all categories IDs for products in one SQL request
                var allProductsCategoryIds = _categoryService.GetProductCategoryIds(allProductsBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all categories in one SQL request
                var allCategories = _categoryService.GetAllCategories(showHidden: true);

                //performance optimization, load all manufacturers IDs for products in one SQL request
                var allProductsManufacturerIds = _manufacturerService.GetProductManufacturerIds(allProductsBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all manufacturers in one SQL request
                var allManufacturers = _manufacturerService.GetAllManufacturers(showHidden: true);

                //product to import images
                var productPictureMetadata = new List<ProductPictureMetadata>();

                Product lastLoadedProduct = null;

                //rel
                var productAndAttrValuePictures = new List<ProductAttributeValueMetadata>();
                //rel

                //F
                errorMessage += "F - LOOPING ROWS. ";
                for (var iRow = 2; iRow < endRow; iRow++)
                {
                    //imports product attributes
                    if (worksheet.Row(iRow).OutlineLevel != 0)
                    {
                        if (_catalogSettings.ExportImportProductAttributes)
                        {
                            //G
                            errorMessage += "G - ExportImportProductAttributes. ";
                            managerProductAttribute.ReadFromXlsx(worksheet, iRow,
                                ExportProductAttribute.ProducAttributeCellOffset);
                            if (lastLoadedProduct == null || managerProductAttribute.IsCaption)
                                continue;

                            var productAttributeId = managerProductAttribute.GetProperty("AttributeId").IntValue;
                            var attributeControlTypeId = managerProductAttribute.GetProperty("AttributeControlType").IntValue;

                            //rel for conditions
                            var hasCondition = managerProductAttribute.GetProperty("HasCondition").BooleanValue;
                            var rowValueIdForCondition = managerProductAttribute.GetProperty("RowValueIdForCondition").IntValue;
                            var rowValueIsSelected = managerProductAttribute.GetProperty("RowValueIsSelected").BooleanValue;
                            var rowValueId = managerProductAttribute.GetProperty("RowValueId").IntValue;
                            //end rel

                            var productAttributeValueId = managerProductAttribute.GetProperty("ProductAttributeValueId").IntValue;
                            var associatedProductId = managerProductAttribute.GetProperty("AssociatedProductId").IntValue;
                            var valueName = managerProductAttribute.GetProperty("ValueName").StringValue;
                            var attributeValueTypeId = managerProductAttribute.GetProperty("AttributeValueType").IntValue;
                            var colorSquaresRgb = managerProductAttribute.GetProperty("ColorSquaresRgb").StringValue;
                            var imageSquaresPictureId = managerProductAttribute.GetProperty("ImageSquaresPictureId").IntValue;
                            var priceAdjustment = managerProductAttribute.GetProperty("PriceAdjustment").DecimalValue;
                            var weightAdjustment = managerProductAttribute.GetProperty("WeightAdjustment").DecimalValue;
                            var cost = managerProductAttribute.GetProperty("Cost").DecimalValue;
                            var quantity = managerProductAttribute.GetProperty("Quantity").IntValue;
                            var isPreSelected = managerProductAttribute.GetProperty("IsPreSelected").BooleanValue;
                            var displayOrder = managerProductAttribute.GetProperty("DisplayOrder").IntValue;
                            var pictureId = managerProductAttribute.GetProperty("PictureId").IntValue;
                            var textPrompt = managerProductAttribute.GetProperty("AttributeTextPrompt").StringValue;
                            var isRequired = managerProductAttribute.GetProperty("AttributeIsRequired").BooleanValue;
                            var isStoneAttribute = managerProductAttribute.GetProperty("IsStoneAttribute").BooleanValue;//rel
                            var attributeDisplayOrder = managerProductAttribute.GetProperty("AttributeDisplayOrder").IntValue;
                            //rel
                            //var isStoneProductAttributeValue = managerProductAttribute.GetProperty("IsStoneProductAttributeValue").BooleanValue;
                            var zIndex = managerProductAttribute.GetProperty("ZIndex").IntValue;
                            var skuCode = managerProductAttribute.GetProperty("SKUCode").StringValue;
                            var stoneNameId = managerProductAttribute.GetProperty("StoneNameId").StringValue;
                            var bigPicture = string.IsNullOrWhiteSpace(managerProductAttribute.GetProperty("BigPicture").StringValue) ? "" : basePath + managerProductAttribute.GetProperty("BigPicture").StringValue;
                            var smallPicture = string.IsNullOrWhiteSpace(managerProductAttribute.GetProperty("SmallPicture").StringValue) ? "" : basePath + managerProductAttribute.GetProperty("SmallPicture").StringValue;
                            //


                            var productAttributeMapping = lastLoadedProduct.ProductAttributeMappings.FirstOrDefault(pam => pam.ProductAttributeId == productAttributeId);

                            //rel
                            if (productAttributeMapping == null || productAttributeMapping.ProductId != lastLoadedProduct.Id)
                            {
                                //H
                                errorMessage += "H - INSERTINT PAMAPPING. ";
                                //insert mapping
                                productAttributeMapping = new ProductAttributeMapping
                                {
                                    ProductId = lastLoadedProduct.Id,
                                    ProductAttributeId = productAttributeId,
                                    TextPrompt = textPrompt,
                                    IsRequired = isRequired,
                                    AttributeControlTypeId = attributeControlTypeId,
                                    DisplayOrder = attributeDisplayOrder,
                                    IsStoneAttribute = isStoneAttribute//rel
                                };
                                _productAttributeService.InsertProductAttributeMapping(productAttributeMapping);
                            }
                            else
                            {
                                //I
                                errorMessage += "I - UPDATEING PAMAPPING. ";
                                productAttributeMapping.AttributeControlTypeId = attributeControlTypeId;
                                productAttributeMapping.TextPrompt = textPrompt;
                                productAttributeMapping.IsRequired = isRequired;
                                productAttributeMapping.DisplayOrder = attributeDisplayOrder;
                                productAttributeMapping.IsStoneAttribute = isStoneAttribute;
                                _productAttributeService.UpdateProductAttributeMapping(productAttributeMapping);
                            }

                            var pav = _productAttributeService.GetProductAttributeValueById(productAttributeValueId);
                            //if productAttributeValue is associated to product
                            if ((int)AttributeValueType.AssociatedToProduct == productAttributeValueId)
                            {
                                //first check by sku
                                pav = _productAttributeService.GetProductAttributeValueByProductAttributeMappingIdAndAttributeValueSKUCode(productAttributeMapping.Id, skuCode);
                                //rel
                                //if pav is null try checking by id
                                if (pav == null)
                                {
                                    pav = _productAttributeService.GetProductAttributeValueById(productAttributeValueId);
                                }
                            }
                            //end rel
                            var attributeControlType = (AttributeControlType)attributeControlTypeId;

                            //rel
                            if (pav == null || pav.ProductAttributeMappingId != productAttributeMapping.Id)
                            {
                                switch (attributeControlType)
                                {
                                    case AttributeControlType.Datepicker:
                                    case AttributeControlType.FileUpload:
                                    case AttributeControlType.MultilineTextbox:
                                    case AttributeControlType.TextBox:
                                        continue;
                                }

                                //J
                                errorMessage += "J - NEW PAVALUE. ";
                                pav = new ProductAttributeValue
                                {
                                    ProductAttributeMappingId = productAttributeMapping.Id,
                                    AttributeValueType = (AttributeValueType)attributeValueTypeId,
                                    AssociatedProductId = associatedProductId,
                                    Name = valueName,
                                    PriceAdjustment = priceAdjustment,
                                    WeightAdjustment = weightAdjustment,
                                    Cost = cost,
                                    IsPreSelected = isPreSelected,
                                    DisplayOrder = displayOrder,
                                    ColorSquaresRgb = colorSquaresRgb,
                                    ImageSquaresPictureId = imageSquaresPictureId,
                                    Quantity = quantity,
                                    PictureId = pictureId,
                                    //rel
                                    SKUCode = skuCode,
                                    //IsStoneProductAttributeValue = isStoneProductAttributeValue,
                                    StoneNameId = stoneNameId,
                                    ZIndex = zIndex
                                    //rel
                                };

                                _productAttributeService.InsertProductAttributeValue(pav);

                                //rel
                                if (!string.IsNullOrWhiteSpace(bigPicture) || !string.IsNullOrWhiteSpace(smallPicture))
                                {
                                    productAndAttrValuePictures.Add(new ProductAttributeValueMetadata()
                                    {
                                        ProductItem = lastLoadedProduct,
                                        BigPicture_ProductPicturePath = bigPicture,
                                        SmallPicture_PAV_BoxPicturePath = smallPicture,
                                        ProductAttributeValue = pav,
                                        ProductIsNew = false,//allways check products pictures
                                        AttributeValueIsNew = true
                                    });
                                }
                                //rel
                            }
                            else
                            {
                                //K
                                errorMessage += "K - UPDATE PAVALUE. ";
                                pav.AttributeValueTypeId = attributeValueTypeId;
                                pav.AssociatedProductId = associatedProductId;
                                pav.Name = valueName;
                                pav.ColorSquaresRgb = colorSquaresRgb;
                                //rel
                                if (imageSquaresPictureId > 0)
                                    pav.ImageSquaresPictureId = imageSquaresPictureId;
                                pav.PriceAdjustment = priceAdjustment;
                                pav.WeightAdjustment = weightAdjustment;
                                pav.Cost = cost;
                                pav.Quantity = quantity;
                                pav.IsPreSelected = isPreSelected;
                                pav.DisplayOrder = displayOrder;
                                //rel
                                if (pictureId > 0)
                                    pav.PictureId = pictureId;
                                //rel
                                pav.SKUCode = skuCode;
                                //pav.IsStoneProductAttributeValue = isStoneProductAttributeValue;
                                pav.StoneNameId = stoneNameId;
                                pav.ZIndex = zIndex;
                                //rel

                                _productAttributeService.UpdateProductAttributeValue(pav);

                                //rel
                                if (!string.IsNullOrWhiteSpace(bigPicture) || !string.IsNullOrWhiteSpace(smallPicture))
                                {
                                    productAndAttrValuePictures.Add(new ProductAttributeValueMetadata()
                                    {
                                        ProductItem = lastLoadedProduct,
                                        BigPicture_ProductPicturePath = bigPicture,
                                        SmallPicture_PAV_BoxPicturePath = smallPicture,
                                        ProductAttributeValue = pav,
                                        ProductIsNew = false,//allways check products pictures
                                        AttributeValueIsNew = false
                                    });
                                }
                                //rel
                            }
                            //rel conditions
                            conditionsList.Add(new Conditions()
                            {
                                AttributeValueId = pav.Id,
                                ProductAttributeMappingId = productAttributeMapping.Id,
                                ProductId = lastLoadedProduct.Id,
                                RowValueId = rowValueId,
                                RowValueIdForCondition = rowValueIdForCondition,
                                RowValueIsSelected = rowValueIsSelected,
                                HasCondition = hasCondition
                            });
                        }
                        continue;
                    }

                    manager.ReadFromXlsx(worksheet, iRow);

                    var product = skuCellNum > 0 ? allProductsBySku.FirstOrDefault(p => p.Sku == manager.GetProperty("SKU").StringValue) : null;

                    var isNew = product == null;

                    product = product ?? new Product();

                    if (isNew)
                        product.CreatedOnUtc = DateTime.UtcNow;

                    foreach (var property in manager.GetProperties)
                    {
                        //L
                        errorMessage += "L - GETTING PRODUCT PROPERTIES: " + property.PropertyName + ". ";
                        switch (property.PropertyName)
                        {
                            case "ProductType":
                                product.ProductTypeId = property.IntValue;
                                break;
                            case "ParentGroupedProductId":
                                product.ParentGroupedProductId = property.IntValue;
                                break;
                            case "VisibleIndividually":
                                product.VisibleIndividually = property.BooleanValue;
                                break;
                            case "Name":
                                product.Name = property.StringValue;
                                break;
                            case "ShortDescription":
                                product.ShortDescription = property.StringValue;
                                break;
                            case "FullDescription":
                                product.FullDescription = property.StringValue;
                                break;
                            case "Vendor":
                                product.VendorId = property.IntValue;
                                break;
                            case "ProductTemplate":
                                product.ProductTemplateId = property.IntValue;
                                break;
                            case "ShowOnHomePage":
                                product.ShowOnHomePage = property.BooleanValue;
                                break;
                            case "MetaKeywords":
                                product.MetaKeywords = property.StringValue;
                                break;
                            case "MetaDescription":
                                product.MetaDescription = property.StringValue;
                                break;
                            case "MetaTitle":
                                product.MetaTitle = property.StringValue;
                                break;
                            case "AllowCustomerReviews":
                                product.AllowCustomerReviews = property.BooleanValue;
                                break;
                            case "Published":
                                product.Published = property.BooleanValue;
                                break;
                            case "SKU":
                                product.Sku = property.StringValue;
                                break;
                            case "ManufacturerPartNumber":
                                product.ManufacturerPartNumber = property.StringValue;
                                break;
                            case "Gtin":
                                product.Gtin = property.StringValue;
                                break;
                            case "IsGiftCard":
                                product.IsGiftCard = property.BooleanValue;
                                break;
                            case "GiftCardType":
                                product.GiftCardTypeId = property.IntValue;
                                break;
                            case "OverriddenGiftCardAmount":
                                product.OverriddenGiftCardAmount = property.DecimalValue;
                                break;
                            case "RequireOtherProducts":
                                product.RequireOtherProducts = property.BooleanValue;
                                break;
                            case "RequiredProductIds":
                                product.RequiredProductIds = property.StringValue;
                                break;
                            case "AutomaticallyAddRequiredProducts":
                                product.AutomaticallyAddRequiredProducts = property.BooleanValue;
                                break;
                            case "IsDownload":
                                product.IsDownload = property.BooleanValue;
                                break;
                            case "DownloadId":
                                product.DownloadId = property.IntValue;
                                break;
                            case "UnlimitedDownloads":
                                product.UnlimitedDownloads = property.BooleanValue;
                                break;
                            case "MaxNumberOfDownloads":
                                product.MaxNumberOfDownloads = property.IntValue;
                                break;
                            case "DownloadActivationType":
                                product.DownloadActivationTypeId = property.IntValue;
                                break;
                            case "HasSampleDownload":
                                product.HasSampleDownload = property.BooleanValue;
                                break;
                            case "SampleDownloadId":
                                product.SampleDownloadId = property.IntValue;
                                break;
                            case "HasUserAgreement":
                                product.HasUserAgreement = property.BooleanValue;
                                break;
                            case "UserAgreementText":
                                product.UserAgreementText = property.StringValue;
                                break;
                            case "IsRecurring":
                                product.IsRecurring = property.BooleanValue;
                                break;
                            case "RecurringCycleLength":
                                product.RecurringCycleLength = property.IntValue;
                                break;
                            case "RecurringCyclePeriod":
                                product.RecurringCyclePeriodId = property.IntValue;
                                break;
                            case "RecurringTotalCycles":
                                product.RecurringTotalCycles = property.IntValue;
                                break;
                            case "IsRental":
                                product.IsRental = property.BooleanValue;
                                break;
                            case "RentalPriceLength":
                                product.RentalPriceLength = property.IntValue;
                                break;
                            case "RentalPricePeriod":
                                product.RentalPricePeriodId = property.IntValue;
                                break;
                            case "IsShipEnabled":
                                product.IsShipEnabled = property.BooleanValue;
                                break;
                            case "IsFreeShipping":
                                product.IsFreeShipping = property.BooleanValue;
                                break;
                            case "ShipSeparately":
                                product.ShipSeparately = property.BooleanValue;
                                break;
                            case "AdditionalShippingCharge":
                                product.AdditionalShippingCharge = property.DecimalValue;
                                break;
                            case "DeliveryDate":
                                product.DeliveryDateId = property.IntValue;
                                break;
                            case "IsTaxExempt":
                                product.IsTaxExempt = property.BooleanValue;
                                break;
                            case "TaxCategory":
                                product.TaxCategoryId = property.IntValue;
                                break;
                            case "IsTelecommunicationsOrBroadcastingOrElectronicServices":
                                product.IsTelecommunicationsOrBroadcastingOrElectronicServices = property.BooleanValue;
                                break;
                            case "ManageInventoryMethod":
                                product.ManageInventoryMethodId = property.IntValue;
                                break;
                            case "UseMultipleWarehouses":
                                product.UseMultipleWarehouses = property.BooleanValue;
                                break;
                            case "WarehouseId":
                                product.WarehouseId = property.IntValue;
                                break;
                            case "StockQuantity":
                                product.StockQuantity = property.IntValue;
                                break;
                            case "DisplayStockAvailability":
                                product.DisplayStockAvailability = property.BooleanValue;
                                break;
                            case "DisplayStockQuantity":
                                product.DisplayStockQuantity = property.BooleanValue;
                                break;
                            case "MinStockQuantity":
                                product.MinStockQuantity = property.IntValue;
                                break;
                            case "LowStockActivity":
                                product.LowStockActivityId = property.IntValue;
                                break;
                            case "NotifyAdminForQuantityBelow":
                                product.NotifyAdminForQuantityBelow = property.IntValue;
                                break;
                            case "BackorderModeId":
                                product.BackorderModeId = property.IntValue;
                                break;
                            case "AllowBackInStockSubscriptions":
                                product.AllowBackInStockSubscriptions = property.BooleanValue;
                                break;
                            case "OrderMinimumQuantity":
                                product.OrderMinimumQuantity = property.IntValue;
                                break;
                            case "OrderMaximumQuantity":
                                product.OrderMaximumQuantity = property.IntValue;
                                break;
                            case "AllowedQuantities":
                                product.AllowedQuantities = property.StringValue;
                                break;
                            case "AllowAddingOnlyExistingAttributeCombinations":
                                product.AllowAddingOnlyExistingAttributeCombinations = property.BooleanValue;
                                break;
                            case "NotReturnable":
                                product.NotReturnable = property.BooleanValue;
                                break;
                            case "DisableBuyButton":
                                product.DisableBuyButton = property.BooleanValue;
                                break;
                            case "DisableWishlistButton":
                                product.DisableWishlistButton = property.BooleanValue;
                                break;
                            case "AvailableForPreOrder":
                                product.AvailableForPreOrder = property.BooleanValue;
                                break;
                            case "PreOrderAvailabilityStartDateTimeUtc":
                                product.PreOrderAvailabilityStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "CallForPrice":
                                product.CallForPrice = property.BooleanValue;
                                break;
                            case "Price":
                                product.Price = property.DecimalValue;
                                break;
                            case "OldPrice":
                                product.OldPrice = property.DecimalValue;
                                break;
                            case "ProductCost":
                                product.ProductCost = property.DecimalValue;
                                break;
                            case "SpecialPrice":
                                product.SpecialPrice = property.DecimalValueNullable;
                                break;
                            case "SpecialPriceStartDateTimeUtc":
                                product.SpecialPriceStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "SpecialPriceEndDateTimeUtc":
                                product.SpecialPriceEndDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "CustomerEntersPrice":
                                product.CustomerEntersPrice = property.BooleanValue;
                                break;
                            case "MinimumCustomerEnteredPrice":
                                product.MinimumCustomerEnteredPrice = property.DecimalValue;
                                break;
                            case "MaximumCustomerEnteredPrice":
                                product.MaximumCustomerEnteredPrice = property.DecimalValue;
                                break;
                            case "BasepriceEnabled":
                                product.BasepriceEnabled = property.BooleanValue;
                                break;
                            case "BasepriceAmount":
                                product.BasepriceAmount = property.DecimalValue;
                                break;
                            case "BasepriceUnit":
                                product.BasepriceUnitId = property.IntValue;
                                break;
                            case "BasepriceBaseAmount":
                                product.BasepriceBaseAmount = property.DecimalValue;
                                break;
                            case "BasepriceBaseUnit":
                                product.BasepriceBaseUnitId = property.IntValue;
                                break;
                            case "MarkAsNew":
                                product.MarkAsNew = property.BooleanValue;
                                break;
                            case "MarkAsNewStartDateTimeUtc":
                                product.MarkAsNewStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "MarkAsNewEndDateTimeUtc":
                                product.MarkAsNewEndDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "Weight":
                                product.Weight = property.DecimalValue;
                                break;
                            case "Length":
                                product.Length = property.DecimalValue;
                                break;
                            case "Width":
                                product.Width = property.DecimalValue;
                                break;
                            case "Height":
                                product.Height = property.DecimalValue;
                                break;
                            //rel
                            case "IsStoneProduct":
                                product.IsStoneProduct = property.BooleanValue;
                                break;
                            case "StoneNameId":
                                product.StoneNameId = property.StringValue;
                                break;
                                //end rel
                        }
                    }

                    //set default product type id
                    if (isNew && properties.All(p => p.PropertyName != "ProductTypeId"))
                        product.ProductType = ProductType.SimpleProduct;

                    //M
                    errorMessage += "M - UPDATEING PRODUCT. ";
                    product.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                    {
                        _productService.InsertProduct(product);
                    }
                    else
                    {
                        _productService.UpdateProduct(product);
                    }

                    tempProperty = manager.GetProperty("SeName");
                    if (tempProperty != null)
                    {
                        var seName = tempProperty.StringValue;
                        //search engine name
                        _urlRecordService.SaveSlug(product, product.ValidateSeName(seName, product.Name, true), 0);
                    }
                    //rel
                    else
                    {
                        _urlRecordService.SaveSlug(product, product.ValidateSeName(product.Name, product.Name, true), 0);
                    }
                    //end rel

                    tempProperty = manager.GetProperty("Categories");

                    //N
                    errorMessage += "N - CATEGORIES. ";
                    if (tempProperty != null)
                    {
                        var categoryNames = tempProperty.StringValue;

                        //category mappings
                        var categories = isNew || !allProductsCategoryIds.ContainsKey(product.Id) ? new int[0] : allProductsCategoryIds[product.Id];
                        foreach (var categoryId in categoryNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => allCategories.First(c => c.Name == x.Trim()).Id))
                        {
                            if (categories.Any(c => c == categoryId))
                                continue;

                            var productCategory = new ProductCategory
                            {
                                ProductId = product.Id,
                                CategoryId = categoryId,
                                IsFeaturedProduct = false,
                                DisplayOrder = 1
                            };
                            _categoryService.InsertProductCategory(productCategory);
                        }
                    }

                    //O
                    errorMessage += "O - MANUFACTURES. ";
                    tempProperty = manager.GetProperty("Manufacturers");
                    if (tempProperty != null)
                    {
                        var manufacturerNames = tempProperty.StringValue;

                        //manufacturer mappings
                        var manufacturers = isNew || !allProductsManufacturerIds.ContainsKey(product.Id) ? new int[0] : allProductsManufacturerIds[product.Id];
                        foreach (var manufacturerId in manufacturerNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => allManufacturers.First(m => m.Name == x.Trim()).Id))
                        {
                            if (manufacturers.Any(c => c == manufacturerId))
                                continue;

                            var productManufacturer = new ProductManufacturer
                            {
                                ProductId = product.Id,
                                ManufacturerId = manufacturerId,
                                IsFeaturedProduct = false,
                                DisplayOrder = 1
                            };
                            _manufacturerService.InsertProductManufacturer(productManufacturer);
                        }
                    }

                    //P
                    errorMessage += "P - PICTURES. ";
                    var picture1 = string.IsNullOrWhiteSpace(manager.GetProperty("Picture1").Return(p => p.StringValue, String.Empty)) ? "" : basePath + manager.GetProperty("Picture1").Return(p => p.StringValue, String.Empty);
                    var picture2 = string.IsNullOrWhiteSpace(manager.GetProperty("Picture2").Return(p => p.StringValue, String.Empty)) ? "" : basePath + manager.GetProperty("Picture2").Return(p => p.StringValue, String.Empty);
                    var picture3 = string.IsNullOrWhiteSpace(manager.GetProperty("Picture3").Return(p => p.StringValue, String.Empty)) ? "" : basePath + manager.GetProperty("Picture3").Return(p => p.StringValue, String.Empty);

                    productPictureMetadata.Add(new ProductPictureMetadata
                    {
                        ProductItem = product,
                        Picture1Path = picture1,
                        Picture2Path = picture2,
                        Picture3Path = picture3,
                        IsNew = isNew
                    });

                    lastLoadedProduct = product;

                    //update "HasTierPrices" and "HasDiscountsApplied" properties
                    //_productService.UpdateHasTierPricesProperty(product);
                    //_productService.UpdateHasDiscountsApplied(product);
                }

                //Q
                errorMessage += "Q - ADDING PICTURES. ";
                var pictureErrors = "";
                if (_mediaSettings.ImportProductImagesUsingHash && _pictureService.StoreInDb && _dataProvider.SupportedLengthOfBinaryHash() > 0)
                {                    
                    ImportProductImagesUsingHash_Test(productPictureMetadata, allProductsBySku, out pictureErrors);
                    //rel
                    ImportProductAndAttributeValueImagesUsingHash_Test(productAndAttrValuePictures, allProductsBySku, out pictureErrors);
                    //rel
                }
                else
                {
                    ImportProductImagesUsingServices_Test(productPictureMetadata, out pictureErrors);
                    //rel
                    ImportProductAndAttributeValueImagesUsingServices_Test(productAndAttrValuePictures, out pictureErrors);
                    //rel
                }
                errorMessage += pictureErrors;
                if (conditionsList.Where(x => x.HasCondition).Count() > 0)
                    AddConditions(conditionsList);
            }
            //Trace.WriteLine(DateTime.Now-start);
        }

        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportProductsFromXlsx(Stream stream)
        {
            //var start = DateTime.Now;
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = new List<PropertyByName<Product>>();
                var poz = 1;
                while (true)
                {
                    try
                    {
                        var cell = worksheet.Cells[1, poz];

                        if (cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()))
                            break;

                        poz += 1;
                        properties.Add(new PropertyByName<Product>(cell.Value.ToString()));
                    }
                    catch
                    {
                        break;
                    }
                }

                var manager = new PropertyManager<Product>(properties.ToArray());

                var attributProperties = new[]
                   {
                        new PropertyByName<ExportProductAttribute>("AttributeId"),
                        new PropertyByName<ExportProductAttribute>("AttributeName"),
                        new PropertyByName<ExportProductAttribute>("AttributeTextPrompt"),
                        new PropertyByName<ExportProductAttribute>("AttributeIsRequired"),
                        new PropertyByName<ExportProductAttribute>("AttributeControlType")
                        {
                            DropDownElements = AttributeControlType.TextBox.ToSelectList(useLocalization: false)
                        },
                        new PropertyByName<ExportProductAttribute>("AttributeDisplayOrder"),
                        new PropertyByName<ExportProductAttribute>("ProductAttributeValueId"),
                        new PropertyByName<ExportProductAttribute>("ValueName"),
                        new PropertyByName<ExportProductAttribute>("AttributeValueType")
                        {
                            DropDownElements = AttributeValueType.Simple.ToSelectList(useLocalization: false)
                        },
                        new PropertyByName<ExportProductAttribute>("AssociatedProductId"),
                        new PropertyByName<ExportProductAttribute>("ColorSquaresRgb"),
                        new PropertyByName<ExportProductAttribute>("ImageSquaresPictureId"),
                        new PropertyByName<ExportProductAttribute>("PriceAdjustment"),
                        new PropertyByName<ExportProductAttribute>("WeightAdjustment"),
                        new PropertyByName<ExportProductAttribute>("Cost"),
                        new PropertyByName<ExportProductAttribute>("Quantity"),
                        new PropertyByName<ExportProductAttribute>("IsPreSelected"),
                        new PropertyByName<ExportProductAttribute>("DisplayOrder"),
                        new PropertyByName<ExportProductAttribute>("PictureId")
                    };

                var managerProductAttribute = new PropertyManager<ExportProductAttribute>(attributProperties);

                var endRow = 2;
                var allCategoriesNames = new List<string>();
                var allSku = new List<string>();

                var tempProperty = manager.GetProperty("Categories");
                var categoryCellNum = tempProperty.Return(p => p.PropertyOrderPosition, -1);

                tempProperty = manager.GetProperty("SKU");
                var skuCellNum = tempProperty.Return(p => p.PropertyOrderPosition, -1);

                var allManufacturersNames = new List<string>();
                tempProperty = manager.GetProperty("Manufacturers");
                var manufacturerCellNum = tempProperty.Return(p => p.PropertyOrderPosition, -1);

                manager.SetSelectList("ProductType", ProductType.SimpleProduct.ToSelectList(useLocalization: false));
                manager.SetSelectList("GiftCardType", GiftCardType.Virtual.ToSelectList(useLocalization: false));
                manager.SetSelectList("DownloadActivationType", DownloadActivationType.Manually.ToSelectList(useLocalization: false));
                manager.SetSelectList("ManageInventoryMethod", ManageInventoryMethod.DontManageStock.ToSelectList(useLocalization: false));
                manager.SetSelectList("LowStockActivity", LowStockActivity.Nothing.ToSelectList(useLocalization: false));
                manager.SetSelectList("BackorderMode", BackorderMode.NoBackorders.ToSelectList(useLocalization: false));
                manager.SetSelectList("RecurringCyclePeriod", RecurringProductCyclePeriod.Days.ToSelectList(useLocalization: false));
                manager.SetSelectList("RentalPricePeriod", RentalPricePeriod.Days.ToSelectList(useLocalization: false));

                manager.SetSelectList("Vendor", _vendorService.GetAllVendors(showHidden: true).Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor).Return(v => v.Name, String.Empty)));
                manager.SetSelectList("ProductTemplate", _productTemplateService.GetAllProductTemplates().Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate).Return(pt => pt.Name, String.Empty)));
                manager.SetSelectList("DeliveryDate", _shippingService.GetAllDeliveryDates().Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate).Return(dd => dd.Name, String.Empty)));
                manager.SetSelectList("TaxCategory", _taxCategoryService.GetAllTaxCategories().Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory).Return(tc => tc.Name, String.Empty)));
                manager.SetSelectList("BasepriceUnit", _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight).Return(mw => mw.Name, String.Empty)));
                manager.SetSelectList("BasepriceBaseUnit", _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight).Return(mw => mw.Name, String.Empty)));

                var allAttributeIds = new List<int>();
                var attributeIdCellNum = managerProductAttribute.GetProperty("AttributeId").PropertyOrderPosition + ExportProductAttribute.ProducAttributeCellOffset;

                //find end of data
                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[endRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    if (new[] { 1, 2 }.Select(cellNum => worksheet.Cells[endRow, cellNum]).All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString())) && worksheet.Row(endRow).OutlineLevel == 0)
                    {
                        var cellValue = worksheet.Cells[endRow, attributeIdCellNum].Value;
                        try
                        {
                            var aid = cellValue.Return(Convert.ToInt32, -1);

                            var productAttribute = _productAttributeService.GetProductAttributeById(aid);

                            if (productAttribute != null)
                                worksheet.Row(endRow).OutlineLevel = 1;
                        }
                        catch (FormatException)
                        {
                            if (cellValue.Return(cv => cv.ToString(), String.Empty) == "AttributeId")
                                worksheet.Row(endRow).OutlineLevel = 1;
                        }
                    }

                    if (worksheet.Row(endRow).OutlineLevel != 0)
                    {
                        managerProductAttribute.ReadFromXlsx(worksheet, endRow, ExportProductAttribute.ProducAttributeCellOffset);
                        if (!managerProductAttribute.IsCaption)
                        {
                            var aid = worksheet.Cells[endRow, attributeIdCellNum].Value.Return(Convert.ToInt32, -1);
                            allAttributeIds.Add(aid);
                        }

                        endRow++;
                        continue;
                    }

                    if (categoryCellNum > 0)
                    {
                        var categoryIds = worksheet.Cells[endRow, categoryCellNum].Value.Return(p => p.ToString(), string.Empty);

                        if (!categoryIds.IsEmpty())
                            allCategoriesNames.AddRange(categoryIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                    }

                    if (skuCellNum > 0)
                    {
                        var sku = worksheet.Cells[endRow, skuCellNum].Value.Return(p => p.ToString(), string.Empty);

                        if (!sku.IsEmpty())
                            allSku.Add(sku);
                    }

                    if (manufacturerCellNum > 0)
                    {
                        var manufacturerIds = worksheet.Cells[endRow, manufacturerCellNum].Value.Return(p => p.ToString(), string.Empty);
                        if (!manufacturerIds.IsEmpty())
                            allManufacturersNames.AddRange(manufacturerIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                    }

                    endRow++;
                }

                //performance optimization, the check for the existence of the categories in one SQL request
                var notExistingCategories = _categoryService.GetNotExistingCategories(allCategoriesNames.ToArray());
                if (notExistingCategories.Any())
                {
                    throw new ArgumentException(string.Format("The following category name(s) don't exist - {0}", string.Join(", ", notExistingCategories)));
                }

                //performance optimization, the check for the existence of the manufacturers in one SQL request
                var notExistingManufacturers = _manufacturerService.GetNotExistingManufacturers(allManufacturersNames.ToArray());
                if (notExistingManufacturers.Any())
                {
                    throw new ArgumentException(string.Format("The following manufacturer name(s) don't exist - {0}", string.Join(", ", notExistingManufacturers)));
                }

                //performance optimization, the check for the existence of the product attributes in one SQL request
                var notExistingProductAttributes = _productAttributeService.GetNotExistingAttributes(allAttributeIds.ToArray());
                if (notExistingProductAttributes.Any())
                {
                    throw new ArgumentException(string.Format("The following product attribute ID(s) don't exist - {0}", string.Join(", ", notExistingProductAttributes)));
                }

                //performance optimization, load all products by SKU in one SQL request
                var allProductsBySku = _productService.GetProductsBySku(allSku.ToArray());

                //performance optimization, load all categories IDs for products in one SQL request
                var allProductsCategoryIds = _categoryService.GetProductCategoryIds(allProductsBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all categories in one SQL request
                var allCategories = _categoryService.GetAllCategories(showHidden: true);

                //performance optimization, load all manufacturers IDs for products in one SQL request
                var allProductsManufacturerIds = _manufacturerService.GetProductManufacturerIds(allProductsBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all manufacturers in one SQL request
                var allManufacturers = _manufacturerService.GetAllManufacturers(showHidden: true);

                //product to import images
                var productPictureMetadata = new List<ProductPictureMetadata>();

                Product lastLoadedProduct = null;

                for (var iRow = 2; iRow < endRow; iRow++)
                {
                    //imports product attributes
                    if (worksheet.Row(iRow).OutlineLevel != 0)
                    {
                        if (_catalogSettings.ExportImportProductAttributes)
                        {
                            managerProductAttribute.ReadFromXlsx(worksheet, iRow,
                                ExportProductAttribute.ProducAttributeCellOffset);
                            if (lastLoadedProduct == null || managerProductAttribute.IsCaption)
                                continue;

                            var productAttributeId = managerProductAttribute.GetProperty("AttributeId").IntValue;
                            var attributeControlTypeId = managerProductAttribute.GetProperty("AttributeControlType").IntValue;

                            var productAttributeValueId = managerProductAttribute.GetProperty("ProductAttributeValueId").IntValue;
                            var associatedProductId = managerProductAttribute.GetProperty("AssociatedProductId").IntValue;
                            var valueName = managerProductAttribute.GetProperty("ValueName").StringValue;
                            var attributeValueTypeId = managerProductAttribute.GetProperty("AttributeValueType").IntValue;
                            var colorSquaresRgb = managerProductAttribute.GetProperty("ColorSquaresRgb").StringValue;
                            var imageSquaresPictureId = managerProductAttribute.GetProperty("ImageSquaresPictureId").IntValue;
                            var priceAdjustment = managerProductAttribute.GetProperty("PriceAdjustment").DecimalValue;
                            var weightAdjustment = managerProductAttribute.GetProperty("WeightAdjustment").DecimalValue;
                            var cost = managerProductAttribute.GetProperty("Cost").DecimalValue;
                            var quantity = managerProductAttribute.GetProperty("Quantity").IntValue;
                            var isPreSelected = managerProductAttribute.GetProperty("IsPreSelected").BooleanValue;
                            var displayOrder = managerProductAttribute.GetProperty("DisplayOrder").IntValue;
                            var pictureId = managerProductAttribute.GetProperty("PictureId").IntValue;
                            var textPrompt = managerProductAttribute.GetProperty("AttributeTextPrompt").StringValue;
                            var isRequired = managerProductAttribute.GetProperty("AttributeIsRequired").BooleanValue;
                            var attributeDisplayOrder = managerProductAttribute.GetProperty("AttributeDisplayOrder").IntValue;

                            var productAttributeMapping = lastLoadedProduct.ProductAttributeMappings.FirstOrDefault(pam => pam.ProductAttributeId == productAttributeId);

                            if (productAttributeMapping == null)
                            {
                                //insert mapping
                                productAttributeMapping = new ProductAttributeMapping
                                {
                                    ProductId = lastLoadedProduct.Id,
                                    ProductAttributeId = productAttributeId,
                                    TextPrompt = textPrompt,
                                    IsRequired = isRequired,
                                    AttributeControlTypeId = attributeControlTypeId,
                                    DisplayOrder = attributeDisplayOrder
                                };
                                _productAttributeService.InsertProductAttributeMapping(productAttributeMapping);
                            }
                            else
                            {
                                productAttributeMapping.AttributeControlTypeId = attributeControlTypeId;
                                productAttributeMapping.TextPrompt = textPrompt;
                                productAttributeMapping.IsRequired = isRequired;
                                productAttributeMapping.DisplayOrder = attributeDisplayOrder;
                                _productAttributeService.UpdateProductAttributeMapping(productAttributeMapping);
                            }

                            var pav = _productAttributeService.GetProductAttributeValueById(productAttributeValueId);

                            var attributeControlType = (AttributeControlType)attributeControlTypeId;

                            if (pav == null)
                            {
                                switch (attributeControlType)
                                {
                                    case AttributeControlType.Datepicker:
                                    case AttributeControlType.FileUpload:
                                    case AttributeControlType.MultilineTextbox:
                                    case AttributeControlType.TextBox:
                                        continue;
                                }

                                pav = new ProductAttributeValue
                                {
                                    ProductAttributeMappingId = productAttributeMapping.Id,
                                    AttributeValueType = (AttributeValueType)attributeValueTypeId,
                                    AssociatedProductId = associatedProductId,
                                    Name = valueName,
                                    PriceAdjustment = priceAdjustment,
                                    WeightAdjustment = weightAdjustment,
                                    Cost = cost,
                                    IsPreSelected = isPreSelected,
                                    DisplayOrder = displayOrder,
                                    ColorSquaresRgb = colorSquaresRgb,
                                    ImageSquaresPictureId = imageSquaresPictureId,
                                    Quantity = quantity,
                                    PictureId = pictureId
                                };

                                _productAttributeService.InsertProductAttributeValue(pav);
                            }
                            else
                            {
                                pav.AttributeValueTypeId = attributeValueTypeId;
                                pav.AssociatedProductId = associatedProductId;
                                pav.Name = valueName;
                                pav.ColorSquaresRgb = colorSquaresRgb;
                                pav.ImageSquaresPictureId = imageSquaresPictureId;
                                pav.PriceAdjustment = priceAdjustment;
                                pav.WeightAdjustment = weightAdjustment;
                                pav.Cost = cost;
                                pav.Quantity = quantity;
                                pav.IsPreSelected = isPreSelected;
                                pav.DisplayOrder = displayOrder;
                                pav.PictureId = pictureId;

                                _productAttributeService.UpdateProductAttributeValue(pav);
                            }
                        }
                        continue;
                    }

                    manager.ReadFromXlsx(worksheet, iRow);

                    var product = skuCellNum > 0 ? allProductsBySku.FirstOrDefault(p => p.Sku == manager.GetProperty("SKU").StringValue) : null;

                    var isNew = product == null;

                    product = product ?? new Product();

                    if (isNew)
                        product.CreatedOnUtc = DateTime.UtcNow;

                    foreach (var property in manager.GetProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "ProductType":
                                product.ProductTypeId = property.IntValue;
                                break;
                            case "ParentGroupedProductId":
                                product.ParentGroupedProductId = property.IntValue;
                                break;
                            case "VisibleIndividually":
                                product.VisibleIndividually = property.BooleanValue;
                                break;
                            case "Name":
                                product.Name = property.StringValue;
                                break;
                            case "ShortDescription":
                                product.ShortDescription = property.StringValue;
                                break;
                            case "FullDescription":
                                product.FullDescription = property.StringValue;
                                break;
                            case "Vendor":
                                product.VendorId = property.IntValue;
                                break;
                            case "ProductTemplate":
                                product.ProductTemplateId = property.IntValue;
                                break;
                            case "ShowOnHomePage":
                                product.ShowOnHomePage = property.BooleanValue;
                                break;
                            case "MetaKeywords":
                                product.MetaKeywords = property.StringValue;
                                break;
                            case "MetaDescription":
                                product.MetaDescription = property.StringValue;
                                break;
                            case "MetaTitle":
                                product.MetaTitle = property.StringValue;
                                break;
                            case "AllowCustomerReviews":
                                product.AllowCustomerReviews = property.BooleanValue;
                                break;
                            case "Published":
                                product.Published = property.BooleanValue;
                                break;
                            case "SKU":
                                product.Sku = property.StringValue;
                                break;
                            case "ManufacturerPartNumber":
                                product.ManufacturerPartNumber = property.StringValue;
                                break;
                            case "Gtin":
                                product.Gtin = property.StringValue;
                                break;
                            case "IsGiftCard":
                                product.IsGiftCard = property.BooleanValue;
                                break;
                            case "GiftCardType":
                                product.GiftCardTypeId = property.IntValue;
                                break;
                            case "OverriddenGiftCardAmount":
                                product.OverriddenGiftCardAmount = property.DecimalValue;
                                break;
                            case "RequireOtherProducts":
                                product.RequireOtherProducts = property.BooleanValue;
                                break;
                            case "RequiredProductIds":
                                product.RequiredProductIds = property.StringValue;
                                break;
                            case "AutomaticallyAddRequiredProducts":
                                product.AutomaticallyAddRequiredProducts = property.BooleanValue;
                                break;
                            case "IsDownload":
                                product.IsDownload = property.BooleanValue;
                                break;
                            case "DownloadId":
                                product.DownloadId = property.IntValue;
                                break;
                            case "UnlimitedDownloads":
                                product.UnlimitedDownloads = property.BooleanValue;
                                break;
                            case "MaxNumberOfDownloads":
                                product.MaxNumberOfDownloads = property.IntValue;
                                break;
                            case "DownloadActivationType":
                                product.DownloadActivationTypeId = property.IntValue;
                                break;
                            case "HasSampleDownload":
                                product.HasSampleDownload = property.BooleanValue;
                                break;
                            case "SampleDownloadId":
                                product.SampleDownloadId = property.IntValue;
                                break;
                            case "HasUserAgreement":
                                product.HasUserAgreement = property.BooleanValue;
                                break;
                            case "UserAgreementText":
                                product.UserAgreementText = property.StringValue;
                                break;
                            case "IsRecurring":
                                product.IsRecurring = property.BooleanValue;
                                break;
                            case "RecurringCycleLength":
                                product.RecurringCycleLength = property.IntValue;
                                break;
                            case "RecurringCyclePeriod":
                                product.RecurringCyclePeriodId = property.IntValue;
                                break;
                            case "RecurringTotalCycles":
                                product.RecurringTotalCycles = property.IntValue;
                                break;
                            case "IsRental":
                                product.IsRental = property.BooleanValue;
                                break;
                            case "RentalPriceLength":
                                product.RentalPriceLength = property.IntValue;
                                break;
                            case "RentalPricePeriod":
                                product.RentalPricePeriodId = property.IntValue;
                                break;
                            case "IsShipEnabled":
                                product.IsShipEnabled = property.BooleanValue;
                                break;
                            case "IsFreeShipping":
                                product.IsFreeShipping = property.BooleanValue;
                                break;
                            case "ShipSeparately":
                                product.ShipSeparately = property.BooleanValue;
                                break;
                            case "AdditionalShippingCharge":
                                product.AdditionalShippingCharge = property.DecimalValue;
                                break;
                            case "DeliveryDate":
                                product.DeliveryDateId = property.IntValue;
                                break;
                            case "IsTaxExempt":
                                product.IsTaxExempt = property.BooleanValue;
                                break;
                            case "TaxCategory":
                                product.TaxCategoryId = property.IntValue;
                                break;
                            case "IsTelecommunicationsOrBroadcastingOrElectronicServices":
                                product.IsTelecommunicationsOrBroadcastingOrElectronicServices = property.BooleanValue;
                                break;
                            case "ManageInventoryMethod":
                                product.ManageInventoryMethodId = property.IntValue;
                                break;
                            case "UseMultipleWarehouses":
                                product.UseMultipleWarehouses = property.BooleanValue;
                                break;
                            case "WarehouseId":
                                product.WarehouseId = property.IntValue;
                                break;
                            case "StockQuantity":
                                product.StockQuantity = property.IntValue;
                                break;
                            case "DisplayStockAvailability":
                                product.DisplayStockAvailability = property.BooleanValue;
                                break;
                            case "DisplayStockQuantity":
                                product.DisplayStockQuantity = property.BooleanValue;
                                break;
                            case "MinStockQuantity":
                                product.MinStockQuantity = property.IntValue;
                                break;
                            case "LowStockActivity":
                                product.LowStockActivityId = property.IntValue;
                                break;
                            case "NotifyAdminForQuantityBelow":
                                product.NotifyAdminForQuantityBelow = property.IntValue;
                                break;
                            case "BackorderModeId":
                                product.BackorderModeId = property.IntValue;
                                break;
                            case "AllowBackInStockSubscriptions":
                                product.AllowBackInStockSubscriptions = property.BooleanValue;
                                break;
                            case "OrderMinimumQuantity":
                                product.OrderMinimumQuantity = property.IntValue;
                                break;
                            case "OrderMaximumQuantity":
                                product.OrderMaximumQuantity = property.IntValue;
                                break;
                            case "AllowedQuantities":
                                product.AllowedQuantities = property.StringValue;
                                break;
                            case "AllowAddingOnlyExistingAttributeCombinations":
                                product.AllowAddingOnlyExistingAttributeCombinations = property.BooleanValue;
                                break;
                            case "NotReturnable":
                                product.NotReturnable = property.BooleanValue;
                                break;
                            case "DisableBuyButton":
                                product.DisableBuyButton = property.BooleanValue;
                                break;
                            case "DisableWishlistButton":
                                product.DisableWishlistButton = property.BooleanValue;
                                break;
                            case "AvailableForPreOrder":
                                product.AvailableForPreOrder = property.BooleanValue;
                                break;
                            case "PreOrderAvailabilityStartDateTimeUtc":
                                product.PreOrderAvailabilityStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "CallForPrice":
                                product.CallForPrice = property.BooleanValue;
                                break;
                            case "Price":
                                product.Price = property.DecimalValue;
                                break;
                            case "OldPrice":
                                product.OldPrice = property.DecimalValue;
                                break;
                            case "ProductCost":
                                product.ProductCost = property.DecimalValue;
                                break;
                            case "SpecialPrice":
                                product.SpecialPrice = property.DecimalValueNullable;
                                break;
                            case "SpecialPriceStartDateTimeUtc":
                                product.SpecialPriceStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "SpecialPriceEndDateTimeUtc":
                                product.SpecialPriceEndDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "CustomerEntersPrice":
                                product.CustomerEntersPrice = property.BooleanValue;
                                break;
                            case "MinimumCustomerEnteredPrice":
                                product.MinimumCustomerEnteredPrice = property.DecimalValue;
                                break;
                            case "MaximumCustomerEnteredPrice":
                                product.MaximumCustomerEnteredPrice = property.DecimalValue;
                                break;
                            case "BasepriceEnabled":
                                product.BasepriceEnabled = property.BooleanValue;
                                break;
                            case "BasepriceAmount":
                                product.BasepriceAmount = property.DecimalValue;
                                break;
                            case "BasepriceUnit":
                                product.BasepriceUnitId = property.IntValue;
                                break;
                            case "BasepriceBaseAmount":
                                product.BasepriceBaseAmount = property.DecimalValue;
                                break;
                            case "BasepriceBaseUnit":
                                product.BasepriceBaseUnitId = property.IntValue;
                                break;
                            case "MarkAsNew":
                                product.MarkAsNew = property.BooleanValue;
                                break;
                            case "MarkAsNewStartDateTimeUtc":
                                product.MarkAsNewStartDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "MarkAsNewEndDateTimeUtc":
                                product.MarkAsNewEndDateTimeUtc = property.DateTimeNullable;
                                break;
                            case "Weight":
                                product.Weight = property.DecimalValue;
                                break;
                            case "Length":
                                product.Length = property.DecimalValue;
                                break;
                            case "Width":
                                product.Width = property.DecimalValue;
                                break;
                            case "Height":
                                product.Height = property.DecimalValue;
                                break;
                        }
                    }

                    //set default product type id
                    if (isNew && properties.All(p => p.PropertyName != "ProductTypeId"))
                        product.ProductType = ProductType.SimpleProduct;

                    product.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                    {
                        _productService.InsertProduct(product);
                    }
                    else
                    {
                        _productService.UpdateProduct(product);
                    }

                    tempProperty = manager.GetProperty("SeName");
                    if (tempProperty != null)
                    {
                        var seName = tempProperty.StringValue;
                        //search engine name
                        _urlRecordService.SaveSlug(product, product.ValidateSeName(seName, product.Name, true), 0);
                    }

                    tempProperty = manager.GetProperty("Categories");

                    if (tempProperty != null)
                    {
                        var categoryNames = tempProperty.StringValue;

                        //category mappings
                        var categories = isNew || !allProductsCategoryIds.ContainsKey(product.Id) ? new int[0] : allProductsCategoryIds[product.Id];
                        foreach (var categoryId in categoryNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => allCategories.First(c => c.Name == x.Trim()).Id))
                        {
                            if (categories.Any(c => c == categoryId))
                                continue;

                            var productCategory = new ProductCategory
                            {
                                ProductId = product.Id,
                                CategoryId = categoryId,
                                IsFeaturedProduct = false,
                                DisplayOrder = 1
                            };
                            _categoryService.InsertProductCategory(productCategory);
                        }
                    }

                    tempProperty = manager.GetProperty("Manufacturers");
                    if (tempProperty != null)
                    {
                        var manufacturerNames = tempProperty.StringValue;

                        //manufacturer mappings
                        var manufacturers = isNew || !allProductsManufacturerIds.ContainsKey(product.Id) ? new int[0] : allProductsManufacturerIds[product.Id];
                        foreach (var manufacturerId in manufacturerNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => allManufacturers.First(m => m.Name == x.Trim()).Id))
                        {
                            if (manufacturers.Any(c => c == manufacturerId))
                                continue;

                            var productManufacturer = new ProductManufacturer
                            {
                                ProductId = product.Id,
                                ManufacturerId = manufacturerId,
                                IsFeaturedProduct = false,
                                DisplayOrder = 1
                            };
                            _manufacturerService.InsertProductManufacturer(productManufacturer);
                        }
                    }

                    var picture1 = manager.GetProperty("Picture1").Return(p => p.StringValue, String.Empty);
                    var picture2 = manager.GetProperty("Picture2").Return(p => p.StringValue, String.Empty);
                    var picture3 = manager.GetProperty("Picture3").Return(p => p.StringValue, String.Empty);

                    productPictureMetadata.Add(new ProductPictureMetadata
                    {
                        ProductItem = product,
                        Picture1Path = picture1,
                        Picture2Path = picture2,
                        Picture3Path = picture3,
                        IsNew = isNew
                    });

                    lastLoadedProduct = product;

                    //update "HasTierPrices" and "HasDiscountsApplied" properties
                    //_productService.UpdateHasTierPricesProperty(product);
                    //_productService.UpdateHasDiscountsApplied(product);
                }

                if (_mediaSettings.ImportProductImagesUsingHash && _pictureService.StoreInDb && _dataProvider.SupportedLengthOfBinaryHash() > 0)
                    ImportProductImagesUsingHash(productPictureMetadata, allProductsBySku);
                else
                    ImportProductImagesUsingServices(productPictureMetadata);
            }
            //Trace.WriteLine(DateTime.Now-start);
        }
        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported subscribers</returns>
        public virtual int ImportNewsletterSubscribersFromTxt(Stream stream)
        {
            int count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] tmp = line.Split(',');

                    string email;
                    bool isActive = true;
                    int storeId = _storeContext.CurrentStore.Id;
                    //parse
                    if (tmp.Length == 1)
                    {
                        //"email" only
                        email = tmp[0].Trim();
                    }
                    else if (tmp.Length == 2)
                    {
                        //"email" and "active" fields specified
                        email = tmp[0].Trim();
                        isActive = Boolean.Parse(tmp[1].Trim());
                    }
                    else if (tmp.Length == 3)
                    {
                        //"email" and "active" and "storeId" fields specified
                        email = tmp[0].Trim();
                        isActive = Boolean.Parse(tmp[1].Trim());
                        storeId = Int32.Parse(tmp[2].Trim());
                    }
                    else
                        throw new NopException("Wrong file format");

                    //import
                    var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(email, storeId);
                    if (subscription != null)
                    {
                        subscription.Email = email;
                        subscription.Active = isActive;
                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
                    }
                    else
                    {
                        subscription = new NewsLetterSubscription
                        {
                            Active = isActive,
                            CreatedOnUtc = DateTime.UtcNow,
                            Email = email,
                            StoreId = storeId,
                            NewsLetterSubscriptionGuid = Guid.NewGuid()
                        };
                        _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    }
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported states</returns>
        public virtual int ImportStatesFromTxt(Stream stream)
        {
            int count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] tmp = line.Split(',');

                    if (tmp.Length != 5)
                        throw new NopException("Wrong file format");

                    //parse
                    var countryTwoLetterIsoCode = tmp[0].Trim();
                    var name = tmp[1].Trim();
                    var abbreviation = tmp[2].Trim();
                    bool published = Boolean.Parse(tmp[3].Trim());
                    int displayOrder = Int32.Parse(tmp[4].Trim());

                    var country = _countryService.GetCountryByTwoLetterIsoCode(countryTwoLetterIsoCode);
                    if (country == null)
                    {
                        //country cannot be loaded. skip
                        continue;
                    }

                    //import
                    var states = _stateProvinceService.GetStateProvincesByCountryId(country.Id, showHidden: true);
                    var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (state != null)
                    {
                        state.Abbreviation = abbreviation;
                        state.Published = published;
                        state.DisplayOrder = displayOrder;
                        _stateProvinceService.UpdateStateProvince(state);
                    }
                    else
                    {
                        state = new StateProvince
                        {
                            CountryId = country.Id,
                            Name = name,
                            Abbreviation = abbreviation,
                            Published = published,
                            DisplayOrder = displayOrder,
                        };
                        _stateProvinceService.InsertStateProvince(state);
                    }
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Import manufacturers from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportManufacturersFromXlsx(Stream stream)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Manufacturer>("Id"),
                new PropertyByName<Manufacturer>("Name"),
                new PropertyByName<Manufacturer>("Description"),
                new PropertyByName<Manufacturer>("ManufacturerTemplateId"),
                new PropertyByName<Manufacturer>("MetaKeywords"),
                new PropertyByName<Manufacturer>("MetaDescription"),
                new PropertyByName<Manufacturer>("MetaTitle"),
                new PropertyByName<Manufacturer>("SeName"),
                new PropertyByName<Manufacturer>("Picture"),
                new PropertyByName<Manufacturer>("PageSize"),
                new PropertyByName<Manufacturer>("AllowCustomersToSelectPageSize"),
                new PropertyByName<Manufacturer>("PageSizeOptions"),
                new PropertyByName<Manufacturer>("PriceRanges"),
                new PropertyByName<Manufacturer>("Published"),
                new PropertyByName<Manufacturer>("DisplayOrder")
            };

            var manager = new PropertyManager<Manufacturer>(properties);

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                var iRow = 2;

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadFromXlsx(worksheet, iRow);

                    var manufacturer = _manufacturerService.GetManufacturerById(manager.GetProperty("Id").IntValue);

                    var isNew = manufacturer == null;

                    manufacturer = manufacturer ?? new Manufacturer();

                    if (isNew)
                        manufacturer.CreatedOnUtc = DateTime.UtcNow;

                    manufacturer.Name = manager.GetProperty("Name").StringValue;
                    manufacturer.Description = manager.GetProperty("Description").StringValue;
                    manufacturer.ManufacturerTemplateId = manager.GetProperty("ManufacturerTemplateId").IntValue;
                    manufacturer.MetaKeywords = manager.GetProperty("MetaKeywords").StringValue;
                    manufacturer.MetaDescription = manager.GetProperty("MetaDescription").StringValue;
                    manufacturer.MetaTitle = manager.GetProperty("MetaTitle").StringValue;
                    var picture = LoadPicture(manager.GetProperty("Picture").StringValue, manufacturer.Name,
                        isNew ? null : (int?)manufacturer.PictureId);
                    manufacturer.PageSize = manager.GetProperty("PageSize").IntValue;
                    manufacturer.AllowCustomersToSelectPageSize = manager.GetProperty("AllowCustomersToSelectPageSize").BooleanValue;
                    manufacturer.PageSizeOptions = manager.GetProperty("PageSizeOptions").StringValue;
                    manufacturer.PriceRanges = manager.GetProperty("PriceRanges").StringValue;
                    manufacturer.Published = manager.GetProperty("Published").BooleanValue;
                    manufacturer.DisplayOrder = manager.GetProperty("DisplayOrder").IntValue;

                    if (picture != null)
                        manufacturer.PictureId = picture.Id;

                    manufacturer.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                        _manufacturerService.InsertManufacturer(manufacturer);
                    else
                        _manufacturerService.UpdateManufacturer(manufacturer);

                    //search engine name
                    var seName = manager.GetProperty("SeName").StringValue;
                    _urlRecordService.SaveSlug(manufacturer, manufacturer.ValidateSeName(seName, manufacturer.Name, true), 0);

                    iRow++;
                }
            }
        }

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportCategoriesFromXlsx(Stream stream)
        {
            var properties = new[]
            {
                new PropertyByName<Category>("Id"),
                new PropertyByName<Category>("Name"),
                new PropertyByName<Category>("Description"),
                new PropertyByName<Category>("CategoryTemplateId"),
                new PropertyByName<Category>("MetaKeywords"),
                new PropertyByName<Category>("MetaDescription"),
                new PropertyByName<Category>("MetaTitle"),
                new PropertyByName<Category>("SeName"),
                new PropertyByName<Category>("ParentCategoryId"),
                new PropertyByName<Category>("Picture"),
                new PropertyByName<Category>("PageSize"),
                new PropertyByName<Category>("AllowCustomersToSelectPageSize"),
                new PropertyByName<Category>("PageSizeOptions"),
                new PropertyByName<Category>("PriceRanges"),
                new PropertyByName<Category>("ShowOnHomePage"),
                new PropertyByName<Category>("IncludeInTopMenu"),
                new PropertyByName<Category>("Published"),
                new PropertyByName<Category>("DisplayOrder")
            };

            var manager = new PropertyManager<Category>(properties);

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                var iRow = 2;

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadFromXlsx(worksheet, iRow);

                    var category = _categoryService.GetCategoryById(manager.GetProperty("Id").IntValue);

                    var isNew = category == null;

                    category = category ?? new Category();

                    if (isNew)
                        category.CreatedOnUtc = DateTime.UtcNow;

                    category.Name = manager.GetProperty("Name").StringValue;
                    category.Description = manager.GetProperty("Description").StringValue;

                    category.CategoryTemplateId = manager.GetProperty("CategoryTemplateId").IntValue;
                    category.MetaKeywords = manager.GetProperty("MetaKeywords").StringValue;
                    category.MetaDescription = manager.GetProperty("MetaDescription").StringValue;
                    category.MetaTitle = manager.GetProperty("MetaTitle").StringValue;
                    category.ParentCategoryId = manager.GetProperty("ParentCategoryId").IntValue;
                    var picture = LoadPicture(manager.GetProperty("Picture").StringValue, category.Name, isNew ? null : (int?)category.PictureId);
                    //rel
                    var pictureB = LoadPicture(manager.GetProperty("PictureB").StringValue, category.Name, isNew ? null : (int?)category.PictureBId);
                    category.PageSize = manager.GetProperty("PageSize").IntValue;
                    category.AllowCustomersToSelectPageSize = manager.GetProperty("AllowCustomersToSelectPageSize").BooleanValue;
                    category.PageSizeOptions = manager.GetProperty("PageSizeOptions").StringValue;
                    category.PriceRanges = manager.GetProperty("PriceRanges").StringValue;
                    category.ShowOnHomePage = manager.GetProperty("ShowOnHomePage").BooleanValue;
                    category.IncludeInTopMenu = manager.GetProperty("IncludeInTopMenu").BooleanValue;
                    category.Published = manager.GetProperty("Published").BooleanValue;
                    category.DisplayOrder = manager.GetProperty("DisplayOrder").IntValue;

                    if (picture != null)
                        category.PictureId = picture.Id;
                    //rel
                    if (pictureB != null)
                        category.PictureBId = pictureB.Id;

                    category.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                        _categoryService.InsertCategory(category);
                    else
                        _categoryService.UpdateCategory(category);

                    //search engine name
                    var seName = manager.GetProperty("SeName").StringValue;
                    _urlRecordService.SaveSlug(category, category.ValidateSeName(seName, category.Name, true), 0);

                    iRow++;
                }
            }
        }

        #endregion

        #region Nested classes

        protected class ProductPictureMetadata
        {
            public Product ProductItem { get; set; }
            public string Picture1Path { get; set; }
            public string Picture2Path { get; set; }
            public string Picture3Path { get; set; }
            public bool IsNew { get; set; }
        }

        protected class ProductAttributeValueMetadata
        {
            public Product ProductItem { get; set; }
            public ProductAttributeValue ProductAttributeValue { get; set; }
            public string BigPicture_ProductPicturePath { get; set; }
            public string SmallPicture_PAV_BoxPicturePath { get; set; }
            public bool ProductIsNew { get; set; }
            public bool AttributeValueIsNew { get; set; }
        }

        private class Conditions
        {
            public int ProductAttributeMappingId { get; set; }
            public int ProductId { get; set; }
            public int AttributeValueId { get; set; }
            public int RowValueId { get; set; }
            public int RowValueIdForCondition { get; set; }
            public bool RowValueIsSelected { get; set; }
            public bool HasCondition { get; set; }
        }

        #endregion
    }
}
