using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Caching
{
    /// <summary>
    /// This class includes cache options for Redis Cache.
    /// </summary>
    internal class RedisCacheOption
    {
        /// <summary>
        /// For Example : localhost
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Instance name For Example : CleanArchitecture
        /// </summary>
        public string InstanceName { get; set; }
    }
}
