using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WC.DataAccess.SqlServer.Models;

namespace WC.DataAccess.SqlServer.Map
{
    public static class WC_Map
    {
        public static IMapper Mapper
        {
            get;
        }

        static WC_Map()
        {
            var WCMapper = new MapperConfiguration
            (
                cfg =>
                {
                    cfg.AddMaps(typeof(WhichCountryContext).Assembly);
                    cfg.AddMaps(Assembly.GetExecutingAssembly());
                },
                NullLoggerFactory.Instance
            );
            Mapper = WCMapper.CreateMapper();

        }
    }
}
