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
using Windows.Foundation;
using Windows.Foundation.Collections;
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

         /*   Posts SenderPost = args.Item as Eco_Reddit.Models.Posts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var textBlock = templateRoot.Children[5] as HyperlinkButton;
            var TextDateBlock = templateRoot.Children[3] as TextBlock;
            var TextFlairBlock = templateRoot.Children[6] as TextBlock;
            var TextTitleBlock = templateRoot.Children[2] as TextBlock;
            TextTitleBlock.Text = post.Title;
            textBlock.Content = post.Subreddit;
            TextDateBlock.Text = "Created: " + post.Created;
            TextFlairBlock.Text = "    Flair: " + post.Listing.LinkFlairText;
            // Posts SenderPost = args.Item as Eco_Reddit.Models.Posts;
            //  Reddit.Controllers.Post post = SenderPost.PostSelf;
            //  var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var img = templateRoot.Children[8] as Image;
            var Upvoted = templateRoot.Children[0] as AppBarToggleButton;
            var Downvoted = templateRoot.Children[1] as AppBarToggleButton;
            Upvoted.Label = post.UpVotes.ToString();
            Upvoted.IsChecked = post.IsUpvoted;
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

            //  args.RegisterUpdateCallback(this.ShowPhase2);*/
        }

        private async void UserList_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(500);
            GetUserPostsClass.limit = 10;
            GetUserPostsClass.skipInt = 0;
            GetUserPostsClass.UserToGetPostsFrom = PostUser;
            var Postscollection = new IncrementalLoadingCollection<GetUserPostsClass, Posts>();
            UserList.ItemsSource = Postscollection;
        }
    }
}
