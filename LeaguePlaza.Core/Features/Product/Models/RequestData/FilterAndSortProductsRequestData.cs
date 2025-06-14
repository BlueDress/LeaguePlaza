namespace LeaguePlaza.Core.Features.Product.Models.RequestData
{
    public class FilterAndSortProductsRequestData
    {
        public string? SearchTerm { get; set; }

        public string? SortBy { get; set; }

        public bool OrderIsDescending { get; set; }

        public string? TypeFilters { get; set; }

        public int CurrentPage { get; set; }
    }
}
