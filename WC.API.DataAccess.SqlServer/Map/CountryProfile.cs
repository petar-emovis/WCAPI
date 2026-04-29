using AutoMapper;
using DTO = WC.Models.DTO;
using Entities = WC.DataAccess.SqlServer.Models;

namespace WC.DataAccess.SqlServer.Map
{
    public class CountryProfile : Profile
    {
        public CountryProfile()
        {
            CreateMap<Entities.Country, DTO.Country>();

            //CreateMap<DTO.CountryRequest, Entities.Country>();

            //CreateMap<DTO.UpdateCountryRequest, Entities.Country>()
            //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
