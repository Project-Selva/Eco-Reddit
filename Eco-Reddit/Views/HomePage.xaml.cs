using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.LinksAndComments;
using Reddit.Inputs.Subreddits;
using Reddit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
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
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            SingletonReference = this;
            LoginFrameFrame = LoginFrame;

            /*   var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
              coreTitleBar.ExtendViewIntoTitleBar = true;*/
          try
           {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                string BackuprefreshToken = "344019503430-ek4oMXyYO7QJci-Cb9jUeuoEhIM";
               /* if (localSettings.Values["refresh_token"].ToString() == BackuprefreshToken) //remove and replace, this is when user is not signed in and should show different ui
                {
                    LoginFrame.Visibility = Visibility.Visible;
                    LoginFrame.Navigate(typeof(LoginPage));
                }
                else
                {*/
                    Client = new RedditClient(appId, refreshToken, secret);
                    GetPostsClass.SortOrder = "Best";
                    PostsSortOrder = "Best";
                    IsHomeEnabled = true;
                    string refreshtoken = localSettings.Values["refresh_token"].ToString();
                    var reddit = new RedditClient(appId, refreshtoken, secret);
                    var PostsCollection = new IncrementalLoadingCollection<GetPostsClass, Posts>();

                    HomeList.ItemsSource = PostsCollection;
                System.Timers.Timer uTimer = new System.Timers.Timer();
                uTimer.Elapsed += new ElapsedEventHandler(TimerElapsed);
                uTimer.Interval = 20000;
                uTimer.Enabled = true;
                uTimer.AutoReset = true;

                //   localSettings.Values["refresh_token"] = BackuprefreshToken;
           // }
            }
            catch
            {
                LoginFrame.Visibility = Visibility.Visible;
               LoginFrame.Navigate(typeof(LoginPage));
            }
        }
 
        public void TimerElapsed(object source, ElapsedEventArgs e)
        {
           string refreshtoken = localSettings.Values["refresh_token"].ToString();
           var reddit = new RedditClient(appId, refreshtoken, secret);
          InboxButton.Label = "Inbox: " + reddit.Account.Messages.Unread.Count.ToString();
        }
        Post SharePost;
        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.SetText("https://www.reddit.com/r/" + SharePost.Subreddit + "/comments/" + SharePost.Id);
            request.Data.Properties.Title = SharePost.Title;
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
                    PostContentPage.Post = P.PostSelf;
                    frame.Navigate(typeof(PostContentPage));

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
                    PostContentPage.Post = P.PostSelf;
                    frame.Navigate(typeof(PostContentPage));
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
                Frame Frames = (Frame)sender;
                Post pp = (Frames).Tag as Post;
                Frame AwardFrame = sender as Frame;
                AwardFrame.Navigate(typeof(AwardsFlyoutFrame));
                AwardsFlyoutFrame.post = pp;
            });
        }

        private async void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                SideBarsplitView.IsPaneOpen = true;
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
                    ModeratedSubsX.Visibility = Visibility.Visible;
                    TrendingSubsdayX.Visibility = Visibility.Visible;
                    SubsdayX.Visibility = Visibility.Visible;
                    TinySubsdayX.Visibility = Visibility.Visible;
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
                    ModeratedSubsX.Visibility = Visibility.Collapsed;
                    TrendingSubsdayX.Visibility = Visibility.Collapsed;
                    SubsdayX.Visibility = Visibility.Collapsed;
                    TinySubsdayX.Visibility = Visibility.Collapsed;
                    HeaderSideBar.Visibility = Visibility.Visible;
                    AboutSidebar.Visibility = Visibility.Visible;
                    SubsSidebar.Visibility = Visibility.Visible;
                    UserssSidebar.Visibility = Visibility.Visible;
                    TimeSidebar.Visibility = Visibility.Visible;
                /*    if (String.IsNullOrEmpty(subreddit.IconImg.ToString()) == false)
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
                    try
                    {
                        if (subreddit.Sidebar.Length < 5000)
                        {
                            SideBarSidebar.Text = subreddit.Sidebar.ToString();
                        }
                        else
                        {
                            SideBarSidebar.Text = "Sidebar too big to load";
                        }
                    }
                    catch
                    {
                        SideBarSidebar.Text = "Couldnt load sidebar";
                    }
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
                    try
                    {
                        CurrentSub = Client.Subreddit(args.QueryText);
                        try
                        {
                            var subreddit = CurrentSub.About();
                        }
                        catch
                        {
                            var m = new MessageDialog("Subreddit doesnt exist");
                            await m.ShowAsync();
                            return;
                        }
                        Subreddit.Text = "r/" + CurrentSub.Name;
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
            Subreddit.Text = "r/" + CurrentSub.Name;
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
                    Subreddit.Text = "Home";
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
            if (SortBox.SelectedItem.ToString() == "Posts")
            {
                var newTab = new WinUI.TabViewItem();
                newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Find };
                newTab.Header = "Search results for: " + args.QueryText;
                Frame frame = new Frame();
                newTab.Content = frame;
                SearchPage.SearchString = args.QueryText;
                if (IsHomeEnabled == true)
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
            else if (SortBox.SelectedItem.ToString() == "Subreddits")
            {
                var newTab = new WinUI.TabViewItem();
                newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Find };
                newTab.Header = "Subreddit search results for: " + args.QueryText;
                Frame frame = new Frame();
                newTab.Content = frame;
                SearchSubredditPage.SearchString = args.QueryText;
                frame.Navigate(typeof(SearchSubredditPage));
                MainTabView.TabItems.Add(newTab);
                MainTabView.SelectedItem = newTab;
            }
            else
            {

            }
        }

        private async void SortOrderItem_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (IsHomeEnabled == true)
                {
                    Subreddit.Text = "Home";
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

        private void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton NewTabButton = (AppBarButton)sender;
            Post pp = (NewTabButton).Tag as Post;
            // Reddit.Controllers.Post post = SenderPost.PostSelf;
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Document };
            newTab.Header = pp.Title;
            Frame frame = new Frame();
            newTab.Content = frame;
            PostContentPage.Post = pp;
            frame.Navigate(typeof(PostContentPage));
            //  PostContentPage.SingletonReference.StartUp();
            MainTabView.TabItems.Add(newTab);
            // }
        }

        private void ClearTabButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabView.TabItems.Clear();
            GC.Collect(2);
        }

        private async void HideButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonHide = (AppBarButton)sender;
            Post pp = (AppBarButtonHide).Tag as Post;
            await pp.HideAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonSave = (AppBarButton)sender;
            Post pp = (AppBarButtonSave).Tag as Post;
            await pp.SaveAsync("");
        }
        private async void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Frame Frames = (Frame)sender;
                Post pp = (Frames).Tag as Post;
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                Reddit.Controllers.User PostUser = reddit.User(pp.Author);
                UserTemporaryInfo.PostUser = PostUser;
                Frame f = sender as Frame;
                f.Navigate(typeof(UserTemporaryInfo));
            });
        }

        private void HyperlinkButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            HyperlinkButton h = sender as HyperlinkButton;
            string s = h.Content.ToString().Replace("u/", "");
            var newTab = new WinUI.TabViewItem();
            string refreshToken = localSettings.Values["refresh_token"].ToString();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Document };
            var reddit = new RedditClient(appId, refreshToken, secret);
            Reddit.Controllers.User u = reddit.User(s);
            newTab.Header = "u/" + s;
            Frame frame = new Frame();
            newTab.Content = frame;
            UserHomePage.PostUser = reddit.User(s);
            frame.Navigate(typeof(UserHomePage));
            HomePage.MainTab.TabItems.Add(newTab);
            HomePage.MainTab.SelectedItem = newTab;
        }

        private void splitView_Loaded(object sender, RoutedEventArgs e)
        {
            FindName("HomeBar");
        }

        private async void Frame_LoadedSubreddit(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Frame Frames = (Frame)sender;
                Post pp = (Frames).Tag as Post;
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                SubredditTemporaryInfo.InfoSubReddit = reddit.Subreddit(pp.Subreddit);
                Frame f = sender as Frame;
                f.Navigate(typeof(SubredditTemporaryInfo));
            });
        }

        private void Subreddit_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            HyperlinkButton h = sender as HyperlinkButton;
            string s = h.Content.ToString().Replace("r/", "");
            NavigateJumper(s);
        }
        private IEnumerable<Subreddit> Subreddits;
        List<SubredditList> SubredditCollection;
        Visibility Nsfw;
        private async void HubButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                HubsplitView.IsPaneOpen = true;
            });
        }

        private async void SubscribedSubs_ItemClick(object sender, ItemClickEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                SubredditList P = e.ClickedItem as SubredditList;
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
              try
               {
 
                    CurrentSub = Client.Subreddit(P.SubredditSelf.Name);
               //     var m = new MessageDialog(CurrentSub.Name.ToLower());
               //     await m.ShowAsync();
                    Subreddit.Text = "r/" + CurrentSub.Name;
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
            });
        }
        private async void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                GetPostsClass.SortOrder = "Best";
                PostsSortOrder = "Best";
                Subreddit.Text = "Home";
                IsHomeEnabled = true;
                var PostsCollection = new IncrementalLoadingCollection<GetPostsClass, Posts>();
                HomeList.ItemsSource = PostsCollection;
            });
        }
        private async void PopularButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);

                CurrentSub = Client.Subreddit("popular");
                Subreddit.Text = "r/" + CurrentSub.Name;
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
            });
        }
        private async void AllButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                CurrentSub = Client.Subreddit("all");
                Subreddit.Text = "r/" + CurrentSub.Name;
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
            });
        }
        private async void TrendingButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                CurrentSub = Client.Subreddit("trendingsubreddits");
                Subreddit.Text = "r/" + CurrentSub.Name;
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
            });
        }
        private async void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                CurrentSub = Client.Subreddit("random");
                Subreddit.Text = "r/" + CurrentSub.Name;
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
            });
        }
        private void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            ProfileFrame.Navigate(typeof(ProfilePage));
        }

        private async void AddBarButton_Click(object sender, RoutedEventArgs e)
        {
                 var newTab = new WinUI.TabViewItem();
                newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Add };
            newTab.Header = "Create new post";
                Frame frame = new Frame();
                newTab.Content = frame;
            if (IsHomeEnabled == false)
            {
                CreateNewPost.currentsubSTATIC = CurrentSub.Name;
            }
            else
            {
                CreateNewPost.currentsubSTATIC = "HOME";
            }
                frame.Navigate(typeof(CreateNewPost));

                //  PostContentPage.SingletonReference.StartUp();
                MainTabView.TabItems.Add(newTab);
                MainTabView.SelectedItem = newTab;
            
        }

        private async void CreatePostDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
          /*  string refreshToken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshToken, secret);
            PivotItem pivotitem = (PivotItem)CreatePostPivot.SelectedItem;
            String RichText;
            EditZone.TextDocument.GetText(Windows.UI.Text.TextGetOptions.None, out RichText);
            if (pivotitem.Header.ToString() == "Text")
            {
                reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text, kind: "self", text: RichText, sr: CurrentSub.Name));
            }
            else
            {
                reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text, url: Link.Text, sr: CurrentSub.Name));
            }*/
        }

        private async void OpenPostInWebButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonWEB = (AppBarButton)sender;
            Post pp = (AppBarButtonWEB).Tag as Post;
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/" + pp.Subreddit + "/comments/" + pp.Id));
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

            Posts SenderPost = args.Item as Posts;
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

        private async void SubscribedSubs_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                GetSubreddit.Load = true;
                //   var SubredditsCollection = new IncrementalLoadingCollection<GetSubreddit, SubredditList>();
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                Subreddits = reddit.Account.MySubscribedSubreddits();
                List <SubredditList>   SubredditCollection = new List<SubredditList>();
                await Task.Run(() =>
                {
                    foreach (Subreddit subreddit in Subreddits)
                    {
                        if (subreddit.Over18 == true)
                        {
                            Nsfw = Visibility.Visible;
                        }
                        else
                        {
                            Nsfw = Visibility.Collapsed;
                        }
                        // Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
                        SubredditCollection.Add(new SubredditList()
                        {
                            IsNSFW = Nsfw,
                            TitleSubreddit = subreddit.Name,
                            SubredditSelf = subreddit,
                            SubredditIcon = subreddit.CommunityIcon
                        });
                    }
                });
                SubscribedSubs.ItemsSource = SubredditCollection;
                Expander_Expanded();
                Expander_Expanded_1();
                Expander_Expanded_2();
                Expander_Expanded_3();
            }
            catch
            {
                return;
            }
        }

        private async void Expander_Expanded()
        {
            try
            {
                GetSubreddit.Load = true;
                //   var SubredditsCollection = new IncrementalLoadingCollection<GetSubreddit, SubredditList>();
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var RSubreddits = reddit.Account.Me.GetModeratedSubreddits();
                SubredditCollection = new List<SubredditList>();
                await Task.Run(() =>
                {
                    foreach (Reddit.Things.ModeratedListItem subreddit in RSubreddits)
                    {
                        if (subreddit.Over18 == true)
                        {
                            Nsfw = Visibility.Visible;
                        }
                        else
                        {
                            Nsfw = Visibility.Collapsed;
                        }
                        SubredditCollection.Add(new SubredditList()
                        {
                            TitleSubreddit = subreddit.SRDisplayNamePrefixed.Replace("r/",""),
                            IsNSFW = Nsfw
                        });
                    }
                });
                ModeratedSubs.ItemsSource = SubredditCollection;
                ModeratedSubsX.ItemsSource = SubredditCollection;
            }
            catch
            {
                return;
            }
        }
        private async void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            // await PostLocal.ReportAsync(violatorUsername: PostLocal.Author, reason: Reason.Text, ruleReason: RuleReason.Text, banEvadingAccountsNames: PostLocal.Author, siteReason: SiteReason.Text, additionalInfo: AdditionalInfo.Text, customText: Reason.Text, otherReason: OtherInfo.Text, fromHelpCenter: false);
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

        private void SearchTipButton_Click(object sender, RoutedEventArgs e)
        {
            // SearchTip.IsOpen = true;
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Find };
            newTab.Header = "Search";
            Frame frame = new Frame();
            newTab.Content = frame;
            SearchPage.SearchString = "";
            if (IsHomeEnabled == true)
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

    

        private void InboxButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Mail };
            newTab.Header = "Inbox";
            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(InboxHubPage));
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Mail };
            newTab.Header = "Chat";
            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(ChatWebViewPage));
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        private void NetWorkButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Mail };
            newTab.Header = "Reddit Network";
            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(RPAN));
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        private void PremiumButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Mail };
            newTab.Header = "Premium";
            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(Premium));
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        private void GoldButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Mail };
            newTab.Header = "Coins";
            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(Gold));
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        private async void Expander_Expanded_1()
        {
          try
           {
                GetSubreddit.Load = true;
                //   var SubredditsCollection = new IncrementalLoadingCollection<GetSubreddit, SubredditList>();
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
            var subreddit = reddit.Subreddit("subredditoftheday");
           List<Post> post = subreddit.Posts.GetNew(limit: 1);
            string s = "";
            await Task.Run(() =>
            { 
                 foreach (Post posts in post)
                 {
                    SelfPost p = posts as SelfPost;
                    s = p.SelfText;
                 }
             });
            string first = new StringReader(s).ReadLine();
                string eeee = first.Replace(" ", "");
                Regex.Replace(eeee, @"\s+", "");
                string ss = eeee.Replace("####/r/", "");
          //  string ssss = sss.Replace("####", "");
            //string ss = ssss.Replace(" /r/", "");
            var subredditD = reddit.Subreddit(ss);
            List<SubredditList> SubredditCollectionD = new List<SubredditList>();
            SubredditCollectionD.Add(new SubredditList()
            {
                IsNSFW = Nsfw,
                TitleSubreddit = subredditD.Name,
                SubredditSelf = subredditD,
                SubredditIcon = subredditD.CommunityIcon
            });
                Subsday.ItemsSource = SubredditCollectionD;
            SubsdayX.ItemsSource = SubredditCollectionD;
               }
                
               catch
               {
                   return;
               }
        }
        private async void Expander_Expanded_2()
        {
              try
            {
            GetSubreddit.Load = true;
            //   var SubredditsCollection = new IncrementalLoadingCollection<GetSubreddit, SubredditList>();
            string refreshToken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshToken, secret);
            var subreddit = reddit.Subreddit("tinysubredditoftheday");
            List<Post> post = subreddit.Posts.GetNew(limit: 1);
            string s = "";
            await Task.Run(() =>
            {
                foreach (Post posts in post)
                {
                    LinkPost p = posts as LinkPost;
                    s = p.URL;
                }
            });
            string first = new StringReader(s).ReadLine();
            string ss = first.Replace("/", "");
            string sss = ss.Replace("http:www.reddit.comr", "");
            var subredditD = reddit.Subreddit(sss);
            List<SubredditList> SubredditCollectionD = new List<SubredditList>();
            SubredditCollectionD.Add(new SubredditList()
            {
                IsNSFW = Nsfw,
                TitleSubreddit = subredditD.Name,
                SubredditSelf = subredditD,
                SubredditIcon = subredditD.CommunityIcon
            });
            TinySubsday.ItemsSource = SubredditCollectionD;
            TinySubsdayX.ItemsSource = SubredditCollectionD;
               }
               catch
               {
                   return;
               }
        }
        private async void Expander_Expanded_3()
        {
            try
            {
                GetSubreddit.Load = true;
                //   var SubredditsCollection = new IncrementalLoadingCollection<GetSubreddit, SubredditList>();
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var subreddit = reddit.Subreddit("trendingsubreddits");
                List<Post> post = subreddit.Posts.GetNew(limit: 1);
                List<SubredditList> SubredditCollectionD = new List<SubredditList>();
                string s = "";
                await Task.Run(() =>
                {
                    foreach (Post posts in post)
                    {
                        s = posts.Title;
                    }
                });
                string first = new StringReader(s).ReadLine();
                string ss = first.Remove(0, 36);
                string[] sss = ss.Split(",");
                foreach (var wordss in sss)
                {
                    string eee = wordss.Replace("/r/", "");
                    string eeee = eee.Replace(" ", "");
                     Regex.Replace(eeee, @"\s+", "");
                    var subredditD = reddit.Subreddit(eeee);

                    SubredditCollectionD.Add(new SubredditList()
                    {
                        IsNSFW = Nsfw,
                        TitleSubreddit = subredditD.Name,
                        SubredditSelf = subredditD,
                        SubredditIcon = subredditD.CommunityIcon
                    });
                }
                TrendingSubsday.ItemsSource = SubredditCollectionD;
                TrendingSubsdayX.ItemsSource = SubredditCollectionD;
            }
            catch
            {
                return;
            }
        }

        private void InboxButton_Loaded(object sender, RoutedEventArgs e)
        {
            string refreshtoken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshtoken, secret);
            InboxButton.Label = "Inbox: " + reddit.Account.Messages.Unread.Count.ToString();
        }

        private async void Upvote_Checked(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
          PostLocal.Upvote();

        }

        private async void DownVote_Checked(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Post PostLocal = (AppBarButtonObject).Tag as Post;
            await PostLocal.DownvoteAsync();
        }
    }


}
