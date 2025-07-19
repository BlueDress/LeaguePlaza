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

        public const string ImageUploadPath = "/products/{0}/{1}/{2}";
        public const string HealingDefaultImageUrl = "https://www.dropbox.com/scl/fi/whb5luj0oh5kp2cpe7x82/healing-default.jpg?rlkey=2t4akdjnz5c6sxbd4lku9g0jg&st=ua4xrg64&raw=1";
        public const string EnhancementDefaultImageUrl = "https://www.dropbox.com/scl/fi/v3ez1ce7ftd4d7zeg3pja/enhancement-default.jpg?rlkey=iu0at7i5r3nnzbajdqpxsgdn4&st=4cpejjwu&raw=1";
        public const string ImpairmentDefaultImageUrl = "https://www.dropbox.com/scl/fi/aheohoh9av3kvoxa5crlb/impairment-default.jpg?rlkey=vincatdic90xdwko627bi4cbj&st=5nsgr1ri&raw=1";
    }
}
