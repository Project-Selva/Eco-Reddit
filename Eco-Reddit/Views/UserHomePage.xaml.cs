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
using Eco_Reddit.Core.Models;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Controllers;
using User = Reddit.Controllers.User;
using WinUI = Microsoft.UI.Xaml.Controls;
using Telerik;
using System.Collections.Generic;
using Windows.UI.Popups;
using Eco_Reddit.UserControls;

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

        public User User;
        public List<SimpleData> items;
        public class SimpleData
        {
            public double Value { get; set; }

            public string Title { get; set; }

            public bool IsSelected { get; set; }
        }
        private Post SharePost;
        public UserHomePage()
        {
            InitializeComponent();
            User = PostUser.About();
            ViewModel.CurrentUser = PostUser;
            UserList.ItemTemplate = PostTemplate;
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            TitleAuthor.Text = "u/" + User.Name;
            var Karma = ((int)User.CommentKarma + (int)User.LinkKarma).ToString();
            KarmaPie.Text = Karma;

            items = new List<SimpleData>();

            SimpleData data = new SimpleData();
            data.Title = "Link Karma: " + User.LinkKarma;
            data.Value = User.LinkKarma;


            items.Add(data);
            SimpleData datax = new SimpleData();
            datax.Title = "Comment Karma: " + User.CommentKarma;
            datax.Value = User.CommentKarma;
            items.Add(datax);
            if (User.IsFriend) FriendUser.Visibility = Visibility.Visible;
            if (User.Over18) NSFWUser.Visibility = Visibility.Visible;
            if (User.IsGold) PremiumUser.Visibility = Visibility.Visible;
            if (User.IsMod) ModUser.Visibility = Visibility.Visible;
            if (User.IsSuspended) SuspendedUser.Visibility = Visibility.Visible;
            if (User.IsVerified) VerifiedUser.Visibility = Visibility.Visible;
            CurrentUser = PostUser;

        }

        private async void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            SharePost = AppBarButtonObject.Tag as Post;
            await Task.Delay(500);
            DataTransferManager.ShowShareUI();
        }
        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetText("https://www.reddit.com/r/" + SharePost.Subreddit + "/comments/" + SharePost.Id);
            request.Data.Properties.Title = SharePost.Title;
        }
        private async void Frame_LoadedSubreddit(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var Frames = (Frame)sender;
                var pp = Frames.Tag as Post;
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                SubredditTemporaryInfo.InfoSubReddit = reddit.Subreddit(pp.Subreddit);
                var f = sender as Frame;
                f.Navigate(typeof(SubredditTemporaryInfo));
            });
        }
        public Post post;
        public async void ReplyBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as Comment;
            await Comment.ReplyAsync(sender.Text);
        }

        public async void DistinguishEditButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as RedditSharp.Things.Comment;
            try
            {
                await Comment.DistinguishAsync(RedditSharp.Things.ModeratableThing.DistinguishType.Moderator);
            }
            catch
            {
            }
        }

        public async void saveEditButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as RedditSharp.Things.Comment;
            try
            {
                await Comment.SaveAsync();
            }
            catch
            {
            }
        }

        public async void UnsaveEditButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as RedditSharp.Things.Comment;
            try
            {
                await Comment.UnsaveAsync();
            }
            catch
            {
            }
        }

        public async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as RedditSharp.Things.Comment;
            try
            {
                await Comment.RemoveAsync();
            }
            catch
            {
            }
        }

        public async void RemoveAsSpamButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as RedditSharp.Things.Comment;
            try
            {
                await Comment.RemoveSpamAsync();
            }
            catch
            {
            }
        }

        public async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as RedditSharp.Things.Comment;
            try
            {
                await Comment.DelAsync();
            }
            catch
            {
            }
        }
        public void Award_Loaded(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as Posts;
            var post = SenderPost.PostSelf;
            var AwardFrame = sender as Frame;
            AwardFrame.Navigate(typeof(AwardsFlyoutFrame));
            AwardsFlyoutFrame.post = post;
        }
        public void Subreddit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var h = sender as HyperlinkButton;
            var s = h.Content.ToString().Replace("r/", "");
            HomePage.SingletonReference.NavigateJumper(s);
        }


        public async void HideButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as Post;
            var post = SenderPost;
            await post.HideAsync();
        }



        public async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var post = DataContext as Post;
            await post.SaveAsync("");
        }
        public async void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            // await PostLocal.ReportAsync(violatorUsername: PostLocal.Author, reason: Reason.Text, ruleReason: RuleReason.Text, banEvadingAccountsNames: PostLocal.Author, siteReason: SiteReason.Text, additionalInfo: AdditionalInfo.Text, customText: Reason.Text, otherReason: OtherInfo.Text, fromHelpCenter: false);
        }

        public async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        public async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        public void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            //  PostLocal.set
        }
        public async void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as RedditSharp.Things.Comment;
            try
            {
                await Comment.ApproveAsync();
            }
            catch
            {
            }
        }
        public async void DeleteButtonPost_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.DeleteAsync();
        }

        public async void DistinguishButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.DistinguishAsync("yes");
        }


        public async void StickyButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.SetSubredditStickyAsync(1, false);
        }

        public async void UnHideEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnhideAsync();
        }

        public async void UnSaveditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnsaveAsync();
        }

        public async void CrosspostButton_Click(object sender, RoutedEventArgs e)
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

        public async void RemoveEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.RemoveAsync();
        }

        public async void UnStickyEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnsetSubredditStickyAsync(1, false);
        }

        public async void SpoilerEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.SpoilerAsync();
        }

        public async void UnSpoilerEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnspoilerAsync();
        }

        public async void NSFWEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.MarkNSFWAsync();
        }

        public async void UNNSFWEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnmarkNSFWAsync();
        }

        public async void LockEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.LockAsync();
        }

        public async void UnlockEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            await PostLocal.UnlockAsync();
        }

        public async void PermaLinkButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton)sender;
            var PostLocal = AppBarButtonObject.Tag as Post;
            var pl = PostLocal.Permalink;
            var dataPackage = new DataPackage();
            dataPackage.SetText("https://www.reddit.com" + pl);
            Clipboard.SetContent(dataPackage);
        }

        private void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (UserList.ItemTemplate == PostTemplate)
            {
                var P = e.ClickedItem as Post;
                var newTab = HomePage.MainTab.SelectedItem as WinUI.TabViewItem;
                HomePage.MainTab.TabItems.Remove(newTab);
                var SelectedView = new WinUI.TabViewItem();
                SelectedView.Header = P.Title;
                var frame = new Frame();
                SelectedView.Content = frame;
                frame.Navigate(typeof(PostContentPage));
                PostContentPage.Post = P;
                HomePage.MainTab.TabItems.Add(SelectedView);
                HomePage.MainTab.SelectedItem = SelectedView;
            }
        }

        private void HomeList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 0) throw new Exception("We should be in phase 0, but we are not.");

            // It's phase 0, so this item's title will already be bound and displayed.

            args.RegisterUpdateCallback(ShowPhase1);

            args.Handled = true;
        }


        private async void ShowPhase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1) throw new Exception("We should be in phase 1, but we are not.");
            LoadingControl.IsLoading = false;
            var post = args.Item as Post;
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var img = templateRoot.Children[11] as Image;
            var Text = templateRoot.Children[9] as MarkdownTextBlock;
  
            //Downvoted.IsChecked = post.IsDownvoted;
            try
            {
                var s = (SelfPost)post;
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

     

        private async void UserList_Loaded(object sender, RoutedEventArgs e)
        {
            /*  await Task.Run(async () =>
              {
                  //   GetUserPostsClass.limit = 10;
                  //    GetUserPostsClass.skipInt = 0;

                  GetUserPostsClass.UserToGetPostsFrom = PostUser;
                  var Postscollection = new IncrementalLoadingCollection<GetUserPostsClass, Post>();
                  await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                      async () => { UserList.ItemsSource = Postscollection; });
              });*/
         //   var Text = sender as TextBlock;
           // Text.Text = "u/" + CurrentUser.About().Name;
            await ViewModel.StartPosts(PostUser);
            UserList.ItemsSource = ViewModel.Posts;
        }


    }
}
