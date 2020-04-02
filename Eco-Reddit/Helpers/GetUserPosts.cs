﻿using Eco_Reddit.Models;
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
    public class GetUserPostsClass : IIncrementalSource<Posts>
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static int limit = 10;
        public static int skipInt = 0;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public static User UserToGetPostsFrom { get; set; }
        List<Posts> PostCollection;
        private IEnumerable<Post> posts;
          
        public async Task<IEnumerable<Posts>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(async () =>
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                PostCollection = new List<Posts>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                posts = UserToGetPostsFrom.GetPostHistory(limit: limit).Skip(skipInt); ;
                limit = limit + 10;
                await Task.Run(() =>
                {
                    foreach (Post post in posts)
                    {

                        // pageContent += Environment.NewLine + "### [" + post.Title + "](" + post.Permalink + ")" + Environment.NewLine;
                        // Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
                        PostCollection.Add(new Posts()
                        {
                          //  TitleText = post.Title,
                            PostSelf = post,
                           /* PostDate = "Created: " + post.Created,
                            PostUpvoted = post.IsUpvoted,
                            PostSubreddit = "r/" + post.Subreddit,
                            PostDownVoted = post.IsDownvoted,
                            PostUpvotes = post.UpVotes.ToString(),
                            PostCommentCount = "Comments: " + post.Comments.GetComments("new").Count.ToString(),
                            PostFlair = Nsfw + "    Flair: " + post.Listing.LinkFlairText,
                            PostFlairColor = post.Listing.LinkFlairBackgroundColor,
                            PostType = LinkPostType,
                            LinkSource = ImageUrl,*/
                            //  IsNSFW = Nsfw
                        });
                    }
                });

                // Simulates a longer request...
                skipInt = skipInt + 10;
            });


            return PostCollection;


        }
    }
}
