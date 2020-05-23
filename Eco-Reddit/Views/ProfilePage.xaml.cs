using Reddit;
using Reddit.Controllers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Eco_Reddit.Views
{
    public sealed partial class ProfilePage : Page, INotifyPropertyChanged
    {
        public string appId = "mp8hDB_HfbctBg";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        User CurrentUser;
        public ProfilePage()
        {
            InitializeComponent();         
        }
        public void StartUp(object sender, RoutedEventArgs e)
        {
            string refreshToken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshToken, secret);
            var User = reddit.Account.Me;
            CurrentUser = reddit.Account.Me;
            TitleAuthor.Text = "u/" + User.Name;
            FullNameAuthor.Text = User.Id;
            AuthorKarma.Text = "Karma total: " + (User.CommentKarma + User.LinkKarma);
            AuthorPostKarma.Text = " Post Karma: " + User.LinkKarma;
            AuthorCommentKarma.Text = " Comment Karma: " + User.CommentKarma;
            AuthorDate.Text = "Created: " + User.Created.ToString();
            AuthorFriends.Text = "Friends: " + User.NumFriends.ToString();
            if (User.IsFriend == true)
            {
                NSFWUser.Visibility = Visibility.Visible;
            }
            if (User.IsGold == true)
            {
                PremiumUser.Visibility = Visibility.Visible;
            }
            if (User.IsMod == true)
            {
                ModUser.Visibility = Visibility.Visible;
            }
            if (User.IsSuspended == true)
            {
                SuspendedUser.Visibility = Visibility.Visible;
            }
            if (User.IsVerified == true)
            {
                VerifiedUser.Visibility = Visibility.Visible;
            }
            LoadingControl.Visibility = Visibility.Collapsed;
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Document };
            newTab.Header = "u/" + CurrentUser.About().Name;
            Frame frame = new Frame();
            newTab.Content = frame;
            UserHomePage.PostUser = CurrentUser;
            frame.Navigate(typeof(UserHomePage));
            HomePage.MainTab.TabItems.Add(newTab);
            HomePage.MainTab.SelectedItem = newTab;
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

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["refresh_token"] = null;
           await WebView.ClearTemporaryWebDataAsync();
            var m = new MessageDialog("Signed out please restart app to avoid possible crashes(pre - alpha issue)");
               await m.ShowAsync();
        }
    }
}
