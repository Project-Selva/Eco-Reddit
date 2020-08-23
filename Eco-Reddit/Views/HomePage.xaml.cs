using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
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
using WinUI = Microsoft.UI.Xaml.Controls;
using Things = Reddit.Things;
using Windows.UI.Xaml.Controls.Primitives;
using Eco_Reddit.Helpers;

namespace Eco_Reddit.Views
{
    public sealed partial class HomePage : Page, INotifyPropertyChanged
    {
        public static WinUI.TabView MainTab;
        public static ListView L;
        public static IncrementalLoadingCollection<GetPostsClass, Post> HomePostList;
        public static Frame LoginFrameFrame;
        public static HomePage SingletonReference;
        public string appId = "mp8hDB_HfbctBg";
        public RedditClient Client;
        public  Subreddit CurrentSub { get; set; }
        public bool IsHomeEnabled;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private Visibility Nsfw;
        public string PostsSortOrder;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        private Post SharePost;
        private List<SubredditList> SubredditCollection;
        private IEnumerable<Subreddit> Subreddits;
        public bool isenabled { get; set; }
        public HomePage()
        {
            InitializeComponent();
            LoadingControl.IsLoading = true;
            //   localSettings.Values["refresh_token"] = BackuprefreshToken;
            // }
        }
    

public event PropertyChangedEventHandler PropertyChanged;

        public void TimerElapsed(object source, ElapsedEventArgs e)
        {
            try
            {
                var refreshtoken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshtoken, secret);
                InboxButton.Label = "Inbox: " + reddit.Account.Messages.Unread.Count;
            }
            catch
            {
            }
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetText("https://www.reddit.com/r/" + SharePost.Subreddit + "/comments/" + SharePost.Id);
            request.Data.Properties.Title = SharePost.Title;
        }

        private void OnTabCloseRequested(WinUI.TabView sender, WinUI.TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
            GC.Collect(2);
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var P = e.ClickedItem as Post;
                if (MainTabView.TabItems.Count == 0)
                {
                    var newTab = new WinUI.TabViewItem();
                    newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Document};
                    newTab.Header = P.Title;
                    var frame = new Frame();
                    newTab.Content = frame;
                    PostContentPage.Post = P;
                    frame.Navigate(typeof(PostContentPage));

                    //  PostContentPage.SingletonReference.StartUp();
                    MainTabView.TabItems.Add(newTab);
                    MainTabView.SelectedItem = newTab;
                }
                else
                {
                    var newTab = MainTabView.SelectedItem as WinUI.TabViewItem;
                    MainTabView.TabItems.Remove(newTab);
                    var SelectedView = new WinUI.TabViewItem();
                    SelectedView.Header = P.Title;
                    var frame = new Frame();
                    SelectedView.Content = frame;
                    PostContentPage.Post = P;
                    frame.Navigate(typeof(PostContentPage));
                    MainTabView.TabItems.Add(SelectedView);
                    MainTabView.SelectedItem = SelectedView;
                    //  PostContentPage.SingletonReference.StartUp();
                }
            });
        }



        private async void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SideBarsplitView.IsPaneOpen = true;
                if ((bool) localSettings.Values["AdEnabled"] && (bool) localSettings.Values["SideBarAdEnabled"])
                {
                    var r = new Random();
                    if (r.Next(100) < (double) localSettings.Values["SideBarAdFrequency"])
                    {
                    }
                    else
                    {
                        vungleBannerSideBarControl.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    vungleBannerSideBarControl.Visibility = Visibility.Collapsed;
                }

                if (IsHomeEnabled)
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
                    //   RecommendX.Visibility = Visibility.Collapsed;
                    TinySubsdayX.Visibility = Visibility.Visible;
                }
                else
                {
                    var refreshToken = localSettings.Values["refresh_token"].ToString();
                    var reddit = new RedditClient(appId, refreshToken, secret);
                    var subreddit = CurrentSub.About();
                    /* RecommendX.Visibility = Visibility.Visible;
                     var recommended = reddit.Models.Subreddits.Recommended(CurrentSub.Name, new SubredditsRecommendInput());
                     List<SubredditList> SubredditCollection = new List<SubredditList>();
                     foreach (Reddit.Things.SubredditRecommendations r in recommended)
                     {
                         SubredditCollection.Add(new SubredditList()
                         {
                             TitleSubreddit = r.Name
                         });
                     }
                     RecommendX.ItemsSource = SubredditCollection;*/
                    TitleSidebar.Text = "r/" + subreddit.Name;
                    HeaderSideBar.Text = subreddit.Title;
                    SubscribeButton.Visibility = Visibility.Visible;
                    UnSubscribeButton.Visibility = Visibility.Visible;
                    SideBarSidebar.Visibility = Visibility.Visible;

                    //  ModeratedSubsX.Visibility = Visibility.Collapsed;
                    //   TrendingSubsdayX.Visibility = Visibility.Collapsed;
                    // SubsdayX.Visibility = Visibility.Collapsed;
                    // TinySubsdayX.Visibility = Visibility.Collapsed;
                    HeaderSideBar.Visibility = Visibility.Visible;
                    AboutSidebar.Visibility = Visibility.Visible;
                    SubsSidebar.Visibility = Visibility.Visible;
                    UserssSidebar.Visibility = Visibility.Visible;
                    TimeSidebar.Visibility = Visibility.Visible;
                    /*    if (String.IsNullOrEmpty(subreddit.IconImg.ToString()) == false)
                        {
                            BitmapImage img = new BitmapImage();
                            //  img.UriSource = new Uri("/Core/Assets/RedditImages/1409938.png");
                            //  SubIcon.ProfilePicture = img;
                        }
                        if (String.IsNullOrEmpty(subreddit.BannerImg.ToString()) == false)
                        {
                            BitmapImage Bimg = new BitmapImage();
                            //    Bimg.UriSource = new Uri("/Core/Assets/RedditImages/1409938.png");
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
                    SubsSidebar.Text = "Active Users: " + subreddit.ActiveUserCount;
                    UserssSidebar.Text = "Subscribers: " + subreddit.Subscribers;
                    TimeSidebar.Text = "Created: " + subreddit.Created;
                    if (subreddit.Over18 == true) NSFWGrid.Visibility = Visibility.Visible;
                    SideBarSidebar.Text = "Sidebar isnt supported in EcoReddit Alpha";
                    try
                    {
                        if (subreddit.Sidebar.Length < 5000)
                            SideBarSidebar.Text = subreddit.Sidebar;
                        else
                            SideBarSidebar.Text = "Sidebar too big to load";
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
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        private async void UnSubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var subreddit = CurrentSub.About();
                await subreddit.UnsubscribeAsync();
            }
            catch
            {
            }
        }

        private async void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var subreddit = CurrentSub.About();
                await subreddit.SubscribeAsync();
            }
            catch
            {
            }
        }

        private async void Jumper_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                if (string.IsNullOrEmpty(args.QueryText))
                    return;
               // try
              //  {
                  CurrentSub = reddit.Subreddit(args.QueryText);
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


                    IsHomeEnabled = false;
                    PostsSortOrder = "Hot";
                    SortOrderButton.Label = "Hot";
                    GetPostsClass.SortOrder = "Hot";
                    var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                    HomeList.ItemsSource = Postscollection;
                    SortOrderButton.Visibility = Visibility.Visible;
               /* }
                catch
                {
                    var m = new MessageDialog("Subreddit doesnt exist");
                    await m.ShowAsync();
                }*/
            });
        }

        public void NavigateJumper(string subredditclicked)
        {
            var refreshToken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshToken, secret);
            CurrentSub = reddit.Subreddit(subredditclicked);
            IsHomeEnabled = false;
            Subreddit.Text = "r/" + CurrentSub.Name;
            GetPostsClass.Subreddit = CurrentSub.Name;


            IsHomeEnabled = false;
            PostsSortOrder = "Hot";
            SortOrderButton.Label = "Hot";
            GetPostsClass.SortOrder = "Hot";
            var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
            HomeList.ItemsSource = Postscollection;
            SortOrderButton.Visibility = Visibility.Visible;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsFrame.Navigate(typeof(SettingsPage));
        }

        private async void RefreshBarButton_Click(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (IsHomeEnabled)
                {
                    Subreddit.Text = "Home";
                    GetPostsClass.SortOrder = "Best";


                    SortOrderButton.Visibility = Visibility.Collapsed;
                    IsHomeEnabled = true;

                    var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                    HomeList.ItemsSource = Postscollection;
                }
                else
                {
                    GetPostsClass.SortOrder = PostsSortOrder;
                    GetPostsClass.Subreddit = CurrentSub.Name;


                    var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                    HomeList.ItemsSource = Postscollection;
                }
            });
        }


        private async void SortOrderItem_Click(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (IsHomeEnabled)
                {
                    Subreddit.Text = "Home";
                    GetPostsClass.SortOrder = "Best";
                    SortOrderButton.Visibility = Visibility.Collapsed;


                    IsHomeEnabled = true;
                    var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                    HomeList.ItemsSource = Postscollection;
                }
                else
                {
                    var SortMenu = sender as MenuFlyoutItem;
                    GetPostsClass.SortOrder = SortMenu.Text;
                    SortOrderButton.Label = SortMenu.Text;
                    PostsSortOrder = SortMenu.Text;
                    IsHomeEnabled = false;
                    GetPostsClass.Subreddit = CurrentSub.Name;


                    var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                    HomeList.ItemsSource = Postscollection;
                }
            });
        }

     
        private void ClearTabButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabView.TabItems.Clear();
            GC.Collect(2);
        }



        private void Subreddit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var h = sender as HyperlinkButton;
            var s = h.Content.ToString().Replace("r/", "");
            NavigateJumper(s);
        }

        private async void HubButton_Click(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () => { HubsplitView.IsPaneOpen = true; });
        }

        private async void SubscribedSubs_ItemClick(object sender, ItemClickEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var P = e.ClickedItem as Subreddit;
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                 try
                  {

               CurrentSub = reddit.Subreddit(P.Name);

                 Subreddit.Text = "r/" + CurrentSub.Name;
                GetPostsClass.Subreddit = CurrentSub.Name;


                IsHomeEnabled = false;
                PostsSortOrder = "Hot";
                SortOrderButton.Label = "Hot";
                GetPostsClass.SortOrder = "Hot";
                var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
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
        private async void SubscribedSubsO_ItemClick(object sender, ItemClickEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var P = e.ClickedItem as SubredditList;
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                 try
                 {

                CurrentSub = reddit.Subreddit(P.TitleSubreddit);

                Subreddit.Text = "r/" + CurrentSub.Name;
                GetPostsClass.Subreddit = CurrentSub.Name;


                IsHomeEnabled = false;
                PostsSortOrder = "Hot";
                SortOrderButton.Label = "Hot";
                GetPostsClass.SortOrder = "Hot";
                var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
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
        private async void ModeratedSubs_ItemClick(object sender, ItemClickEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var P = e.ClickedItem as Things.ModeratedListItem;
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                try
                {
                    string s = P.SRDisplayNamePrefixed.Replace("r/", "");
                    CurrentSub = reddit.Subreddit(s);

                    Subreddit.Text = "r/" + CurrentSub.Name;
                    GetPostsClass.Subreddit = CurrentSub.Name;


                    IsHomeEnabled = false;
                    PostsSortOrder = "Hot";
                    SortOrderButton.Label = "Hot";
                    GetPostsClass.SortOrder = "Hot";
                    var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
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
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                GetPostsClass.SortOrder = "Best";
                PostsSortOrder = "Best";
                Subreddit.Text = "Home";
                IsHomeEnabled = true;
                var PostsCollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                HomeList.ItemsSource = PostsCollection;
            });
        }

        private async void PopularButton_Click(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);

                CurrentSub = reddit.Subreddit("popular");
                Subreddit.Text = "r/" + CurrentSub.Name;
                GetPostsClass.Subreddit = CurrentSub.Name;


                IsHomeEnabled = false;
                PostsSortOrder = "Hot";
                SortOrderButton.Label = "Hot";
                GetPostsClass.SortOrder = "Hot";
                var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                HomeList.ItemsSource = Postscollection;
                SortOrderButton.Visibility = Visibility.Visible;
            });
        }

        private async void AllButton_Click(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                CurrentSub = reddit.Subreddit("all");
                Subreddit.Text = "r/" + CurrentSub.Name;
                GetPostsClass.Subreddit = CurrentSub.Name;


                IsHomeEnabled = false;
                PostsSortOrder = "Hot";
                SortOrderButton.Label = "Hot";
                GetPostsClass.SortOrder = "Hot";
                var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                HomeList.ItemsSource = Postscollection;
                SortOrderButton.Visibility = Visibility.Visible;
            });
        }

        private async void TrendingButton_Click(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                CurrentSub = reddit.Subreddit("trendingsubreddits");
                Subreddit.Text = "r/" + CurrentSub.Name;
                GetPostsClass.Subreddit = CurrentSub.Name;


                IsHomeEnabled = false;
                PostsSortOrder = "Hot";
                SortOrderButton.Label = "Hot";
                GetPostsClass.SortOrder = "Hot";
                var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                HomeList.ItemsSource = Postscollection;
                SortOrderButton.Visibility = Visibility.Visible;
            });
        }

        private async void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                CurrentSub = reddit.Subreddit("random");
                Subreddit.Text = "r/" + CurrentSub.Name;
                GetPostsClass.Subreddit = CurrentSub.Name;


                IsHomeEnabled = false;
                PostsSortOrder = "Hot";
                SortOrderButton.Label = "Hot";
                GetPostsClass.SortOrder = "Hot";
                var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
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
            newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Add};
            newTab.Header = "Create new post";
            var frame = new Frame();
            newTab.Content = frame;
            if (IsHomeEnabled == false)
                CreateNewPost.currentsubSTATIC = CurrentSub.Name;
            else
                CreateNewPost.currentsubSTATIC = "HOME";
            frame.Navigate(typeof(CreateNewPost));

            //  PostContentPage.SingletonReference.StartUp();
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

    



        private async void Expander_Expanded()
        {
            try
            {
                GetSubreddit.Load = true;
                //   var SubredditsCollection = new IncrementalLoadingCollection<GetSubreddit, SubredditList>();
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var RSubreddits = reddit.Account.Me.GetModeratedSubreddits();
               List<Things.ModeratedListItem> SubredditCollection = new List<Things.ModeratedListItem>();
                await Task.Run(() =>
                {
                    foreach (var subreddit in RSubreddits)
                    {
                        SubredditCollection.Add(subreddit);
                    }
                });
                ModeratedSubs.ItemsSource = SubredditCollection;
                ModeratedSubsX.ItemsSource = SubredditCollection;
            }
            catch
            {
            }
        }

        
        private void SearchTipButton_Click(object sender, RoutedEventArgs e)
        {
            // SearchTip.IsOpen = true;
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Find};
            newTab.Header = "Search";
            var frame = new Frame();
            newTab.Content = frame;
            SearchPage.SearchString = "";
            if (IsHomeEnabled)
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
            newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Mail};
            newTab.Header = "Inbox";
            var frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(InboxHubPage));
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Mail};
            newTab.Header = "Chat";
            var frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(ChatWebViewPage));
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        private void NetWorkButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Mail};
            newTab.Header = "Reddit Network";
            var frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(RPAN));
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        private void PremiumButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Mail};
            newTab.Header = "Premium";
            var frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(Premium));
            MainTabView.TabItems.Add(newTab);
            MainTabView.SelectedItem = newTab;
        }

        private void GoldButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Mail};
            newTab.Header = "Coins";
            var frame = new Frame();
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
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var subreddit = reddit.Subreddit("subredditoftheday");
                var post = subreddit.Posts.GetNew(limit: 1);
                var s = "";
                await Task.Run(() =>
                {
                    foreach (var posts in post)
                    {
                        var p = posts as SelfPost;
                        s = p.SelfText;
                    }
                });
                var first = new StringReader(s).ReadLine();
                var eeee = first.Replace(" ", "");
                Regex.Replace(eeee, @"\s+", "");
                var ss = eeee.Replace("####/r/", "");
                //  string ssss = sss.Replace("####", "");
                //string ss = ssss.Replace(" /r/", "");
                var ssss = ss.Replace("r/", "");
                var sssss = ssss.Replace("**", "");
                var ssssss = sssss.Replace("#", "");
                var subredditD = reddit.Subreddit(ssssss);
                var SubredditCollectionD = new List<SubredditList>();
                await Task.Run(() =>
                {
                    SubredditCollectionD.Add(new SubredditList
                    {
                        TitleSubreddit = subredditD.Name,
                        SubredditSelf = subredditD,
                        SubredditIcon = subredditD.CommunityIcon
                    });
                });
                //Subsday.ItemsSource = SubredditCollectionD;
                SubsdayX.ItemsSource = SubredditCollectionD;
            }

            catch
            {
            }
        }

        private async void Expander_Expanded_2()
        {
            try
            {
                GetSubreddit.Load = true;
                //   var SubredditsCollection = new IncrementalLoadingCollection<GetSubreddit, SubredditList>();
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var subreddit = reddit.Subreddit("tinysubredditoftheday");
                var post = subreddit.Posts.GetNew(limit: 1);
                var SubredditCollectionD = new List<SubredditList>();
                var s = "";
                await Task.Run(() =>
                {
                    foreach (var posts in post)
                    {
                        var p = posts as LinkPost;
                        s = p.URL;
                    }


                    var first = new StringReader(s).ReadLine();
                    var ss = first.Replace("/", "");
                    var sss = ss.Replace("http:www.reddit.comr", "");
                    var ssss = sss.Replace("r/", "");
                    var subredditD = reddit.Subreddit(ssss);


                    SubredditCollectionD.Add(new SubredditList
                    {
                        TitleSubreddit = subredditD.Name,
                        SubredditSelf = subredditD,
                    });
                });
                //TinySubsday.ItemsSource = SubredditCollectionD;
                TinySubsdayX.ItemsSource = SubredditCollectionD;
            }
            catch
            {
            }
        }

        private async void Expander_Expanded_3()
        {
            try
            {
                GetSubreddit.Load = true;
                //   var SubredditsCollection = new IncrementalLoadingCollection<GetSubreddit, SubredditList>();
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var subreddit = reddit.Subreddit("trendingsubreddits");
                var post = subreddit.Posts.GetNew(limit: 1);
                var SubredditCollectionD = new List<SubredditList>();
                var s = "";
                await Task.Run(() =>
                {
                    foreach (var posts in post) s = posts.Title;

                    var first = new StringReader(s).ReadLine();
                    var ss = first.Remove(0, 36);
                    var sss = ss.Split(",");
                    foreach (var wordss in sss)
                    {
                        var eee = wordss.Replace("/r/", "");
                        var eeee = eee.Replace(" ", "");
                        Regex.Replace(eeee, @"\s+", "");
                        var subredditD = reddit.Subreddit(eeee);

                        SubredditCollectionD.Add(new SubredditList
                        {
                            TitleSubreddit = subredditD.Name,
                            SubredditSelf = subredditD,
                        });
                    }
                });
                //TrendingSubsday.ItemsSource = SubredditCollectionD;
                TrendingSubsdayX.ItemsSource = SubredditCollectionD;
                LoadingControl.IsLoading = false;
            }
            catch
            {
            }
        }

        private void InboxButton_Loaded(object sender, RoutedEventArgs e)
        {
            var refreshtoken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshtoken, secret);
            InboxButton.Label = reddit.Account.Messages.Unread.Count.ToString();
        }

 

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SingletonReference = this;
            LoginFrameFrame = LoginFrame;
            MainTab = MainTabView;
            var refreshToken = localSettings.Values["refresh_token"].ToString();
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            PostsSortOrder = "Best";
            IsHomeEnabled = true;
            L = HomeList;
            if (localSettings.Values["refresh_token"].ToString() == "589558656590-c5zKzVmhsgqmJGqobEebBOAQVl8")
            {
                isenabled = false;
                Subreddit.Text = "r/" + "popular";
                GetPostsClass.Subreddit = "popular";

                Bindings.Update();
                IsHomeEnabled = false;
                PostsSortOrder = "Hot";
                SortOrderButton.Label = "Hot";
                GetPostsClass.SortOrder = "Hot";
                var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                HomeList.ItemsSource = Postscollection;
                HomePostList = null;
                SortOrderButton.Visibility = Visibility.Visible;
                LoginButton.Visibility = Visibility.Visible;
                try
                {
                   // GetSubreddit.Load = true;
                  /*  var reddit = new RedditClient(appId, refreshToken, secret);
                    Subreddits = reddit.Account.MySubscribedSubreddits();
                    var SubredditCollection = new List<SubredditList>();
                    await Task.Run(() =>
                    {
                        Parallel.ForEach(Subreddits, subreddit =>
                        {
                            if (subreddit.Over18 == true)
                                Nsfw = Visibility.Visible;
                            else
                                Nsfw = Visibility.Collapsed;
                            // Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
                            SubredditCollection.Add(new SubredditList
                            {
                                IsNSFW = Nsfw,
                                TitleSubreddit = subreddit.Name,
                                SubredditSelf = subreddit,
                                SubredditIcon = subreddit.CommunityIcon
                            });
                        }
                        );
                    });
                    SubscribedSubs.ItemsSource = SubredditCollection;*/
                
                    FindName("SecondPaneGrid");
                    LoadingControl.IsLoading = false;
                }
                catch
                {
                }
            }
            else
            {
                isenabled = true;
                Bindings.Update();
                if (localSettings.Values["refresh_token"].ToString() != "589558656590-c5zKzVmhsgqmJGqobEebBOAQVl8")
                {
                    GetPostsClass.SortOrder = "Best";
                    var PostsCollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                    HomePage.HomePostList = PostsCollection;
                }
  
                var dataTransferManager = DataTransferManager.GetForCurrentView();
                dataTransferManager.DataRequested += DataTransferManager_DataRequested;
                HomeList.ItemsSource = HomePostList;
                LoginButton.Visibility = Visibility.Collapsed;
                /*   var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                  coreTitleBar.ExtendViewIntoTitleBar = true;*/
                try
                {
                    GetSubreddit.Load = true;
                    var reddit = new RedditClient(appId, refreshToken, secret);
                    Subreddits = reddit.Account.MySubscribedSubreddits();
                    var SubredditCollection = new List<Subreddit>();
                    await Task.Run(() =>
                    {
                        Parallel.ForEach(Subreddits, subreddit =>
                        {
                            // Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
                            SubredditCollection.Add(subreddit);
                        }
                        );
                    });
                    SubscribedSubs.ItemsSource = SubredditCollection;
                    LoadingControl.IsLoading = false;
                    FindName("SecondPaneGrid");
                    Expander_Expanded();
                    Expander_Expanded_1();
                    Expander_Expanded_2();
                    Expander_Expanded_3();

                }
                catch
                {
                }


                var uTimer = new Timer();
                uTimer.Elapsed += TimerElapsed;
                uTimer.Interval = 20000;
                uTimer.Enabled = true;
                uTimer.AutoReset = true;
                
            }
        }
      
        public async void startup()
        {
            SingletonReference = this;
            LoginFrameFrame = LoginFrame;
            MainTab = MainTabView;
            var refreshToken = localSettings.Values["refresh_token"].ToString();


            PostsSortOrder = "Best";
            IsHomeEnabled = true;
            L = HomeList;
            var reddit = new RedditClient(appId, refreshToken, secret);
            CurrentSub = reddit.Subreddit("popular");
            if (localSettings.Values["refresh_token"].ToString() == "589558656590-c5zKzVmhsgqmJGqobEebBOAQVl8")
            {
                isenabled = false;
                Subreddit.Text = "r/" + "popular";
                GetPostsClass.Subreddit = "popular";
          
                Bindings.Update();
                IsHomeEnabled = false;
                PostsSortOrder = "Hot";
                SortOrderButton.Label = "Hot";
                GetPostsClass.SortOrder = "Hot";
                var Postscollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                HomeList.ItemsSource = Postscollection;
                HomePostList = null;
                SortOrderButton.Visibility = Visibility.Visible;
                LoginButton.Visibility = Visibility.Visible;
                try
                {
                    // GetSubreddit.Load = true;
                    /*  var reddit = new RedditClient(appId, refreshToken, secret);
                      Subreddits = reddit.Account.MySubscribedSubreddits();
                      var SubredditCollection = new List<SubredditList>();
                      await Task.Run(() =>
                      {
                          Parallel.ForEach(Subreddits, subreddit =>
                          {
                              if (subreddit.Over18 == true)
                                  Nsfw = Visibility.Visible;
                              else
                                  Nsfw = Visibility.Collapsed;
                              // Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
                              SubredditCollection.Add(new SubredditList
                              {
                                  IsNSFW = Nsfw,
                                  TitleSubreddit = subreddit.Name,
                                  SubredditSelf = subreddit,
                                  SubredditIcon = subreddit.CommunityIcon
                              });
                          }
                          );
                      });
                      SubscribedSubs.ItemsSource = SubredditCollection;*/

                    FindName("SecondPaneGrid");
                    LoadingControl.IsLoading = false;
                }
                catch
                {
                }
            }
            else
            {
                isenabled = true;
                Bindings.Update();
                if (localSettings.Values["refresh_token"].ToString() != "589558656590-c5zKzVmhsgqmJGqobEebBOAQVl8")
                {
                    GetPostsClass.SortOrder = "Best";
                    var PostsCollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                    HomePage.HomePostList = PostsCollection;
                }

                var dataTransferManager = DataTransferManager.GetForCurrentView();
                dataTransferManager.DataRequested += DataTransferManager_DataRequested;
                HomeList.ItemsSource = HomePostList;
                LoginButton.Visibility = Visibility.Collapsed;
                /*   var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                  coreTitleBar.ExtendViewIntoTitleBar = true;*/
                try
                {
                    GetSubreddit.Load = true;
                    Subreddits = reddit.Account.MySubscribedSubreddits();
                    List<SubredditList> SubredditCollection = new List<SubredditList>();
                    await Task.Run(() =>
                    {
                        Parallel.ForEach(Subreddits, subreddit =>
                        {
                            // Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
                            SubredditCollection.Add(new SubredditList
                            {
                                TitleSubreddit = subreddit.Name,
                                SubredditSelf = subreddit,
                            });
                        }
                        );
                    });
                    SubscribedSubs.ItemsSource = SubredditCollection;
                    LoadingControl.IsLoading = false;
                    FindName("SecondPaneGrid");
                    Expander_Expanded();
                    Expander_Expanded_1();
                    Expander_Expanded_2();
                    Expander_Expanded_3();

                }
                catch
                {
                }


                var uTimer = new Timer();
                uTimer.Elapsed += TimerElapsed;
                uTimer.Interval = 20000;
                uTimer.Enabled = true;
                uTimer.AutoReset = true;
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            FindName("LoginFrame");
            LoginFrame.Navigate(typeof(LoginPage));
        }

        private void PersonPicture_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
}
