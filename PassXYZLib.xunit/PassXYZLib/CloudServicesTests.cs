using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace PassXYZLib.xunit.PassXYZLib
{
    public class CloudServicesTests
    {
        ICloudServices<PxUser> _cloudServices;
        public CloudServicesTests() 
        {
            _cloudServices = PxCloudConfig.GetCloudServices();
        }

        [Fact]
        public async void TestLoginAsync() 
        { 
            await _cloudServices.LoginAsync();
        }
    }
}
