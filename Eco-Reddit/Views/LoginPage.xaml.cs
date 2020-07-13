using Eco_Reddit.Helpers;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinUI = Microsoft.UI.Xaml.Controls;
namespace Eco_Reddit.Views
{
    public sealed partial class LoginPage : Page, INotifyPropertyChanged
    {
        // TODO WTS: Set the URI of the page to show by default
        private const string DefaultUrl = "https://developer.microsoft.com/en-us/windows/apps";
        public string refreshToken;
        public string BackuprefreshToken = "344019503430-ek4oMXyYO7QJci-Cb9jUeuoEhIM";
        public string accessToken;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private bool _isLoading;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            set
            {
                if (value)
                {
                    IsShowingFailedMessage = false;
                }

                Set(ref _isLoading, value);
                IsLoadingVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _isLoadingVisibility;

        public Visibility IsLoadingVisibility
        {
            get { return _isLoadingVisibility; }
            set { Set(ref _isLoadingVisibility, value); }
        }

        private bool _isShowingFailedMessage;

        public bool IsShowingFailedMessage
        {
            get
            {
                return _isShowingFailedMessage;
            }

            set
            {
                if (value)
                {
                    IsLoading = false;
                }

                Set(ref _isShowingFailedMessage, value);
                FailedMesageVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _failedMesageVisibility;

        public Visibility FailedMesageVisibility
        {
            get { return _failedMesageVisibility; }
            set { Set(ref _failedMesageVisibility, value); }
        }

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

            LoginHelper loginHelper = new LoginHelper(appId, secret);
            if (args.Uri.AbsoluteUri.Contains("http://127.0.0.1:3000/reddit_callback"))
            {
                var result = await loginHelper.Login_Stage2(args.Uri);
             
                accessToken = result.AccessToken;
                refreshToken = result.RefreshToken;
                // NewPivotItem.Header = result.RefreshToken;
                if(String.IsNullOrEmpty(refreshToken) == false)// if user accepts
                { 
                localSettings.Values["refresh_token"] = result.RefreshToken;
                    localSettings.Values["access_token"] = result.AccessToken;
                  Eco_Reddit.Models.TokenSharpData.Reddit = new RedditSharp.Reddit(result.AccessToken);
                    await Eco_Reddit.Models.TokenSharpData.Reddit.InitOrUpdateUserAsync();
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
                else// if user declines then show login page again
                {
                  /*  MainPage.UniversalTabView.TabItems.Remove(MainPage.UniversalTabView.SelectedItem); // delete login tab
                    var newTab = new TabViewItem();
                    newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Document }; // create new home Tab
                    newTab.Header = "Login";

                    // The Content of a TabViewItem is often a frame which hosts a page.
                    Frame frame = new Frame();
                    newTab.Content = frame;
                    frame.Navigate(typeof(LoginPage));
                    MainPage.UniversalTabView.TabItems.Add(newTab);
                    var messageDialog = new MessageDialog("Cancelled");
                    await messageDialog.ShowAsync();*/
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

        public bool IsBackEnabled
        {
            get { return webView.CanGoBack; }
        }

        public bool IsForwardEnabled
        {
            get { return webView.CanGoForward; }
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
            await Windows.System.Launcher.LaunchUriAsync(webView.Source);
        }

        public LoginPage()
        {
            InitializeComponent();
            IsLoading = true;
            StartUp();
        }
        private void StartUp()
        {
            //UnloadObject(Wblock);
            var scopes = Constants.Constants.scopeList.Aggregate("", (acc, x) => acc + " " + x);
            var urlParams = "client_id=" + appId + "&response_type=code&state=uyagsjgfhjs&duration=permanent&redirect_uri=" + HttpUtility.UrlEncode("http://127.0.0.1:3000/reddit_callback") + "&scope=" + HttpUtility.UrlEncode(scopes);
            Uri targetUri = new Uri(Constants.Constants.redditApiBaseUrl + "authorize?" + urlParams);
            webView.Navigate(targetUri);
            SettingsFrame.Navigate(typeof(SettingsPage));
             // UnloadObject(loginView);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/ProjectEcoReddit/"));
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/ProjectEcoReddit/comments/f7eje3/faq_about_the_app/"));
        }
    }
}
