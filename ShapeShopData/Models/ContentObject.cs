namespace ShapeShopData.Models
{
    public abstract class ContentObject
    {
        /// The name of the content pipeline asset that contained this object.
        private string assetName;
        public string AssetName
        {
            get { return assetName; }
            set { assetName = value; }
        }
    }
}
