using Xunit;
using PassXYZ.Utils;
using System.Diagnostics;
using PassXYZ.Services;

namespace PassXYZLib.xunit.PassXYZ
{
    public class NewDatabaseTests
    {
        const string TEST_USER = "NewKeyDataTest";
        const string TEST_DB_KEY = "123123";
        readonly User _testUser;
        readonly PxDatabase _db;

        public NewDatabaseTests() 
        {
            _testUser = new User()
            {
                Username = TEST_USER,
                Password = TEST_DB_KEY,
                IsDeviceLockEnabled = true
            };
            _db = new();
        }

        void Connect() 
        {
            var deviceInfo = new MockDeviceInfo();
            NewKeyData newKeyData = new() 
            { 
                Name = deviceInfo.Name,
                Idiom = deviceInfo.Idiom.ToString(),
                Manufacturer = deviceInfo.Manufacturer,
                Model = deviceInfo.Model,
                Platform = deviceInfo.Platform.ToString(),
                Version = deviceInfo.Version.ToString(),
                VersionString = deviceInfo.VersionString,
                DeviceType = deviceInfo.DeviceType.ToString(),
            };
            PxKeyProvider pxKeyProvider = new(newKeyData);
            if (_testUser.IsKeyFileExist)
            {
                _db.Open(_testUser);
            }
            else
            {
                _db.New(_testUser, pxKeyProvider);
                ShowWarningsLogger swLogger = new();
                _db.Save(swLogger);
            }
        }

        [Fact]
        public void IsOpenDbTest()
        {
            Connect();
            Debug.WriteLine($"Username is {_testUser.Username}.");
            Assert.True((_db.IsOpen));
        }

        [Fact]
        public void DeleteUserTest()
        {
            _testUser.Delete();
            Assert.False(_testUser.IsKeyFileExist);
        }
    }
}
