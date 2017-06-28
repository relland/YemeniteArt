﻿//@model Nop.Plugin.BrainStation.QuickView.Models.BsQuickViewModel
//@using System;
//@using System.Collections.Generic;
//@using Nop.Core.Domain.Seo;
//@using Nop.Core.Infrastructure;
//@using Nop.Web.Models.Catalog;
//@using Nop.Web.Framework.UI;
//@using System.Web.Mvc;
//@using Nop.Web.Framework;
//@using Nop.Web.Framework.Mvc;
//@{
   

//    //title, meta
//    Html.AddTitleParts(!String.IsNullOrEmpty(Model.ProductDetailsModel.MetaTitle) ? Model.ProductDetailsModel.MetaTitle : Model.ProductDetailsModel.Name);
//Html.AddMetaDescriptionParts(Model.ProductDetailsModel.MetaDescription);
//    Html.AddMetaKeywordParts(Model.ProductDetailsModel.MetaKeywords);

//    var canonicalUrlsEnabled = EngineContext.Current.Resolve<SeoSettings>().CanonicalUrlsEnabled;
//    if (canonicalUrlsEnabled)
//    {
//        var productUrl = Url.RouteUrl("Product", new { SeName = Model.ProductDetailsModel.SeName }, this.Request.Url.Scheme);
//Html.AddCanonicalUrlParts(productUrl);
//    }
//}


//<div data-productid="@Model.ProductDetailsModel.Id" class="modal modal-static fade quick-view-product-details-modal"  id="quick-view-product-details-modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
//    <div class="modal-dialog">
//        <div class="modal-content">
//            <div class="modal-header">
//                <button type = "button" class="close" data-dismiss="modal" aria-hidden="true"><i class="fa fa-times"></i></button>
//                <h4 class="modal-title">@Model.ProductDetailsModel.Name</h4>
//            </div>
//            <div class="modal-body">
//                <div class="row">
//                    <div class="col-lg-5 col-md-5 col-sm-6 col-xs-12">
                        

                       


//                        <div class="quick-view-product-details-carousal-controller-container">
                            
//                            <span class="pull-right">
//                                <a class="left-controller" href="#modal-product-picture-carousal" data-slide="prev">
//                                    <i class="fa fa-angle-left fa-2x"></i>
//                                </a>
//                                <a class="right-controller" href="#modal-product-picture-carousal" data-slide="next">
//                                    <i class="fa fa-angle-right fa-2x"></i>
//                                </a>

//                            </span>

//                        </div>


//                        <div id = "modal-product-picture-carousal" class="carousel slide" data-ride="carousel">
//                            <div class="carousel-inner">


//                                @for(int i = 0; i<Model.ProductDetailsModel.PictureModels.Count; i++)
            
//                                {
//                                    var picture = Model.ProductDetailsModel.PictureModels[i];
//                                    if(i==0)
//                                    {
//                                        @:<div class="item active">
                
//                                    }else
//                                    {
//                                        @:<div class="item">
//                                    }
                  

//                                    <div class="quick-view-product-details-slide-image">
//                                        <img src = "@picture.FullSizeImageUrl" alt="@picture.AlternateText" title="@picture.Title" />
                
//                                    </div>

                    

//                                    @:</Div>
            
//                                }
        
        
//                            </div>

   

//                        </div>


//                    </div>

//                    <div class="col-lg-7 col-md-7 col-sm-6 col-xs-12">
//                        @using(Html.BeginRouteForm("Product", new { SeName = Model.ProductDetailsModel.SeName }, FormMethod.Post, new { id = "product-details-form" }))
//                        {
//                            <div itemscope itemtype="http://schema.org/Product" data-productid="@Model.ProductDetailsModel.Id">
//                                <div class=" product-essential">
                    
                                    

//                                    <div class="quick-view-product-details-information">
//                                        <!--Price-->
//                                        @{
//                                            var dataDictPrice = new ViewDataDictionary();
//dataDictPrice.TemplateInfo.HtmlFieldPrefix = string.Format("price_{0}", Model.ProductDetailsModel.Id);
//@Html.Partial("_QuickViewProductPrice", Model.ProductDetailsModel.ProductPrice, dataDictPrice)
//                                        }

//                                        <!--product reviews-->
//                                        @Html.Partial("_QuickViewProductReviewOverview", Model.ProductDetailsModel.ProductReviewOverview)

//                                        @if(!String.IsNullOrEmpty(Model.ProductDetailsModel.ShortDescription))
//                                        {
//                                            <div class="quick-view-short-description">
//                                                @Html.Raw(Model.ProductDetailsModel.ShortDescription)
//                                            </div>@:</br>
//                                        }
//                                        <!--product SKU, manufacturer part number, stock info-->
//                                        @Html.Partial("_QuickViewSKU_Man_Stock", Model.ProductDetailsModel)

//                                       <!--product manufacturers-->
//                                         @Html.Partial("_QuickViewProductManufacturers", Model.ProductDetailsModel.ProductManufacturers)
//                                        <!--sample download-->
//                                        @Html.Partial("_QuickViewDownloadSample", Model.ProductDetailsModel)
//                                        <!--attributes-->
//                                        @{         
                                            
//                                            var dataDictAttributes = new ViewDataDictionary();
//dataDictAttributes.TemplateInfo.HtmlFieldPrefix = string.Format("attributes_{0}", Model.ProductDetailsModel.Id);
//@Html.Partial("_QuickViewProductAttributes", Model.ProductDetailsModel.ProductAttributes, dataDictAttributes)                
//                                        }
//                                        <!--gift card-->
//                                        @{
//                                            var dataDictGiftCard = new ViewDataDictionary();
//dataDictGiftCard.TemplateInfo.HtmlFieldPrefix = string.Format("giftcard_{0}", Model.ProductDetailsModel.Id);
//@Html.Partial("_QuickViewGiftCardInfo", Model.ProductDetailsModel.GiftCard, dataDictGiftCard)
//                                        }
//                                        <!-- add to cart-->
                                        
//                                        @{
                                            
                            
//                                            var dataDictAddToCart = new ViewDataDictionary();
//dataDictAddToCart.TemplateInfo.HtmlFieldPrefix = string.Format("addtocart_{0}", Model.ProductDetailsModel.Id);
//@Html.Partial("_QuickViewAddToCart", Model.ProductDetailsModel.AddToCart, dataDictAddToCart)                    
//                                        }
//                                        <div id = "quick-view-details-error-message-div" ></ div >
//                                        < div data-product-name="@Model.ProductDetailsModel.SeName" data-productid="@Model.ProductDetailsModel.Id" data-pictureurl="@Model.ProductDetailsModel.DefaultPictureModel.ImageUrl" class="quick-view-details-page-image-and-name-info" style="display: none" ></div>
//                                        @*<div class= "form-group row">
//                                            @Html.Action("QuickViewProductEmailAFriendButton", "QuickView", new { productId = Model.ProductDetailsModel.Id })

//                                            @Html.Action("QuickViewCompareProductsButton", "QuickView", new { productId = Model.ProductDetailsModel.Id })
//                                        </div>*@

//                                       @Html.Partial("_QuickViewShareButton", Model.ProductDetailsModel)
                        
//                                    </div>

//                                </div>
                
                               
//                            </div>

//                        }
//                    </div>

                  
//                </div>
//                <div class="space15px"></div>
//                <div class="row">
//                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
//                        <div class="category-tab quick-view-menu-product-details-nav">
//                            <ul class="nav nav-tabs ">
//                                @if(!String.IsNullOrEmpty(Model.ProductDetailsModel.FullDescription))
//                                {
                          
//                                    <li class="active"><a href = "#description" data-toggle="tab">Description</a></li>
//                                }
//                                @if(!String.IsNullOrEmpty(@Html.Partial("_QuickViewProductSpecifications", Model.ProductDetailsModel.ProductSpecifications).ToString()))
//                                {

//                                    <li><a href = "#specification" data-toggle="tab">Specifications</a></li>
//                                }
//                                @if(!String.IsNullOrEmpty(@Html.Partial("_QuickViewProductTags", Model.ProductDetailsModel.ProductTags).ToString()))
//                                {
//                                    <li><a href = "#tags" data-toggle="tab">Products Tags</a></li>
//                                }@if(!String.IsNullOrEmpty(@Html.Partial("_QuickViewProductTierPrices", Model.ProductDetailsModel.TierPrices).ToString()))
//                                 {
//                                     <li><a href = "#tierprice" data-toggle="tab">Special Prices</a></li>
//                                 }
//                            </ul>


//                            <div class="tab-content">
//                                @if(!String.IsNullOrEmpty(Model.ProductDetailsModel.FullDescription))
//                                {
//                                    <div class="quick-view-full-description tab-pane active" id="description" itemprop="description">
//                                        @Html.Raw(Model.ProductDetailsModel.FullDescription)
//                                    </div>
//                                }
//                                <div class="quick-view-full-description tab-pane" id="specification">
//                                    @Html.Partial("_QuickViewProductSpecifications", Model.ProductDetailsModel.ProductSpecifications)
//                                </div>
//                                <div class="quick-view-full-description tab-pane" id="tags">
//                                    @Html.Partial("_QuickViewProductTags", Model.ProductDetailsModel.ProductTags)
//                                </div>
                            
//                                <div class="quick-view-full-description tab-pane" id="tierprice">
//                                    @Html.Partial("_QuickViewProductTierPrices", Model.ProductDetailsModel.TierPrices)
//                                </div>


//                            </div> 
//                        </div>

                        



                        
//                    </div>


//                </div>
//                <div class="product-collateral">
//                    @if(Model.BsQuickViewSettingsModel.ShowAlsoPurchased)
//{
//    @Html.Action("ProductsAlsoPurchased", "BsQuickView", new { productId = Model.ProductDetailsModel.Id })
//                    }
//                    @if(Model.BsQuickViewSettingsModel.ShowRelatedProducts)
//{
//    @Html.Action("RelatedProducts", "BsQuickView", new { productId = Model.ProductDetailsModel.Id })
//                    }
//                </div>

              
//            </div>
//            <div class="modal-footer">
//                <button type = "button" class="btn btn-primary" data-dismiss="modal">Close</button>
          
//            </div>
//        </div>
//    </div>
//</div>
















