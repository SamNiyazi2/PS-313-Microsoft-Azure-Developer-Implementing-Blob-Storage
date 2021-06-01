using Autofac;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using WiredBrainCoffee.AdminApp.Startup;
using WiredBrainCoffee.AdminApp.ViewModel;

namespace WiredBrainCoffee.AdminApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;

            ViewModel = App.Current.Container.Resolve<MainViewModel>();

            ApplicationView.PreferredLaunchViewSize = new Size(800, 620);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        // 05/19/2021 07:26 am - SSN - [20210519-0709] - [001] - M04-02 - List the blobs of a container
        private async void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.LoadCoffeeVideosAsync();
        }

        public MainViewModel ViewModel { get; }
    }
}
