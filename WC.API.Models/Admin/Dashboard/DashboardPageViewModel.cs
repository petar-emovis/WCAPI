namespace WC.Admin.Models.Dashboard
{
    public class DashboardPageViewModel
    {
        public int TotalCountries { get; set; }
        public int TotalIpRanges { get; set; }
        public int ActiveIpRanges { get; set; }
        public int IPv4Ranges { get; set; }
        public int IPv6Ranges { get; set; }

        public DateTime? LastImportDate { get; set; }
        public string? LastImportStatus { get; set; }
    }
}