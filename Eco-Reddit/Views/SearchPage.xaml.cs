using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Uwp;
using Reddit;
using Reddit.Controllers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public SearchPage()
        {
            InitializeComponent();
            LocalSearchString = SearchString;
            Sub = Subreddit;
            SearchString = null;
            TextSearched.Text = "Search results for: " + LocalSearchString + " in r/" + Subreddit;
            Search.Text = LocalSearchString;
            GetSearchResults.Input = LocalSearchString;
            GetSearchResults.Sub = Subreddit;
            GetSearchResults.limit = 10;
            GetSearchResults.TimeSort = "all";
            GetSearchResults.SearchSort = "relevance";
            GetSearchResults.skipInt = 0;
            var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Posts>();
            SearchList.ItemsSource = Postscollection;
            Subreddit = null;
            //  UnloadObject(HomePage.L);
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

            Posts SenderPost = args.Item as Eco_Reddit.Models.Posts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var textBlock = templateRoot.Children[5] as HyperlinkButton;
            var AuthorBlock = templateRoot.Children[4] as HyperlinkButton;
            var TextDateBlock = templateRoot.Children[3] as TextBlock;
            var TextFlairBlock = templateRoot.Children[6] as TextBlock;
            var TextTitleBlock = templateRoot.Children[2] as TextBlock;
            TextTitleBlock.Text = post.Title;
            textBlock.Content = post.Subreddit;
            AuthorBlock.Content = post.Author;
            TextDateBlock.Text = "Created: " + post.Created;
            TextFlairBlock.Text = "    Flair: " + post.Listing.LinkFlairText;

            ///   TextFlairBlock.Foreground = post.Listing.LinkFlairBackgroundColor;

            args.RegisterUpdateCallback(this.ShowPhase2);
        }
    
        private void ShowPhase2(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 2)
            {
                throw new System.Exception("We should be in phase 2, but we are not.");
            }
            Posts SenderPost = args.Item as Eco_Reddit.Models.Posts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var img = templateRoot.Children[8] as Image;
            var Upvoted = templateRoot.Children[0] as AppBarToggleButton;
            var Downvoted = templateRoot.Children[1] as AppBarToggleButton;
            Upvoted.Label = post.UpVotes.ToString();
            Upvoted.IsChecked = post.IsUpvoted;
            Downvoted.IsChecked = post.IsDownvoted;
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
            TextSearched.Text = "Search results for: " + LocalSearchString + " in r/" + Subreddit;
            Search.Text = LocalSearchString;
            GetSearchResults.Input = LocalSearchString;
            GetSearchResults.Sub = Subreddit;
            GetSearchResults.limit = 10;
            GetSearchResults.TimeSort = TimeBox.SelectedItem.ToString();
            GetSearchResults.SearchSort = SortBox.SelectedItem.ToString();
            GetSearchResults.skipInt = 0;
            var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Posts>();
            SearchList.ItemsSource = Postscollection;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            TextSearched.Text = "Search results for: " + LocalSearchString + " in r/" + Sub;
            Search.Text = LocalSearchString;
            GetSearchResults.Input = LocalSearchString;
            GetSearchResults.Sub = Sub;
            GetSearchResults.limit = 10;
            GetSearchResults.TimeSort = TimeBox.SelectedItem.ToString();
            var m = new MessageDialog(TimeBox.SelectedItem.ToString());
            m.ShowAsync();
            GetSearchResults.SearchSort = SortBox.SelectedItem.ToString();
            GetSearchResults.skipInt = 0;
            var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Posts>();
            SearchList.ItemsSource = Postscollection;
        }
    }
}
