using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;

namespace Eco_Reddit.Helpers
{
    public class GetSubreddit : IIncrementalSource<SubredditList>
    {
        public static bool Load;
        public static User UserToGetPostsFrom;
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private Visibility Nsfw;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        private List<SubredditList> SubredditCollection;
        private IEnumerable<Subreddit> Subreddits;

        public async Task<IEnumerable<SubredditList>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                SubredditCollection = new List<SubredditList>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                if (Load)
                {
                    Subreddits = reddit.Account.MySubscribedSubreddits();
                    await Task.Run(() =>
                    {
                        foreach (var subreddit in Subreddits)
                        {
                            if (subreddit.Over18 == true)
                                Nsfw = Visibility.Visible;
                            else
                                Nsfw = Visibility.Collapsed;
                            // Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
                            SubredditCollection.Add(new SubredditList
                            {
                                IsNSFW = Nsfw,
                                TitleSubreddit = subreddit.Name,
                                SubredditSelf = subreddit,
                                SubredditIcon = subreddit.CommunityIcon
                            });
                        }
                    });
                }

                Load = false;
                // Simulates a longer request...
            });


            return SubredditCollection;
        }
    }
}
