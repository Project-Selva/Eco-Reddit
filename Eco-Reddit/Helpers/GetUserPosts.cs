using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;

namespace Eco_Reddit.Helpers
{
    public class GetUserPostsClass : IIncrementalSource<Posts>
    {
        public static int limit = 10;
        public static int skipInt = 0;
        public static User UserToGetPostsFrom;
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private List<Posts> PostCollection;
        private IEnumerable<Post> posts;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public string thing;

        public async Task<IEnumerable<Posts>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                PostCollection = new List<Posts>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                //   limit = limit + 10;
                //posts = UserToGetPostsFrom.GetPostHistory(limit: limit).Skip(skipInt);
                posts = UserToGetPostsFrom.GetPostHistory(limit: 10, after: thing);
                await Task.Run(() =>
                {
                    foreach (var post in posts)
                    {
                        PostCollection.Add(new Posts
                        {
                            PostSelf = post
                        });
                        thing = post.Fullname;
                    }
                });

                // Simulates a longer request...
                //  skipInt = skipInt + 10;
            });
            return PostCollection;
        }
    }
}
