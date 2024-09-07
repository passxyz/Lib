using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZLib
{
    /// <summary>
    /// A class for the synchronization with remote Git repository
    /// </summary>
    public class PxRemoteUser : PxUser
    {
        /// <summary>
        /// Create an instance from filename
        /// </summary>
        /// <param name="fileName">File name used to decode username</param>
        public PxRemoteUser(string fileName) : base (fileName)
        {
        }
    }
}
