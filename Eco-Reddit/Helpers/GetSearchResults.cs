using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;

namespace Eco_Reddit.Helpers
{
    public class GetSearchResults : IIncrementalSource<Posts>
    {
        public static int limit = 10;
        public static int skipInt = 0;
        public static string SearchSort;
        public static string TimeSort;
        public static string Sub;
        public static string Input;
        public string appId = "mp8hDB_HfbctBg";
        private List<Posts> ResultsCollection;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public string thing;

        public async Task<IEnumerable<Posts>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                ResultsCollection = new List<Posts>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                IEnumerable<Post> SearchResultsSearch = reddit.Subreddit(Sub)
                    .Search(new SearchGetSearchInput(Input, limit: 10, after: thing, sort: SearchSort, t: TimeSort));

                await Task.Run(() =>
                {
                    foreach (var post in SearchResultsSearch)
                    {
                        // Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
                        ResultsCollection.Add(new Posts
                        {
                            PostSelf = post
                        });
                        thing = post.Fullname;
                    }
                });

                return ResultsCollection;
            });
            return ResultsCollection;
        }
    }
}
