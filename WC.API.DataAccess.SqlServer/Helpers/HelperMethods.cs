using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WC.DataAccess.SqlServer.Helpers
{
    public static class HelperMethods
    {
        public static long IpToLong(IPAddress ip)
        {
            byte[] bytes = ip.GetAddressBytes();

            if (bytes.Length == 4)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                return BitConverter.ToUInt32(bytes, 0);
            }

            return 0;
        }
        public static int CompareIpv6(byte[] left, byte[] right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            if (left.Length != 16 || right.Length != 16)
                throw new ArgumentException("Both IPv6 byte arrays must be exactly 16 bytes long.");

            for (int i = 0; i < 16; i++)
            {
                if (left[i] < right[i]) return -1;
                if (left[i] > right[i]) return 1;
            }

            return 0;
        }

        public static bool IsInRangeIpv6(byte[] candidate, byte[] start, byte[] end)
        {
            return CompareIpv6(start, candidate) <= 0
                && CompareIpv6(candidate, end) <= 0;
        }
    }
        
}
