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
    public class GetMessagesClass : IIncrementalSource<PrivateMessage>
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public static int limit = 10;
        public static int skipInt = 0;
        List<PrivateMessage> MessageCollection;
        private IEnumerable<Reddit.Things.Message> MessagesInbox;
        public string thing;
        public async Task<IEnumerable<PrivateMessage>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(async() =>
            {

                    string refreshToken = localSettings.Values["refresh_token"].ToString();
                    // Gets items from the collection according to pageIndex and pageSize parameters.
                    MessageCollection = new List<PrivateMessage>();
                    var reddit = new RedditClient(appId, refreshToken, secret);
                MessagesInbox = reddit.Account.Messages.GetMessagesUnread(limit: limit).Skip(skipInt);
            

                await Task.Run(() =>
                     {
                         foreach (Reddit.Things.Message message in MessagesInbox)
                         {
                             MessageCollection.Add(new PrivateMessage()
                             {
                                 MessageSelf = message,
                             });
                             thing = message.Fullname;
                         }
                     });
                skipInt = skipInt + 10;
                limit = limit + 10;
                // Simulates a longer request...
            });
            return MessageCollection;
        }
    }
   }
