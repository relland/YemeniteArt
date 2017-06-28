


var quickViewApi = function() {
};


quickViewApi.prototype.viewProductDetails = function(options) {
    var config = $.extend({
        data: { },
        success: function() {
        },
        error: function() {
        }
    }, options);

    $.apiCall({
        type: 'POST',
        data: config.data,
        url: '/bs_product_details',
        success: function(reponse) {

            $("#quick-view-modal").html(reponse.html).dialog({
                classes: {
                    "ui-widget": "quick-view-modal"
                }
            });//rel
            console.log(reponse);
            $(document).trigger("hide-ajax-loading");
            $("#quick-view-loading-modal").hide();//rel .modal("hide");
            $("#quick-view-product-details-modal").show();//rel .modal("show");
            
        }
    });

};






var api = new quickViewApi();