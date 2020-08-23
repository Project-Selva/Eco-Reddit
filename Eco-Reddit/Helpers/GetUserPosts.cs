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
    public class GetUserPostsClass : IIncrementalSource<Post>
    {
        public static int limit = 10;
        public static int skipInt = 0;
        public static User UserToGetPostsFrom;
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private List<Post> PostCollection;
        private IEnumerable<Post> posts;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public string thing;

        public async Task<IEnumerable<Post>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                PostCollection = new List<Post>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                //   limit = limit + 10;
                //posts = UserToGetPostsFrom.GetPostHistory(limit: limit).Skip(skipInt);
                posts = UserToGetPostsFrom.GetPostHistory(limit: 10, after: thing);
                await Task.Run(() =>
                {
                    Parallel.ForEach(posts, post =>
                    {
                        PostCollection.Add(post);
                        thing = post.Fullname;
                    }
                    );
                });

                // Simulates a longer request...
                //  skipInt = skipInt + 10;
            });
            return PostCollection;
        }
    }
}
