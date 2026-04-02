using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using WC.DataAccess;

namespace WC.DataAccess.SqlServer.Configuration
{
    public class WcDataAccessConfiguration : IWcDataAccessConfiguration
    {
        public string? ConnectionString { get; set; }

        public int? Timeout { get; set; }

        public WcDataAccessConfiguration(IConfiguration configuration)
        {
            ConnectionString = configuration["WC.DataAccess:WC_Test:ConnectionString"];
            Timeout = Convert.ToInt32(configuration["WC.DataAccess:WC_Test:TimeoutInSeconds"]);

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(this, new ValidationContext(this), validationResults, true))
            {
                throw new ValidationException(string.Join(", ", validationResults.Select(r => r.ErrorMessage)));
            }
        }
    }
}
