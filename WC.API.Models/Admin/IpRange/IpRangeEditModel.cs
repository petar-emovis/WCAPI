namespace WC.Models.Admin
{
    public class IpRangeEditModel
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
        public int IpVersion { get; set; }
        public string StartIp { get; set; } = string.Empty;
        public string EndIp { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
