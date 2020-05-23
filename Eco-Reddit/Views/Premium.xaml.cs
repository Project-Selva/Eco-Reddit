using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Premium : Page
    {
        public Premium()
        {
            this.InitializeComponent();
            wEB.Navigate(new Uri("https://www.reddit.com/premium"));
            wEB.Height = Window.Current.Bounds.Height - 15;
            wEB.Width = Window.Current.Bounds.Width - 570;
        }

        private async void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                wEB.Height = Window.Current.Bounds.Height -15;
                wEB.Width = Window.Current.Bounds.Width - 570;
            }
            catch
            {
                await Task.Delay(1000);
                wEB.Height = Window.Current.Bounds.Height - 15;
                wEB.Width = Window.Current.Bounds.Width - 570;
            }
        }
        private async void WEB_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.ToString() == "https://www.reddit.com/premium")
            {
         
            }
            else
            {
                args.Cancel = true;
                await Windows.System.Launcher.LaunchUriAsync(args.Uri);
            }
        }
    }
}
