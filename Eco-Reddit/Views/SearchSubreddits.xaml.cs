using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Uwp;
using Reddit.Controllers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchSubredditPage : Page, INotifyPropertyChanged
    {
        public static string SearchString;
        public string appId = "mp8hDB_HfbctBg";
        private readonly string LocalSearchString;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public string Sub;

        public SearchSubredditPage()
        {
            InitializeComponent();
            LocalSearchString = SearchString;
            SearchString = null;
            TextSearched.Text = "Subreddit search results for: " + LocalSearchString;
            Search.Text = LocalSearchString;
            GetSearchSubreddits.Input = LocalSearchString;
            GetSearchSubreddits.limit = 10;
            GetSearchSubreddits.SearchSort = "relevance";
            GetSearchSubreddits.skipInt = 0;
            var Subredditscollection = new IncrementalLoadingCollection<GetSearchSubreddits, Subreddits>();
            SearchList.ItemsSource = Subredditscollection;
            //  UnloadObject(HomePage.L);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var SubredditLocal = e.ClickedItem as Subreddits;
            HomePage.SingletonReference.NavigateJumper(SubredditLocal.SubredditSelf.Name);
        }

        private void HomeList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 0) throw new Exception("We should be in phase 0, but we are not.");

            // It's phase 0, so this item's title will already be bound and displayed.

            args.RegisterUpdateCallback(ShowPhase1);

            args.Handled = true;
        }

        private void ShowPhase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1) throw new Exception("We should be in phase 1, but we are not.");

            var SenderPost = args.Item as Subreddits;
            var post = SenderPost.SubredditSelf;
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var TextTitleBlock = templateRoot.Children[1] as TextBlock;
            var TextDTitleBlock = templateRoot.Children[2] as TextBlock;
            TextTitleBlock.Text = "r/" + post.Name;
            TextDTitleBlock.Text = post.Title;
        }

        private async void SubEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var SubredditLocal = AppBarButtonObject.Tag as Subreddit;
            await SubredditLocal.SubscribeAsync();
        }

        private async void unsubEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var SubredditLocal = AppBarButtonObject.Tag as Subreddit;
            await SubredditLocal.UnsubscribeAsync();
        }

        private void PermaLinkSubredditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var SubredditLocal = AppBarButtonObject.Tag as Subreddit;
            var dataPackage = new DataPackage();
            dataPackage.SetText("https://www.reddit.com/r/" + SubredditLocal.Title);
            Clipboard.SetContent(dataPackage);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            TextSearched.Text = "Subreddit search results for: " + args.QueryText;
            Search.Text = args.QueryText;
            GetSearchSubreddits.Input = sender.Text;
            GetSearchSubreddits.limit = 10;
            GetSearchSubreddits.SearchSort = SortBox.SelectedItem.ToString();
            GetSearchSubreddits.skipInt = 0;
            var Subredditscollection = new IncrementalLoadingCollection<GetSearchSubreddits, Subreddits>();
            SearchList.ItemsSource = Subredditscollection;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            TextSearched.Text = "Subreddit search results for: " + Search.Text;
            Search.Text = Search.Text;
            GetSearchSubreddits.Input = Search.Text;
            GetSearchSubreddits.limit = 10;
            GetSearchSubreddits.SearchSort = SortBox.SelectedItem.ToString();
            GetSearchSubreddits.skipInt = 0;
            var Subredditscollection = new IncrementalLoadingCollection<GetSearchSubreddits, Subreddits>();
            SearchList.ItemsSource = Subredditscollection;
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var SubredditLocal = AppBarButtonObject.Tag as Subreddit;
            await Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/" + SubredditLocal.Name));
        }
    }
}
