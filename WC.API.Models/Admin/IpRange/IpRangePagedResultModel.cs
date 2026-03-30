using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.Models.Admin.IpRange
{
    public class IpRangePagedResultModel
    {
        public List<IpRangeListItemModel> Items { get; set; } = new();

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public int? CountryId { get; set; }
        public int? IpVersion { get; set; }
        public bool ActiveOnly { get; set; }
        public string? Search { get; set; }
    }
}
