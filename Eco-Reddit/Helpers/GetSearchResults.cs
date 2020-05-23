using Eco_Reddit.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Eco_Reddit.Helpers
{
    public class GetSearchResults : IIncrementalSource<Posts>
    {
        public static int limit = 10;
        public static int skipInt = 0;
        public static string SearchSort { get; set; }
        public static string TimeSort { get; set; }
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        List<Posts> ResultsCollection;
        public static string Sub { get; set; }
        public string thing;
        public static string Input { get; set; }
        public async Task<IEnumerable<Posts>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(async () =>
            {
                  Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        string refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                ResultsCollection = new List<Posts>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                //  posts = GenericUriParser;
                var LinkPostType = Visibility.Collapsed;
                string ImageUrl = "e";
                string Nsfw = "";

                IEnumerable<Post> SearchResultsSearch = reddit.Subreddit(Sub).Search(new SearchGetSearchInput(Input, limit: 25, after: thing, sort: SearchSort, t: TimeSort));
                /* if (SearchResultsSearch.Count == 0)
                 {
                     SearchResultsSearch = reddit.Subreddit("all").Search(new SearchGetSearchInput(Input, limit: limit, sort: "top")).Skip(skipInt);  // Search r/all
                 }*/
              //  limit = limit + 10;
                await Task.Run(() =>
                {

                    foreach (Post post in SearchResultsSearch)
                    {
                        // Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
                        ResultsCollection.Add(new Posts()
                        {
                            PostSelf = post,
                        });
                        thing = post.Fullname;
                    }
                });
                // Simulates a longer request...
                // skipInt = skipInt + 10;

                return ResultsCollection;

            });
            return ResultsCollection;
        }
    }
}
