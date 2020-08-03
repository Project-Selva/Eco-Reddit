using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Reddit.Controllers;
using WinUI = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserTemporaryInfo : Page
    {
        public static User PostUser;
        private User CurrentUser;

        public UserTemporaryInfo()
        {
            InitializeComponent();
        }

        public void StartUp(object sender, RoutedEventArgs e)
        {
            var User = PostUser.About();
            CurrentUser = PostUser;
            TitleAuthor.Text = "u/" + User.Name;
            FullNameAuthor.Text = User.Id;
            AuthorKarma.Text = "Karma total: " + (User.CommentKarma + User.LinkKarma);
            AuthorPostKarma.Text = " Post Karma: " + User.LinkKarma;
            AuthorCommentKarma.Text = " Comment Karma: " + User.CommentKarma;
            AuthorDate.Text = "Created: " + User.Created;
            AuthorFriends.Text = "Friends: " + User.NumFriends;
            if (User.IsFriend) FriendUser.Visibility = Visibility.Visible;
            if (User.Over18) NSFWUser.Visibility = Visibility.Visible;
            if (User.IsGold) PremiumUser.Visibility = Visibility.Visible;
            if (User.IsMod) ModUser.Visibility = Visibility.Visible;
            if (User.IsSuspended) SuspendedUser.Visibility = Visibility.Visible;
            if (User.IsVerified) VerifiedUser.Visibility = Visibility.Visible;
            LoadingControl.Visibility = Visibility.Collapsed;
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await CurrentUser.BlockAsync();
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            //  await CurrentUser.ReportAsync(reason: Reason.Text);
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Document};
            newTab.Header = "u/" + CurrentUser.About().Name;
            var frame = new Frame();
            newTab.Content = frame;
            UserHomePage.PostUser = PostUser;
            frame.Navigate(typeof(UserHomePage));
            HomePage.MainTab.TabItems.Add(newTab);
            HomePage.MainTab.SelectedItem = newTab;
        }
    }
}
