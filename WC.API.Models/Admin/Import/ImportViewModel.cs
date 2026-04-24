using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.Models.Admin.Import
{
    public class ImportViewModel
    {
        public IFormFile? File { get; set; }

        public string? ResultMessage { get; set; }
        public bool? Success { get; set; }

        public int ProcessedCount { get; set; }
        public int InsertedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedCount { get; set; }
    }
}
