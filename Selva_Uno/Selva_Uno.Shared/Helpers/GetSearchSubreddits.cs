using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Selva.Core.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;

namespace Selva.Helpers
{
    public class GetSearchSubreddits : IIncrementalSource<Subreddits>
    {
        public static int limit = 10;
        public static string SearchSort;
        public static int skipInt;
        public static string Input;
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        private List<Subreddits> SubredditCollection;
        private IEnumerable<Subreddit> Subreddits;
        public string thing;

        public async Task<IEnumerable<Subreddits>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                SubredditCollection = new List<Subreddits>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                IEnumerable<Subreddit> SearchResultsSearch =
                    reddit.SearchSubreddits(limit: 25, query: Input, after: thing, sort: SearchSort);
                limit = limit + 10;
                await Task.Run(() =>
                {
                    foreach (var Subreddit in SearchResultsSearch)
                    {
                        SubredditCollection.Add(new Subreddits
                        {
                            SubredditSelf = Subreddit
                        });
                        thing = Subreddit.Fullname;
                    }
                });
                // Simulates a longer request...
                skipInt = skipInt + 10;
            });
            return SubredditCollection;
        }
    }
}
