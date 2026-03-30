using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WC.Service.Helper
{
    public static class IpParser
    {
        public static IPAddress Parse(string input)
        {
            if (!IPAddress.TryParse(input, out var ip))
                throw new ArgumentException("Invalid IP address.");

            if (ip == null)
                throw new ArgumentException("Error in parsing IP address.");

            return ip;
        }
    }
}
