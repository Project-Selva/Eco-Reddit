using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Controllers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Eco_Reddit.Views
{
    public sealed partial class HomePage : Page, INotifyPropertyChanged
    {
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Subreddit CurrentSub;
        public RedditClient Client;
        public bool IsHomeEnabled;
        public string PostsSortOrder;
        public static WinUI.TabView MainTab { get; set; }
        public static ListView L { get; set; }
        public static Frame LoginFrameFrame { get; set; }
        public static HomePage SingletonReference { get; set; }
        public HomePage()
        {
            InitializeComponent();
            MainTab = MainTabView;
            L = HomeList;
            SingletonReference = this;
            LoginFrameFrame = LoginFrame;
         /*   var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;*/
            try { 
            string refreshToken = localSettings.Values["refresh_token"].ToString();
                string BackuprefreshToken = "344019503430-ek4oMXyYO7QJci-Cb9jUeuoEhIM";
                if (localSettings.Values["refresh_token"].ToString() == BackuprefreshToken) //remove and replace, this is when user is not signed in and should show different ui
                {
                    LoginFrame.Visibility = Visibility.Visible;
                    LoginFrame.Navigate(typeof(LoginPage));
                }
                else
                {
                    Client = new RedditClient(appId, refreshToken, secret);
                    GetPostsClass.SortOrder = "Best";
                    PostsSortOrder = "Best";
                    IsHomeEnabled = true;
                    var PostsCollection = new IncrementalLoadingCollection<GetPostsClass, Posts>();

                    HomeList.ItemsSource = PostsCollection;
                 //   localSettings.Values["refresh_token"] = BackuprefreshToken;
                }
           }
            catch
            {
                LoginFrame.Visibility = Visibility.Visible;
                LoginFrame.Navigate(typeof(LoginPage));
            }

        }

    


        private void OnTabCloseRequested(WinUI.TabView sender, WinUI.TabViewTabCloseRequestedEventArgs args)
        {
                sender.TabItems.Remove(args.Tab);
            GC.Collect(2);
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

        private async void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Posts P = e.ClickedItem as Posts;
                if (MainTabView.TabItems.Count == 0)
                {

                    var newTab = new WinUI.TabViewItem();
                    newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Document };
                    newTab.Header = P.PostSelf.Title;
                    Frame frame = new Frame();
                    newTab.Content = frame;
                    frame.Navigate(typeof(PostContentPage));
                    PostContentPage.Post = P.PostSelf;
                    //  PostContentPage.SingletonReference.StartUp();
                    MainTabView.TabItems.Add(newTab);
                    MainTabView.SelectedItem = newTab;
                }
                else
                {
                     WinUI.TabViewItem newTab = MainTabView.SelectedItem as WinUI.TabViewItem;
                    MainTabView.TabItems.Remove(newTab);
                    var SelectedView = new WinUI.TabViewItem();
                    SelectedView.Header = P.PostSelf.Title;
                     Frame frame = new Frame();
                     SelectedView.Content = frame;
                     frame.Navigate(typeof(PostContentPage));
                    PostContentPage.Post = P.PostSelf;
                    MainTabView.TabItems.Add(SelectedView);
                    MainTabView.SelectedItem = SelectedView;
                    //  PostContentPage.SingletonReference.StartUp();
                }
            });
        }

 

        private async void Award_Loaded(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var SenderFramework = (FrameworkElement)sender;
                var DataContext = SenderFramework.DataContext;
                var SenderPost = DataContext as Posts;
                Reddit.Controllers.Post post = SenderPost.PostSelf;
                Frame AwardFrame = sender as Frame;
                AwardFrame.Navigate(typeof(AwardsFlyoutFrame));
                AwardsFlyoutFrame.post = post;
            });
        }

        private async void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (IsHomeEnabled == true)
                {
                    TitleSidebar.Text = "r/Home";
                    HeaderSideBar.Text = "Your personalised reddit feed";
                    SubscribeButton.Visibility = Visibility.Collapsed;
                    UnSubscribeButton.Visibility = Visibility.Collapsed;
                    SideBarSidebar.Visibility = Visibility.Collapsed;
                    AboutSidebar.Visibility = Visibility.Collapsed;
                    SubsSidebar.Visibility = Visibility.Collapsed;
                    UserssSidebar.Visibility = Visibility.Collapsed;
                    TimeSidebar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    string refreshToken = localSettings.Values["refresh_token"].ToString();
                    var reddit = new RedditClient(appId, refreshToken, secret);
                    var subreddit = CurrentSub.About();
                    TitleSidebar.Text = "r/" + subreddit.Name;

                    HeaderSideBar.Text = subreddit.Title;
                    SubscribeButton.Visibility = Visibility.Visible;
                    UnSubscribeButton.Visibility = Visibility.Visible;
                    SideBarSidebar.Visibility = Visibility.Visible;
                    HeaderSideBar.Visibility = Visibility.Visible;
                    AboutSidebar.Visibility = Visibility.Visible;
                    SubsSidebar.Visibility = Visibility.Visible;
                    UserssSidebar.Visibility = Visibility.Visible;
                    TimeSidebar.Visibility = Visibility.Visible;
                    if (String.IsNullOrEmpty(subreddit.IconImg.ToString()) == false)
                    {
                        BitmapImage img = new BitmapImage();
                      //  img.UriSource = new Uri("/Images/1409938.png");
                      //  SubIcon.ProfilePicture = img;
                    }
                    if (String.IsNullOrEmpty(subreddit.BannerImg.ToString()) == false)
                    {
                        BitmapImage Bimg = new BitmapImage();
                    //    Bimg.UriSource = new Uri("/Images/1409938.png");
                      //  BannerIMG.Source = Bimg;
                    }
              /*      if (String.IsNullOrEmpty(subreddit.HeaderImg.ToString()) == false)
                    {
                        BitmapImage Himg = new BitmapImage();
                        Himg.UriSource = new Uri(subreddit.HeaderImg.ToString());
                        HeaderIMG.Source = Himg;
                    }*/

                    SubscribeButton.Visibility = Visibility.Visible;
                    UnSubscribeButton.Visibility = Visibility.Visible;
                    AboutSidebar.Text = subreddit.Description;
                    SubsSidebar.Text = "Active Users: " + subreddit.ActiveUserCount.ToString();
                    UserssSidebar.Text = "Subscribers: " + subreddit.Subscribers.ToString();
                    TimeSidebar.Text = "Created: " + subreddit.Created.ToString();
                    if (subreddit.Over18 == true)
                    {
                        NSFWGrid.Visibility = Visibility.Visible;
                    }
                    SideBarSidebar.Text = "Sidebar isnt supported in EcoReddit Alpha";
                    /*  try { 
                      SideBarSidebar.Text = subreddit.Sidebar.ToString();
                      }
                      catch
                      {
                          SideBarSidebar.Text = "Couldnt load sidebar";
                      }*/
                }
            });

        }
        private async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                await Launcher.LaunchUriAsync(link);
            }
        }
        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                await Launcher.LaunchUriAsync(link);
            }
        }
        private async void UnSubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var subreddit = CurrentSub.About();
                await subreddit.UnsubscribeAsync();
            }
            catch
            {
                return;
            }
        }

        private async void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var subreddit = CurrentSub.About();
                await subreddit.SubscribeAsync();
            }
            catch
            {
                return;
            }
        }

        private async void Jumper_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                    if (String.IsNullOrEmpty(args.QueryText) == true)
                    {
                        return;
                    }
                    else
                    {
                    try {
                        CurrentSub = Client.Subreddit(args.QueryText);
                        SubredditText.Text = "r/" + CurrentSub.Name;
                        GetPostsClass.Subreddit = CurrentSub.Name;
                        GetPostsClass.limit = 10;
                        GetPostsClass.skipInt = 0;
                        IsHomeEnabled = false;
                        PostsSortOrder = "Hot";
                        SortOrderButton.Label = "Hot";
                        GetPostsClass.SortOrder = "Hot";
                        var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Posts>();
                        HomeList.ItemsSource = Postscollection;
                        SortOrderButton.Visibility = Visibility.Visible;
                    }
                    catch
                    {
                        var m = new MessageDialog("Subreddit doesnt exist");
                        await m.ShowAsync();
                    }
                    }
            });
        }
        public void NavigateJumper(string subredditclicked)
        {
            CurrentSub = Client.Subreddit(subredditclicked);
            IsHomeEnabled = false;
            SubredditText.Text = "r/" + CurrentSub.Name;
            GetPostsClass.Subreddit = CurrentSub.Name;
            GetPostsClass.limit = 10;
            GetPostsClass.skipInt = 0;
            IsHomeEnabled = false;
            PostsSortOrder = "Hot";
            SortOrderButton.Label = "Hot";
            GetPostsClass.SortOrder = "Hot";
            var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Posts>();
            HomeList.ItemsSource = Postscollection;
            SortOrderButton.Visibility = Visibility.Visible;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsFrame.Navigate(typeof(SettingsPage));
        }

        private async void RefreshBarButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (IsHomeEnabled == true)
                {
                    SubredditText.Text = "Home";
                    GetPostsClass.SortOrder = "Best";
                    GetPostsClass.limit = 10;
                    GetPostsClass.skipInt = 0;
                    SortOrderButton.Visibility = Visibility.Collapsed;
                    IsHomeEnabled = true;

                    var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Posts>();
                    HomeList.ItemsSource = Postscollection;


                }
                else
                {
                    GetPostsClass.SortOrder = PostsSortOrder;
                    GetPostsClass.Subreddit = CurrentSub.Name;
                    GetPostsClass.limit = 10;
                    GetPostsClass.skipInt = 0;
                    var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Posts>();
                    HomeList.ItemsSource = Postscollection;
                }
            });
        }
        private void Search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Document };
            newTab.Header = "Search results for: " + args.QueryText;
            Frame frame = new Frame();
            newTab.Content = frame;
            SearchPage.SearchString =  args.QueryText;
            if(IsHomeEnabled == true)
            {
                SearchPage.Subreddit = "all";
              }
            else
            {
                SearchPage.Subreddit = CurrentSub.Name;
            }
            frame.Navigate(typeof(SearchPage));
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        private void Search_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private async void SortOrderItem_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (IsHomeEnabled == true)
                {
                    SubredditText.Text = "Home";
                    GetPostsClass.SortOrder = "Best";
                    SortOrderButton.Visibility = Visibility.Collapsed;
                    GetPostsClass.limit = 10;
                    GetPostsClass.skipInt = 0;
                    IsHomeEnabled = true;
                    var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Posts>();
                    HomeList.ItemsSource = Postscollection;
                }
                else
                {
                    MenuFlyoutItem SortMenu = sender as MenuFlyoutItem;
                    GetPostsClass.SortOrder = SortMenu.Text;
                    SortOrderButton.Label = SortMenu.Text;
                    PostsSortOrder = SortMenu.Text;
                    IsHomeEnabled = false;
                    GetPostsClass.Subreddit = CurrentSub.Name;
                    GetPostsClass.limit = 10;
                    GetPostsClass.skipInt = 0;
                    var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Posts>();
                    HomeList.ItemsSource = Postscollection;
                }
            });
        }

        private async void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var SenderFramework = (FrameworkElement)sender;
                var DataContext = SenderFramework.DataContext;
                var SenderPost = DataContext as Posts;
                Reddit.Controllers.Post post = SenderPost.PostSelf;
                var newTab = new WinUI.TabViewItem();
                newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Document };
                newTab.Header = post.Title;
                Frame frame = new Frame();
                newTab.Content = frame;
                frame.Navigate(typeof(PostContentPage));
                PostContentPage.Post = post;
                //  PostContentPage.SingletonReference.StartUp();
                MainTabView.TabItems.Add(newTab);
            });
        }

        private void ClearTabButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabView.TabItems.Clear();
            GC.Collect(2);
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
        private async void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var SenderFramework = (FrameworkElement)sender;
                var DataContext = SenderFramework.DataContext;
                var SenderPost = DataContext as Posts;
                Reddit.Controllers.Post post = SenderPost.PostSelf;
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                Reddit.Controllers.User PostUser = reddit.User(post.Author);
                UserTemporaryInfo.PostUser = PostUser;
                Frame f = sender as Frame;
                f.Navigate(typeof(UserTemporaryInfo));
            });
        }

        private void HyperlinkButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void splitView_Loaded(object sender, RoutedEventArgs e)
        {
            FindName("HomeBar");
        }

        private void Frame_LoadedSubreddit(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as Posts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            string refreshToken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshToken, secret);
            SubredditTemporaryInfo.InfoSubReddit = reddit.Subreddit(post.Subreddit);
            Frame f = sender as Frame;
            f.Navigate(typeof(SubredditTemporaryInfo));
        }

        private void Subreddit_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
        /* private  void SubredditlinkButton_Loaded(object sender, RoutedEventArgs e)
{
//  var SenderFramework = (FrameworkElement)sender;
//  var DataContext = SenderFramework.DataContext;

HyperlinkButton Sub = sender as HyperlinkButton;

var senderframework = (FrameworkElement)Sub;
var DataContext = Sub.DataContext;

//  Post SenderPost = DataContext as Post;

//  Reddit.Controllers.Post post = SenderPost;
Reddit.Controllers.Post post = DataContext as Post;
Sub.Content = "r/" + post.Subreddit;
}*/
    }
 

}
