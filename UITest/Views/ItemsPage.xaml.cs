using PassXYZLib;
using Renci.SshNet.Messages;
using System.Diagnostics;
using UITest.Models;
using UITest.ViewModels;
using UITest.Views;

namespace UITest.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public ItemsPage(ItemsViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
            logData.Text = PxEnvironment.GetRoot();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppearing();
        }
    }
}