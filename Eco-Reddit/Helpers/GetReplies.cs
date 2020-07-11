using AutoMapper;
using Eco_Reddit.Models;
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
    public class Getreplies : IIncrementalSource<Comments>
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static int limit = 10;
        public static int skipInt = 0;
        private readonly IMapper _mapper;
        public string appId = "mp8hDB_HfbctBg";
        public static string commentid { get; set; }
        public static string postId;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        List<Comments> CommentCollection;
        private IEnumerable<Comment> replies;
        public static Post PostToGetrepliesFrom { get; set; }
        public static String SortOrder { get; set; }

        public async Task<IEnumerable<Comments>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            string refreshToken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshToken, secret);
            var idString = string.Join(',', commentid.Take(100));
            var children = reddit.Models.LinksAndComments.MoreChildren(
                new Reddit.Inputs.LinksAndComments.LinksAndCommentsMoreChildrenInput(linkId: postId, children: idString));

          var mapped = _mapper.Map<List<Comment>>(children.Comments);

            foreach (var item in mapped)
            {
                if (item.Replies == null)
                    item.Replies = new List<Comment>();
                CommentCollection.Add(new Comments()
                {
                    CommentSelf = item,
                });
               /* item.Replies.AddRange(
                    mapped.Where(x => x.ParentFullname == item.Fullname)
                          .OrderByDescending(x => x.Score)
                          .ThenByDescending(x => x.Created)
                          .ToList());*/
            }

         //   var minDepth = mapped.Select(x => x.Depth).Min();
       //     var result = mapped.Where(x => x.Depth == minDepth).ToList();

        //    return result;
          return CommentCollection;
        }
    }
}
