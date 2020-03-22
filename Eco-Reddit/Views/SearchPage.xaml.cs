using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Uwp;
using Reddit;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using WinUI = Microsoft.UI.Xaml.Controls;
namespace Eco_Reddit.Views
{
    public sealed partial class SearchPage : Page, INotifyPropertyChanged
    {
        public static string SearchString { get; set; }
        public static string Subreddit { get; set; }
        string LocalSearchString { get; set; }
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public SearchPage()
        {
            InitializeComponent();
            LocalSearchString = SearchString;
            SearchString = null;
            TextSearched.Text = "Search results for: " + LocalSearchString + " in r/" + Subreddit;
            GetSearchResults.Input = LocalSearchString;
            GetSearchResults.Sub = Subreddit;
            GetSearchResults.limit = 10;
            GetSearchResults.skipInt = 0;
            var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Posts>();
            SearchList.ItemsSource = Postscollection;
            Subreddit = null;
            //  UnloadObject(HomePage.L);
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
        private async void HideButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as Posts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            await post.HideAsync();
        }
        private void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Posts P = e.ClickedItem as Posts;
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
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement)sender;
            var DataContext = SenderFramework.DataContext;
            var SenderPost = DataContext as Posts;
            Reddit.Controllers.Post post = SenderPost.PostSelf;
            await post.SaveAsync("");
        }
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
