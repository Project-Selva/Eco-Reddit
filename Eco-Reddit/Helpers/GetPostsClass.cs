using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs;

namespace Eco_Reddit.Helpers
{
    public class GetPostsClass : IIncrementalSource<Post>
    {
        public static string Subreddit;
        public static string SortOrder;
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private List<Post> PostCollection;
        private IEnumerable<Post> posts;
        int counter = 0;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public string thing;

        public async Task<IEnumerable<Post>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                PostCollection = new List<Post>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var subreddit = reddit.Subreddit(Subreddit);
                switch (SortOrder)
                {
                    case "all":
                        posts = subreddit.Posts.GetTop(new TimedCatSrListingInput("all", limit: 50, after: thing));

                        break;
                    case "year":
                        posts = subreddit.Posts.GetTop(new TimedCatSrListingInput("year", limit: 50, after: thing));

                        break;
                    case "month":
                        posts = subreddit.Posts.GetTop(new TimedCatSrListingInput("month", limit: 50, after: thing));

                        break;
                    case "week":
                        posts = subreddit.Posts.GetTop(new TimedCatSrListingInput("week", limit: 50, after: thing));

                        break;
                    case "day":
                        posts = subreddit.Posts.GetTop(new TimedCatSrListingInput("day", limit: 50, after: thing));

                        break;
                    case "Hot":
                        posts = subreddit.Posts.GetHot(limit: 50, after: thing);

                        break;
                    case "New":
                        posts = subreddit.Posts.GetNew(limit: 50, after: thing);

                        break;
                    case "Best":
                        posts = subreddit.Posts.GetBest(limit: 50, after: thing);

                        break;
                    case "Rising":
                        posts = subreddit.Posts.GetRising(limit: 50, after: thing);

                        break;
                    case "Controversial":
                        posts = subreddit.Posts.GetControversial(limit: 50, after: thing);

                        break;
                }

                await Task.Run(() =>
                {
                    Parallel.ForEach(posts, post =>
                        {
                            PostCollection.Add(post);
                            thing = post.Fullname;
                            counter++;
                            if(counter == 50)
                            {
                                counter = 0;
                                         thing = post.Fullname;
                            }
                        }
                    );
                });
                // Simulates a longer request...
            });
            return PostCollection;
        }
    }
}
