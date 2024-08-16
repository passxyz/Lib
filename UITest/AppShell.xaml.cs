using System.Diagnostics;
using UITest.Views;

namespace UITest
{
    public partial class AppShell : Shell
    {
        public static AppShell? CurrentAppShell { get; private set; } = default!;

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
            CurrentAppShell = this;
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            if (args.Current != null)
            {
                Debug.WriteLine($"AppShell: source={args.Current.Location}, target={args.Target.Location}");
            }
        }
    }
}
