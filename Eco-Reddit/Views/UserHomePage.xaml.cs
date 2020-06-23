using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Uwp;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinUI = Microsoft.UI.Xaml.Controls;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserHomePage : Page
    {
        public static User PostUser { get; set; }
        User CurrentUser;
        public UserHomePage()
        {
            this.InitializeComponent();
            var User = PostUser.About();
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            TitleAuthor.Text = "u/" + User.Name;
            FullNameAuthor.Text = "Id: " + User.Fullname;
            int Karma = User.CommentKarma + User.LinkKarma;
            AuthorKarma.Text = "Karma total: " + Karma;
            AuthorPostKarma.Text = " Post Karma: " + User.LinkKarma;
            AuthorCommentKarma.Text = " Comment Karma: " + User.CommentKarma;
            AuthorDate.Text = "Created: " + User.Created.ToString();
            AuthorFriends.Text = "Friends: " + User.NumFriends.ToString();
            if (User.IsFriend == true)
            {
                FriendUser.Visibility = Visibility.Visible;
            }
            if (User.Over18 == true)
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
            CurrentUser = PostUser;
        }

        private void User_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock Text = sender as TextBlock;
            Text.Text = "u/" + CurrentUser.About().Name;
        }

        private async void HideButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as Posts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            await post.HideAsync();
        }
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await CurrentUser.BlockAsync();
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            await CurrentUser.ReportAsync(reason: Reason.Text);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as Posts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            await post.SaveAsync("");
        }
        private  void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {
                Posts P = e.ClickedItem as Posts;
                WinUI.TabViewItem newTab = HomePage.MainTab.SelectedItem as WinUI.TabViewItem;
                    HomePage.MainTab.TabItems.Remove(newTab);
                    var SelectedView = new WinUI.TabViewItem();
            SelectedView.Header = P.PostSelf.Title;
                    Frame frame = new Frame();
                    SelectedView.Content = frame;
                    frame.Navigate(typeof(PostContentPage));
                    PostContentPage.Post = P.PostSelf;
                HomePage.MainTab.TabItems.Add(SelectedView);
                HomePage.MainTab.SelectedItem = SelectedView;
        }
        private void Award_Loaded(object sender, RoutedEventArgs e)
        {
                var SenderFramework = (FrameworkElement)sender;
                var DataContext = SenderFramework.DataContext;
                var SenderPost = DataContext as Posts;
                Reddit.Controllers.Post post = SenderPost.PostSelf;
                Frame AwardFrame = sender as Frame;
                AwardFrame.Navigate(typeof(AwardsFlyoutFrame));
                AwardsFlyoutFrame.post = post;
        }
        private async void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            // await PostLocal.ReportAsync(violatorUsername: PostLocal.Author, reason: Reason.Text, ruleReason: RuleReason.Text, banEvadingAccountsNames: PostLocal.Author, siteReason: SiteReason.Text, additionalInfo: AdditionalInfo.Text, customText: Reason.Text, otherReason: OtherInfo.Text, fromHelpCenter: false);
        }
        Post SharePost;
        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.SetText("https://www.reddit.com/r/" + SharePost.Subreddit + "/comments/" + SharePost.Id);
            request.Data.Properties.Title = SharePost.Title;
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            //  PostLocal.set
        }
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.DeleteAsync();
        }
        private async void DistinguishButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.DistinguishAsync(how: "yes");
        }
        private async void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            SharePost = (AppBarButtonObject).Tag as Post;
            await Task.Delay(500);
            DataTransferManager.ShowShareUI();
        }
        private async void StickyButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.SetSubredditStickyAsync(num: 1, toProfile: false);
        }
        private async void UnHideEditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.UnhideAsync();
        }
        private async void UnSaveditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
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
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.RemoveAsync();
        }
        private async void UnStickyEditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.UnsetSubredditStickyAsync(num: 1, toProfile: false);
        }
        private async void SpoilerEditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.SpoilerAsync();
        }
        private async void UnSpoilerEditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.UnspoilerAsync();
        }
        private async void NSFWEditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.MarkNSFWAsync();
        }
        private async void UNNSFWEditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.UnmarkNSFWAsync();
        }
        private async void LockEditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.LockAsync();
        }
        private async void UnlockEditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.UnlockAsync();
        }
        private async void PermaLinkButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            string pl = PostLocal.Permalink;
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText("https://www.reddit.com" + pl);
            Clipboard.SetContent(dataPackage);
        }

        private void HomeList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {

            if (args.Phase != 0)
            {
                throw new System.Exception("We should be in phase 0, but we are not.");
            }

            // It's phase 0, so this item's title will already be bound and displayed.

            args.RegisterUpdateCallback(this.ShowPhase1);

            args.Handled = true;
        }

        private async void ShowPhase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1)
            {
                throw new System.Exception("We should be in phase 1, but we are not.");
            }

          Posts SenderPost = args.Item as Eco_Reddit.Models.Posts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var img = templateRoot.Children[10] as Image;
            //Downvoted.IsChecked = post.IsDownvoted;
            try
            {
                var p = post as LinkPost;
                BitmapImage bit = new BitmapImage();
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
            await Task.Run(async () =>
            {
               //   GetUserPostsClass.limit = 10;
              //    GetUserPostsClass.skipInt = 0;
                GetUserPostsClass.UserToGetPostsFrom = PostUser;
                var Postscollection = new IncrementalLoadingCollection<GetUserPostsClass, Posts>();
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    UserList.ItemsSource = Postscollection;
                });
            });
        }
    }
}
