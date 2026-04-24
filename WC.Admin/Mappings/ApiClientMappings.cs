// WC.Admin/Mappings/ApiClientMappings.cs
using WC.Admin.ApiClient;
using WC.Admin.Models;
using WC.Models.Admin;
using CountryViewModel = WC.Models.Admin.Country.CountryViewModel;

namespace WC.Admin.Mappings;

public static class ApiClientMappings
{
    public static WC.Models.Admin.IpRange.IpRangePagedResultModel ToLocalModel(
        this ApiClient.IpRangePagedResultModel src)
        => new()
        {
            TotalCount = src.TotalCount,
            Page = src.Page,
            PageSize = src.PageSize,
            Items = src.Items
                .Select(x => x.ToLocalModel())
                .ToList()
        };

    public static WC.Models.Admin.IpRangeViewModel ToLocalModel(
        this ApiClient.IpRangeViewModel src)
        => new()
        {
            Id = src.Id,
            CountryId = src.CountryId,
            CountryName = src.CountryName,
            IpVersion = src.IpVersion,
            StartIp = src.StartIp,
            EndIp = src.EndIp,
            Active = src.Active
        };

    public static ApiClient.IpRangeViewModel ToApiModel(
        this IpRangeEditModel src)
        => new()
        {
            Id = src.Id,
            CountryId = src.CountryId,
            IpVersion = src.IpVersion,
            StartIp = src.StartIp,
            EndIp = src.EndIp,
            Active = src.Active
        };

    public static IpRangeEditModel ToEditModel(
        this ApiClient.IpRangeViewModel src)
        => new()
        {
            Id = src.Id,
            CountryId = src.CountryId,
            CountryName = src.CountryName,
            IpVersion = src.IpVersion,
            StartIp = src.StartIp,
            EndIp = src.EndIp,
            Active = src.Active
        };

    public static CountryViewModel ToViewModel(
    this ApiClient.CountryViewModel src)
    => new()
    {
        Id = src.Id,
        Name = src.Name,
        Iso2Code = src.Iso2Code,
        Iso3Code = src.Iso3Code,
        IpRangeCount = src.IpRangeCount,
        ActiveIpRangeCount = src.ActiveIpRangeCount
    };
}