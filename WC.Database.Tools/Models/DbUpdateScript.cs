using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.Database.Tools.Models
{
    public sealed class DbUpdateScript
    {
        public string Name { get; init; } = null!;
        public string ResourceName { get; init; } = null!;
        public string Sql { get; init; } = null!;
        public Version Version { get; init; } = null!;
        public string ChecksumSha256 { get; init; } = null!;
    }
}
