using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Eco_Reddit.Core.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Things;

namespace Eco_Reddit.Helpers
{
    public class GetRecievedMessagesClass : IIncrementalSource<PrivateMessage>
    {
        public static int limit = 10;
        public static int skipInt;
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private List<PrivateMessage> MessageCollection;
        private IEnumerable<Message> MessagesInbox;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public string thing;

        public async Task<IEnumerable<PrivateMessage>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                MessageCollection = new List<PrivateMessage>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                MessagesInbox = reddit.Account.Messages.GetMessagesInbox(limit: limit).Skip(skipInt);


                await Task.Run(() =>
                {
                    foreach (var message in MessagesInbox)
                    {
                        MessageCollection.Add(new PrivateMessage
                        {
                            MessageSelf = message
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
