using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using PassXYZLib;
using UITest.Models;
using UITest.Services;
using PassXYZ.Models;
using User = PassXYZLib.User;
using System.Text;

namespace UITest.ViewModels
{
    public class StorageTestMethod:TestMethod<ItemsViewModel> { }

    public partial class ItemsViewModel : BaseViewModel
    {
        private readonly ILogger<ItemsViewModel> logger;

        public ObservableCollection<StorageTestMethod> Items { get; }

        public ItemsViewModel(ILogger<ItemsViewModel> logger)
        {
            this.logger = logger;
            Title = "Storage Tests";
            Items = [];
            IsBusy = false;
            LogData = PxEnvironment.GetRoot();
        }

        [ObservableProperty]
        private StorageTestMethod? selectedItem = default;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string logData;

        public override void OnItemSelecteion(object sender)
        {
            StorageTestMethod? item = sender as StorageTestMethod;
            if (item == null)
            {
                logger.LogWarning("item is null.");
                return;
            }
            logger.LogDebug($"Selected item is {item.Name}");
            SelectedItem = item;
            item.Info?.Invoke(item.Value, null);
        }

        [RelayCommand]
        private void LoadItems()
        {
            logger.LogDebug($"IsBusy={IsBusy}");

            try
            {
                Items.Clear();

                TestRunner<ItemsViewModel> runner = new();
                foreach (var methodInfo in runner.GetTestMethods(this))
                {
                    StorageTestMethod item = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = methodInfo.Name,
                        Info = methodInfo,
                        Description = $"Testing {methodInfo.Name}",
                        Value = this
                    };
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex);
            }
            finally
            {
                IsBusy = false;
                logger.LogDebug("Set IsBusy to false");
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        private static string GetRootGroupName(User user)
        {
            PxDatabase db = new PxDatabase();
            if (db != null)
            {
                db.Open(user);
                if (db.RootGroup != null)
                    return $"{db.RootGroup.Name}";
            }
            return $"Cannot open database {user.Username}.";
        }

        private static string Users()
        {
            string userList = "";
            User.GetUsersList().ForEach(u => {
                User user = new User();
                user.Username = u;
                userList = userList + $"<b>{u}</b>: {user.Path}" + "\n";
            });
            return userList;
        }

        [TestCase]
        public void Get_User_List()
        {
            Debug.WriteLine("Get_User_List");
            LogData = Users();
        }

        [TestCase]
        public void Get_Root_Group_Name()
        {
            Debug.WriteLine("Get_Root_Group_Name");
            User user = new()
            {
                Username = "test1",
                Password = "12345"
            };
            LogData = $"<b>Root Group (no keylock)</b>: {GetRootGroupName(user)}";
        }

        [TestCase]
        public void Get_Root_Group_Name_MixedKeyData()
        {
            Debug.WriteLine("Get_Root_Group_Name_MixedKeyData");
            User user = new()
            {
                Username = "MixedKeyData",
                Password = "123123"
            };
            LogData = $"<b>Root Group (MixedKey)</b>: {GetRootGroupName(user)}";
        }

        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        [TestCase]
        public async void FilePicker_Read_Test()
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.com.passxyz" } }, // UTType values
                    { DevicePlatform.Android, new[] { "text/plain" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".txt", ".md" } }, // file extension
                    { DevicePlatform.macOS, new[] { "txt", "md" } }, // UTType values
                });

            Debug.WriteLine("FilePicker_Read_Test");
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Please pick a text file",
                FileTypes = customFileType
            });

            if (result == null)
                return;

            using var stream = await result.OpenReadAsync();
            string text = StreamToString(stream);
            LogData = $"<b>FilePicker_Read_Test</b>: {text}";
        }
    }
}