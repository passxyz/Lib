using UITest.Services;
using UITest.Views;

namespace UITest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
