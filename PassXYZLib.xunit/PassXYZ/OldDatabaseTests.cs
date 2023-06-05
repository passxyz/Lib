using PassXYZ.Services;
using PassXYZ.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PassXYZLib.xunit.PassXYZ
{
    public class OldDatabaseTests
    {
        const string TEST_USER = "OldKeyDataTest";
        const string TEST_DB_KEY = "123123";
        readonly User _testUser;
        readonly PxDatabase _db;

        public OldDatabaseTests()
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
            OldKeyData keyData = new()
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
            keyData.SetData();
            PxKeyProvider pxKeyProvider = new(keyData);
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
        public void GetDeviceLockDataTest()
        {
            Connect();
            var msg = PxDatabase.GetDeviceLockData(_testUser);
            Debug.WriteLine(msg);
        }

        [Fact]
        public void DeleteUserTest()
        {
            _testUser.Delete();
            Assert.False(_testUser.IsKeyFileExist);
        }
    }
}
