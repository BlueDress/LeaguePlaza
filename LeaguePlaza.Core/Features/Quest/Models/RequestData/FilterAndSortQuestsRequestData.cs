namespace LeaguePlaza.Core.Features.Quest.Models.RequestData
{
    public class FilterAndSortQuestsRequestData
    {
        public string? SearchTerm { get; set; }

        public string? SortBy { get; set; }

        public bool OrderIsDescending { get; set; }

        public IEnumerable<string> StatusFilters { get; set; } = new List<string>();

        public IEnumerable<string> TypeFilters { get; set; } = new List<string>();

        public int CurrentPage { get; set; }
    }
}
