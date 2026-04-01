using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.Models
{
    public enum IpVersionEnum
    {
        [Description("IPv4")]
        IPv4 = 1,
        [Description("IPv6")]
        IPv6 = 2

    }
}
