﻿using Eco_Reddit.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Comments = Eco_Reddit.Models.Comments;

namespace Eco_Reddit.Helpers
{
    public class GetComments : IIncrementalSource<Comments>
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static int limit = 10;
        public static int skipInt = 0;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        List<Comments> CommentCollection;
        private IEnumerable<Comment> Comments;
        public static Post PostToGetCommentsFrom { get; set; }
        public static String SortOrder { get; set; }
        public async Task<IEnumerable<Comments>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(async () =>
            {

                string refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                CommentCollection = new List<Comments>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                switch (SortOrder)
                {

                    case "Random":
                        Comments = PostToGetCommentsFrom.Comments.GetRandom(limit: limit).Skip(skipInt);
                        break;
                    case "Top":
                        Comments = PostToGetCommentsFrom.Comments.GetTop(limit: limit).Skip(skipInt);
                        break;
                    case "Q and A":
                        Comments = PostToGetCommentsFrom.Comments.GetQA(limit: limit).Skip(skipInt);
                        break;
                    case "New":
                        Comments = PostToGetCommentsFrom.Comments.GetNew(limit: limit).Skip(skipInt);
                        break;
                    case "Old":
                        Comments = PostToGetCommentsFrom.Comments.GetOld(limit: limit).Skip(skipInt);
                        break;
                    case "Live":
                        Comments = PostToGetCommentsFrom.Comments.GetLive(limit: limit).Skip(skipInt);
                        break;
                    case "Controversial":
                        Comments = PostToGetCommentsFrom.Comments.GetControversial(limit: limit).Skip(skipInt);
                        break;
                    case "Confidence":
                        Comments = PostToGetCommentsFrom.Comments.GetConfidence(limit: limit).Skip(skipInt);
                        break;
                }
                limit = limit + 10;
                await Task.Run(() =>
                {
                    foreach (Comment comment in Comments)
                    {
                        CommentCollection.Add(new Comments()
                        {
                            CommentSelf = comment,
                        });
                    }
                });
                // Simulates a longer request...
                skipInt = skipInt + 10;

            });
            return CommentCollection;
        }
    }
}