using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChatWebViewPage : Page
    {
        public ChatWebViewPage()
        {
            InitializeComponent();
            wEB.Navigate(new Uri("https://www.reddit.com/chat/"));
            wEB.Height = Window.Current.Bounds.Height - 100;
            wEB.Width = Window.Current.Bounds.Width - 570;
        }

        private async void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                wEB.Height = Window.Current.Bounds.Height - 100;
                wEB.Width = Window.Current.Bounds.Width - 570;
            }
            catch
            {
                await Task.Delay(1000);
                wEB.Height = Window.Current.Bounds.Height - 100;
                wEB.Width = Window.Current.Bounds.Width - 570;
            }
        }

        private async void WEB_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.ToString().Contains("https://www.reddit.com/chat") == false)
            {
                args.Cancel = true;
                await Launcher.LaunchUriAsync(args.Uri);
            }
        }
    }
}
