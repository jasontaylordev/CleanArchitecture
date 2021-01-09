using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Caching
{
    /// <summary>
    /// This class includes cache options for MemCache
    /// </summary>
    internal class MemCacheOption
    {
        /// <summary>
        /// Server Name For Example : localhost
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Port number of Server. For Example = 11211
        /// </summary>
        public int Port { get; set; }
    }
}
