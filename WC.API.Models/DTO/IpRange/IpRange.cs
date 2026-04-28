using WC.Models.Enums;

namespace WC.Models.DTO
{
    public class IpRange
    {
        public int Id { get; set; }

        public int CountryId { get; set; }

        public IpVersionEnum? IpVersion { get; set; }

        public string? StartIp { get; set; }

        public string? EndIp { get; set; }

        public long? StartIpNumeric { get; set; }

        public long? EndIpNumeric { get; set; }

        public byte[] StartIpBinary { get; set; } = null!;

        public byte[] EndIpBinary { get; set; } = null!;

        public bool Active { get; set; }
        public long? StartIpv6High { get; set; }

        public long? StartIpv6Low { get; set; }

        public long? EndIpv6High { get; set; }

        public long? EndIpv6Low { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public Country Country { get; set; } = null!;
    }
}
