using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Eco_Reddit.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Inputs.Search;

namespace Eco_Reddit.Helpers
{
    public class GetSearchUsers : IIncrementalSource<Users>
    {
        public static int limit = 1;
        public static int skipInt;
        public static string SearchSort;
        public static string TimeSort;
        public static string Input;
        public string appId = "mp8hDB_HfbctBg";
        private List<Users> ResultsCollection;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public string thing;

        public async Task<IEnumerable<Users>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                //  try
                //  {
                var localSettings = ApplicationData.Current.LocalSettings;
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                ResultsCollection = new List<Users>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var SearchResultsSearch =
                    reddit.SearchUsers(new SearchGetSearchInput(Input, limit: limit, sort: SearchSort, t: TimeSort,
                        type: "user")).Skip(skipInt);

                await Task.Run(() =>
                {
                    foreach (var User in SearchResultsSearch)
                    {
                        ResultsCollection.Add(new Users
                        {
                            UserSelf = User
                        });
                        // thing = User.Fullname;
                        skipInt = skipInt + 1;
                        limit = limit + 1;
                    }
                });
                /*    }
                    catch
                    {

                    }*/
            });
            return ResultsCollection;
        }
    }
}
