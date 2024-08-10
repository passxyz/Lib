using PassXYZLib;
using User = PassXYZLib.User;

namespace UITest
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        readonly MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            
            Message.Text = PxEnvironment.GetRoot();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            Message.Text = _viewModel.RunTest(count);
            CounterBtn.Text = $"Run Test {count}.";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
