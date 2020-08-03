using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Controllers;
using User = Reddit.Controllers.User;
using WinUI = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserHomePage : Page
    {
        public static User PostUser;
        public string appId = "mp8hDB_HfbctBg";
        private readonly User CurrentUser;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        private Post SharePost;

        public UserHomePage()
        {
            InitializeComponent();
            var User = PostUser.About();
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            TitleAuthor.Text = "u/" + User.Name;
            FullNameAuthor.Text = "Id: " + User.Fullname;
            var Karma = User.CommentKarma + User.LinkKarma;
            AuthorKarma.Text = "Karma total: " + Karma;
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
            CurrentUser = PostUser;
        }

        private void Subreddit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var h = sender as HyperlinkButton;
            var s = h.Content.ToString().Replace("r/", "");
            HomePage.SingletonReference.NavigateJumper(s);
        }

        private void User_Loaded(object sender, RoutedEventArgs e)
        {
            var Text = sender as TextBlock;
            Text.Text = "u/" + CurrentUser.About().Name;
        }

        private async void HideButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as Posts;
            var post = SenderPost.PostSelf;
            await post.HideAsync();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await CurrentUser.BlockAsync();
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            await CurrentUser.ReportAsync(Reason.Text);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as Posts;
            var post = SenderPost.PostSelf;
            await post.SaveAsync("");
        }

        private void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var P = e.ClickedItem as Posts;
            var newTab = HomePage.MainTab.SelectedItem as WinUI.TabViewItem;
            HomePage.MainTab.TabItems.Remove(newTab);
            var SelectedView = new WinUI.TabViewItem();
            SelectedView.Header = P.PostSelf.Title;
            var frame = new Frame();
            SelectedView.Content = frame;
            frame.Navigate(typeof(PostContentPage));
            PostContentPage.Post = P.PostSelf;
            HomePage.MainTab.TabItems.Add(SelectedView);
            HomePage.MainTab.SelectedItem = SelectedView;
        }

        private void Award_Loaded(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as Posts;
            var post = SenderPost.PostSelf;
            var AwardFrame = sender as Frame;
            AwardFrame.Navigate(typeof(AwardsFlyoutFrame));
            AwardsFlyoutFrame.post = post;
        }

        private async void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            // await PostLocal.ReportAsync(violatorUsername: PostLocal.Author, reason: Reason.Text, ruleReason: RuleReason.Text, banEvadingAccountsNames: PostLocal.Author, siteReason: SiteReason.Text, additionalInfo: AdditionalInfo.Text, customText: Reason.Text, otherReason: OtherInfo.Text, fromHelpCenter: false);
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetText("https://www.reddit.com/r/" + SharePost.Subreddit + "/comments/" + SharePost.Id);
            request.Data.Properties.Title = SharePost.Title;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            //  PostLocal.set
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.DeleteAsync();
        }

        private async void DistinguishButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.DistinguishAsync("yes");
        }

        private async void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            SharePost = AppBarButtonObject.Tag as Post;
            await Task.Delay(500);
            DataTransferManager.ShowShareUI();
        }

        private async void StickyButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.SetSubredditStickyAsync(1, false);
        }

        private async void UnHideEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnhideAsync();
        }

        private async void UnSaveditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnsaveAsync();
        }

        private async void CrosspostButton_Click(object sender, RoutedEventArgs e)
        {
            /* AppBarButton AppBarButtonObject = (AppBarButton)sender;
             Post PostLocal = (AppBarButtonObject).Tag as Post;
             // try
             // {
             if (PostLocal.Listing.IsSelf == true)
             {
                 var newSelfPost = (PostLocal as SelfPost).About().XPostToAsync(CrosspsotText.Text);
             }
             else
             {
                 var newSelfPost = (PostLocal as LinkPost).About().XPostToAsync(CrosspsotText.Text);
             }
             /* }
              catch
              {
                  return;
              }*/
        }

        private async void RemoveEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.RemoveAsync();
        }

        private async void UnStickyEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnsetSubredditStickyAsync(1, false);
        }

        private async void SpoilerEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.SpoilerAsync();
        }

        private async void UnSpoilerEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnspoilerAsync();
        }

        private async void NSFWEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.MarkNSFWAsync();
        }

        private async void UNNSFWEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnmarkNSFWAsync();
        }

        private async void LockEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.LockAsync();
        }

        private async void UnlockEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnlockAsync();
        }

        private async void PermaLinkButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            var pl = PostLocal.Permalink;
            var dataPackage = new DataPackage();
            dataPackage.SetText("https://www.reddit.com" + pl);
            Clipboard.SetContent(dataPackage);
        }

        private void HomeList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 0) throw new Exception("We should be in phase 0, but we are not.");

            // It's phase 0, so this item's title will already be bound and displayed.

            args.RegisterUpdateCallback(ShowPhase1);

            args.Handled = true;
        }

        private async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        private async void ShowPhase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1) throw new Exception("We should be in phase 1, but we are not.");

            var SenderPost = args.Item as Posts;
            var post = SenderPost.PostSelf;
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var img = templateRoot.Children[11] as Image;
            var Text = templateRoot.Children[9] as MarkdownTextBlock;
            //Downvoted.IsChecked = post.IsDownvoted;
            try
            {
                var s = (SelfPost) SenderPost.PostSelf;
                if (s.SelfText.Length < 800)
                    Text.Text = s.SelfText;
                else
                    Text.Text = s.SelfText.Substring(0, 800) + "...";
            }
            catch
            {
            }

            //Downvoted.IsChecked = post.IsDownvoted;
            try
            {
                var p = post as LinkPost;
                var bit = new BitmapImage();
                bit.UriSource = new Uri(p.URL);
                img.Source = bit;
                img.Visibility = Visibility.Visible;
            }
            catch
            {
                img.Visibility = Visibility.Collapsed;
            }
            ///   TextFlairBlock.Foreground = post.Listing.LinkFlairBackgroundColor;

            //  args.RegisterUpdateCallback(this.ShowPhase2);
        }

        private async void Frame_LoadedSubreddit(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var Frames = (Frame) sender;
                var pp = Frames.Tag as Post;
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                SubredditTemporaryInfo.InfoSubReddit = reddit.Subreddit(pp.Subreddit);
                var f = sender as Frame;
                f.Navigate(typeof(SubredditTemporaryInfo));
            });
        }

        private async void UserList_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                //   GetUserPostsClass.limit = 10;
                //    GetUserPostsClass.skipInt = 0;
                GetUserPostsClass.UserToGetPostsFrom = PostUser;
                var Postscollection = new IncrementalLoadingCollection<GetUserPostsClass, Posts>();
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () => { UserList.ItemsSource = Postscollection; });
            });
        }
    }
}
