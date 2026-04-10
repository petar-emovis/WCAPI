using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.Database.Tools.Models
{
    internal sealed class AppliedScriptInfo
    {
        public string ScriptName { get; init; } = null!;
        public Version Version { get; init; } = null!;
        public string ChecksumSha256 { get; init; } = null!;
    }
}
