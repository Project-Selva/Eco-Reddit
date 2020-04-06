using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
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
    public sealed partial class SearchSubredditPage : Page, INotifyPropertyChanged
    {
        public static string SearchString { get; set; }
        public string Sub;
        string LocalSearchString { get; set; }
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
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
            Subreddits SubredditLocal = e.ClickedItem as Subreddits;
            HomePage.SingletonReference.NavigateJumper(SubredditLocal.SubredditSelf.Name);
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

            Subreddits SenderPost = args.Item as Eco_Reddit.Models.Subreddits;
            Reddit.Controllers.Subreddit post = SenderPost.SubredditSelf;
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var TextTitleBlock = templateRoot.Children[1] as TextBlock;
            var TextDTitleBlock = templateRoot.Children[2] as TextBlock;
            TextTitleBlock.Text = "r/" + post.Name;
            TextDTitleBlock.Text = post.Title;
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
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
            AppBarButton AppBarButtonObject = (AppBarButton)sender;
            Subreddit SubredditLocal = (AppBarButtonObject).Tag as Subreddit;
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/" + SubredditLocal.Name));
        }
    }
}
