namespace WC.Models.Admin
{
    public class IpRangeFilterModel
    {
        public int? CountryId { get; set; }
        public int? IpVersion { get; set; }
        public bool ActiveOnly { get; set; } = true;
        public string? Search { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
