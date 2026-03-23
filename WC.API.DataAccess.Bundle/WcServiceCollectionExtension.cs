using Microsoft.Extensions.DependencyInjection;
using WC.DataAccess.SqlServer;
using WC.DataAccess.SqlServer.Configuration;

namespace WC.DataAccess.Bundle
{
    public static class WcServiceCollectionExtension
    {
        public static void AddSeDataAccess(this IServiceCollection services)
        {
            services.AddSingleton<IWcDataAccessConfiguration, WcDataAccessConfiguration>();

            services.AddSingleton<IWcDataAccess, WcDataAccess>();
        }
    }
}
