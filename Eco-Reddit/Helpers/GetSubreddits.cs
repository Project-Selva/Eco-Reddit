using Eco_Reddit.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Eco_Reddit.Helpers
{
    public class GetSubreddit : IIncrementalSource<SubredditList>
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        List<SubredditList> SubredditCollection;
        private IEnumerable<Subreddit> Subreddits;
        public static bool Load { get; set; }
        Visibility Nsfw;
        public static User UserToGetPostsFrom { get; set; }
        public async Task<IEnumerable<SubredditList>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(async () =>
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                SubredditCollection = new List<SubredditList>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                if(Load == true)
                {
                Subreddits = reddit.Account.MySubscribedSubreddits();
                await Task.Run(() =>
                {
                    foreach (Subreddit subreddit in Subreddits)
                    {
                        if (subreddit.Over18 == true)
                        {
                            Nsfw = Visibility.Visible;
                        }
                        else
                        {
                            Nsfw = Visibility.Collapsed;
                        }
                        // Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
                        SubredditCollection.Add(new SubredditList()
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
