using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WC.Models.Admin
{
    public class IpRangeEditPageViewModel
    {
        public int Id { get; set; }

        [Required]
        public int CountryId { get; set; }

        [Required]
        public int IpVersion { get; set; }

        [Required]
        public string StartIp { get; set; } = string.Empty;

        [Required]
        public string EndIp { get; set; } = string.Empty;

        public bool Active { get; set; } = true;

        public List<SelectListItem> CountryOptions { get; set; } = new();
        public List<SelectListItem> IpVersionOptions { get; set; } = new();
    }
}