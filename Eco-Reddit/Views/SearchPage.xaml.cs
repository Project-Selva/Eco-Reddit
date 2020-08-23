using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Eco_Reddit.Helpers;
using Eco_Reddit.Core.Models;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Newtonsoft.Json;
using Reddit;
using Reddit.Controllers;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Eco_Reddit.Views
{
    public sealed partial class SearchPage : Page, INotifyPropertyChanged
    {
        public static string SearchString;
        public static string Subreddit;
        public string appId = "mp8hDB_HfbctBg";
        private string LocalSearchString;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private readonly string ogsub;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        private Post SharePost;
        public string Sub;
        private List<string> UserList;

        public SearchPage()
        {
            InitializeComponent();
            LocalSearchString = SearchString;
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            Sub = Subreddit;
            SearchString = null;
            TextSearched.Text = "Search:";
            // Search.Text = LocalSearchString;
            // GetSearchResults.Input = LocalSearchString;
            if (Subreddit == "all")
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
            //  var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Post>();
            //  SearchList.ItemsSource = Postscollection;
            Subreddit = null;
            LoadingControl.IsLoading = false;
            //  UnloadObject(HomePage.L);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetText("https://www.reddit.com/r/" + SharePost.Subreddit + "/comments/" + SharePost.Id);
            request.Data.Properties.Title = SharePost.Title;
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement) sender);
        }


        private void Subreddit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement) sender);
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (SearchList.ItemTemplate == POST)
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

        private void HomeList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 0) throw new Exception("We should be in phase 0, but we are not.");

            // It's phase 0, so this item's title will already be bound and displayed.

            args.RegisterUpdateCallback(ShowPhase1);

            args.Handled = true;
        }

        private async void ShowPhase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1) throw new Exception("We should be in phase 1, but we are not.");
            if (SearchList.ItemTemplate == POST)
            {
              
            }
            else if (SearchList.ItemTemplate == SubredditTemplate)
            {
                var SenderPost = args.Item as Subreddits;
                var post = SenderPost.SubredditSelf;
                var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
                var TextTitleBlock = templateRoot.Children[1] as TextBlock;
                var TextDTitleBlock = templateRoot.Children[2] as TextBlock;
                TextTitleBlock.Text = "r/" + post.Name;
                TextDTitleBlock.Text = post.Title;
            }
            else
            {
                var SenderPost = args.Item as Users;
                var post = SenderPost.UserSelf;
                var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
                var TextTitleBlock = templateRoot.Children[1] as TextBlock;
                TextTitleBlock.Text = "u/" + post.Name;
            }

            ///   TextFlairBlock.Foreground = post.Listing.LinkFlairBackgroundColor;

            //        args.RegisterUpdateCallback(this.ShowPhase2);
        }

        private async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (nvSearch.SelectedItem == Posts)
            {
                if (string.IsNullOrEmpty(Search.Text) == false)
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
                    var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Post>();
                    SearchList.ItemsSource = Postscollection;
                }

                //   contentFrame.Navigate(typeof(SearchPosts));
            }
            else if (nvSearch.SelectedItem == Subreddits)
            {
                if (string.IsNullOrEmpty(Search.Text) == false)
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
            }
            else
            {
                if (string.IsNullOrEmpty(Search.Text) == false)
                    try
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
                    catch
                    {
                        var M = new MessageDialog("DID NOT FIND");
                        await M.ShowAsync();
                    }
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (nvSearch.SelectedItem == Posts)
            {
                if (string.IsNullOrEmpty(Search.Text) == false)
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
                    var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Post>();
                    SearchList.ItemsSource = Postscollection;
                }

                //  contentFrame.Navigate(typeof(SearchPosts));
            }
            else if (nvSearch.SelectedItem == Subreddits)
            {
                if (string.IsNullOrEmpty(Search.Text) == false)
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
            }
            else
            {
                if (string.IsNullOrEmpty(Search.Text) == false)
                    try
                    {
                        /*  TextSearched.Text = "User search results for: " + Search.Text;
                          Search.Text = Search.Text;
                          GetSearchUsers.Input = Search.Text;
                          GetSearchUsers.limit = 1;
                          GetSearchUsers.SearchSort = SortBox.SelectedItem.ToString();
                          GetSearchUsers.skipInt = 0;
                          SearchList.ItemTemplate = UserTemplate;
                          var Subredditscollection = new IncrementalLoadingCollection<GetSearchUsers, Users>();
                          SearchList.ItemsSource = Subredditscollection;*/
                        var client = new HttpClient();
                        var response =
                            await client.GetAsync(
                                new Uri("https://www.reddit.com/users/search.json?q=firecubestudios"));

                        var json = await response.Content.ReadAsStringAsync();
                        var UserList = JsonConvert.DeserializeObject<UsersClass>(json);
                        var M = new MessageDialog(json);
                        await M.ShowAsync();
                        // SearchList.ItemsSource = UserList;
                        var Ms = new MessageDialog(UserList.data.children.data.name);
                        await Ms.ShowAsync();
                    }
                    catch
                    {
                        var M = new MessageDialog("DID NOT FIND");
                        await M.ShowAsync();
                    }
            }
        }

        private async void NvSearch_SelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (nvSearch.SelectedItem == Posts)
            {
                PostSearchPanel.Visibility = Visibility.Visible;
                if (string.IsNullOrEmpty(Search.Text) == false)
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
                    var Postscollection = new IncrementalLoadingCollection<GetSearchResults, Post>();
                    SearchList.ItemsSource = Postscollection;
                }

                //  contentFrame.Navigate(typeof(SearchPosts));
            }
            else if (nvSearch.SelectedItem == Subreddits)
            {
                PostSearchPanel.Visibility = Visibility.Collapsed;
                if (string.IsNullOrEmpty(Search.Text) == false)
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
            }
            else
            {
                PostSearchPanel.Visibility = Visibility.Collapsed;
                if (string.IsNullOrEmpty(Search.Text) == false)
                    try
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
                    catch
                    {
                        var M = new MessageDialog("DID NOT FIND");
                        await M.ShowAsync();
                    }
            }
        }

        private void OtherSubSearch_Checked(object sender, RoutedEventArgs e)
        {
            OtherSubText.Visibility = Visibility.Visible;
            if (OtherSubText.Text != null) Sub = OtherSubText.Text;
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

        public class UsersClass
        {
            public dataClass data;
        }

        public class dataClass
        {
            public ChildClass children;
        }

        public class DataChildClass
        {
            public string name;
        }

        public class ChildClass
        {
            public DataChildClass data;
        }
    }
}
