using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UITest.Models;
using UITest.Services;

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
        }

        [ObservableProperty]
        private StorageTestMethod? selectedItem = default;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool isBusy;

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

        [TestCase(Skip = "Skip this test method 1")]
        public void Test_Method1()
        {
            Debug.WriteLine("Test_Method1");
        }

        [TestCase]
        public void Test_Method2()
        {
            Debug.WriteLine("Test_Method2");
        }

        [TestCase]
        public void Test_Method3()
        {
            Debug.WriteLine("Test_Method3");
        }
    }
}