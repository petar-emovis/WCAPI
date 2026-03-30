using AutoMapper;
using WC.Models;
using DTO = WC.Models.DTO;
using Entities = WC.DataAccess.SqlServer.Models;

namespace WC.DataAccess.SqlServer.Map
{
    public class IpRangeProfile : Profile
    {
        public IpRangeProfile()
        {
            CreateMap<Entities.IpRange, DTO.IpRange>()
                .ForMember
                (
                    dest => dest.IpVersion,
                    map => map.MapFrom(src => (IpVersionEnum)src.IpVersion)
                )
                .ForAllMembers(o => o.Condition((src, dest, member) => src != null));

            CreateMap<DTO.IpRangeRequest, Entities.IpRange>();

            //CreateMap<DTO.UpdateIpRangeRequest, Entities.IpRange>()
            //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
