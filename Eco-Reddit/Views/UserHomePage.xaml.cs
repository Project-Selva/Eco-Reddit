using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Uwp;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
            FullNameAuthor.Text = User.Fullname;
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
            GetUserPostsClass.limit = 10;
            GetUserPostsClass.skipInt = 0;
            GetUserPostsClass.UserToGetPostsFrom = PostUser;
            var Postscollection = new IncrementalLoadingCollection<GetUserPostsClass, UserPosts>();
            UserList.ItemsSource = Postscollection;
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
            var SenderPost = DataContext as UserPosts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            await post.HideAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as UserPosts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            await post.SaveAsync("");
        }
        private  void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {
                UserPosts P = e.ClickedItem as UserPosts;
                WinUI.TabViewItem newTab = HomePage.MainTab.SelectedItem as WinUI.TabViewItem;
                    HomePage.MainTab.TabItems.Remove(newTab);
                    var SelectedView = new WinUI.TabViewItem();
                    SelectedView.Header = P.TitleText;
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
                var SenderPost = DataContext as UserPosts;
                Reddit.Controllers.Post post = SenderPost.PostSelf;
                Frame AwardFrame = sender as Frame;
                AwardFrame.Navigate(typeof(AwardsFlyoutFrame));
                AwardsFlyoutFrame.post = post;
        }

    }
}
