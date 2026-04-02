using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.Models.Admin.Country
{
    public class CountryViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Iso2Code { get; set; } = string.Empty;
        public string? Iso3Code { get; set; } = string.Empty;
        public int IpRangeCount { get; set; }
        public int ActiveIpRangeCount { get; set; }
    }
}
