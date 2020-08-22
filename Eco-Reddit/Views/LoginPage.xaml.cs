using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Eco_Reddit.Helpers;
using Eco_Reddit.Core.Models;
using Eco_Reddit.Models;

namespace Eco_Reddit.Views
{
    public sealed partial class LoginPage : Page, INotifyPropertyChanged
    {
        // TODO WTS: Set the URI of the page to show by default
        private const string DefaultUrl = "https://developer.microsoft.com/en-us/windows/apps";

        private Visibility _failedMesageVisibility;

        private bool _isLoading;

        private Visibility _isLoadingVisibility;

        private bool _isShowingFailedMessage;
        public string accessToken;
        public string appId = "mp8hDB_HfbctBg";
        public string BackuprefreshToken = "589558656590-c5zKzVmhsgqmJGqobEebBOAQVl8";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string refreshToken;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";

        public LoginPage()
        {
            InitializeComponent();
            IsLoading = true;
            StartUp();
        }

        public bool IsLoading
        {
            get => _isLoading;

            set
            {
                if (value) IsShowingFailedMessage = false;

                Set(ref _isLoading, value);
                IsLoadingVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility IsLoadingVisibility
        {
            get => _isLoadingVisibility;
            set => Set(ref _isLoadingVisibility, value);
        }

        public bool IsShowingFailedMessage
        {
            get => _isShowingFailedMessage;

            set
            {
                if (value) IsLoading = false;

                Set(ref _isShowingFailedMessage, value);
                FailedMesageVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility FailedMesageVisibility
        {
            get => _failedMesageVisibility;
            set => Set(ref _failedMesageVisibility, value);
        }

        public bool IsBackEnabled => webView.CanGoBack;

        public bool IsForwardEnabled => webView.CanGoForward;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            IsLoading = false;
            OnPropertyChanged(nameof(IsBackEnabled));
            OnPropertyChanged(nameof(IsForwardEnabled));
        }

        private void OnNavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            // Use `e.WebErrorStatus` to vary the displayed message based on the error reason
            IsShowingFailedMessage = true;
        }

        private void OnRetry(object sender, RoutedEventArgs e)
        {
            IsShowingFailedMessage = false;
            IsLoading = true;

            webView.Refresh();
        }

        private async void LoginView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            var loginHelper = new Eco_Reddit.Core.Helpers.LoginHelper(appId, secret);
            if (args.Uri.AbsoluteUri.Contains("http://127.0.0.1:3000/reddit_callback"))
            {
                var result = await loginHelper.Login_Stage2(args.Uri);

                accessToken = result.AccessToken;
                refreshToken = result.RefreshToken;
                // NewPivotItem.Header = result.RefreshToken;
                if (string.IsNullOrEmpty(refreshToken) == false) // if user accepts
                {
                   // Fillbox.Text = result.RefreshToken + " hhhhhh " + result.AccessToken;
                    localSettings.Values["refresh_token"] = result.RefreshToken;
                    localSettings.Values["access_token"] = result.AccessToken;
                    TokenSharpData.Reddit = new Eco_Reddit.Core.Reddit(result.AccessToken);
                    await TokenSharpData.Reddit.InitOrUpdateUserAsync();
                    FinishedFrame.Visibility = Visibility.Visible;
                  FinishedFrame.Navigate(typeof(HomePage));

                    ///     HomePage.LoginFrameFrame.Navigate(typeof(HomePage));
                    ///     
                    /* MainPage.UniversalTabView.TabItems.Remove(MainPage.UniversalTabView.SelectedItem); // delete login tab
                     var newTab = new TabViewItem();
                     newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Document }; // create new home Tab
                     newTab.Header = "Home";

                     // The Content of a TabViewItem is often a frame which hosts a page.
                     Frame frame = new Frame();
                     newTab.Content = frame;
                     frame.Navigate(typeof(HomePage));
                     MainPage.UniversalTabView.TabItems.Add(newTab);*/
                }

                //NewPivotItem.Header = refreshToken;
                /*    if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                    {
                        FindName("MobileBar");
                        FindName("SB");
                        // PivotBar.Visibility = Visibility.Collapsed;
                    }
                    FindName("Block");*/
            }

            /*  else if (args.Uri.ToString() == "https://www.reddit.com/coins/" && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
              {
                  UnloadObject(loginView);
                  FindName("Fail");
                  FindName("Block");
                  UnloadObject(Wblock);
              }
              else if (args.Uri.AbsoluteUri.Contains("https://play.google.com/store/apps/details?id=com.reddit.frontpage") && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
              {
                  UnloadObject(loginView);
                  FindName("Fail");
                  FindName("Block");
                  UnloadObject(Wblock);
              }
              else if (args.Uri.ToString() == "https://www.reddit.com/premium/" && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
              {
                  UnloadObject(loginView);
                  FindName("Fail");
                  FindName("Block");
                  UnloadObject(Wblock);
              }
              else if (args.Uri.ToString() == "https://old.reddit.com/premium/" && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
              {
                  UnloadObject(loginView);
                  FindName("Fail");
                  FindName("Block");
                  UnloadObject(Wblock);
              }
              else if (args.Uri.ToString() == "https://old.reddit.com/coins/" && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
              {
                  UnloadObject(loginView);
                  FindName("Fail");
                  FindName("Block");
                  UnloadObject(Wblock);
              }
              else if (args.Uri.ToString() == "https://www.redditgifts.com/" && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
              {
                  UnloadObject(loginView);
                  FindName("Fail");
                  FindName("Block");
                  UnloadObject(Wblock);
              }
              else if (args.Uri.ToString() == "https://apps.apple.com/us/app/reddit-the-official-app/id1064216828" && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
              {
                  UnloadObject(loginView);
                  FindName("Fail");
                  FindName("Block");
                  UnloadObject(Wblock);
              }
              else
              {
                  return;
              }*/
        }

        private void OnGoBack(object sender, RoutedEventArgs e)
        {
            webView.GoBack();
        }

        private void OnGoForward(object sender, RoutedEventArgs e)
        {
            webView.GoForward();
        }

        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            webView.Refresh();
        }

        private async void OnOpenInBrowser(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(webView.Source);
        }

        private void StartUp()
        {
            //UnloadObject(Wblock);
            var scopes = Eco_Reddit.Core.Constants.Constants.scopeList.Aggregate("", (acc, x) => acc + " " + x);
            var urlParams = "client_id=" + appId +
                            "&response_type=code&state=uyagsjgfhjs&duration=permanent&redirect_uri=" +
                            HttpUtility.UrlEncode("http://127.0.0.1:3000/reddit_callback") + "&scope=" +
                            HttpUtility.UrlEncode(scopes);
            var targetUri = new Uri(Eco_Reddit.Core.Constants.Constants.redditApiBaseUrl + "authorize?" + urlParams);
            webView.Navigate(targetUri);
            SettingsFrame.Navigate(typeof(SettingsPage));
            // UnloadObject(loginView);
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/ProjectEcoReddit/"));
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
                new Uri("https://www.reddit.com/r/ProjectEcoReddit/comments/f7eje3/faq_about_the_app/"));
        }
    }
}
