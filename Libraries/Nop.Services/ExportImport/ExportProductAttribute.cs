namespace Nop.Services.ExportImport
{
    public partial class ExportProductAttribute
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeTextPrompt { get; set; }
        public bool AttributeIsRequired { get; set; }
        //rel
        public bool IsStoneAttribute { get; set; }
        public bool HasCondition { get; set; }
        public int RowValueIdForCondition { get; set; }
        public bool RowValueIsSelected { get; set; }
        public int RowValueId { get; set; }
        //rel
        public int AttributeDisplayOrder { get; set; }
        public int PictureId { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int AttributeValueTypeId { get; set; }
        public int AssociatedProductId { get; set; }
        public int Id { get; set; }
        public int ImageSquaresPictureId { get; set; }
        public string Name { get; set; }
        public decimal WeightAdjustment { get; set; }
        public int Quantity { get; set; }
        public bool IsPreSelected { get; set; }
        public string ColorSquaresRgb { get; set; }
        public decimal PriceAdjustment { get; set; }
        public decimal Cost { get; set; }
        public int DisplayOrder { get; set; }
        //rel
        //public bool IsStoneProductAttributeValue { get; set; }
        public int ZIndex { get; set; }
        public string SKUCode { get; set; }
        public string StoneNameId { get; set; }
        public string BigPicture { get; set; }
        public string SmallPicture { get; set; }
        
        //rel

        public static int ProducAttributeCellOffset = 2;
    }
}