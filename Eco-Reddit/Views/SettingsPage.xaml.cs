using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Eco_Reddit.Helpers;
using Eco_Reddit.Services;
using Microsoft.Services.Store.Engagement;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.System;
using Reddit.Controllers;
using Reddit;
using RedditSharp;
using Eco_Reddit.Models;

namespace Eco_Reddit.Views
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings-codebehind.md
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        private string _versionDescription;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string appId = "mp8hDB_HfbctBg";
        private readonly LoginHelper loginHelper = new LoginHelper("mp8hDB_HfbctBg", "UCIGqKPDABnjb0XtMh0Q_LhrNks");
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public SettingsPage()
        {
            InitializeComponent();


            PostSlider.Value = (double) localSettings.Values["PostAdFrequency"];
            SidebarSlider.Value = (double) localSettings.Values["SideBarAdFrequency"];
            if ((bool) localSettings.Values["AdEnabled"])
            {
                AdToggle.IsOn = true;
                AdOptionsContainer.Visibility = Visibility.Visible;
                if ((bool) localSettings.Values["PostAdEnabled"])
                {
                    PostSlider.Visibility = Visibility.Visible;
                    PostAdToggle.IsOn = true;
                }
                else
                {
                    PostSlider.Visibility = Visibility.Collapsed;
                    PostAdToggle.IsOn = false;
                }

                if ((bool) localSettings.Values["SideBarAdEnabled"])
                {
                    SideBarAdToggle.IsOn = true;
                    SidebarSlider.Visibility = Visibility.Visible;
                }
                else
                {
                    SideBarAdToggle.IsOn = false;
                    SidebarSlider.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                AdToggle.IsOn = false;
                AdOptionsContainer.Visibility = Visibility.Collapsed;
            }
            StartUp();
        }
        public async void StartUp()
        {
            var refreshToken = localSettings.Values["refresh_token"].ToString();
            var accessToken = localSettings.Values["access_token"].ToString();
            var result = await loginHelper.Login_Refresh((string)localSettings.Values["refresh_token"]);
            //   localSettings.Values["refresh_token"] = result.RefreshToken;
            localSettings.Values["access_token"] = result.AccessToken;
            TokenSharpData.Reddit = new RedditSharp.Reddit(result.AccessToken);
            await TokenSharpData.Reddit.InitOrUpdateUserAsync();
            var s = TokenSharpData.Reddit.User.GetUsernameMentions();
           RedditSharp.Things.Post selectedPost = await TokenSharpData.Reddit.GetPostAsync(new Uri("https://www.reddit.com/r/ProjectEcoReddit/comments/f7eje3/faq_about_the_app/"));
            MarkDownBlock.Text = selectedPost.SelfText;
               }
        public ElementTheme ElementTheme
        {
            get => _elementTheme;

            set => Set(ref _elementTheme, value);
        }

        public string VersionDescription
        {
            get => _versionDescription;

            set => Set(ref _versionDescription, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            await Task.CompletedTask;

            if (StoreServicesFeedbackLauncher.IsSupported()) FeedbackLink.Visibility = Visibility.Visible;
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void ThemeChanged_CheckedAsync(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null) await ThemeSelectorService.SetThemeAsync((ElementTheme) param);
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

        private async void FeedbackLink_Click(object sender, RoutedEventArgs e)
        {
            // This launcher is part of the Store Services SDK https://docs.microsoft.com/windows/uwp/monetize/microsoft-store-services-sdk
            var launcher = StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        private void AdToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (AdToggle.IsOn)
            {
                localSettings.Values["AdEnabled"] = true;
                AdOptionsContainer.Visibility = Visibility.Visible;
            }
            else
            {
                localSettings.Values["AdEnabled"] = false;
                AdOptionsContainer.Visibility = Visibility.Collapsed;
            }
        }
        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        private async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }
        private void PostAdToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (PostAdToggle.IsOn)
            {
                localSettings.Values["PostAdEnabled"] = true;
                PostSlider.Visibility = Visibility.Visible;
            }
            else
            {
                localSettings.Values["PostAdEnabled"] = false;
                PostSlider.Visibility = Visibility.Collapsed;
            }
        }

        private void SideBarAdToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (SideBarAdToggle.IsOn)
            {
                localSettings.Values["SideBarAdEnabled"] = true;
                SidebarSlider.Visibility = Visibility.Visible;
            }
            else
            {
                localSettings.Values["SideBarAdEnabled"] = false;
                SidebarSlider.Visibility = Visibility.Collapsed;
            }
        }

        private void PostSlider_ValueChanged(object sender, NumberBoxValueChangedEventArgs e)
        {
            localSettings.Values["PostAdFrequency"] = PostSlider.Value;
        }

        private void SidebarSlider_ValueChanged(object sender, NumberBoxValueChangedEventArgs e)
        {
            localSettings.Values["SideBarAdFrequency"] = SidebarSlider.Value;
        }
    }
}
