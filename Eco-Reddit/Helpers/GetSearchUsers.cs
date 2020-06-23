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
    public class GetSearchUsers : IIncrementalSource<Users>
    {
        public static int limit = 1;
        public static int skipInt = 0;
        public static string SearchSort { get; set; }
       public static string TimeSort { get; set; }
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        List<Users> ResultsCollection;
        public string thing;
        public static string Input { get; set; }
        public async Task<IEnumerable<Users>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(async () =>
            {
              //  try
              //  {
                    Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    string refreshToken = localSettings.Values["refresh_token"].ToString();
                    // Gets items from the collection according to pageIndex and pageSize parameters.
                    ResultsCollection = new List<Users>();
                    var reddit = new RedditClient(appId, refreshToken, secret);

                IEnumerable<User> SearchResultsSearch = reddit.SearchUsers(new SearchGetSearchInput(Input, limit: limit, sort: SearchSort, t: TimeSort, type: "user")).Skip(skipInt);
                    await Task.Run(() =>
                    {

                        foreach (User User in SearchResultsSearch)
                        {
                            ResultsCollection.Add(new Users()
                            {
                                UserSelf = User,
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
