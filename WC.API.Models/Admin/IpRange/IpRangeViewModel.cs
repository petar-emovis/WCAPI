namespace WC.Models.Admin
{
    public class IpRangeViewModel
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string? CountryName { get; set; } = string.Empty;
        public string? CountryCodeIso2 { get; set; } = string.Empty;
        public int IpVersion { get; set; }
        public string StartIp { get; set; } = string.Empty;
        public string EndIp { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
