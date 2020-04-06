using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchHubPage : Page
    {
        public static string SearchString { get; set; }
        public static string Subreddit { get; set; }
        string localsearch;
        string localsub;
        public SearchHubPage()
        {
            InitializeComponent();
            SearchPage.SearchString = SearchString;
            SearchPage.Subreddit = Subreddit;
            localsearch = SearchString;
            localsub = Subreddit;
            PostsFrame.Navigate(typeof(SearchPage));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Pivot PivotSearch = sender as Pivot;
                PivotItem Selecteditem = sender as PivotItem;
                switch (Selecteditem.Header)
                {
                    case "Posts":
                        SearchPage.SearchString = localsearch;
                        SearchPage.Subreddit = localsub;
                        PostsFrame.Navigate(typeof(SearchPage));
                        break;
                    case "Subreddits":
                        SubredditsFrame.Navigate(typeof(SearchPage));
                        break;
                    case "Comments":
                        CommentsFrame.Navigate(typeof(SearchPage));
                        break;
                    case "Mixed":
                        MixedFrame.Navigate(typeof(SearchPage));
                        break;
                }
            }
            catch
            {

            }
        }
    }
}
