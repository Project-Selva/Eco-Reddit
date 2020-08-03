using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Reddit;
using Reddit.Controllers;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Eco_Reddit.Views
{
    public sealed partial class ProfilePage : Page, INotifyPropertyChanged
    {
        public string appId = "mp8hDB_HfbctBg";
        private User CurrentUser;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";

        public ProfilePage()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void StartUp(object sender, RoutedEventArgs e)
        {
            var refreshToken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshToken, secret);
            var User = reddit.Account.Me;
            CurrentUser = reddit.Account.Me;
            TitleAuthor.Text = "u/" + User.Name;
            FullNameAuthor.Text = User.Id;
            AuthorKarma.Text = "Karma total: " + (User.CommentKarma + User.LinkKarma);
            AuthorPostKarma.Text = " Post Karma: " + User.LinkKarma;
            AuthorCommentKarma.Text = " Comment Karma: " + User.CommentKarma;
            AuthorDate.Text = "Created: " + User.Created;
            AuthorFriends.Text = "Friends: " + User.NumFriends;

            if (User.IsFriend) NSFWUser.Visibility = Visibility.Visible;
            if (User.IsGold) PremiumUser.Visibility = Visibility.Visible;
            if (User.IsMod) ModUser.Visibility = Visibility.Visible;
            if (User.IsSuspended) SuspendedUser.Visibility = Visibility.Visible;
            if (User.IsVerified) VerifiedUser.Visibility = Visibility.Visible;
            LoadingControl.IsLoading = false;
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Document};
            newTab.Header = "u/" + CurrentUser.About().Name;
            var frame = new Frame();
            newTab.Content = frame;
            UserHomePage.PostUser = CurrentUser;
            frame.Navigate(typeof(UserHomePage));
            HomePage.MainTab.TabItems.Add(newTab);
            HomePage.MainTab.SelectedItem = newTab;
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

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["refresh_token"] = "589558656590-c5zKzVmhsgqmJGqobEebBOAQVl8";
            localSettings.Values["access_token"] = "589558656590-ZeYF5TpcDpIaJy6EEVpKFaz_KOU";
            await WebView.ClearTemporaryWebDataAsync();
            HomePage.SingletonReference.isenabled = false;
            HomePage.SingletonReference.startup();
        }
    }
}
