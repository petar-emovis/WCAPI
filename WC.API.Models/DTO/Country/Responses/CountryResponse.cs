using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.Models.DTO
{
    public class CountryResponse
    {
        public string? CountryName { get; set; }
        public string? CountryCodeIso2 { get; set; }
        public string? CountryCodeIso3 { get; set; }
        public string? StartIp { get; set; }
        public string? EndIp { get; set; }
    }
}
