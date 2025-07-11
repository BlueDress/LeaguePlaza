namespace LeaguePlaza.Common.Constants
{
    public static class OrderConstants
    {
        public const int OrderCountryMinLength = 1;
        public const int OrderCountryMaxLength = 200;
        public const int OrderCityMinLength = 1;
        public const int OrderCityMaxLength = 200;
        public const int OrderStreetMinLength = 1;
        public const int OrderStreetMaxLength = 200;
        public const int OrderPostalCodeMinLength = 1;
        public const int OrderPostalCodeMaxLength = 20;
        public const int OrderAdditionalInformationMaxLength = 200;

        public const string OrderHistoryContainerWithPagination = "~/Views/Order/Partials/_OrderHistoryContainerWithPagination.cshtml";
        public const string OrderInformation = "~/Views/Order/Partials/_OrderInformation.cshtml";
        public const string CartItems = "~/Views/Order/Partials/_CartItems.cshtml";
        public const string SubmitOrder = "~/Views/Order/Partials/_SubmitOrder.cshtml";
        public const string OrderSuccessful = "~/Views/Order/Partials/_OrderSuccessful.cshtml";
        public const string OrderFailed = "~/Views/Order/Partials/_OrderFailed.cshtml";

        public const string ProductAddedToCartSuccessfully = "Product added to cart";
    }
}
