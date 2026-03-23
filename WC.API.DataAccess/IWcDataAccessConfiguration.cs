using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.DataAccess
{
    public interface IWcDataAccessConfiguration
    {
        public string ConnectionString { get; }

        public int? Timeout { get; }
    }
}
