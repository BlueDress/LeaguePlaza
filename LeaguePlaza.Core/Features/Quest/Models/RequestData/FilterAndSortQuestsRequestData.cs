namespace LeaguePlaza.Core.Features.Quest.Models.RequestData
{
    public class FilterAndSortQuestsRequestData
    {
        public string? SearchTerm { get; set; }

        public string? SortBy { get; set; }

        public bool OrderIsDescending { get; set; }

        public string? StatusFilters { get; set; }

        public string? TypeFilters { get; set; }

        public int CurrentPage { get; set; }
    }
}
