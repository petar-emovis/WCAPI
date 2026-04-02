namespace WC.Models.DTO
{
    public class Country
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? CountryCodeIso2 { get; set; }

        public string? CountryCodeIso3 { get; set; }

        public string? CountryCodeNumerical { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public virtual ICollection<IpRange> IpRanges { get; set; } = new List<IpRange>();
    }
}
