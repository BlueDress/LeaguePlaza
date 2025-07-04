namespace LeaguePlaza.Common.Constants
{
    public static class ProductConstants
    {
        public const string NoProductDescriptionAvailable = "No description available";

        public const int RecommendedProductsCount = 3;

        public const int ProductNameMinLength = 1;
        public const int ProductNameMaxLength = 50;
        public const int ProductDescriptionMaxLength = 500;
        public const int ProductImageUrlMaxLength = 500;
        public const int ProductFileMaxSize = 5 * 1024 * 1024;

        public const string ProductCardsContainerWithPagination = "~/Views/Product/Partials/_ProductCardsContainerWithPagination.cshtml";
    }
}
