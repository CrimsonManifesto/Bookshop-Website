namespace Bookshop_Website.Models
{
    public class AdvancedSearchModel
    {
        public string? Title { get; set; }
        public string? Language { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Discount { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public List<string>? Languages { get; set; } = new List<string>();

    }
}
