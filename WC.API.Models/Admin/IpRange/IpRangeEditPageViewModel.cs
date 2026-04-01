using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WC.Models.Admin
{
    public class IpRangeEditPageViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please  select Country")]
        public int CountryId { get; set; }
        
        public string? CountryName { get; set; }

        [Required(ErrorMessage = "Please  select IpVersion")]
        public int IpVersion { get; set; }
        
        public string IpVersionString
        {
            get
            {
                return ((IpVersionEnum)IpVersion).ToString();
            }
        }

        [Required(ErrorMessage = "Please  select StartIp")]
        public string StartIp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please  select EndIp")]
        public string EndIp { get; set; } = string.Empty;

        public bool Active { get; set; } = true;

        public List<SelectListItem> CountryOptions { get; set; } = new();
        public List<SelectListItem> IpVersionOptions { get; set; } = new();
    }
}