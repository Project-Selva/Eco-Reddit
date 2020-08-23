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
        private User CurrentUser;
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

        public UserHomePage()
        {
            InitializeComponent();
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
            LoadingControl.IsLoading = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            User = PostUser.About();
            ViewModel.CurrentUser = PostUser;
            UserList.ItemTemplate = PostTemplate;

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
    }
}
