namespace LeaguePlaza.Core.Features.Mount.Models.RequestData
{
    public class FilterAndSortMountsRequestData
    {
        public string? SearchTerm { get; set; }

        public string? SortBy { get; set; }

        public bool OrderIsDescending { get; set; }

        public string? TypeFilters { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int CurrentPage { get; set; }
    }
}
