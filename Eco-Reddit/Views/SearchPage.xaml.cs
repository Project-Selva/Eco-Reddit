using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Uwp;
using Reddit;
using Reddit.Controllers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using WinUI = Microsoft.UI.Xaml.Controls;
namespace Eco_Reddit.Views
{
    public sealed partial class SearchPage : Page, INotifyPropertyChanged
    {
        public static string SearchString { get; set; }
        public static string Subreddit { get; set; }
        public string Sub;
        string LocalSearchString { get; set; }
        string ogsub;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public SearchPage()
        {
            InitializeComponent();
            LocalSearchString = SearchString;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            Sub = Subreddit;
            SearchString = null;
            TextSearched.Text = "Search:";
            // Search.Text = LocalSearchString;
            // GetSearchResults.Input = LocalSearchString;
            if(Subreddit == "all")
            {
                MainSubSearch.Visibility = Visibility.Collapsed;
                AllSearch.IsChecked = true;
            }
            else
            {
                MainSubSearch.Content = "r/" + Subreddit;
                MainSubSearch.IsChecked = true;
                ogsub = Subreddit;
            }
            GetSearchResults.Sub = Subreddit;
            GetSearchResults.limit = 10;
            GetSearchResults.TimeSort = "all";
            GetSearchResults.SearchSort = "relevance";
            GetSearchResults.skipInt = 0;
            nvSearch.SelectedItem = Posts;
            //  var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Posts>();
            //  SearchList.ItemsSource = Postscollection;
            Subreddit = null;
            //  UnloadObject(HomePage.L);
        }
        Post SharePost;
        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.SetText("https://www.reddit.com/r/" + SharePost.Subreddit + "/comments/" + SharePost.Id);
            request.Data.Properties.Title = SharePost.Title;
        }

        private void HyperlinkButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }


        private void Subreddit_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
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

        private void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Posts P = e.ClickedItem as Posts;
            WinUI.TabViewItem newTab = HomePage.MainTab.SelectedItem as WinUI.TabViewItem;
            HomePage.MainTab.TabItems.Remove(newTab);
            var SelectedView = new WinUI.TabViewItem();
            SelectedView.Header = P.PostSelf.Title;
            Frame frame = new Frame();
            SelectedView.Content = frame;
            PostContentPage.Post = P.PostSelf;
            frame.Navigate(typeof(PostContentPage));
            HomePage.MainTab.TabItems.Add(SelectedView);
            HomePage.MainTab.SelectedItem = SelectedView;
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

        private void ShowPhase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1)
            {
                throw new System.Exception("We should be in phase 1, but we are not.");
            }
            if (SearchList.ItemTemplate == POST)
            {
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
            }
            else if(SearchList.ItemTemplate == SubredditTemplate)
            {
                Subreddits SenderPost = args.Item as Eco_Reddit.Models.Subreddits;
                Reddit.Controllers.Subreddit post = SenderPost.SubredditSelf;
                var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
                var TextTitleBlock = templateRoot.Children[1] as TextBlock;
                var TextDTitleBlock = templateRoot.Children[2] as TextBlock;
                TextTitleBlock.Text = "r/" + post.Name;
                TextDTitleBlock.Text = post.Title;

            }
            else
            {
                Users SenderPost = args.Item as Eco_Reddit.Models.Users;
                Reddit.Controllers.User post = SenderPost.UserSelf;
                var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
                var TextTitleBlock = templateRoot.Children[1] as TextBlock;
                TextTitleBlock.Text = "u/" + post.Name;
            }

            ///   TextFlairBlock.Foreground = post.Listing.LinkFlairBackgroundColor;

    //        args.RegisterUpdateCallback(this.ShowPhase2);
        }
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Subreddit SubredditLocal = (AppBarButtonObject).Tag as Subreddit;
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/" + SubredditLocal.Name));
        }
        private async void SubEditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Subreddit SubredditLocal = (AppBarButtonObject).Tag as Subreddit;
            await SubredditLocal.SubscribeAsync();
        }
        private async void unsubEditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Subreddit SubredditLocal = (AppBarButtonObject).Tag as Subreddit;
            await SubredditLocal.UnsubscribeAsync();
        }
        private async void PermaLinkSubredditButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Subreddit SubredditLocal = (AppBarButtonObject).Tag as Subreddit;
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText("https://www.reddit.com/r/" + SubredditLocal.Title);
            Clipboard.SetContent(dataPackage);
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
        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            SharePost = (AppBarButtonObject).Tag as Post;
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
        private void ShowPhase2(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 2)
            {
                throw new System.Exception("We should be in phase 2, but we are not.");
            }
           
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
            HomePage.MainTab.TabItems.Add(newTab);
            // }
        }

        private void ClearTabButton_Click(object sender, RoutedEventArgs e)
        {
            HomePage.MainTab.TabItems.Clear();
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
        private async void OpenPostInWebButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton AppBarButtonWEB = (AppBarButton)sender;
            Post pp = (AppBarButtonWEB).Tag as Post;
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/" + pp.Subreddit + "/comments/" + pp.Id));
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
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (nvSearch.SelectedItem == Posts)
            {
                TextSearched.Text = "Search results for: " + Search.Text + " in r/" + Sub;
                Search.Text = Search.Text;
                GetSearchResults.Input = Search.Text;
                GetSearchResults.Sub = Sub;
                GetSearchResults.limit = 10;
                GetSearchResults.TimeSort = TimeBox.SelectedItem.ToString();
                GetSearchResults.SearchSort = SortBox.SelectedItem.ToString();
                GetSearchResults.skipInt = 0;
                SearchList.ItemTemplate = POST;
                var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Posts>();
                SearchList.ItemsSource = Postscollection;
                //   contentFrame.Navigate(typeof(SearchPosts));
            }
            else if (nvSearch.SelectedItem == Subreddits)
            {
                TextSearched.Text = "Subreddit search results for: " + Search.Text;
                Search.Text = Search.Text;
                GetSearchSubreddits.Input = Search.Text;
                GetSearchSubreddits.limit = 10;
                GetSearchSubreddits.SearchSort = SortBox.SelectedItem.ToString();
                GetSearchSubreddits.skipInt = 0;
                SearchList.ItemTemplate = SubredditTemplate;
                var Subredditscollection = new IncrementalLoadingCollection<GetSearchSubreddits, Subreddits>();
                SearchList.ItemsSource = Subredditscollection;
            }
            else
            {
                TextSearched.Text = "User search results for: " + Search.Text;
                Search.Text = Search.Text;
                GetSearchUsers.Input = Search.Text;
                GetSearchUsers.limit = 1;
                GetSearchUsers.SearchSort = SortBox.SelectedItem.ToString();
                GetSearchUsers.skipInt = 0;
                SearchList.ItemTemplate = UserTemplate;
                var Subredditscollection = new IncrementalLoadingCollection<GetSearchUsers, Users>();
                SearchList.ItemsSource = Subredditscollection;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (nvSearch.SelectedItem == Posts)
            {
                TextSearched.Text = "Search results for: " + Search.Text + " in r/" + Sub;
                Search.Text = Search.Text;
                GetSearchResults.Input = Search.Text;
                GetSearchResults.Sub = Sub;
                GetSearchResults.limit = 10;
                GetSearchResults.TimeSort = TimeBox.SelectedItem.ToString();
                GetSearchResults.SearchSort = SortBox.SelectedItem.ToString();
                GetSearchResults.skipInt = 0;
                SearchList.ItemTemplate = POST;
                var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Posts>();
                SearchList.ItemsSource = Postscollection;
                //  contentFrame.Navigate(typeof(SearchPosts));
            }
            else if(nvSearch.SelectedItem == Subreddits)
            {
                TextSearched.Text = "Subreddit search results for: " + Search.Text;
                Search.Text = Search.Text;
                GetSearchSubreddits.Input = Search.Text;
                GetSearchSubreddits.limit = 10;
                GetSearchSubreddits.SearchSort = SortBox.SelectedItem.ToString();
                GetSearchSubreddits.skipInt = 0;
                SearchList.ItemTemplate = SubredditTemplate;
                var Subredditscollection = new IncrementalLoadingCollection<GetSearchSubreddits, Subreddits>();
                SearchList.ItemsSource = Subredditscollection;
            }
            else
            {
                TextSearched.Text = "User search results for: " + Search.Text;
                Search.Text = Search.Text;
                GetSearchUsers.Input = Search.Text;
                GetSearchUsers.limit = 1;
                GetSearchUsers.SearchSort = SortBox.SelectedItem.ToString();
                GetSearchUsers.skipInt = 0;
                SearchList.ItemTemplate = UserTemplate;
                var Subredditscollection = new IncrementalLoadingCollection<GetSearchUsers, Users>();
                SearchList.ItemsSource = Subredditscollection;
            }
        }

        private void NvSearch_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (nvSearch.SelectedItem == Posts)
            {
                PostSearchPanel.Visibility = Visibility.Visible;
                TextSearched.Text = "Search results for: " + Search.Text + " in r/" + Sub;
                Search.Text = Search.Text;
                GetSearchResults.Input = Search.Text;
                GetSearchResults.Sub = Sub;
                GetSearchResults.limit = 10;
                GetSearchResults.TimeSort = TimeBox.SelectedItem.ToString();
                GetSearchResults.SearchSort = SortBox.SelectedItem.ToString();
                GetSearchResults.skipInt = 0;
                SearchList.ItemTemplate = POST;
                var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Posts>();
                SearchList.ItemsSource = Postscollection;
                //  contentFrame.Navigate(typeof(SearchPosts));
            }
            else if (nvSearch.SelectedItem == Subreddits)
            {
                PostSearchPanel.Visibility = Visibility.Collapsed;
                TextSearched.Text = "Subreddit search results for: " + Search.Text;
                Search.Text = Search.Text;
                GetSearchSubreddits.Input = Search.Text;
                GetSearchSubreddits.limit = 10;
                GetSearchSubreddits.SearchSort = SortBox.SelectedItem.ToString();
                GetSearchSubreddits.skipInt = 0;
                SearchList.ItemTemplate = SubredditTemplate;
                var Subredditscollection = new IncrementalLoadingCollection<GetSearchSubreddits, Subreddits>();
                SearchList.ItemsSource = Subredditscollection;
            }
            else
            {
                PostSearchPanel.Visibility = Visibility.Collapsed;
                TextSearched.Text = "User search results for: " + Search.Text;
                Search.Text = Search.Text;
                GetSearchUsers.Input = Search.Text;
                GetSearchUsers.limit = 1;
                GetSearchUsers.SearchSort = SortBox.SelectedItem.ToString();
                GetSearchUsers.skipInt = 0;
                SearchList.ItemTemplate = UserTemplate;
                var Subredditscollection = new IncrementalLoadingCollection<GetSearchUsers, Users>();
                SearchList.ItemsSource = Subredditscollection;
            }
        }

        private void OtherSubSearch_Checked(object sender, RoutedEventArgs e)
        {
            OtherSubText.Visibility = Visibility.Visible;
            if(OtherSubText.Text != null)
            {
                Sub = OtherSubText.Text;
            }
        }

        private void AllSearch_Checked(object sender, RoutedEventArgs e)
        {
            OtherSubText.Visibility = Visibility.Collapsed;
            Sub = "all";
        }

        private void MainSubSearch_Checked(object sender, RoutedEventArgs e)
        {
            Sub = ogsub;
        }

        private void OtherSubText_TextChanged(object sender, TextChangedEventArgs e)
        {
            Sub = OtherSubText.Text;
        }
    }
}
