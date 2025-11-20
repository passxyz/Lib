using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace PassXYZLib.xunit.PassXYZLib
{
#if PASSXYZ_CLOUD_SERVICE
    public class CloudServicesTests
    {
        ICloudServices<PxUser> _cloudServices;
        public CloudServicesTests() 
        {
            _cloudServices = PxCloudConfig.GetCloudServices();
            PxCloudConfig.Hostname = "localhost";
            PxCloudConfig.IsEnabled = true;
        }

        [Fact]
        public async void TestLoginAsync() 
        {
            Debug.WriteLine($"Username={PxCloudConfig.Username}");
            Debug.WriteLine($"Password={PxCloudConfig.Password}");
            Debug.WriteLine($"Hostname={PxCloudConfig.Hostname}");
            Debug.WriteLine($"Port={PxCloudConfig.Port}");
            Debug.WriteLine($"RemoteHomePath={PxCloudConfig.RemoteHomePath}");
            Debug.WriteLine($"IsConfigured={PxCloudConfig.IsConfigured}");
            Debug.WriteLine($"IsEnabled={PxCloudConfig.IsEnabled}");
            Debug.WriteLine($"IsSshOperationTimeout={_cloudServices.IsSshOperationTimeout}");
            await _cloudServices.LoginAsync();
            if( _cloudServices.IsConnected) 
            {
                Debug.WriteLine("Connected");
            }
            else 
            {
                Debug.WriteLine("Cannot connect, please check environment setup.");
            }
        }

        void ListUsers(IEnumerable<User> users)
        {
            foreach(User user in users) 
            {
                Debug.WriteLine($"{user.Username}\t\t{user.FileName}");
            }
            Debug.WriteLine("================================");
        }

        [Fact]
        public async void TestLoadRemoteUsersAsync()
        {
            await _cloudServices.LoginAsync();
            if (_cloudServices.IsConnected)
            {
                var users = await _cloudServices.LoadRemoteUsersAsync();
                Debug.WriteLine("========Remote Users============");
                if ( users != null ) { ListUsers(users); }
                var localUsers = await PxUser.LoadLocalUsersAsync(false);
                Debug.WriteLine("=========Local Users============");
                if ( localUsers != null ) { ListUsers(localUsers); }
            }
            else
            {
                Debug.WriteLine("Cannot connect, please check environment setup.");
            }
        }

        [Fact]
        public async void TestDownloadFileAsync() 
        {
            await _cloudServices.LoginAsync();
            if (_cloudServices.IsConnected)
            {
                await _cloudServices.DownloadFileAsync("pass_e_EFZGmRz.xyz");
            }
            else
            {
                Debug.WriteLine("Cannot connect, please check environment setup.");
            }
        }

        [Fact]
        public async void TestUploadFileAsync()
        {
            await _cloudServices.LoginAsync();
            if (_cloudServices.IsConnected)
            {
                await _cloudServices.UploadFileAsync("pass_e_EFZGmRz.xyz");
            }
            else
            {
                Debug.WriteLine("Cannot connect, please check environment setup.");
            }
        }

        [Fact]
        public async void TestSynchronizeUsersAsync() 
        {
            await _cloudServices.LoginAsync();
            if (_cloudServices.IsConnected)
            {
                await _cloudServices.SynchronizeUsersAsync();
            }
            else
            {
                Debug.WriteLine("Cannot connect, please check environment setup.");
            }
        }

        [Fact (Skip = "Need to setup a test server")]
        public void TestFileSynchronizationService() 
        {
            var service = new FileSynchronizationService(PxCloudConfig.Hostname, PxCloudConfig.Username, PxCloudConfig.Password, PxCloudConfig.Port);
            service.SynchronizeFiles(PxDataFile.DataFilePath, PxCloudConfig.RemoteHomePath);
        }
    }
#endif // PASSXYZ_CLOUD_SERVICE
}
