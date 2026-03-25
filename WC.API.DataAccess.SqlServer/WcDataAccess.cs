using Microsoft.EntityFrameworkCore;
using System.Net;
using WC.DataAccess.SqlServer.Configuration;
using WC.DataAccess.SqlServer.Helpers;
using WC.DataAccess.SqlServer.Models;
using WC.Models;
using WC.Models.DTO;
using DTO = WC.Models.DTO;
using Entities = WC.DataAccess.SqlServer.Models;

namespace WC.DataAccess.SqlServer
{
    public class WcDataAccess : IWcDataAccess
    {
        private readonly WhichCountryContext _dbContext;
        private readonly IWcDataAccessConfiguration _configuration;
        private readonly DbContextOptionsBuilder<WhichCountryContext> _optionsBuilder;

        public  WcDataAccess(IWcDataAccessConfiguration configuration) 
        {
            _optionsBuilder = new();

            _optionsBuilder.UseSqlServer
                (
                    configuration.ConnectionString,
                    sqlServerOptions => sqlServerOptions.UseCompatibilityLevel(configuration.Timeout ?? 120)
                )
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging();

            _configuration = configuration;
            _dbContext = new WhichCountryContext(_optionsBuilder.Options);            
        }

        public async Task<string> RefreshIpIntegersBinariesAndVersions()
        {
            var ipRanges = await _dbContext.IpRanges.ToListAsync();

            Parallel.ForEach(ipRanges.Where(I => I.IpVersion == null), range =>
            {
                if (IPAddress.TryParse(range.StartIp, out var startIp))
                {
                    if (range.IpVersion == null)
                        range.IpVersion = startIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? (int)IpVersionEnum.IPv4 : (int)IpVersionEnum.IPv6;

                    if (range.IpVersion == (int)IpVersionEnum.IPv4)
                        range.StartIpNumeric = HelperMethods.IpToLong(startIp);
                    else
                        range.StartIpBinary = startIp.GetAddressBytes();
                }

                if (IPAddress.TryParse(range.EndIp, out var endIp))
                {
                    if (range.IpVersion == null)
                        range.IpVersion = endIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? (int)IpVersionEnum.IPv4 : (int)IpVersionEnum.IPv6;

                    if (range.IpVersion == (int)IpVersionEnum.IPv4)
                        range.EndIpNumeric = HelperMethods.IpToLong(endIp);
                    else
                        range.EndIpBinary = endIp.GetAddressBytes();
                }
            });

            int a = await _dbContext.SaveChangesAsync();

            return "Initial refresh done! " + a.ToString();
        }

        public async Task<DTO.CountryResponse> GetCountryFromIpAdress(string ipAddress)
        {
            CountryResponse response = new DTO.CountryResponse();
            IPAddress ip = IPAddress.Parse(ipAddress);

            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                //IPv4

                long ipLong = HelperMethods.IpToLong(ip);

               // var res1 = await _dbContext.IpRanges
               //.AsNoTracking()
               //.Where(i => i.IpVersion == (int)IpVersionEnum.IPv4).ToListAsync();

               // var res2 = await _dbContext.IpRanges
               //.AsNoTracking()
               //.Where(i => i.IpVersion == (int)IpVersionEnum.IPv4
               //&& i.StartIpNumeric <= ipLong
               //&& i.EndIpNumeric >= ipLong).ToListAsync();

                var res = await _dbContext.IpRanges
                .AsNoTracking()
                .Include(x => x.Country)
                .Where(i => i.IpVersion == (int)IpVersionEnum.IPv4
                && i.StartIpNumeric <= ipLong
                && i.EndIpNumeric >= ipLong).FirstOrDefaultAsync();

                if (res != null)
                    response = new CountryResponse { CountryName = res.Country.Name };

            }
            else //if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                //IPv6

                byte[] ipInBytes = ip.GetAddressBytes();

                //var res = await _dbContext.IpRanges
                //.AsNoTracking()
                //.Include(x => x.Country)
                //.Where(i => i.IpVersion == (int)IpVersionEnum.IPv6
                //&& i.StartIpBinary != null
                //&& i.EndIpBinary != null
                //&& HelperMethods.IsInRangeIpv6(ipInBytes, i.StartIpBinary,i.EndIpBinary)).FirstOrDefaultAsync();

                var ipv6List = await _dbContext.IpRanges
                        .AsNoTracking()
                        .Include(x => x.Country)
                        .Where(i => i.IpVersion == (int)IpVersionEnum.IPv6).ToListAsync();

                var res = ipv6List.Where(ip => HelperMethods.IsInRangeIpv6(ipInBytes, ip.StartIpBinary, ip.EndIpBinary)).FirstOrDefault();


                if (res != null)
                    response = new CountryResponse { CountryName = res.Country.Name };
            }

            return response;
        }
    }
}
