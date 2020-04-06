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

namespace Eco_Reddit.Helpers
{
    public class GetSearchSubreddits : IIncrementalSource<Models.Subreddits>
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static int limit = 10;
        public static string SearchSort { get; set; }
        public static int skipInt = 0;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        List<Models.Subreddits> SubredditCollection;
        private IEnumerable<Subreddit> Subreddits;
        public static string Input { get; set; }

        public async Task<IEnumerable<Models.Subreddits>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(async () =>
            {

                string refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                SubredditCollection = new List<Models.Subreddits>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                IEnumerable<Subreddit> SearchResultsSearch = reddit.SearchSubreddits(limit: limit, query: Input, sort: SearchSort);
                  limit = limit + 10;
                await Task.Run(() =>
                {
                    foreach (Subreddit Subreddit in SearchResultsSearch)
                    {
                        SubredditCollection.Add(new Models.Subreddits()
                        {
                            SubredditSelf = Subreddit,
                        });
                    }
                });
                // Simulates a longer request...
                skipInt = skipInt + 10;

            });
            return SubredditCollection;
        }

    }
}
