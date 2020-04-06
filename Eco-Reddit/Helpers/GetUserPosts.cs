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
    public class GetUserPostsClass : IIncrementalSource<Posts>
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static int limit = 10;
        public static int skipInt = 0;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public static User UserToGetPostsFrom { get; set; }
        List<Posts> PostCollection;
          
        public async Task<IEnumerable<Posts>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {

                string refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                PostCollection = new List<Posts>();
                var reddit = new RedditClient(appId, refreshToken, secret);
            limit = limit + 10;
            IEnumerable<Post> posts = reddit.Account.Me.GetPostHistory(limit: limit).Skip(skipInt);
            await Task.Run(() =>
                {
                    foreach (Post post in posts)
                    {
                        PostCollection.Add(new Posts()
                        {
                            PostSelf = post,
                        });
                    }
                });

            // Simulates a longer request...
            skipInt = skipInt + 10;
            return PostCollection;
        }
    }
}
