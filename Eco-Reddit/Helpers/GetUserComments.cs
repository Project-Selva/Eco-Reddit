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
    public class GetUserComments: IIncrementalSource<Comment>
    {
        private readonly Selva.Core.Helpers.LoginHelpers.LoginHelper loginHelper = new Selva.Core.Helpers.LoginHelpers.LoginHelper("mp8hDB_HfbctBg", "UCIGqKPDABnjb0XtMh0Q_LhrNks");
        public static User UserToGetcommentsFrom;
        public string appId = "mp8hDB_HfbctBg";
        private List<Comment> commentCollection;
        private IEnumerable<Comment> comments;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string thing;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";


        public async Task<IEnumerable<Comment>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                commentCollection = new List<Comment>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                 /*  var result = await loginHelper.Login_Refresh((string)localSettings.Values["refresh_token"]);
        //   localSettings.Values["refresh_token"] = result.RefreshToken;
        localSettings.Values["access_token"] = result.AccessToken;
        TokenSharpData.Reddit = new Selva.Core.Reddit(result.AccessToken);
        await TokenSharpData.Reddit.InitOrUpdateUserAsync();
                Selva.Core.Listing< Selva.Core.Things.Post> commentsList = TokenSharpData.Reddit.User.GetDislikedPosts();
                // comments = UserToGetcommentsFrom.GetCommentHistory(limit: 10, after: thing);
   commentsList.Stream().f*/


       
                // Simulates a longer request...
                //  skipInt = skipInt + 10;
            });
            return commentCollection;
        }
    }
}
