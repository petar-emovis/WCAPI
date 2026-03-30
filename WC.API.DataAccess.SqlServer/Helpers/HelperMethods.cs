using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WC.DataAccess.SqlServer.Helpers
{
    public static class HelperMethods
    {
        private const ulong SignBit = 0x8000000000000000UL;

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



        public static (long High, long Low) ToSqlOrderableParts(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out var ip))
                throw new ArgumentException("Invalid IP address.", nameof(ipAddress));

            return ToSqlOrderableParts(ip);
        }

        public static (long High, long Low) ToSqlOrderableParts(IPAddress ip)
        {
            if (ip == null)
                throw new ArgumentNullException(nameof(ip));

            if (ip.AddressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException("Only IPv6 addresses are supported.", nameof(ip));

            byte[] bytes = ip.GetAddressBytes();

            if (bytes.Length != 16)
                throw new InvalidOperationException("IPv6 address must be exactly 16 bytes.");

            ulong high =
                ((ulong)bytes[0] << 56) |
                ((ulong)bytes[1] << 48) |
                ((ulong)bytes[2] << 40) |
                ((ulong)bytes[3] << 32) |
                ((ulong)bytes[4] << 24) |
                ((ulong)bytes[5] << 16) |
                ((ulong)bytes[6] << 8) |
                bytes[7];

            ulong low =
                ((ulong)bytes[8] << 56) |
                ((ulong)bytes[9] << 48) |
                ((ulong)bytes[10] << 40) |
                ((ulong)bytes[11] << 32) |
                ((ulong)bytes[12] << 24) |
                ((ulong)bytes[13] << 16) |
                ((ulong)bytes[14] << 8) |
                bytes[15];

            return
            (
                ToOrderableSignedLong(high),
                ToOrderableSignedLong(low)
            );
        }

        public static IPAddress FromSqlOrderableParts(long high, long low)
        {
            ulong restoredHigh = FromOrderableSignedLong(high);
            ulong restoredLow = FromOrderableSignedLong(low);

            byte[] bytes = new byte[16];

            bytes[0] = (byte)(restoredHigh >> 56);
            bytes[1] = (byte)(restoredHigh >> 48);
            bytes[2] = (byte)(restoredHigh >> 40);
            bytes[3] = (byte)(restoredHigh >> 32);
            bytes[4] = (byte)(restoredHigh >> 24);
            bytes[5] = (byte)(restoredHigh >> 16);
            bytes[6] = (byte)(restoredHigh >> 8);
            bytes[7] = (byte)restoredHigh;

            bytes[8] = (byte)(restoredLow >> 56);
            bytes[9] = (byte)(restoredLow >> 48);
            bytes[10] = (byte)(restoredLow >> 40);
            bytes[11] = (byte)(restoredLow >> 32);
            bytes[12] = (byte)(restoredLow >> 24);
            bytes[13] = (byte)(restoredLow >> 16);
            bytes[14] = (byte)(restoredLow >> 8);
            bytes[15] = (byte)restoredLow;

            return new IPAddress(bytes);
        }

        private static long ToOrderableSignedLong(ulong value)
        {
            return unchecked((long)(value ^ SignBit));
        }

        private static ulong FromOrderableSignedLong(long value)
        {
            return ((ulong)value) ^ SignBit;
        }

        public static bool IsInRange(
            long ipHigh, long ipLow,
            long startHigh, long startLow,
            long endHigh, long endLow)
        {
            return Compare(startHigh, startLow, ipHigh, ipLow) <= 0
                && Compare(ipHigh, ipLow, endHigh, endLow) <= 0;
        }

        public static int Compare(long leftHigh, long leftLow, long rightHigh, long rightLow)
        {
            if (leftHigh < rightHigh) return -1;
            if (leftHigh > rightHigh) return 1;

            if (leftLow < rightLow) return -1;
            if (leftLow > rightLow) return 1;

            return 0;
        }
    }
        
}
