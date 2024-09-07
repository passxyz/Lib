using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Xunit;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using KeePassLib.Utility;
using PassXYZLib;
using PassXYZ.Services;

namespace xunit.PassXYZ
{
    public class DeviceLockFixture : IDisposable
    {
        const string TEST_DB = "pass_e_2TjEf1Dy9V2jiEgbS.xyz";
        const string TEST_DB_KEY = "123123";

        public DeviceLockFixture()
        {
            PxDb = new PxDatabase();
            PxDatabase.DefaultFolder = "../../..";
            PxDb.Open(TEST_DB, TEST_DB_KEY);
        }

        public void Dispose()
        {
            PxDb.Close();
        }

        public PxDatabase PxDb { get; private set; }

        public string Username
        {
            get { return PxDefs.GetUserNameFromDataFile(TEST_DB); }
        }
    }

    [CollectionDefinition("DeviceLock collection")]
    public class DeviceLockCollection : ICollectionFixture<DeviceLockFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("DeviceLock collection")]
    public class DeviceLockTests
    {
        const string TEST_DB_KEY_DATA = "AAEAAAD/////AQAAAAAAAAAMAgAAAD5QYXNzWFlaLCBWZXJzaW9uPTIuNC40LjEsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49bnVsbAUBAAAAGlBhc3NYWVouU2VydmljZXMuUHhLZXlEYXRhCAAAAAhVc2VybmFtZQJJZAtEZXZpY2VNb2RlbBJEZXZpY2VNYW51ZmFjdHVyZXIKRGV2aWNlTmFtZQ1EZXZpY2VWZXJzaW9uDkRldmljZVBsYXRmb3JtC0RldmljZUlkaW9tAQEBAQEBAQECAAAABgMAAAAIa3BjbGlicHkGBAAAACAxQUQyNERDRTVBRTJERDRGQTQ2RDA3MjcwRDNCOTYzMgYFAAAADkxhdGl0dWRlIEU3NDQwBgYAAAAJRGVsbCBJbmMuBgcAAAAPREVTS1RPUC0yTUVLOFI3BggAAAAPMTAuMC4xODM2My4xNDQwBgkAAAADVVdQBgoAAAAHRGVza3RvcAs=";
        DeviceLockFixture passxyz;

        public DeviceLockTests(DeviceLockFixture deviceLockFixture)
        {
            this.passxyz = deviceLockFixture;
        }

        [Fact (Skip = "This test can only work in .NET 7 or below")]
        public void GetKeyDataTest() 
        {
            var data = PxKeyData.FromBase64String(TEST_DB_KEY_DATA);
            Debug.WriteLine($"Username = {data.Username}");
        }

        [Fact]
        public void IsOpenDbTest()
        {
            Debug.WriteLine($"{passxyz.PxDb}, Username is {passxyz.Username}.");
            Assert.True((passxyz.PxDb.IsOpen));
        }

        [Fact]
        public void CurrentGroupTests()
        {
            var currentGroup = passxyz.PxDb.CurrentGroup;
            Debug.WriteLine($"{currentGroup}");
            Assert.NotNull(currentGroup);
        }

        [Fact]
        public void CurrentPathTests()
        {
            var currentPath = passxyz.PxDb.CurrentPath;
            Debug.WriteLine($"Current path is {currentPath}.");
            Assert.NotNull(currentPath);
        }

        [Theory(Skip = "Need to fix this test case")]
        [InlineData("/MixedKeyData/Database/TestDataWithPIN")]
        /// <summary>
        /// Change a protect field
        /// </summary>
        /// <param name="path">Destination path. Must not be <c>null</c>.</param>
        public void ChangeProtectedFieldTests(string path)
        {
            var entry = passxyz.PxDb.FindByPath<PwEntry>(path);

            Debug.WriteLine($"Current path is {entry}.");
            // Update the existing protected field
            PxDefs.UpdatePxEntry(entry, PxDefs.PasswordField, "123456", true);
            Assert.True(entry.Strings.Get(PxDefs.FindEncodeKey(entry.Strings, PxDefs.PasswordField)).IsProtected);
            // Add a new protected field "PIN"
            PxDefs.UpdatePxEntry(entry, "PIN", "1234", true);
            Assert.True(entry.Strings.Get(PxDefs.FindEncodeKey(entry.Strings, "PIN")).IsProtected);
            // Remove a field
            PxDefs.UpdatePxEntry(entry, "002Email", String.Empty, false);
            foreach (KeyValuePair<string, ProtectedString> kvp in entry.Strings)
            {
                Debug.WriteLine($"    {kvp.Key}={kvp.Value.ReadString()}, IsProtected={kvp.Value.IsProtected}");
            }

        }

        // The end of DeviceLockTests
    }

}