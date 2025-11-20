using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;


namespace PassXYZLib.xunit.PassXYZLib
{
#if PASSXYZ_CLOUD_SERVICE
    public class PxCloudConfigTests
    {
        [Fact]
        public void TestDefaultUsername()
        {
            Assert.Equal(PxCloudConfig.DEFAULT_USERNAME, PxCloudConfig.Username);
        }
        [Fact]
        public void TestSetUsername()
        {
            PxCloudConfig.Username = "unit test user";
            Assert.Equal(PxCloudConfig.DEFAULT_USERNAME, PxCloudConfig.Username);
        }
        [Fact]
        public void TestDefaultPassword()
        {
            Assert.Equal(PxCloudConfig.DEFAULT_PASSWORD, PxCloudConfig.Password);
        }

        [Fact]
        public void TestSetPassword()
        {
            PxCloudConfig.Password = "newPassword";
            Assert.Equal(PxCloudConfig.DEFAULT_PASSWORD, PxCloudConfig.Password);
        }

        [Fact]
        public void TestDefaultHostname()
        {
            Assert.Equal(PxCloudConfig.DEFAULT_HOSTNAME, PxCloudConfig.Hostname);
        }

        [Fact]
        public void TestSetHostname()
        {
            PxCloudConfig.Hostname = "newHostname";
            Assert.Equal("newHostname", PxCloudConfig.Hostname);
        }
        [Fact]
        public void TestDefaultPort()
        {
            Assert.Equal(PxCloudConfig.DEFAULT_PORT, PxCloudConfig.Port);
        }

        [Fact]
        public void TestSetPort()
        {
            PxCloudConfig.Port = 2222;
            Assert.Equal(PxCloudConfig.DEFAULT_PORT, PxCloudConfig.Port);
        }

        [Fact]
        public void TestDefaultRemoteHomePath()
        {
            Assert.Equal(PxCloudConfig.DEFAULT_REMOTEHOMEPATH, PxCloudConfig.RemoteHomePath);
        }

        [Fact]
        public void TestSetRemoteHomePath()
        {
            PxCloudConfig.RemoteHomePath = "/home/test";
            Assert.Equal(PxCloudConfig.DEFAULT_REMOTEHOMEPATH, PxCloudConfig.RemoteHomePath);
        }

        [Fact]
        public void TestSetIsEnabled()
        {
            Assert.False(PxCloudConfig.IsEnabled);
            PxCloudConfig.IsEnabled = true;
            Assert.True(PxCloudConfig.IsEnabled);
        }

        [Fact]
        public void TestIsConfigured() 
        {
            PxCloudConfig.Hostname = "localhost";
            Assert.True(PxCloudConfig.IsConfigured);
        }

        [Fact]
        public void TestGetCloudServices() 
        { 
            var cloudServices = PxCloudConfig.GetCloudServices();
            Assert.NotNull(cloudServices);
        }
    }
#endif // PASSXYZ_CLOUD_SERVICE
}
