using Eco_Reddit.Models;
using Eco_Reddit.Views;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using static Eco_Reddit.Views.HomePage;

namespace Eco_Reddit.Helpers
{
    public class GetPostsClass : IIncrementalSource<Posts>
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static int limit = 10;
        public static int skipInt = 0;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        List<Posts> PostCollection;
        private IEnumerable<Post> posts;
        public string thing;
        public static String Subreddit { get; set; }
        public static String SortOrder { get; set; }
        public async Task<IEnumerable<Posts>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
                await Task.Run(async () =>
                {

                    string refreshToken = localSettings.Values["refresh_token"].ToString();
             PostCollection = new List<Posts>();
                    var reddit = new RedditClient(appId, refreshToken, secret);
                    var subreddit = reddit.Subreddit(Subreddit);
                    switch (SortOrder)
                    {

                        case "all":
                            posts = subreddit.Posts.GetTop(new TimedCatSrListingInput(t: "all", limit: 25, after: thing));

                            break;
                        case "year":
                            posts = subreddit.Posts.GetTop(new TimedCatSrListingInput(t: "year", limit: 25, after: thing));

                            break;
                        case "month":
                            posts = subreddit.Posts.GetTop(new TimedCatSrListingInput(t: "month", limit: 25, after: thing));

                            break;
                        case "week":
                            posts = subreddit.Posts.GetTop(new TimedCatSrListingInput(t: "week", limit: 25, after: thing));
                      
                            break;
                        case "day":
                            posts = subreddit.Posts.GetTop(new TimedCatSrListingInput(t: "day", limit: 25, after: thing));

                            break;
                        case "Hot":
                            posts = subreddit.Posts.GetHot(limit: 25, after: thing);

                            break;
                        case "New":
                            posts = subreddit.Posts.GetNew(limit: 25, after: thing);

                            break;
                        case "Best":
                            posts = subreddit.Posts.GetBest(limit: 25, after: thing);

                            break;
                        case "Rising":
                            posts = subreddit.Posts.GetRising(limit: 25, after: thing);

                            break;
                        case "Controversial":
                            posts = subreddit.Posts.GetControversial(limit: 25, after: thing);

                            break;
                    }

                    await Task.Run(() =>
                         {
                             foreach (Post post in posts)
                             {
                                 PostCollection.Add(new Posts()
                                 {
                                     PostSelf = post,
                                 //PostCommentCount = "Comments: " + post.Comments.GetComments("new").Count.ToString(),
                                 // PostID = post.Id
                                 //  IsNSFW = Nsfw
                             });
                                 thing = post.Fullname;
                             }
                         });
                // Simulates a longer request...
            });
                return PostCollection;
        }
    }
   }
