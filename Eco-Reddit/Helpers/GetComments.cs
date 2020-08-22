using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.LinksAndComments;
using Windows.Storage;
namespace Eco_Reddit.Helpers
{
    public class GetComments : IIncrementalSource<Eco_Reddit.Core.Models.Comments>
    {
        public static int limit = 10;
        public static int skipInt;
        public static Post PostToGetCommentsFrom;
        public static string SortOrder;
        private readonly IMapper _mapper;
        public string appId = "mp8hDB_HfbctBg";
        private List<Eco_Reddit.Core.Models.Comments> CommentCollection;
        private IEnumerable<Comment> Comments;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";

        public async Task<IEnumerable<Eco_Reddit.Core.Models.Comments>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                CommentCollection = new List<Eco_Reddit.Core.Models.Comments>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                switch (SortOrder)
                {
                    case "Random":
                        Comments = PostToGetCommentsFrom.Comments
                            .GetRandom(PostToGetCommentsFrom.Comments.GetComments(SortOrder).Count).Skip(skipInt);
                        break;
                    case "Top":
                        Comments = PostToGetCommentsFrom.Comments
                            .GetTop(PostToGetCommentsFrom.Comments.GetComments(SortOrder).Count, showMore: true)
                            .Skip(skipInt);
                        break;
                    case "Q and A":
                        Comments = PostToGetCommentsFrom.Comments
                            .GetQA(PostToGetCommentsFrom.Comments.GetComments(SortOrder).Count).Skip(skipInt);
                        break;
                    case "New":
                        Comments = PostToGetCommentsFrom.Comments
                            .GetNew(PostToGetCommentsFrom.Comments.GetComments(SortOrder).Count).Skip(skipInt);
                        break;
                    case "Old":
                        Comments = PostToGetCommentsFrom.Comments
                            .GetOld(PostToGetCommentsFrom.Comments.GetComments(SortOrder).Count).Skip(skipInt);
                        break;
                    case "Live":
                        Comments = PostToGetCommentsFrom.Comments
                            .GetLive(PostToGetCommentsFrom.Comments.GetComments(SortOrder).Count).Skip(skipInt);
                        break;
                    case "Controversial":
                        Comments = PostToGetCommentsFrom.Comments
                            .GetControversial(PostToGetCommentsFrom.Comments.GetComments(SortOrder).Count)
                            .Skip(skipInt);
                        break;
                    case "Confidence":
                        Comments = PostToGetCommentsFrom.Comments
                            .GetConfidence(PostToGetCommentsFrom.Comments.GetComments(SortOrder).Count).Skip(skipInt);
                        break;
                }

                //      limit = limit + PostToGetCommentsFrom.Comments.GetComments("Top").Count;
                await Task.Run(() =>
                {
                    foreach (var comment in Comments)
                    {
                        CommentCollection.Add(new Eco_Reddit.Core.Models.Comments
                        {
                            CommentSelf = comment
                        });
                        var children = reddit.Models.LinksAndComments.MoreChildren(
                            new LinksAndCommentsMoreChildrenInput(linkId: PostToGetCommentsFrom.Id,
                                children: comment.Id));

                        //  List<Comment> l = new List<Comment>(children.Comments);

                        foreach (var item in children.Comments)
                            //  if (item.Replies == null)
                            //    item.Replies = new List<Comment>();
                            CommentCollection.Add(new Eco_Reddit.Core.Models.Comments
                            {
                                CommentSelfThing = item
                            });
                        /* item.Replies.AddRange(
                                 mapped.Where(x => x.ParentFullname == item.Fullname)
                                       .OrderByDescending(x => x.Score)
                                       .ThenByDescending(x => x.Created)
                                       .ToList());*/
                        // var Replies = new Reddit.Things.MoreChildren();
                        /*          if (comment.replies != null && comment.replies.Count > 0)
                                  {
                                      foreach (Reddit.Controllers.Comment commentReply in comment.replies)
                                      {
                                          //    Replies.Comments.Add(commentReply.Listing);
                                          CommentCollection.Add(new Comments()
                                          {
                                              CommentSelf = commentReply,
                                          });
                                      }
                                  }*/
                    }
                });
                // Simulates a longer request...
                skipInt = skipInt + PostToGetCommentsFrom.Comments.GetComments(SortOrder).Count;
            });
            return CommentCollection;
        }
    }
}
