using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Xunit;

using PassXYZLib;
using KeePassLib;
using PassXYZLib.Resources;
using System.Threading.Tasks;

namespace xunit.PassXYZLib
{
    public class UserFixture : IDisposable
    {
        public User user;

        public UserFixture()
        {
            user = new User
            {
                Username = "test1"
            };
            PxDb = new PxDatabase();
        }

        public void Dispose()
        {
            PxDb.Close();
        }
        public PxDatabase PxDb { get; private set; }
    }

    [CollectionDefinition("User collection")]
    public class UserCollection : ICollectionFixture<UserFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("User collection")]
    public class UserTests
    {
        static bool _isChanged = false;
        readonly UserFixture userFixture;
        /// <summary>
        /// Three built-in users are:
        /// <c>test1</c> - user without Device Lock.
        /// <c>kpclibpy</c> - user with Device Lock.
        /// <c>user1</c> user with a normal key file.
        /// </summary>
        /// <param name="fixture"></param>
        public UserTests(UserFixture fixture)
        {
            this.userFixture = fixture;
        }

        [Fact]
        public void MyDataPathTest()
        {
            userFixture.user.Username = "test1";
            Debug.Print($"DataFilePath={PxDataFile.DataFilePath}");
            Debug.Print($"FileName={userFixture.user.FileName}, LastAccessTime: {userFixture.user.LastAccessTime}");
            Assert.NotNull(PxDataFile.DataFilePath);
            //Assert.True(!userFixture.user.IsDeviceLockEnabled);
        }

        [Fact]
        public void KeyPathTest()
        {
            userFixture.user.Username = "test1";
            Debug.Print($"DataFilePath={PxDataFile.KeyFilePath}");
            Assert.NotNull(PxDataFile.KeyFilePath);
        }

        [Fact]
        public void GetUserNameTest()
        {
            var users = User.GetUsersList();
            foreach (var user in users)
            {
                Debug.WriteLine($"username={user}");
            }
        }

        /// <summary>
        /// Testing <c>IsKeyFileExist</c> and <c>IsUserExist</c>
        /// </summary>
        [Fact]
        public void FileNameTest()
        {
            userFixture.user.Username = "kpclibpy";
            PxDataFile.DataFilePath = System.IO.Directory.GetCurrentDirectory();
            Debug.Print($"FileName={userFixture.user.FileName}");
            Assert.True(userFixture.user.IsKeyFileExist);
            Assert.True(userFixture.user.IsUserExist);
            if (userFixture.user.IsKeyFileExist)
            {
                Debug.WriteLine($"FileNameTest: Found key file {userFixture.user.KeyFileName}");
            }
            Assert.NotNull(userFixture.user.FileName);
        }

        /// <summary>
        /// Testing a KeePass database with key file.
        /// </summary>
        [Fact]
        public void KeePassKeyFileTest()
        {
            PxDataFile.DataFilePath = System.IO.Directory.GetCurrentDirectory();
            userFixture.user.Username = "user1";
            userFixture.user.Password = "123123";
            Debug.WriteLine($"KeePassKeyFileTest: {userFixture.user.Path}");
            userFixture.PxDb.Open(userFixture.user);
            Assert.True(userFixture.PxDb.IsOpen);
        }

        [Fact]
        public void CustomIconTest()
        {
            PxDataFile.DataFilePath = System.IO.Directory.GetCurrentDirectory();
            userFixture.user.Username = "test1";
            userFixture.user.Password = "12345";
            Debug.WriteLine($"KeePassKeyFileTest: {userFixture.user.Path}");
            userFixture.PxDb.Open(userFixture.user);
            List<PwCustomIcon> customIconList = userFixture.PxDb.CustomIcons;
            var icons =
                from icon in customIconList
                where icon.Name.Contains("hp.com")
                select icon;
            Debug.WriteLine($"{icons.Count()}");
            var items = userFixture.PxDb.RootGroup.Items;
            foreach(var item in items) 
            {
                Debug.WriteLine($"Name={item.Name}, Icon={item.GetIconType()}");
            }
        }

        [Fact]
        public void SetFontIconTestAsync() 
        {
            PxDataFile.DataFilePath = System.IO.Directory.GetCurrentDirectory();
            userFixture.user.Username = "test1";
            userFixture.user.Password = "12345";
            Debug.WriteLine($"KeePassKeyFileTest: {userFixture.user.Path}");
            userFixture.PxDb.Open(userFixture.user);

            // Add a group
            var groupIcon = new PxFontIcon()
            {
                FontFamily = "FontAwesomeBrands",
                Glyph = FontAwesomeBrands.Google
            };
            var group = new PwGroup() { Name = "Google", Notes = "SetFontIconTest - Group" };
            // Act
            group.SetFontIcon(groupIcon);
            userFixture.PxDb.RootGroup.AddGroup(group, true);

            // Add a entry
            var entryIcon = new PxFontIcon()
            {
                FontFamily = "FontAwesomeBrands",
                Glyph = FontAwesomeBrands.Yahoo
            };
            var entry = new PwEntry() { Name = "Yahoo", Notes = "SetFontIconTest - Yahoo" };
            entry.SetFontIcon(entryIcon);
            userFixture.PxDb.RootGroup.AddEntry(entry, true);

            // Act
            userFixture.PxDb.RootGroup.Name = group.Name + DateTime.UtcNow;

            KPCLibLogger logger = new();
            userFixture.PxDb.DescriptionChanged = DateTime.UtcNow;
            userFixture.PxDb.Save(logger);
        }

#if PASSXYZ_CLOUD_SERVICE
        static void WatchFileForChanges(string path, string filename)
        {
            using (var watcher = new FileSystemWatcher())
            {
                watcher.Path = path;

                //Watch for changes in LastAccess and LastWrite times, and the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                // Only watch specific file.
                watcher.Filter = filename;

                // Add event handlers.
                watcher.Changed += OnChanged;

                // Start watching.
                watcher.EnableRaisingEvents = true;
            }
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed.
            Debug.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            _isChanged = true;
        }
        [Fact]
        public void FileChangedTest() 
        {
            _isChanged = false;
            PxUser user = new() { Username="test1", Password = "12345" };
            var path = Path.GetDirectoryName(user.Path);
            if (path != null) 
            {
                WatchFileForChanges(path, user.FileName);
                userFixture.PxDb.Open(user);
                Assert.True(userFixture.PxDb.IsOpen);
            }
            Assert.Equal(user.CurrentFileStatus.IsModified, _isChanged);
        }
#endif // PASSXYZ_CLOUD_SERVICE
    }
}
